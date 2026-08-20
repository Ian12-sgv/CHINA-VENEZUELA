using ChinaVenezuela.Application.Pedidos.Contracts;
namespace ChinaVenezuela.Application.Pedidos.Interfaces;
public interface IPedidosService
{
    Task<PaginaProductosPedidoResponse> ObtenerProductosAsync(string? busqueda, DateOnly? fechaPedido, bool? enviado, int pagina, int tamanoPagina, CancellationToken ct);
    Task<ProductoPedidoResponse> CrearProductoAsync(string codigoUsuario, CrearProductoPedidoRequest request, CancellationToken ct);
    Task<ProductoPedidoResponse> ObtenerProductoAsync(Guid id, CancellationToken ct);
    Task<ProductoPedidoResponse> ActualizarProductoAsync(Guid id, ActualizarProductoPedidoRequest request, CancellationToken ct);
    Task EliminarProductoAsync(Guid id, CancellationToken ct);
    Task MarcarComoEnviadoAsync(Guid id, CancellationToken ct);
    Task<ProductoPedidoImagenResponse?> ObtenerImagenAsync(Guid productoPedidoId, CancellationToken ct);
    Task<ProductoPedidoImagenResponse> GuardarImagenAsync(Guid productoPedidoId, GuardarImagenProductoPedidoRequest request, CancellationToken ct);
    Task<ProductoPedidoImagenResponse?> EliminarImagenAsync(Guid productoPedidoId, CancellationToken ct);
    Task<IReadOnlyList<RegistroPrecioPedidoResponse>> ObtenerRegistrosPreciosAsync(string? busqueda, CancellationToken ct);
}