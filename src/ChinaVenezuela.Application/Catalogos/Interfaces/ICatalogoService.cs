using ChinaVenezuela.Application.Catalogos.Contracts;

namespace ChinaVenezuela.Application.Catalogos.Interfaces;

public interface ICatalogoService
{
    Task<IReadOnlyList<CatalogoResponse>> ObtenerEmpresasAsync(CancellationToken cancellationToken);
    Task<CatalogoResponse> CrearEmpresaAsync(CrearCatalogoRequest request, CancellationToken cancellationToken);
    Task<CatalogoResponse> ActualizarEmpresaAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken);
    Task EliminarEmpresaAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CatalogoResponse>> ObtenerMarcasBultoAsync(CancellationToken cancellationToken);
    Task<CatalogoResponse> CrearMarcaBultoAsync(CrearCatalogoRequest request, CancellationToken cancellationToken);
    Task<CatalogoResponse> ActualizarMarcaBultoAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken);
    Task EliminarMarcaBultoAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CatalogoResponse>> ObtenerContenedoresCompartidosAsync(CancellationToken cancellationToken);
    Task<CatalogoResponse> CrearContenedorCompartidoAsync(CrearCatalogoRequest request, CancellationToken cancellationToken);
    Task<CatalogoResponse> ActualizarContenedorCompartidoAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken);
    Task EliminarContenedorCompartidoAsync(Guid id, CancellationToken cancellationToken);
}