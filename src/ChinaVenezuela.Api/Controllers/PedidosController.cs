using System.Security.Claims;
using System.Text.Encodings.Web;
using ChinaVenezuela.Api.Comprobantes;
using ChinaVenezuela.Api.Hubs;
using ChinaVenezuela.Api.Pedidos;
using ChinaVenezuela.Application.Pedidos.Contracts;
using ChinaVenezuela.Application.Pedidos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using ChinaVenezuela.Domain.Pedidos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChinaVenezuela.Api.Controllers;

[ApiController, Authorize(Policy = "AccesoPedidos"), Route("api/pedidos"), Produces("application/json")]
public sealed class PedidosController(
    IPedidosService service,
    IUsuarioRepository usuarios,
    IComprobanteEmailService correos,
    IAlmacenamientoImagenes almacenamientoImagenes,
    IHubContext<ActualizacionesHub> hub) : ControllerBase
{
    private const long TamanoMaximoImagen = 15 * 1024 * 1024;

    [HttpGet("productos")]
    public async Task<ActionResult<PaginaProductosPedidoResponse>> ObtenerProductos([FromQuery] string? busqueda, [FromQuery] bool? enviado, [FromQuery] Guid? pedidoId, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 10, CancellationToken ct = default)
    {
        if (pagina < 1 || tamanoPagina is < 1 or > 5000) return BadRequest("Los valores de paginación no son válidos.");
        return Ok(await service.ObtenerProductosAsync(busqueda, enviado, pedidoId, pagina, tamanoPagina, ct));
    }

    [HttpGet("agentes")]
    public async Task<ActionResult<IReadOnlyList<AgentePedidoResponse>>> ObtenerAgentes(CancellationToken ct) => Ok(await service.ObtenerAgentesAsync(ct));

    [HttpGet("grupos")]
    public async Task<ActionResult<IReadOnlyList<PedidoResumenResponse>>> ObtenerGrupos(CancellationToken ct) => Ok(await service.ObtenerPedidosAsync(ct));
    [HttpPost("productos")]
    public async Task<ActionResult<ProductoPedidoResponse>> CrearProducto(CrearProductoPedidoRequest request, CancellationToken ct)
    {
        var producto = await service.CrearProductoAsync(CodigoSolicitante, request, ct);
        await NotificarAsync(ct);
        return Created($"api/pedidos/productos/{producto.Id}", producto);
    }

    [HttpPut("productos/{id:guid}")]
    public async Task<ActionResult<ProductoPedidoResponse>> ActualizarProducto(Guid id, ActualizarProductoPedidoRequest request, CancellationToken ct)
    {
        var producto = await service.ActualizarProductoAsync(id, CodigoSolicitante, request, ct);
        await NotificarAsync(ct);
        return Ok(producto);
    }

    [HttpPost("productos/{id:guid}/duplicar")]
    public async Task<ActionResult<ProductoPedidoResponse>> DuplicarProducto(Guid id, DuplicarProductoPedidoRequest request, CancellationToken ct)
    {
        var imagenesOriginales = await service.ObtenerImagenesAsync(id, ct);
        ProductoPedidoResponse? producto = null;
        var clavesCreadas = new List<string>();

        try
        {
            producto = await service.DuplicarProductoAsync(id, CodigoSolicitante, request, ct);

            foreach (var imagenOriginal in imagenesOriginales)
            {
                var extension = ObtenerExtensionImagen(imagenOriginal.TipoContenido);
                var claveNueva = $"{imagenOriginal.Tipo.ToString().ToLowerInvariant()}-{Guid.NewGuid():N}{extension}";
                var contenidoOriginal = await almacenamientoImagenes.AbrirLecturaAsync(imagenOriginal.ClaveAlmacenamiento, ct);
                if (contenidoOriginal is null)
                {
                    throw new ValidacionException(new Dictionary<string, string[]>
                    {
                        ["imagen"] = ["No se pudo copiar una imagen del subpedido original."]
                    });
                }

                await using (contenidoOriginal)
                {
                    await almacenamientoImagenes.GuardarAsync(claveNueva, contenidoOriginal, ct);
                }

                clavesCreadas.Add(claveNueva);
                await service.GuardarImagenAsync(
                    producto.Id,
                    imagenOriginal.Tipo,
                    new GuardarImagenProductoPedidoRequest(
                        claveNueva,
                        imagenOriginal.NombreOriginal,
                        imagenOriginal.TipoContenido,
                        imagenOriginal.TamanoBytes),
                    ct);
            }
        }
        catch
        {
            foreach (var clave in clavesCreadas)
            {
                await almacenamientoImagenes.EliminarAsync(clave, ct);
            }

            if (producto is not null)
            {
                await service.EliminarProductoAsync(producto.Id, ct);
            }

            throw;
        }

        await NotificarAsync(ct);
        return Created(
            $"api/pedidos/productos/{producto.Id}",
            producto with
            {
                TieneImagenFabrica = imagenesOriginales.Any(x => x.Tipo == TipoImagenProductoPedido.Fabrica),
                TieneImagenProductoTerminado = imagenesOriginales.Any(x => x.Tipo == TipoImagenProductoPedido.ProductoTerminado)
            });
    }
    [HttpDelete("productos/{id:guid}")]
    public async Task<IActionResult> EliminarProducto(Guid id, CancellationToken ct)
    {
        var imagenes = await service.ObtenerImagenesAsync(id, ct);
        await service.EliminarProductoAsync(id, ct);
        foreach (var imagen in imagenes) await almacenamientoImagenes.EliminarAsync(imagen.ClaveAlmacenamiento, ct);
        await NotificarAsync(ct);
        return NoContent();
    }

    [HttpPost("productos/{id:guid}/enviar")]
    public async Task<ActionResult<ComprobanteEnviadoResponse>> EnviarProducto(Guid id, EnviarProductoPedidoRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.ReceptorCodigoUsuario)) throw new ValidacionException(new Dictionary<string, string[]> { ["receptorCodigoUsuario"] = ["Selecciona un receptor."] });
        var producto = await service.ObtenerProductoAsync(id, ct);
        if (producto.Enviado) throw new ValidacionException(new Dictionary<string, string[]> { ["producto"] = ["Este pedido ya fue enviado."] });
        var remitente = await usuarios.ObtenerPorCodigoAsync(CodigoSolicitante, ct);
        var receptor = await usuarios.ObtenerPorCodigoAsync(request.ReceptorCodigoUsuario, ct);
        if (remitente is null || string.IsNullOrWhiteSpace(remitente.Correo)) throw new ValidacionException(new Dictionary<string, string[]> { ["correoRemitente"] = ["Tu usuario no tiene correo registrado."] });
        if (receptor is null || string.IsNullOrWhiteSpace(receptor.Correo)) throw new ValidacionException(new Dictionary<string, string[]> { ["correoReceptor"] = ["El receptor no tiene correo registrado."] });
        var enviado = await correos.EnviarAsync(new EnvioComprobanteRequest(receptor.Correo, receptor.Nombre, remitente.Correo, remitente.Nombre, $"Producto para pedido - {producto.ReferenciaAsignada}", CrearHtmlProducto(producto, remitente.Nombre, receptor.Nombre)), ct);
        await service.MarcarComoEnviadoAsync(id, ct);
        await NotificarAsync(ct);
        return Ok(enviado);
    }

    [HttpPut("productos/{id:guid}/imagenes/{tipo}"), RequestSizeLimit(ImagenesUploadLimits.MaxRequestBodyBytes)]
    [Consumes("multipart/form-data")]
    public Task<ActionResult<ProductoPedidoImagenResponse>> GuardarImagen(Guid id, string tipo, [FromForm] SubirImagenProductoRequest request, CancellationToken ct) => GuardarImagenInterna(id, ConvertirTipo(tipo), request, ct);

    [HttpGet("productos/{id:guid}/imagenes/{tipo}"), Produces("image/jpeg", "image/png", "image/webp")]
    public Task<IActionResult> ObtenerImagen(Guid id, string tipo, CancellationToken ct) => ObtenerImagenInterna(id, ConvertirTipo(tipo), ct);

    [HttpDelete("productos/{id:guid}/imagenes/{tipo}")]
    public Task<IActionResult> EliminarImagen(Guid id, string tipo, CancellationToken ct) => EliminarImagenInterna(id, ConvertirTipo(tipo), ct);

    // Compatibilidad: la ruta anterior representa la imagen de producto terminado.
    [HttpPut("productos/{id:guid}/imagen"), RequestSizeLimit(ImagenesUploadLimits.MaxRequestBodyBytes)]
    [Consumes("multipart/form-data")]
    public Task<ActionResult<ProductoPedidoImagenResponse>> GuardarImagenTerminada(Guid id, [FromForm] SubirImagenProductoRequest request, CancellationToken ct) => GuardarImagenInterna(id, TipoImagenProductoPedido.ProductoTerminado, request, ct);

    [HttpGet("productos/{id:guid}/imagen"), Produces("image/jpeg", "image/png", "image/webp")]
    public Task<IActionResult> ObtenerImagenTerminada(Guid id, CancellationToken ct) => ObtenerImagenInterna(id, TipoImagenProductoPedido.ProductoTerminado, ct);

    [HttpDelete("productos/{id:guid}/imagen")]
    public Task<IActionResult> EliminarImagenTerminada(Guid id, CancellationToken ct) => EliminarImagenInterna(id, TipoImagenProductoPedido.ProductoTerminado, ct);

    [HttpGet("registros-precios")]
    public async Task<ActionResult<IReadOnlyList<RegistroPrecioPedidoResponse>>> ObtenerRegistrosPrecios([FromQuery] string? busqueda, CancellationToken ct) => Ok(await service.ObtenerRegistrosPreciosAsync(busqueda, ct));

    private async Task<ActionResult<ProductoPedidoImagenResponse>> GuardarImagenInterna(Guid id, TipoImagenProductoPedido tipo, SubirImagenProductoRequest request, CancellationToken ct)
    {
        var imagen = request.Imagen;
        var validada = await ValidarImagenAsync(imagen, ct);
        var anterior = await service.ObtenerImagenAsync(id, tipo, ct);
        var clave = $"{tipo.ToString().ToLowerInvariant()}-{Guid.NewGuid():N}{validada.Extension}";
        try
        {
            await using var contenido = validada.Contenido;
            await almacenamientoImagenes.GuardarAsync(clave, contenido, ct);
            var respuesta = await service.GuardarImagenAsync(id, tipo, new GuardarImagenProductoPedidoRequest(clave, Path.GetFileName(imagen.FileName), validada.TipoContenido, imagen.Length), ct);
            if (anterior is not null) await almacenamientoImagenes.EliminarAsync(anterior.ClaveAlmacenamiento, ct);
            await NotificarAsync(ct);
            return Ok(respuesta);
        }
        catch
        {
            await almacenamientoImagenes.EliminarAsync(clave, ct);
            throw;
        }
    }

    private async Task<IActionResult> ObtenerImagenInterna(Guid id, TipoImagenProductoPedido tipo, CancellationToken ct)
    {
        var imagen = await service.ObtenerImagenAsync(id, tipo, ct);
        if (imagen is null) return NotFound();
        var contenido = await almacenamientoImagenes.AbrirLecturaAsync(imagen.ClaveAlmacenamiento, ct);
        return contenido is null ? NotFound() : File(contenido, imagen.TipoContenido, enableRangeProcessing: true);
    }

    private async Task<IActionResult> EliminarImagenInterna(Guid id, TipoImagenProductoPedido tipo, CancellationToken ct)
    {
        var imagen = await service.EliminarImagenAsync(id, tipo, ct);
        if (imagen is null) return NoContent();
        await almacenamientoImagenes.EliminarAsync(imagen.ClaveAlmacenamiento, ct);
        await NotificarAsync(ct);
        return NoContent();
    }

    private Task NotificarAsync(CancellationToken ct) => hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, ct);
    private string CodigoSolicitante => User.FindFirstValue("codigo_usuario") ?? throw new InvalidOperationException("No existe código de usuario en la sesión.");
    private static TipoImagenProductoPedido ConvertirTipo(string tipo) => tipo.Trim().ToLowerInvariant() switch
    {
        "fabrica" or "fábrica" => TipoImagenProductoPedido.Fabrica,
        "producto-terminado" or "terminado" => TipoImagenProductoPedido.ProductoTerminado,
        _ => throw new ValidacionException(new Dictionary<string, string[]> { ["tipo"] = ["El tipo de imagen debe ser fabrica o producto-terminado."] })
    };

    private static async Task<ImagenValidada> ValidarImagenAsync(IFormFile? imagen, CancellationToken ct)
    {
        if (imagen is null || imagen.Length == 0) throw new ValidacionException(new Dictionary<string, string[]> { ["imagen"] = ["Selecciona una imagen."] });
        if (imagen.Length > ImagenesUploadLimits.MaxFileBytes) throw new ValidacionException(new Dictionary<string, string[]> { ["imagen"] = ["La imagen no puede superar 15 MB."] });
        await using var temporal = new MemoryStream();
        await imagen.CopyToAsync(temporal, ct);
        var datos = temporal.ToArray();
        var tipo = DetectarTipo(datos);
        if (tipo is null) throw new ValidacionException(new Dictionary<string, string[]> { ["imagen"] = ["Solo se permiten imágenes JPEG, PNG o WebP válidas."] });
        return new ImagenValidada(new MemoryStream(datos), tipo.Value.TipoContenido, tipo.Value.Extension);
    }

    private static (string TipoContenido, string Extension)? DetectarTipo(byte[] datos)
    {
        if (datos.Length >= 3 && datos[0] == 0xFF && datos[1] == 0xD8 && datos[2] == 0xFF) return ("image/jpeg", ".jpg");
        if (datos.Length >= 8 && datos.Take(8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) return ("image/png", ".png");
        if (datos.Length >= 12 && datos.Take(4).SequenceEqual("RIFF"u8.ToArray()) && datos.Skip(8).Take(4).SequenceEqual("WEBP"u8.ToArray())) return ("image/webp", ".webp");
        return null;
    }

    private static string ObtenerExtensionImagen(string tipoContenido) => tipoContenido.ToLowerInvariant() switch
    {
        "image/jpeg" => ".jpg",
        "image/png" => ".png",
        "image/webp" => ".webp",
        _ => throw new ValidacionException(new Dictionary<string, string[]>
        {
            ["imagen"] = ["La imagen original tiene un formato no permitido."]
        })
    };
    private static string CrearHtmlProducto(ProductoPedidoResponse producto, string origen, string receptor)
    {
        var encoder = HtmlEncoder.Default;
        string F(string value) => encoder.Encode(value);
        string Row(string nombre, string valor) => $"<tr><td style=\"padding:8px;border:1px solid #dbe5f1;font-weight:600\">{F(nombre)}</td><td style=\"padding:8px;border:1px solid #dbe5f1\">{F(valor)}</td></tr>";
        return $"<div style=\"font-family:Arial,sans-serif;color:#12345b\"><h2>Producto para pedido</h2><p>Detalle del producto enviado desde China - Venezuela.</p><table style=\"border-collapse:collapse\">{Row("Origen", origen)}{Row("Receptor", receptor)}{Row("Código de barra asignado", producto.CodigoBarraAsignado)}{Row("Referencia asignada", producto.ReferenciaAsignada)}{Row("Tipo de pedido", producto.TipoProducto ?? "No aplica")}{Row("Agente", producto.Agente ?? "No aplica")}{Row("Fábrica", producto.Fabrica ?? "No aplica")}{Row("Composición de tela", producto.ComposicionTela ?? "No aplica")}{Row("Color para fabricar", producto.ColorParaFabricar ?? "No aplica")}{Row("Marca del producto", producto.MarcaProducto ?? "No aplica")}{Row("Curva talla", producto.CurvaTalla ?? "No aplica")}{Row("Pack por cajas", producto.PackPorCaja?.ToString() ?? "No aplica")}{Row("Cantidad de unidades", producto.CantidadUnidades?.ToString() ?? "No aplica")}{Row("Marcas y cantidades de bulto", producto.Bultos.Count == 0 ? "No aplica" : string.Join(", ", producto.Bultos.Select(bulto => $"{bulto.Nombre}: {bulto.Cantidad}")))}</table></div>";
    }
    private sealed record ImagenValidada(MemoryStream Contenido, string TipoContenido, string Extension);
    public sealed class SubirImagenProductoRequest { public IFormFile Imagen { get; init; } = null!; }
}