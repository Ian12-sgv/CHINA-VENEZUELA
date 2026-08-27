using ChinaVenezuela.Application.Pedidos.Contracts;
using ChinaVenezuela.Domain.Pedidos;

namespace ChinaVenezuela.Application.Pedidos.Interfaces;

public interface IPedidosService
{
    Task<PaginaProductosPedidoResponse> ObtenerProductosAsync(string? busqueda, bool? enviado, Guid? pedidoId, int pagina, int tamanoPagina, CancellationToken ct);
    Task<IReadOnlyList<AgentePedidoResponse>> ObtenerAgentesAsync(CancellationToken ct);
    Task<AgentePedidoResponse> CrearAgenteAsync(CrearAgentePedidoRequest request, CancellationToken ct);
    Task<AgentePedidoResponse> ActualizarAgenteAsync(Guid id, ActualizarAgentePedidoRequest request, CancellationToken ct);
    Task EliminarAgenteAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<PedidoResumenResponse>> ObtenerPedidosAsync(CancellationToken ct);
    Task<ProductoPedidoResponse> CrearProductoAsync(string codigoUsuario, CrearProductoPedidoRequest request, CancellationToken ct);
    Task<ProductoPedidoResponse> ObtenerProductoAsync(Guid id, CancellationToken ct);
    Task<ProductoPedidoResponse> ActualizarProductoAsync(Guid id, string codigoUsuario, ActualizarProductoPedidoRequest request, CancellationToken ct);
    Task<ProductoPedidoResponse> DuplicarProductoAsync(Guid id, string codigoUsuario, DuplicarProductoPedidoRequest request, CancellationToken ct);
    Task EliminarProductoAsync(Guid id, CancellationToken ct);
    Task MarcarComoEnviadoAsync(Guid id, CancellationToken ct);
    Task<ProductoPedidoImagenResponse?> ObtenerImagenAsync(Guid productoPedidoId, TipoImagenProductoPedido tipo, CancellationToken ct);
    Task<IReadOnlyList<ProductoPedidoImagenResponse>> ObtenerImagenesAsync(Guid productoPedidoId, CancellationToken ct);
    Task<ProductoPedidoImagenResponse> GuardarImagenAsync(Guid productoPedidoId, TipoImagenProductoPedido tipo, GuardarImagenProductoPedidoRequest request, CancellationToken ct);
    Task<ProductoPedidoImagenResponse?> EliminarImagenAsync(Guid productoPedidoId, TipoImagenProductoPedido tipo, CancellationToken ct);
    Task<IReadOnlyList<RegistroPrecioPedidoResponse>> ObtenerRegistrosPreciosAsync(string? busqueda, CancellationToken ct);
}