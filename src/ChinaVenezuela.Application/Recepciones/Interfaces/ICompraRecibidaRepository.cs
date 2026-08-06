using ChinaVenezuela.Domain.Recepciones;

namespace ChinaVenezuela.Application.Recepciones.Interfaces;

public interface ICompraRecibidaRepository
{
    Task<CompraRecibida?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CompraRecibida>> ObtenerTodasAsync(CancellationToken cancellationToken);
    Task AgregarAsync(CompraRecibida compraRecibida, CancellationToken cancellationToken);
    void Eliminar(CompraRecibida compraRecibida);
    Task GuardarCambiosAsync(CancellationToken cancellationToken);
}