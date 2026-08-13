using ChinaVenezuela.Application.Recepciones.Interfaces;
using ChinaVenezuela.Domain.Recepciones;
using ChinaVenezuela.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChinaVenezuela.Infrastructure.Recepciones;

public sealed class CompraRecibidaRepository(ChinaVenezuelaDbContext context) : ICompraRecibidaRepository
{
    public async Task<CompraRecibida?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) => await context.ComprasRecibidas.Include(x => x.Receptor).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<IReadOnlyList<CompraRecibida>> ObtenerTodasAsync(CancellationToken cancellationToken) => await context.ComprasRecibidas.AsNoTracking().Include(x => x.Receptor).OrderByDescending(x => x.FechaLlegada ?? x.FechaSalida).ToListAsync(cancellationToken);
    public Task AgregarAsync(CompraRecibida compraRecibida, CancellationToken cancellationToken) => context.ComprasRecibidas.AddAsync(compraRecibida, cancellationToken).AsTask();
    public void Eliminar(CompraRecibida compraRecibida) => context.ComprasRecibidas.Remove(compraRecibida);
    public Task GuardarCambiosAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
