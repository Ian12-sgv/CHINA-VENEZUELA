using ChinaVenezuela.Application.Catalogos.Contracts;

namespace ChinaVenezuela.Application.Catalogos.Interfaces;

public interface ICatalogoService
{
    Task<IReadOnlyList<EmpresaResponse>> ObtenerEmpresasAsync(CancellationToken cancellationToken);
    Task<EmpresaResponse> CrearEmpresaAsync(CrearEmpresaRequest request, CancellationToken cancellationToken);
    Task<EmpresaResponse> ActualizarEmpresaAsync(Guid id, ActualizarEmpresaRequest request, CancellationToken cancellationToken);
    Task EliminarEmpresaAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CatalogoResponse>> ObtenerAduanasAsync(CancellationToken cancellationToken); Task<CatalogoResponse> CrearAduanaAsync(CrearCatalogoRequest request, CancellationToken cancellationToken); Task<CatalogoResponse> ActualizarAduanaAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken); Task EliminarAduanaAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CatalogoResponse>> ObtenerPuertosLlegadaAsync(CancellationToken cancellationToken); Task<CatalogoResponse> CrearPuertoLlegadaAsync(CrearCatalogoRequest request, CancellationToken cancellationToken); Task<CatalogoResponse> ActualizarPuertoLlegadaAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken); Task EliminarPuertoLlegadaAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CatalogoResponse>> ObtenerMarcasBultoAsync(CancellationToken cancellationToken);
    Task<CatalogoResponse> CrearMarcaBultoAsync(CrearCatalogoRequest request, CancellationToken cancellationToken);
    Task<CatalogoResponse> ActualizarMarcaBultoAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken);
    Task EliminarMarcaBultoAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CatalogoResponse>> ObtenerContenedoresCompartidosAsync(CancellationToken cancellationToken);
    Task<CatalogoResponse> CrearContenedorCompartidoAsync(CrearCatalogoRequest request, CancellationToken cancellationToken);
    Task<CatalogoResponse> ActualizarContenedorCompartidoAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken);
    Task EliminarContenedorCompartidoAsync(Guid id, CancellationToken cancellationToken);
}