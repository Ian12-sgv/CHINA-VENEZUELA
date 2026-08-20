using System.Security.Claims;
using ChinaVenezuela.Application.Usuarios.Contracts;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Route("api/cuenta")]
[Authorize]
[Produces("application/json")]
public sealed class CuentaController(IUsuarioService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CuentaUsuarioResponse>> Obtener(CancellationToken cancellationToken) =>
        Ok(await service.ObtenerCuentaAsync(CodigoUsuario, cancellationToken));

    private string CodigoUsuario => User.FindFirstValue("codigo_usuario") ?? throw new InvalidOperationException("No existe codigo de usuario en la sesion.");
}