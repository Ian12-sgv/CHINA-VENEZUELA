using ChinaVenezuela.Application.Grupos.Contracts;

namespace ChinaVenezuela.Application.Grupos.Interfaces;

public interface IGrupoService
{
    Task<IReadOnlyList<GrupoResponse>> ObtenerTodosAsync(CancellationToken cancellationToken);
    Task<GrupoResponse> CrearAsync(CrearGrupoRequest request, CancellationToken cancellationToken);
    Task<GrupoResponse> ActualizarAsync(string nombreActual, ActualizarGrupoRequest request, CancellationToken cancellationToken);
    Task EliminarAsync(string nombre, CancellationToken cancellationToken);
}