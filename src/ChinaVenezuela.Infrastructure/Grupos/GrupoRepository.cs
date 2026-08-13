using ChinaVenezuela.Application.Grupos.Interfaces;
using ChinaVenezuela.Domain.Usuarios;
using ChinaVenezuela.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChinaVenezuela.Infrastructure.Grupos;

public sealed class GrupoRepository(ChinaVenezuelaDbContext dbContext) : IGrupoRepository
{
    public async Task<IReadOnlyList<Grupo>> ObtenerTodosAsync(CancellationToken cancellationToken) =>
        await dbContext.Grupos.OrderBy(grupo => grupo.Nombre).ToArrayAsync(cancellationToken);

    public Task<bool> ExisteAsync(string nombre, CancellationToken cancellationToken) =>
        dbContext.Grupos.AnyAsync(grupo => grupo.Nombre.ToUpper() == nombre.ToUpper(), cancellationToken);

    public async Task AgregarAsync(Grupo grupo, CancellationToken cancellationToken)
    {
        await dbContext.Grupos.AddAsync(grupo, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RenombrarAsync(string nombreActual, string nombreNuevo, CancellationToken cancellationToken)
    {
        await dbContext.GruposUsuario.Where(grupo => grupo.NombreGrupo == nombreActual)
            .ExecuteUpdateAsync(setters => setters.SetProperty(grupo => grupo.NombreGrupo, nombreNuevo), cancellationToken);
        await dbContext.Grupos.Where(grupo => grupo.Nombre == nombreActual).ExecuteDeleteAsync(cancellationToken);
        await dbContext.Grupos.AddAsync(new Grupo(nombreNuevo), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task EliminarAsync(string nombre, CancellationToken cancellationToken)
    {
        await dbContext.GruposUsuario.Where(grupo => grupo.NombreGrupo == nombre).ExecuteDeleteAsync(cancellationToken);
        await dbContext.Grupos.Where(grupo => grupo.Nombre == nombre).ExecuteDeleteAsync(cancellationToken);
    }
}