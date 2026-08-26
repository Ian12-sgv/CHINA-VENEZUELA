using ChinaVenezuela.Domain.Pedidos;

namespace ChinaVenezuela.Application.Pedidos.Interfaces;

public interface IPedidosRepository
{
    Task<(IReadOnlyList<ProductoPedido> Items, int Total)> ObtenerProductosAsync(string? busqueda, bool? enviado, Guid? pedidoId, int pagina, int tamanoPagina, CancellationToken ct);
    Task<ProductoPedido?> ObtenerPorCodigoBarraAsignadoAsync(string codigoBarraAsignado, CancellationToken ct);
    Task<ProductoPedido?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<AgentePedido>> ObtenerAgentesAsync(CancellationToken ct);
    Task<AgentePedido?> ObtenerAgentePorIdAsync(Guid id, CancellationToken ct);
    Task<AgentePedido?> ObtenerAgentePorNombreAsync(string nombre, CancellationToken ct);
    Task<IReadOnlyList<Pedido>> ObtenerPedidosAsync(CancellationToken ct);
    Task<Pedido?> ObtenerPedidoPorIdAsync(Guid id, CancellationToken ct);
    Task<PedidoGrupo?> ObtenerGrupoPorProductoIdAsync(Guid productoPedidoId, CancellationToken ct);
    Task AgregarProductoAsync(ProductoPedido producto, CancellationToken ct);
    Task AgregarPedidoAsync(Pedido pedido, CancellationToken ct);
    Task AgregarPedidoGrupoAsync(PedidoGrupo pedidoGrupo, CancellationToken ct);
    Task AgregarAgenteAsync(AgentePedido agente, CancellationToken ct);
    void EliminarProducto(ProductoPedido producto);
    void EliminarAgente(AgentePedido agente);
    Task<ProductoPedidoImagen?> ObtenerImagenPorProductoIdAsync(Guid productoPedidoId, TipoImagenProductoPedido tipo, CancellationToken ct);
    Task<IReadOnlyList<ProductoPedidoImagen>> ObtenerImagenesPorProductoIdAsync(Guid productoPedidoId, CancellationToken ct);
    Task AgregarImagenAsync(ProductoPedidoImagen imagen, CancellationToken ct);
    void EliminarImagen(ProductoPedidoImagen imagen);
    Task<IReadOnlyList<RegistroPrecioPedido>> ObtenerRegistrosPreciosAsync(string? busqueda, CancellationToken ct);
    Task GuardarCambiosAsync(CancellationToken ct);
}