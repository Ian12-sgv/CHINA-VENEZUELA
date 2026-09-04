using ChinaVenezuela.Api.Hubs;
using ChinaVenezuela.Api.Comprobantes;
using ChinaVenezuela.Api.Pedidos;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using System.Text.Encodings.Web;
using ChinaVenezuela.Application.Pedidos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Contracts;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Application.Recepciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Authorize(Policy = "AccesoCompras")]
[Route("api/compras-recibidas")]
[Produces("application/json")]
public sealed class ComprasRecibidasController(ICompraRecibidaService service, IUsuarioRepository usuarios, IComprobanteEmailService comprobantes, IComprobanteCompraAdjuntosService adjuntosComprobante, IAlmacenamientoImagenes almacenamiento, IHubContext<ActualizacionesHub> hub) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CompraRecibidaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompraRecibidaResponse>> Crear([FromBody] CrearCompraRecibidaRequest request, CancellationToken cancellationToken)
    {
        var response = await service.CrearAsync(CodigoSolicitante, request, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CompraRecibidaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CompraRecibidaResponse>>> ObtenerTodas(CancellationToken cancellationToken) => Ok(await service.ObtenerTodasAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompraRecibidaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompraRecibidaResponse>> ObtenerPorId(Guid id, CancellationToken cancellationToken) => Ok(await service.ObtenerPorIdAsync(id, cancellationToken));

    [HttpPost("{id:guid}/comprobante/enviar")]
    [ProducesResponseType(typeof(ComprobanteEnviadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ComprobanteEnviadoResponse>> EnviarComprobante(Guid id, CancellationToken cancellationToken)
    {
        var compra = await service.ObtenerPorIdAsync(id, cancellationToken);
        if (compra.FechaComprobanteEnviadoUtc is not null)
            throw new ChinaVenezuela.Application.Recepciones.Exceptions.ValidacionException(new Dictionary<string, string[]> { ["compra"] = ["El comprobante de esta compra ya fue enviado."] });
        var remitente = await usuarios.ObtenerPorCodigoAsync(CodigoSolicitante, cancellationToken);
        if (remitente is null || string.IsNullOrWhiteSpace(remitente.Correo))
            throw new ChinaVenezuela.Application.Recepciones.Exceptions.ValidacionException(new Dictionary<string, string[]> { ["correoRemitente"] = ["Tu usuario no tiene un correo registrado. Agregalo en Usuarios antes de enviar el comprobante."] });
        if (string.IsNullOrWhiteSpace(compra.ReceptorCorreo))
            throw new ChinaVenezuela.Application.Recepciones.Exceptions.ValidacionException(new Dictionary<string, string[]> { ["correoReceptor"] = ["El receptor no tiene un correo registrado. Agregalo en Usuarios antes de enviar el comprobante."] });

        var adjuntos = adjuntosComprobante.Generar(compra, remitente.Nombre).ToList();
        if (!string.IsNullOrEmpty(compra.ClaveArchivoComprobante))
        {
            var contenidoArchivo = await almacenamiento.AbrirLecturaAsync(compra.ClaveArchivoComprobante, cancellationToken);
            if (contenidoArchivo is not null)
            {
                await using var flujo = contenidoArchivo;
                using var memoria = new MemoryStream();
                await flujo.CopyToAsync(memoria, cancellationToken);
                adjuntos.Add(new ArchivoAdjuntoCorreo(compra.NombreArchivoComprobante ?? "comprobante-adjunto", memoria.ToArray()));
            }
        }

        var enviado = await comprobantes.EnviarAsync(new EnvioComprobanteRequest(
            compra.ReceptorCorreo,
            compra.ReceptorNombre ?? compra.ReceptorCodigoUsuario ?? "Receptor",
            remitente.Correo,
            remitente.Nombre,
            $"Comprobante de compra - {compra.NumeroContenedor}",
            CrearHtmlComprobante(compra, remitente.Nombre),
            adjuntos), cancellationToken);
        await service.MarcarComprobanteEnviadoAsync(id, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return Ok(enviado);
    }
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(CompraRecibidaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompraRecibidaResponse>> ActualizarStatus(Guid id, [FromBody] ActualizarStatusCompraRecibidaRequest request, CancellationToken cancellationToken)
    {
        var response = await service.ActualizarStatusAsync(id, request, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return Ok(response);
    }
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CompraRecibidaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompraRecibidaResponse>> Actualizar(Guid id, [FromBody] ActualizarCompraRecibidaRequest request, CancellationToken cancellationToken)
    {
        var response = await service.ActualizarAsync(CodigoSolicitante, id, request, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}/archivo-comprobante"), RequestSizeLimit(ImagenesUploadLimits.MaxRequestBodyBytes)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(CompraRecibidaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompraRecibidaResponse>> GuardarArchivoComprobante(Guid id, [FromForm] SubirArchivoComprobanteRequest request, CancellationToken cancellationToken)
    {
        var validado = await ValidarArchivoAsync(request.Archivo, cancellationToken);
        var anterior = await service.ObtenerPorIdAsync(id, cancellationToken);
        var clave = $"comprobante-{Guid.NewGuid():N}{validado.Extension}";
        try
        {
            await using var contenido = validado.Contenido;
            await almacenamiento.GuardarAsync(clave, contenido, cancellationToken);
            var respuesta = await service.GuardarArchivoComprobanteAsync(id, new GuardarArchivoComprobanteCompraRequest(clave, Path.GetFileName(request.Archivo.FileName), validado.TipoContenido, request.Archivo.Length), cancellationToken);
            if (!string.IsNullOrEmpty(anterior.ClaveArchivoComprobante)) await almacenamiento.EliminarAsync(anterior.ClaveArchivoComprobante, cancellationToken);
            await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
            return Ok(respuesta);
        }
        catch
        {
            await almacenamiento.EliminarAsync(clave, cancellationToken);
            throw;
        }
    }

    [HttpGet("{id:guid}/archivo-comprobante")]
    [Produces("application/pdf", "image/jpeg", "image/png", "image/webp")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerArchivoComprobante(Guid id, CancellationToken cancellationToken)
    {
        var compra = await service.ObtenerPorIdAsync(id, cancellationToken);
        if (string.IsNullOrEmpty(compra.ClaveArchivoComprobante)) return NotFound();
        var contenido = await almacenamiento.AbrirLecturaAsync(compra.ClaveArchivoComprobante, cancellationToken);
        return contenido is null ? NotFound() : File(contenido, compra.TipoContenidoArchivoComprobante ?? "application/octet-stream", enableRangeProcessing: true);
    }

    [HttpDelete("{id:guid}/archivo-comprobante")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarArchivoComprobante(Guid id, CancellationToken cancellationToken)
    {
        var anterior = await service.ObtenerPorIdAsync(id, cancellationToken);
        await service.EliminarArchivoComprobanteAsync(id, cancellationToken);
        if (!string.IsNullOrEmpty(anterior.ClaveArchivoComprobante)) await almacenamiento.EliminarAsync(anterior.ClaveArchivoComprobante, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return NoContent();
    }

    private static string CrearHtmlComprobante(CompraRecibidaResponse compra, string origen)
    {
        var encoder = HtmlEncoder.Default;
        string Row(string label, string? value) => $"<tr><td style=\"padding:8px;border:1px solid #dbe5f1;font-weight:600\">{encoder.Encode(label)}</td><td style=\"padding:8px;border:1px solid #dbe5f1\">{encoder.Encode(value ?? "No aplica")}</td></tr>";
        return $"<div style=\"font-family:Arial,sans-serif;color:#12345b\"><h2>Comprobante de compra</h2><p>Resumen del recibo de mercancía China - Venezuela.</p><table style=\"border-collapse:collapse\">{Row("Origen", origen)}{Row("Receptor", compra.ReceptorNombre ?? compra.ReceptorCodigoUsuario ?? "Sin asignar")}{Row("Contenedor", compra.NombreContenedor)}{Row("Numero", compra.NumeroContenedor)}{Row("Fecha de salida", compra.FechaSalida.ToString("dd/MM/yyyy"))}{Row("Fecha de llegada", compra.FechaLlegada?.ToString("dd/MM/yyyy"))}{Row("Puerto", compra.PuertoLlegada)}{Row("Aduana", compra.Aduana)}{Row("Descripcion", compra.Descripcion)}{Row("Status", compra.Status)}</table></div>";
    }
    private string CodigoSolicitante => User.FindFirstValue("codigo_usuario") ?? throw new InvalidOperationException("No existe codigo de usuario en la sesion.");
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken cancellationToken)
    {
        await service.EliminarAsync(id, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return NoContent();
    }

    private static async Task<ArchivoComprobanteValidado> ValidarArchivoAsync(IFormFile? archivo, CancellationToken ct)
    {
        if (archivo is null || archivo.Length == 0) throw new ValidacionException(new Dictionary<string, string[]> { ["archivo"] = ["Selecciona un archivo."] });
        if (archivo.Length > ImagenesUploadLimits.MaxFileBytes) throw new ValidacionException(new Dictionary<string, string[]> { ["archivo"] = ["El archivo no puede superar 15 MB."] });
        await using var temporal = new MemoryStream();
        await archivo.CopyToAsync(temporal, ct);
        var datos = temporal.ToArray();
        var tipo = DetectarTipoArchivo(datos);
        if (tipo is null) throw new ValidacionException(new Dictionary<string, string[]> { ["archivo"] = ["Solo se permiten archivos PDF, JPEG, PNG o WebP."] });
        return new ArchivoComprobanteValidado(new MemoryStream(datos), tipo.Value.TipoContenido, tipo.Value.Extension);
    }

    private static (string TipoContenido, string Extension)? DetectarTipoArchivo(byte[] datos)
    {
        if (datos.Length >= 5 && datos.Take(5).SequenceEqual("%PDF-"u8.ToArray())) return ("application/pdf", ".pdf");
        if (datos.Length >= 3 && datos[0] == 0xFF && datos[1] == 0xD8 && datos[2] == 0xFF) return ("image/jpeg", ".jpg");
        if (datos.Length >= 8 && datos.Take(8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) return ("image/png", ".png");
        if (datos.Length >= 12 && datos.Take(4).SequenceEqual("RIFF"u8.ToArray()) && datos.Skip(8).Take(4).SequenceEqual("WEBP"u8.ToArray())) return ("image/webp", ".webp");
        return null;
    }

    private sealed record ArchivoComprobanteValidado(MemoryStream Contenido, string TipoContenido, string Extension);
    public sealed class SubirArchivoComprobanteRequest { public IFormFile Archivo { get; init; } = null!; }
}






