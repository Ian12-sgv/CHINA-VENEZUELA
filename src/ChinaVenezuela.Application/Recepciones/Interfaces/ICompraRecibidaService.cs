using ChinaVenezuela.Application.Recepciones.Contracts;

namespace ChinaVenezuela.Application.Recepciones.Interfaces;

public interface ICompraRecibidaService
{
    Task<CompraRecibidaResponse> CrearAsync(string codigoRemitente, CrearCompraRecibidaRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<CompraRecibidaResponse>> ObtenerTodasAsync(CancellationToken cancellationToken);
    Task<CompraRecibidaResponse> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CompraRecibidaResponse> ActualizarAsync(string codigoRemitente, Guid id, ActualizarCompraRecibidaRequest request, CancellationToken cancellationToken);
    Task EliminarAsync(Guid id, CancellationToken cancellationToken);
    Task MarcarComprobanteEnviadoAsync(Guid id, CancellationToken cancellationToken);
}
