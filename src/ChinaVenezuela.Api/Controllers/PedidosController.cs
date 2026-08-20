using System.Security.Claims;
using System.Text.Encodings.Web;
using ChinaVenezuela.Api.Comprobantes;
using ChinaVenezuela.Api.Hubs;
using ChinaVenezuela.Application.Pedidos.Contracts;
using ChinaVenezuela.Application.Pedidos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Application.Usuarios.Interfaces;
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
    public async Task<ActionResult<PaginaProductosPedidoResponse>> ObtenerProductos([FromQuery] string? busqueda, [FromQuery] DateOnly? fechaPedido, [FromQuery] bool? enviado, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 10, CancellationToken ct = default)
    {
        if (pagina < 1 || tamanoPagina is < 1 or > 100) return BadRequest("Los valores de paginación no son válidos.");
        return Ok(await service.ObtenerProductosAsync(busqueda, fechaPedido, enviado, pagina, tamanoPagina, ct));
    }

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
        var producto = await service.ActualizarProductoAsync(id, request, ct);
        await NotificarAsync(ct);
        return Ok(producto);
    }

    [HttpDelete("productos/{id:guid}")]
    public async Task<IActionResult> EliminarProducto(Guid id, CancellationToken ct)
    {
        var imagen = await service.ObtenerImagenAsync(id, ct);
        await service.EliminarProductoAsync(id, ct);
        if (imagen is not null) await almacenamientoImagenes.EliminarAsync(imagen.ClaveAlmacenamiento, ct);
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
        var enviado = await correos.EnviarAsync(new EnvioComprobanteRequest(receptor.Correo, receptor.Nombre, remitente.Correo, remitente.Nombre, $"Producto para pedido - {producto.Nombre}", CrearHtmlProducto(producto, remitente.Nombre, receptor.Nombre)), ct);
        await service.MarcarComoEnviadoAsync(id, ct);
        await NotificarAsync(ct);
        return Ok(enviado);
    }

    [HttpPut("productos/{id:guid}/imagen"), RequestSizeLimit(TamanoMaximoImagen)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProductoPedidoImagenResponse>> GuardarImagen(Guid id, [FromForm] SubirImagenProductoRequest request, CancellationToken ct)
    {
        var imagen = request.Imagen;
        var validada = await ValidarImagenAsync(imagen, ct);
        var anterior = await service.ObtenerImagenAsync(id, ct);
        var clave = $"{Guid.NewGuid():N}{validada.Extension}";
        try
        {
            await using var contenido = validada.Contenido;
            await almacenamientoImagenes.GuardarAsync(clave, contenido, ct);
            var respuesta = await service.GuardarImagenAsync(id, new GuardarImagenProductoPedidoRequest(clave, Path.GetFileName(imagen.FileName), validada.TipoContenido, imagen.Length), ct);
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

    [HttpGet("productos/{id:guid}/imagen"), Produces("image/jpeg", "image/png", "image/webp")]
    public async Task<IActionResult> ObtenerImagen(Guid id, CancellationToken ct)
    {
        var imagen = await service.ObtenerImagenAsync(id, ct);
        if (imagen is null) return NotFound();
        var clave = imagen.ClaveAlmacenamiento;
        var contenido = await almacenamientoImagenes.AbrirLecturaAsync(clave, ct);
        return contenido is null ? NotFound() : File(contenido, imagen.TipoContenido, enableRangeProcessing: true);
    }

    [HttpDelete("productos/{id:guid}/imagen")]
    public async Task<IActionResult> EliminarImagen(Guid id, CancellationToken ct)
    {
        var imagen = await service.EliminarImagenAsync(id, ct);
        if (imagen is null) return NoContent();
        await almacenamientoImagenes.EliminarAsync(imagen.ClaveAlmacenamiento, ct);
        await NotificarAsync(ct);
        return NoContent();
    }

    [HttpGet("registros-precios")]
    public async Task<ActionResult<IReadOnlyList<RegistroPrecioPedidoResponse>>> ObtenerRegistrosPrecios([FromQuery] string? busqueda, CancellationToken ct) => Ok(await service.ObtenerRegistrosPreciosAsync(busqueda, ct));

    private Task NotificarAsync(CancellationToken ct) => hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, ct);
    private string CodigoSolicitante => User.FindFirstValue("codigo_usuario") ?? throw new InvalidOperationException("No existe código de usuario en la sesión.");

    private static async Task<ImagenValidada> ValidarImagenAsync(IFormFile? imagen, CancellationToken ct)
    {
        if (imagen is null || imagen.Length == 0) throw new ValidacionException(new Dictionary<string, string[]> { ["imagen"] = ["Selecciona una imagen."] });
        if (imagen.Length > TamanoMaximoImagen) throw new ValidacionException(new Dictionary<string, string[]> { ["imagen"] = ["La imagen no puede superar 15 MB."] });
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

    private static string CrearHtmlProducto(ProductoPedidoResponse producto, string origen, string receptor)
    {
        var encoder = HtmlEncoder.Default;
        string F(string value) => encoder.Encode(value);
        string Row(string nombre, string valor) => $"<tr><td style=\"padding:8px;border:1px solid #dbe5f1;font-weight:600\">{F(nombre)}</td><td style=\"padding:8px;border:1px solid #dbe5f1\">{F(valor)}</td></tr>";
        return $"<div style=\"font-family:Arial,sans-serif;color:#12345b\"><h2>Producto para pedido</h2><p>Detalle del producto enviado desde China - Venezuela.</p><table style=\"border-collapse:collapse\">{Row("Origen", origen)}{Row("Receptor", receptor)}{Row("Fecha del pedido", producto.FechaPedido.ToString("dd/MM/yyyy"))}{Row("Código de barra", producto.CodigoBarra)}{Row("Referencia", producto.Referencia)}{Row("Producto", producto.Nombre)}{Row("Categoría", producto.Categoria)}{Row("Marca", producto.Marca ?? "No aplica")}{Row("Precio detal", producto.PrecioDetal.ToString("0.00"))}</table></div>";
    }

    private sealed record ImagenValidada(MemoryStream Contenido, string TipoContenido, string Extension);
    public sealed class SubirImagenProductoRequest { public IFormFile Imagen { get; init; } = null!; }
}