using ChinaVenezuela.Api.Hubs;
using ChinaVenezuela.Application.Pedidos.Contracts;
using ChinaVenezuela.Application.Pedidos.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Route("api/agentes")]
[Authorize(Policy = "AccesoOperativo")]
[Produces("application/json")]
public sealed class AgentesController(IPedidosService service, IHubContext<ActualizacionesHub> hub) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AgentePedidoResponse>>> ObtenerTodos(CancellationToken ct) => Ok(await service.ObtenerAgentesAsync(ct));

    [HttpPost]
    public async Task<ActionResult<AgentePedidoResponse>> Crear(CrearAgentePedidoRequest request, CancellationToken ct)
    {
        var agente = await service.CrearAgenteAsync(request, ct);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, ct);
        return Created($"api/agentes/{agente.Id}", agente);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AgentePedidoResponse>> Actualizar(Guid id, ActualizarAgentePedidoRequest request, CancellationToken ct)
    {
        var agente = await service.ActualizarAgenteAsync(id, request, ct);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, ct);
        return Ok(agente);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        await service.EliminarAgenteAsync(id, ct);
        await hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, ct);
        return NoContent();
    }
}