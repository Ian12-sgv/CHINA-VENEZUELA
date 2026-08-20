using ChinaVenezuela.Application.Pedidos.Interfaces;
using ChinaVenezuela.Domain.Pedidos;
using ChinaVenezuela.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChinaVenezuela.Infrastructure.Pedidos;

public sealed class PedidosRepository(ChinaVenezuelaDbContext context) : IPedidosRepository
{
    public async Task<(IReadOnlyList<ProductoPedido> Items, int Total)> ObtenerProductosAsync(string? busqueda, DateOnly? fechaPedido, bool? enviado, int pagina, int tamanoPagina, CancellationToken ct)
    {
        IQueryable<ProductoPedido> query = context.ProductosPedido.AsNoTracking().Include(x => x.Imagen);
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var patron = $"%{busqueda.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(x.CodigoBarra, patron) || EF.Functions.ILike(x.Referencia, patron) || EF.Functions.ILike(x.Nombre, patron) || (x.Marca != null && EF.Functions.ILike(x.Marca, patron)) || EF.Functions.ILike(x.Categoria, patron));
        }
        if (fechaPedido is not null) query = query.Where(x => x.FechaPedido == fechaPedido.Value);
        if (enviado is not null) query = query.Where(x => x.Enviado == enviado.Value);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Nombre).Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina).ToListAsync(ct);
        return (items, total);
    }

    public Task<ProductoPedido?> ObtenerPorCodigoBarraAsync(string codigoBarra, CancellationToken ct) => context.ProductosPedido.SingleOrDefaultAsync(x => x.CodigoBarra == codigoBarra, ct);
    public Task<ProductoPedido?> ObtenerPorIdAsync(Guid id, CancellationToken ct) => context.ProductosPedido.Include(x => x.Imagen).SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task AgregarProductoAsync(ProductoPedido producto, CancellationToken ct) => context.ProductosPedido.AddAsync(producto, ct).AsTask();
    public void EliminarProducto(ProductoPedido producto) => context.ProductosPedido.Remove(producto);
    public Task<ProductoPedidoImagen?> ObtenerImagenPorProductoIdAsync(Guid productoPedidoId, CancellationToken ct) => context.ProductosPedidoImagenes.SingleOrDefaultAsync(x => x.ProductoPedidoId == productoPedidoId, ct);
    public Task AgregarImagenAsync(ProductoPedidoImagen imagen, CancellationToken ct) => context.ProductosPedidoImagenes.AddAsync(imagen, ct).AsTask();
    public void EliminarImagen(ProductoPedidoImagen imagen) => context.ProductosPedidoImagenes.Remove(imagen);
    public async Task<IReadOnlyList<RegistroPrecioPedido>> ObtenerRegistrosPreciosAsync(string? busqueda, CancellationToken ct)
    {
        var query = context.RegistrosPrecioPedido.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var patron = $"%{busqueda.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(x.CodigoBarra, patron) || EF.Functions.ILike(x.Producto, patron) || EF.Functions.ILike(x.Sucursal, patron));
        }
        return await query.OrderBy(x => x.Producto).ToListAsync(ct);
    }
    public Task GuardarCambiosAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}