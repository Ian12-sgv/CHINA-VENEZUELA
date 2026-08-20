using ChinaVenezuela.Application.Usuarios.Contracts;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using ChinaVenezuela.Application.Usuarios.Services;
using ChinaVenezuela.Domain.Usuarios;

namespace ChinaVenezuela.Application.Tests;

public sealed class UsuarioServiceAutenticacionTests
{
    [Fact]
    public async Task IniciarSesion_CuandoElCorreoNoEstaMarcadoComoVerificado_PermiteAcceso()
    {
        var usuario = new Usuario("NUEVO", "Nuevo", "clave", true, "nuevo@ejemplo.com", false, "token-hash", DateTimeOffset.UtcNow.AddHours(1));
        var service = new UsuarioService(new RepositorioUsuarios(usuario), new ContrasenaPrueba());

        var sesion = await service.ValidarCredencialesAsync(new IniciarSesionRequest("Nuevo", "clave"), CancellationToken.None);

        Assert.Equal("NUEVO", sesion.CodigoUsuario);
    }

    private sealed class ContrasenaPrueba : IContrasenaHasher
    {
        public string Hash(string contrasena) => contrasena;
        public bool Verificar(string contrasena, string hash) => contrasena == hash;
    }

    private sealed class RepositorioUsuarios(Usuario usuario) : IUsuarioRepository
    {
        public Task<bool> ExisteAsync(string codigoUsuario, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> ExisteNombreAsync(string nombre, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<int> ContarPorNombreAsync(string nombre, CancellationToken cancellationToken) => Task.FromResult(string.Equals(nombre, usuario.Nombre, StringComparison.OrdinalIgnoreCase) ? 1 : 0);
        public Task<Usuario?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken) => Task.FromResult<Usuario?>(string.Equals(nombre, usuario.Nombre, StringComparison.OrdinalIgnoreCase) ? usuario : null);
        public Task<Usuario?> ObtenerPorCodigoAsync(string codigoUsuario, CancellationToken cancellationToken) => Task.FromResult<Usuario?>(usuario.CodigoUsuario == codigoUsuario ? usuario : null);
        public Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken) => Task.FromResult<Usuario?>(string.Equals(correo, usuario.Correo, StringComparison.OrdinalIgnoreCase) ? usuario : null);
        public Task<Usuario?> ObtenerPorTokenVerificacionHashAsync(string tokenHash, CancellationToken cancellationToken) => Task.FromResult<Usuario?>(usuario.TokenVerificacionHash == tokenHash ? usuario : null);
        public Task<IReadOnlyList<Usuario>> ObtenerTodosAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Usuario>>([usuario]);
        public Task<IReadOnlyList<string>> ObtenerNombresGruposAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<string>>(["oficina"]);
        public Task AgregarAsync(Usuario nuevo, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task ReemplazarGruposAsync(string codigoUsuario, IReadOnlyCollection<string> grupos, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task ActualizarAsync(string codigoUsuario, string nombre, string? correo, string contrasenaHash, bool status, IReadOnlyCollection<string> grupos, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task EliminarAsync(string codigoUsuario, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task GuardarAsync(Usuario usuario, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task MarcarCorreoPendienteAsync(string codigoUsuario, string tokenHash, DateTimeOffset expiraUtc, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}