using ChinaVenezuela.Application.Recepciones.Contracts;
using ChinaVenezuela.Application.Recepciones.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Route("api/compras-recibidas")]
[Produces("application/json")]
public sealed class ComprasRecibidasController(ICompraRecibidaService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CompraRecibidaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompraRecibidaResponse>> Crear([FromBody] CrearCompraRecibidaRequest request, CancellationToken cancellationToken)
    {
        var response = await service.CrearAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CompraRecibidaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CompraRecibidaResponse>>> ObtenerTodas(CancellationToken cancellationToken) => Ok(await service.ObtenerTodasAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompraRecibidaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompraRecibidaResponse>> ObtenerPorId(Guid id, CancellationToken cancellationToken) => Ok(await service.ObtenerPorIdAsync(id, cancellationToken));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CompraRecibidaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompraRecibidaResponse>> Actualizar(Guid id, [FromBody] ActualizarCompraRecibidaRequest request, CancellationToken cancellationToken) => Ok(await service.ActualizarAsync(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken cancellationToken)
    {
        await service.EliminarAsync(id, cancellationToken);
        return NoContent();
    }
}