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
    [HttpGet("grupos")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> ObtenerGrupos(CancellationToken cancellationToken) =>
        Ok(await service.ObtenerGruposAsync(cancellationToken));

    [HttpPost("registrar")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Registrar([FromBody] RegistrarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var usuario = await service.RegistrarAsync(request, cancellationToken);
        return Created($"api/auth/usuarios/{usuario.CodigoUsuario}", usuario);
    }

    [HttpPost("iniciar-sesion")]
    [ProducesResponseType(typeof(InicioSesionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InicioSesionResponse>> IniciarSesion([FromBody] IniciarSesionRequest request, CancellationToken cancellationToken)
    {
        var usuario = await service.ValidarCredencialesAsync(request, cancellationToken);
        return Ok(new InicioSesionResponse(jwtTokenService.Crear(usuario), usuario));
    }
}