using System.Security.Claims;
using ChinaVenezuela.Application.Usuarios.Contracts;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaVenezuela.Api.Controllers;

[ApiController]
[Route("api/receptores")]
[Authorize]
[Produces("application/json")]
public sealed class ReceptoresController(IUsuarioService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioResponse>>> ObtenerTodos(CancellationToken cancellationToken) =>
        Ok(await service.ObtenerReceptoresAsync(CodigoSolicitante, cancellationToken));

    private string CodigoSolicitante => User.FindFirstValue("codigo_usuario") ?? throw new InvalidOperationException("No existe codigo de usuario en la sesion.");
}
