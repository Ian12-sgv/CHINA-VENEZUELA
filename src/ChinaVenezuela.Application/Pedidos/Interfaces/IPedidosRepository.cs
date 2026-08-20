using ChinaVenezuela.Domain.Pedidos;
namespace ChinaVenezuela.Application.Pedidos.Interfaces;
public interface IPedidosRepository
{
    Task<(IReadOnlyList<ProductoPedido> Items, int Total)> ObtenerProductosAsync(string? busqueda, DateOnly? fechaPedido, bool? enviado, int pagina, int tamanoPagina, CancellationToken ct);
    Task<ProductoPedido?> ObtenerPorCodigoBarraAsync(string codigoBarra, CancellationToken ct);
    Task<ProductoPedido?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task AgregarProductoAsync(ProductoPedido producto, CancellationToken ct);
    void EliminarProducto(ProductoPedido producto);
    Task<ProductoPedidoImagen?> ObtenerImagenPorProductoIdAsync(Guid productoPedidoId, CancellationToken ct);
    Task AgregarImagenAsync(ProductoPedidoImagen imagen, CancellationToken ct);
    void EliminarImagen(ProductoPedidoImagen imagen);
    Task<IReadOnlyList<RegistroPrecioPedido>> ObtenerRegistrosPreciosAsync(string? busqueda, CancellationToken ct);
    Task GuardarCambiosAsync(CancellationToken ct);
}