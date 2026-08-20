using ChinaVenezuela.Application.Usuarios.Contracts;

namespace ChinaVenezuela.Application.Usuarios.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioResponse> ValidarCredencialesAsync(IniciarSesionRequest request, CancellationToken cancellationToken);
    Task<CuentaUsuarioResponse> ObtenerCuentaAsync(string codigoUsuario, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> ObtenerGruposAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<UsuarioResponse>> ObtenerTodosAsync(string codigoSolicitante, CancellationToken cancellationToken);
    Task<IReadOnlyList<UsuarioResponse>> ObtenerReceptoresAsync(string codigoSolicitante, CancellationToken cancellationToken);
    Task<UsuarioResponse> ActualizarGruposAsync(string codigoSolicitante, string codigoUsuario, ActualizarGruposUsuarioRequest request, CancellationToken cancellationToken);
    Task<UsuarioResponse> CrearAdministrativoAsync(CrearUsuarioAdministrativoRequest request, CancellationToken cancellationToken);
    Task<UsuarioResponse> ActualizarAdministrativoAsync(string codigoSolicitante, string codigoUsuario, ActualizarUsuarioAdministrativoRequest request, CancellationToken cancellationToken);
    Task EliminarAsync(string codigoSolicitante, string codigoUsuario, CancellationToken cancellationToken);
}