using System.Security.Claims;
using ChinaVenezuela.Api.Hubs;
using ChinaVenezuela.Application.Usuarios.Contracts;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Policy = "GestionGrupos")]
[Produces("application/json")]
public sealed class UsuariosController(IUsuarioService service, IHubContext<ActualizacionesHub> hub) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioResponse>>> ObtenerTodos(CancellationToken cancellationToken) => Ok(await service.ObtenerTodosAsync(CodigoSolicitante, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<UsuarioResponse>> Crear(CrearUsuarioAdministrativoRequest request, CancellationToken cancellationToken)
    {
        var usuario = await service.CrearAdministrativoAsync(request, cancellationToken);
        await NotificarAsync(cancellationToken);
        return Created($"api/usuarios/{usuario.CodigoUsuario}", usuario);
    }

    [HttpPut("{codigoUsuario}")]
    public async Task<ActionResult<UsuarioResponse>> Actualizar(string codigoUsuario, ActualizarUsuarioAdministrativoRequest request, CancellationToken cancellationToken)
    {
        var usuario = await service.ActualizarAdministrativoAsync(CodigoSolicitante, codigoUsuario, request, cancellationToken);
        await NotificarAsync(cancellationToken);
        return Ok(usuario);
    }

    [HttpPut("{codigoUsuario}/grupos")]
    public async Task<ActionResult<UsuarioResponse>> ActualizarGrupos(string codigoUsuario, ActualizarGruposUsuarioRequest request, CancellationToken cancellationToken)
    {
        var usuario = await service.ActualizarGruposAsync(CodigoSolicitante, codigoUsuario, request, cancellationToken);
        await NotificarAsync(cancellationToken);
        return Ok(usuario);
    }

    [HttpDelete("{codigoUsuario}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Eliminar(string codigoUsuario, CancellationToken cancellationToken)
    {
        await service.EliminarAsync(CodigoSolicitante, codigoUsuario, cancellationToken);
        await NotificarAsync(cancellationToken);
        return NoContent();
    }

    private string CodigoSolicitante => User.FindFirstValue("codigo_usuario") ?? throw new InvalidOperationException("No existe codigo de usuario en la sesion.");
    private Task NotificarAsync(CancellationToken cancellationToken) => hub.Clients.All.SendAsync(ActualizacionesHub.DatosActualizados, cancellationToken);
}

