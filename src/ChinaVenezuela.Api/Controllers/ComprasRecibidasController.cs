using ChinaVenezuela.Api.Hubs;
using ChinaVenezuela.Api.Comprobantes;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using System.Text.Encodings.Web;
using ChinaVenezuela.Application.Recepciones.Contracts;
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
public sealed class ComprasRecibidasController(ICompraRecibidaService service, IUsuarioRepository usuarios, IComprobanteEmailService comprobantes, IHubContext<ActualizacionesHub> hub) : ControllerBase
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

        var enviado = await comprobantes.EnviarAsync(new EnvioComprobanteRequest(
            compra.ReceptorCorreo,
            compra.ReceptorNombre ?? compra.ReceptorCodigoUsuario ?? "Receptor",
            remitente.Correo,
            remitente.Nombre,
            $"Comprobante de compra - {compra.NumeroContenedor}",
            CrearHtmlComprobante(compra, remitente.Nombre)), cancellationToken);
        await service.MarcarComprobanteEnviadoAsync(id, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return Ok(enviado);
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

    private static string CrearHtmlComprobante(CompraRecibidaResponse compra, string origen)
    {
        var encoder = HtmlEncoder.Default;
        string Row(string label, string? value) => $"<tr><td style=\"padding:8px;border:1px solid #dbe5f1;font-weight:600\">{encoder.Encode(label)}</td><td style=\"padding:8px;border:1px solid #dbe5f1\">{encoder.Encode(value ?? "No aplica")}</td></tr>";
        return $"<div style=\"font-family:Arial,sans-serif;color:#12345b\"><h2>Comprobante de compra</h2><p>Resumen del recibo de mercancía China - Venezuela.</p><table style=\"border-collapse:collapse\">{Row("Origen", origen)}{Row("Receptor", compra.ReceptorNombre ?? compra.ReceptorCodigoUsuario ?? "Sin asignar")}{Row("Contenedor", compra.NombreContenedor)}{Row("Numero", compra.NumeroContenedor)}{Row("Fecha de salida", compra.FechaSalida.ToString("dd/MM/yyyy"))}{Row("Fecha de llegada", compra.FechaLlegada?.ToString("dd/MM/yyyy"))}{Row("Puerto", compra.PuertoLlegada)}{Row("Aduana", compra.Aduana)}{Row("Descripcion", compra.Descripcion)}</table></div>";
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
}






