using ChinaVenezuela.Api.Hubs;
using ChinaVenezuela.Application.Catalogos.Contracts;
using ChinaVenezuela.Application.Catalogos.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Authorize(Policy = "AccesoCompras")]
[Produces("application/json")]
public sealed class CatalogosController(ICatalogoService service, IHubContext<ActualizacionesHub> hub) : ControllerBase
{
    [HttpGet("api/empresas")]
    public async Task<ActionResult<IReadOnlyList<EmpresaResponse>>> ObtenerEmpresas(CancellationToken ct) => Ok(await service.ObtenerEmpresasAsync(ct));

    [HttpPost("api/empresas")]
    [ProducesResponseType(typeof(CatalogoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<EmpresaResponse>> CrearEmpresa(CrearEmpresaRequest request, CancellationToken ct)
    {
        var result = await service.CrearEmpresaAsync(request, ct);
        await NotificarActualizacionAsync(ct);
        return Created($"api/empresas/{result.Id}", result);
    }

    [HttpPut("api/empresas/{id:guid}")]
    public async Task<ActionResult<EmpresaResponse>> ActualizarEmpresa(Guid id, ActualizarEmpresaRequest request, CancellationToken ct)
    {
        var result = await service.ActualizarEmpresaAsync(id, request, ct);
        await NotificarActualizacionAsync(ct);
        return Ok(result);
    }

    [HttpDelete("api/empresas/{id:guid}")]
    public async Task<IActionResult> EliminarEmpresa(Guid id, CancellationToken ct)
    {
        await service.EliminarEmpresaAsync(id, ct);
        await NotificarActualizacionAsync(ct);
        return NoContent();
    }

    [HttpGet("api/aduanas")]
    public async Task<ActionResult<IReadOnlyList<CatalogoResponse>>> ObtenerAduanas(CancellationToken ct) => Ok(await service.ObtenerAduanasAsync(ct));
    [HttpPost("api/aduanas")]
    public async Task<ActionResult<CatalogoResponse>> CrearAduana(CrearCatalogoRequest request, CancellationToken ct) { var result = await service.CrearAduanaAsync(request, ct); await NotificarActualizacionAsync(ct); return Created($"api/aduanas/{result.Id}", result); }
    [HttpPut("api/aduanas/{id:guid}")]
    public async Task<ActionResult<CatalogoResponse>> ActualizarAduana(Guid id, ActualizarCatalogoRequest request, CancellationToken ct) { var result = await service.ActualizarAduanaAsync(id, request, ct); await NotificarActualizacionAsync(ct); return Ok(result); }
    [HttpDelete("api/aduanas/{id:guid}")]
    public async Task<IActionResult> EliminarAduana(Guid id, CancellationToken ct) { await service.EliminarAduanaAsync(id, ct); await NotificarActualizacionAsync(ct); return NoContent(); }
    [HttpGet("api/puertos-llegada")]
    public async Task<ActionResult<IReadOnlyList<CatalogoResponse>>> ObtenerPuertosLlegada(CancellationToken ct) => Ok(await service.ObtenerPuertosLlegadaAsync(ct));
    [HttpPost("api/puertos-llegada")]
    public async Task<ActionResult<CatalogoResponse>> CrearPuertoLlegada(CrearCatalogoRequest request, CancellationToken ct) { var result = await service.CrearPuertoLlegadaAsync(request, ct); await NotificarActualizacionAsync(ct); return Created($"api/puertos-llegada/{result.Id}", result); }
    [HttpPut("api/puertos-llegada/{id:guid}")]
    public async Task<ActionResult<CatalogoResponse>> ActualizarPuertoLlegada(Guid id, ActualizarCatalogoRequest request, CancellationToken ct) { var result = await service.ActualizarPuertoLlegadaAsync(id, request, ct); await NotificarActualizacionAsync(ct); return Ok(result); }
    [HttpDelete("api/puertos-llegada/{id:guid}")]
    public async Task<IActionResult> EliminarPuertoLlegada(Guid id, CancellationToken ct) { await service.EliminarPuertoLlegadaAsync(id, ct); await NotificarActualizacionAsync(ct); return NoContent(); }
    [HttpGet("api/marcas-bulto")]
    public async Task<ActionResult<IReadOnlyList<CatalogoResponse>>> ObtenerMarcasBulto(CancellationToken ct) => Ok(await service.ObtenerMarcasBultoAsync(ct));

    [HttpPost("api/marcas-bulto")]
    [ProducesResponseType(typeof(CatalogoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CatalogoResponse>> CrearMarcaBulto(CrearCatalogoRequest request, CancellationToken ct)
    {
        var result = await service.CrearMarcaBultoAsync(request, ct);
        await NotificarActualizacionAsync(ct);
        return Created($"api/marcas-bulto/{result.Id}", result);
    }

    [HttpPut("api/marcas-bulto/{id:guid}")]
    public async Task<ActionResult<CatalogoResponse>> ActualizarMarcaBulto(Guid id, ActualizarCatalogoRequest request, CancellationToken ct)
    {
        var result = await service.ActualizarMarcaBultoAsync(id, request, ct);
        await NotificarActualizacionAsync(ct);
        return Ok(result);
    }

    [HttpDelete("api/marcas-bulto/{id:guid}")]
    public async Task<IActionResult> EliminarMarcaBulto(Guid id, CancellationToken ct)
    {
        await service.EliminarMarcaBultoAsync(id, ct);
        await NotificarActualizacionAsync(ct);
        return NoContent();
    }

    [HttpGet("api/contenedores-compartidos")]
    public async Task<ActionResult<IReadOnlyList<CatalogoResponse>>> ObtenerContenedoresCompartidos(CancellationToken ct) => Ok(await service.ObtenerContenedoresCompartidosAsync(ct));

    [HttpPost("api/contenedores-compartidos")]
    [ProducesResponseType(typeof(CatalogoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CatalogoResponse>> CrearContenedorCompartido(CrearCatalogoRequest request, CancellationToken ct)
    {
        var result = await service.CrearContenedorCompartidoAsync(request, ct);
        await NotificarActualizacionAsync(ct);
        return Created($"api/contenedores-compartidos/{result.Id}", result);
    }

    [HttpPut("api/contenedores-compartidos/{id:guid}")]
    public async Task<ActionResult<CatalogoResponse>> ActualizarContenedorCompartido(Guid id, ActualizarCatalogoRequest request, CancellationToken ct)
    {
        var result = await service.ActualizarContenedorCompartidoAsync(id, request, ct);
        await NotificarActualizacionAsync(ct);
        return Ok(result);
    }

    [HttpDelete("api/contenedores-compartidos/{id:guid}")]
    public async Task<IActionResult> EliminarContenedorCompartido(Guid id, CancellationToken ct)
    {
        await service.EliminarContenedorCompartidoAsync(id, ct);
        await NotificarActualizacionAsync(ct);
        return NoContent();
    }

    private Task NotificarActualizacionAsync(CancellationToken ct) => hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, ct);
}