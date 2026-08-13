using ChinaVenezuela.Domain.Usuarios;

namespace ChinaVenezuela.Application.Grupos.Interfaces;

public interface IGrupoRepository
{
    Task<IReadOnlyList<Grupo>> ObtenerTodosAsync(CancellationToken cancellationToken);
    Task<bool> ExisteAsync(string nombre, CancellationToken cancellationToken);
    Task AgregarAsync(Grupo grupo, CancellationToken cancellationToken);
    Task RenombrarAsync(string nombreActual, string nombreNuevo, CancellationToken cancellationToken);
    Task EliminarAsync(string nombre, CancellationToken cancellationToken);
}