using ChinaVenezuela.Application.Catalogos.Contracts;
using ChinaVenezuela.Application.Catalogos.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Produces("application/json")]
public sealed class CatalogosController(ICatalogoService service) : ControllerBase
{
    [HttpGet("api/empresas")]
    public async Task<ActionResult<IReadOnlyList<CatalogoResponse>>> ObtenerEmpresas(CancellationToken ct) => Ok(await service.ObtenerEmpresasAsync(ct));
    [HttpPost("api/empresas")]
    [ProducesResponseType(typeof(CatalogoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CatalogoResponse>> CrearEmpresa(CrearCatalogoRequest request, CancellationToken ct) { var result = await service.CrearEmpresaAsync(request, ct); return Created($"api/empresas/{result.Id}", result); }
    [HttpPut("api/empresas/{id:guid}")]
    public async Task<ActionResult<CatalogoResponse>> ActualizarEmpresa(Guid id, ActualizarCatalogoRequest request, CancellationToken ct) => Ok(await service.ActualizarEmpresaAsync(id, request, ct));
    [HttpDelete("api/empresas/{id:guid}")]
    public async Task<IActionResult> EliminarEmpresa(Guid id, CancellationToken ct) { await service.EliminarEmpresaAsync(id, ct); return NoContent(); }

    [HttpGet("api/marcas-bulto")]
    public async Task<ActionResult<IReadOnlyList<CatalogoResponse>>> ObtenerMarcasBulto(CancellationToken ct) => Ok(await service.ObtenerMarcasBultoAsync(ct));
    [HttpPost("api/marcas-bulto")]
    [ProducesResponseType(typeof(CatalogoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CatalogoResponse>> CrearMarcaBulto(CrearCatalogoRequest request, CancellationToken ct) { var result = await service.CrearMarcaBultoAsync(request, ct); return Created($"api/marcas-bulto/{result.Id}", result); }
    [HttpPut("api/marcas-bulto/{id:guid}")]
    public async Task<ActionResult<CatalogoResponse>> ActualizarMarcaBulto(Guid id, ActualizarCatalogoRequest request, CancellationToken ct) => Ok(await service.ActualizarMarcaBultoAsync(id, request, ct));
    [HttpDelete("api/marcas-bulto/{id:guid}")]
    public async Task<IActionResult> EliminarMarcaBulto(Guid id, CancellationToken ct) { await service.EliminarMarcaBultoAsync(id, ct); return NoContent(); }

    [HttpGet("api/contenedores-compartidos")]
    public async Task<ActionResult<IReadOnlyList<CatalogoResponse>>> ObtenerContenedoresCompartidos(CancellationToken ct) => Ok(await service.ObtenerContenedoresCompartidosAsync(ct));
    [HttpPost("api/contenedores-compartidos")]
    [ProducesResponseType(typeof(CatalogoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CatalogoResponse>> CrearContenedorCompartido(CrearCatalogoRequest request, CancellationToken ct) { var result = await service.CrearContenedorCompartidoAsync(request, ct); return Created($"api/contenedores-compartidos/{result.Id}", result); }
    [HttpPut("api/contenedores-compartidos/{id:guid}")]
    public async Task<ActionResult<CatalogoResponse>> ActualizarContenedorCompartido(Guid id, ActualizarCatalogoRequest request, CancellationToken ct) => Ok(await service.ActualizarContenedorCompartidoAsync(id, request, ct));
    [HttpDelete("api/contenedores-compartidos/{id:guid}")]
    public async Task<IActionResult> EliminarContenedorCompartido(Guid id, CancellationToken ct) { await service.EliminarContenedorCompartidoAsync(id, ct); return NoContent(); }
}