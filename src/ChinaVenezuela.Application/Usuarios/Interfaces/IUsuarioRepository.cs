using ChinaVenezuela.Domain.Usuarios;

namespace ChinaVenezuela.Application.Usuarios.Interfaces;

public interface IUsuarioRepository
{
    Task<bool> ExisteAsync(string codigoUsuario, CancellationToken cancellationToken);
    Task<bool> ExisteNombreAsync(string nombre, CancellationToken cancellationToken);
    Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken);
    Task<int> ContarPorNombreAsync(string nombre, CancellationToken cancellationToken);
    Task<Usuario?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken);
    Task<Usuario?> ObtenerPorCodigoAsync(string codigoUsuario, CancellationToken cancellationToken);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken);
    Task<Usuario?> ObtenerPorTokenVerificacionHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task<IReadOnlyList<Usuario>> ObtenerTodosAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> ObtenerNombresGruposAsync(CancellationToken cancellationToken);
    Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken);
    Task ReemplazarGruposAsync(string codigoUsuario, IReadOnlyCollection<string> grupos, CancellationToken cancellationToken);
    Task ActualizarAsync(string codigoUsuario, string nombre, string? correo, string contrasenaHash, bool status, IReadOnlyCollection<string> grupos, CancellationToken cancellationToken);
    Task EliminarAsync(string codigoUsuario, CancellationToken cancellationToken);
    Task GuardarAsync(Usuario usuario, CancellationToken cancellationToken);
    Task MarcarCorreoPendienteAsync(string codigoUsuario, string tokenHash, DateTimeOffset expiraUtc, CancellationToken cancellationToken);
}