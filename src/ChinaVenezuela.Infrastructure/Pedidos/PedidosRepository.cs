using ChinaVenezuela.Application.Pedidos.Interfaces;
using ChinaVenezuela.Domain.Pedidos;
using ChinaVenezuela.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChinaVenezuela.Infrastructure.Pedidos;

public sealed class PedidosRepository(ChinaVenezuelaDbContext context) : IPedidosRepository
{
    public async Task<(IReadOnlyList<ProductoPedido> Items, int Total)> ObtenerProductosAsync(string? busqueda, bool? enviado, Guid? pedidoId, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var query = context.ProductosPedido.AsNoTracking().Include(x => x.Imagenes).Include(x => x.GrupoPedido).ThenInclude(x => x!.Pedido).AsQueryable();
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var patron = $"%{busqueda.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(x.CodigoBarraAsignado, patron) || EF.Functions.ILike(x.ReferenciaAsignada, patron) || (x.TipoProducto != null && EF.Functions.ILike(x.TipoProducto, patron)) || (x.Agente != null && EF.Functions.ILike(x.Agente, patron)) || (x.Fabrica != null && EF.Functions.ILike(x.Fabrica, patron)) || (x.ComposicionTela != null && EF.Functions.ILike(x.ComposicionTela, patron)) || (x.ColorParaFabricar != null && EF.Functions.ILike(x.ColorParaFabricar, patron)) || (x.MarcaProducto != null && EF.Functions.ILike(x.MarcaProducto, patron)) || (x.CurvaTalla != null && EF.Functions.ILike(x.CurvaTalla, patron)) || (x.MarcaBulto != null && EF.Functions.ILike(x.MarcaBulto, patron)) || (x.GrupoPedido != null && EF.Functions.ILike(x.GrupoPedido.Pedido.Nombre, patron)));
        }
        if (enviado is not null) query = query.Where(x => x.Enviado == enviado.Value);
        if (pedidoId is not null) query = query.Where(x => x.GrupoPedido != null && x.GrupoPedido.PedidoId == pedidoId.Value);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.ReferenciaAsignada).Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina).ToListAsync(ct);
        return (items, total);
    }

    public Task<ProductoPedido?> ObtenerPorCodigoBarraAsignadoAsync(string codigoBarraAsignado, CancellationToken ct) => context.ProductosPedido.SingleOrDefaultAsync(x => x.CodigoBarraAsignado == codigoBarraAsignado, ct);
    public Task<ProductoPedido?> ObtenerPorIdAsync(Guid id, CancellationToken ct) => context.ProductosPedido.Include(x => x.Imagenes).Include(x => x.GrupoPedido).ThenInclude(x => x!.Pedido).SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<IReadOnlyList<AgentePedido>> ObtenerAgentesAsync(CancellationToken ct) => context.AgentesPedido.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync(ct).ContinueWith(x => (IReadOnlyList<AgentePedido>)x.Result, ct);
    public Task<AgentePedido?> ObtenerAgentePorIdAsync(Guid id, CancellationToken ct) => context.AgentesPedido.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<AgentePedido?> ObtenerAgentePorNombreAsync(string nombre, CancellationToken ct) => context.AgentesPedido.SingleOrDefaultAsync(x => EF.Functions.ILike(x.Nombre, nombre), ct);
    public Task<IReadOnlyList<Pedido>> ObtenerPedidosAsync(CancellationToken ct) => context.Pedidos.AsNoTracking().Include(x => x.Detalles).OrderByDescending(x => x.FechaCreacionUtc).ToListAsync(ct).ContinueWith(x => (IReadOnlyList<Pedido>)x.Result, ct);
    public Task<Pedido?> ObtenerPedidoPorIdAsync(Guid id, CancellationToken ct) => context.Pedidos.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<PedidoGrupo?> ObtenerGrupoPorProductoIdAsync(Guid productoPedidoId, CancellationToken ct) => context.PedidosGrupos.Include(x => x.Pedido).SingleOrDefaultAsync(x => x.ProductoPedidoId == productoPedidoId, ct);
    public Task AgregarProductoAsync(ProductoPedido producto, CancellationToken ct) => context.ProductosPedido.AddAsync(producto, ct).AsTask();
    public Task AgregarPedidoAsync(Pedido pedido, CancellationToken ct) => context.Pedidos.AddAsync(pedido, ct).AsTask();
    public Task AgregarPedidoGrupoAsync(PedidoGrupo pedidoGrupo, CancellationToken ct) => context.PedidosGrupos.AddAsync(pedidoGrupo, ct).AsTask();
    public Task AgregarAgenteAsync(AgentePedido agente, CancellationToken ct) => context.AgentesPedido.AddAsync(agente, ct).AsTask();
    public void EliminarProducto(ProductoPedido producto) => context.ProductosPedido.Remove(producto);
    public void EliminarAgente(AgentePedido agente) => context.AgentesPedido.Remove(agente);
    public Task<ProductoPedidoImagen?> ObtenerImagenPorProductoIdAsync(Guid productoPedidoId, TipoImagenProductoPedido tipo, CancellationToken ct) => context.ProductosPedidoImagenes.SingleOrDefaultAsync(x => x.ProductoPedidoId == productoPedidoId && x.Tipo == tipo, ct);
    public async Task<IReadOnlyList<ProductoPedidoImagen>> ObtenerImagenesPorProductoIdAsync(Guid productoPedidoId, CancellationToken ct) => await context.ProductosPedidoImagenes.Where(x => x.ProductoPedidoId == productoPedidoId).ToListAsync(ct);
    public Task AgregarImagenAsync(ProductoPedidoImagen imagen, CancellationToken ct) => context.ProductosPedidoImagenes.AddAsync(imagen, ct).AsTask();
    public void EliminarImagen(ProductoPedidoImagen imagen) => context.ProductosPedidoImagenes.Remove(imagen);
    public async Task<IReadOnlyList<RegistroPrecioPedido>> ObtenerRegistrosPreciosAsync(string? busqueda, CancellationToken ct)
    {
        var query = context.RegistrosPrecioPedido.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(busqueda)) { var patron = $"%{busqueda.Trim()}%"; query = query.Where(x => EF.Functions.ILike(x.CodigoBarra, patron) || EF.Functions.ILike(x.Producto, patron) || EF.Functions.ILike(x.Sucursal, patron)); }
        return await query.OrderBy(x => x.Producto).ToListAsync(ct);
    }
    public Task GuardarCambiosAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}