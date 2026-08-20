using ChinaVenezuela.Api.Auth;
using ChinaVenezuela.Application.Usuarios.Contracts;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[Produces("application/json")]
public sealed class AuthController(IUsuarioService service, JwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("iniciar-sesion")]
    [ProducesResponseType(typeof(InicioSesionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<InicioSesionResponse>> IniciarSesion([FromBody] IniciarSesionRequest request, CancellationToken cancellationToken)
    {
        var usuario = await service.ValidarCredencialesAsync(request, cancellationToken);
        return Ok(new InicioSesionResponse(jwtTokenService.Crear(usuario), usuario));
    }
}