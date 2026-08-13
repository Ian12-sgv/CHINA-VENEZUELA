using ChinaVenezuela.Application.Usuarios.Interfaces;
using ChinaVenezuela.Domain.Usuarios;
using ChinaVenezuela.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChinaVenezuela.Infrastructure.Usuarios;

public sealed class UsuarioRepository(ChinaVenezuelaDbContext dbContext) : IUsuarioRepository
{
    public Task<bool> ExisteAsync(string codigoUsuario, CancellationToken cancellationToken) => dbContext.Usuarios.AnyAsync(usuario => usuario.CodigoUsuario == codigoUsuario, cancellationToken);
    public Task<bool> ExisteNombreAsync(string nombre, CancellationToken cancellationToken) => dbContext.Usuarios.AnyAsync(usuario => usuario.Nombre.ToUpper() == nombre.ToUpper(), cancellationToken);
    public Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken) => dbContext.Usuarios.AnyAsync(usuario => usuario.Correo != null && usuario.Correo.ToUpper() == correo.ToUpper(), cancellationToken);
    public Task<int> ContarPorNombreAsync(string nombre, CancellationToken cancellationToken) => dbContext.Usuarios.CountAsync(usuario => usuario.Nombre.ToUpper() == nombre.ToUpper(), cancellationToken);
    public Task<Usuario?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken) => dbContext.Usuarios.Include(usuario => usuario.Grupos).FirstOrDefaultAsync(usuario => usuario.Nombre.ToUpper() == nombre.ToUpper(), cancellationToken);
    public Task<Usuario?> ObtenerPorCodigoAsync(string codigoUsuario, CancellationToken cancellationToken) => dbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(usuario => usuario.CodigoUsuario == codigoUsuario, cancellationToken);
    public async Task<IReadOnlyList<Usuario>> ObtenerTodosAsync(CancellationToken cancellationToken) => await dbContext.Usuarios.Include(usuario => usuario.Grupos).OrderBy(usuario => usuario.Nombre).ToArrayAsync(cancellationToken);
    public async Task<IReadOnlyList<string>> ObtenerNombresGruposAsync(CancellationToken cancellationToken) => await dbContext.Grupos.Select(grupo => grupo.Nombre).Order().ToArrayAsync(cancellationToken);

    public async Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        await dbContext.Usuarios.AddAsync(usuario, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReemplazarGruposAsync(string codigoUsuario, IReadOnlyCollection<string> grupos, CancellationToken cancellationToken)
    {
        await dbContext.GruposUsuario.Where(grupo => grupo.CodigoUsuario == codigoUsuario).ExecuteDeleteAsync(cancellationToken);
        await dbContext.GruposUsuario.AddRangeAsync(grupos.Select(grupo => new GrupoUsuario(codigoUsuario, grupo)), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ActualizarAsync(string codigoUsuario, string nombre, string? correo, string contrasenaHash, bool status, IReadOnlyCollection<string> grupos, CancellationToken cancellationToken)
    {
        await dbContext.Usuarios.Where(usuario => usuario.CodigoUsuario == codigoUsuario).ExecuteUpdateAsync(setters => setters
            .SetProperty(usuario => usuario.Nombre, nombre)
            .SetProperty(usuario => usuario.Correo, correo)
            .SetProperty(usuario => usuario.ContrasenaHash, contrasenaHash)
            .SetProperty(usuario => usuario.Status, status), cancellationToken);
        await ReemplazarGruposAsync(codigoUsuario, grupos, cancellationToken);
    }

    public Task EliminarAsync(string codigoUsuario, CancellationToken cancellationToken) =>
        dbContext.Usuarios.Where(usuario => usuario.CodigoUsuario == codigoUsuario).ExecuteDeleteAsync(cancellationToken);
}



