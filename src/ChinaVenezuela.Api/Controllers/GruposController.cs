using ChinaVenezuela.Api.Hubs;
using ChinaVenezuela.Application.Grupos.Contracts;
using ChinaVenezuela.Application.Grupos.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Route("api/grupos")]
[Authorize(Policy = "GestionGrupos")]
[Produces("application/json")]
public sealed class GruposController(IGrupoService service, IHubContext<ActualizacionesHub> hub) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GrupoResponse>>> ObtenerTodos(CancellationToken cancellationToken) =>
        Ok(await service.ObtenerTodosAsync(cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(GrupoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<GrupoResponse>> Crear(CrearGrupoRequest request, CancellationToken cancellationToken)
    {
        var grupo = await service.CrearAsync(request, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return Created($"api/grupos/{Uri.EscapeDataString(grupo.Nombre)}", grupo);
    }

    [HttpPut("{nombre}")]
    public async Task<ActionResult<GrupoResponse>> Actualizar(string nombre, ActualizarGrupoRequest request, CancellationToken cancellationToken)
    {
        var grupo = await service.ActualizarAsync(nombre, request, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return Ok(grupo);
    }

    [HttpDelete("{nombre}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Eliminar(string nombre, CancellationToken cancellationToken)
    {
        await service.EliminarAsync(nombre, cancellationToken);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
        return NoContent();
    }
}