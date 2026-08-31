using ChinaVenezuela.Application.Pedidos.Contracts;
using ChinaVenezuela.Application.Pedidos.Interfaces;
using ChinaVenezuela.Application.Pedidos.Services;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Domain.Pedidos;

namespace ChinaVenezuela.Application.Tests;

public sealed class PedidosServiceTests
{
    [Fact]
    public async Task DuplicarProductoAsync_CreaSubpedidoPendienteEnElMismoGrupo()
    {
        var repository = new RepositorioPedidosEnMemoria();
        var fecha = new DateTimeOffset(2026, 8, 27, 12, 0, 0, TimeSpan.Zero);
        var service = new PedidosService(repository, new RelojFijo(fecha));

        var duplicado = await service.DuplicarProductoAsync(repository.ProductoOriginal.Id, "SIS", new DuplicarProductoPedidoRequest("CODIGO-NUEVO"), CancellationToken.None);

        Assert.Equal("CODIGO-NUEVO", duplicado.CodigoBarraAsignado);
        Assert.Equal(repository.ProductoOriginal.ReferenciaAsignada, duplicado.ReferenciaAsignada);
        Assert.Equal(repository.Pedido.Id, duplicado.PedidoId);
        Assert.False(duplicado.Enviado);
        Assert.False(duplicado.TieneImagenFabrica);
        Assert.False(duplicado.TieneImagenProductoTerminado);
        Assert.Equal(fecha, duplicado.FechaCreacionUtc);
        Assert.Equal(2, repository.Productos.Count);
        Assert.Equal(repository.Pedido.Id, repository.Detalles.Single(x => x.ProductoPedidoId == duplicado.Id).PedidoId);
        Assert.True(repository.Guardado);
    }

    [Fact]
    public async Task DuplicarProductoAsync_CuandoCodigoExiste_LanzaValidacion()
    {
        var repository = new RepositorioPedidosEnMemoria();
        var service = new PedidosService(repository, TimeProvider.System);

        var exception = await Assert.ThrowsAsync<ValidacionException>(() => service.DuplicarProductoAsync(repository.ProductoOriginal.Id, "SIS", new DuplicarProductoPedidoRequest(repository.ProductoOriginal.CodigoBarraAsignado), CancellationToken.None));

        Assert.Contains("codigoBarraAsignado", exception.Errores.Keys);
    }

    private sealed class RelojFijo(DateTimeOffset fecha) : TimeProvider { public override DateTimeOffset GetUtcNow() => fecha; }

    private sealed class RepositorioPedidosEnMemoria : IPedidosRepository
    {
        public Pedido Pedido { get; } = new("Pedido de prueba", "SIS", DateTimeOffset.UtcNow);
        public ProductoPedido ProductoOriginal { get; }
        public List<ProductoPedido> Productos { get; } = [];
        public List<PedidoGrupo> Detalles { get; } = [];
        public bool Guardado { get; private set; }

        public RepositorioPedidosEnMemoria()
        {
            ProductoOriginal = new ProductoPedido("CODIGO-ORIGINAL", 12m, 120m, 10, "REF-01", "Nuevo", "Agente", "Fabrica", "Algodon", "Azul", "Marca", "S-M-L", 12, 120, "Caja", 10, new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 5), "SIS", DateTimeOffset.UtcNow);
            Productos.Add(ProductoOriginal);
            Detalles.Add(new PedidoGrupo(Pedido.Id, ProductoOriginal.Id, DateTimeOffset.UtcNow));
        }

        public Task<(IReadOnlyList<ProductoPedido> Items, int Total)> ObtenerProductosAsync(string? busqueda, bool? enviado, Guid? pedidoId, int pagina, int tamanoPagina, CancellationToken ct) => Task.FromResult<(IReadOnlyList<ProductoPedido>, int)>((Productos, Productos.Count));
        public Task<ProductoPedido?> ObtenerPorCodigoBarraAsignadoAsync(string codigo, CancellationToken ct) => Task.FromResult<ProductoPedido?>(Productos.SingleOrDefault(x => x.CodigoBarraAsignado == codigo));
        public Task<ProductoPedido?> ObtenerPorIdAsync(Guid id, CancellationToken ct) => Task.FromResult<ProductoPedido?>(Productos.SingleOrDefault(x => x.Id == id));
        public Task<bool> ExistenMarcasBultoAsync(IReadOnlyList<Guid> marcaBultoIds, CancellationToken ct) => Task.FromResult(true);
        public Task<IReadOnlyList<AgentePedido>> ObtenerAgentesAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<AgentePedido>>([]);
        public Task<AgentePedido?> ObtenerAgentePorIdAsync(Guid id, CancellationToken ct) => Task.FromResult<AgentePedido?>(null);
        public Task<AgentePedido?> ObtenerAgentePorNombreAsync(string nombre, CancellationToken ct) => Task.FromResult<AgentePedido?>(null);
        public Task<IReadOnlyList<Pedido>> ObtenerPedidosAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<Pedido>>([Pedido]);
        public Task<Pedido?> ObtenerPedidoPorIdAsync(Guid id, CancellationToken ct) => Task.FromResult<Pedido?>(id == Pedido.Id ? Pedido : null);
        public Task<PedidoGrupo?> ObtenerGrupoPorProductoIdAsync(Guid productoId, CancellationToken ct) => Task.FromResult<PedidoGrupo?>(Detalles.SingleOrDefault(x => x.ProductoPedidoId == productoId));
        public Task AgregarProductoAsync(ProductoPedido producto, CancellationToken ct) { Productos.Add(producto); return Task.CompletedTask; }
        public Task AgregarBultoAsync(ProductoPedidoBulto bulto, CancellationToken ct) => Task.CompletedTask;
        public void EliminarBulto(ProductoPedidoBulto bulto) { }
        public Task AgregarPedidoAsync(Pedido pedido, CancellationToken ct) => Task.CompletedTask;
        public Task AgregarPedidoGrupoAsync(PedidoGrupo detalle, CancellationToken ct) { Detalles.Add(detalle); return Task.CompletedTask; }
        public Task AgregarAgenteAsync(AgentePedido agente, CancellationToken ct) => Task.CompletedTask;
        public void EliminarProducto(ProductoPedido producto) => Productos.Remove(producto);
        public void EliminarAgente(AgentePedido agente) { }
        public Task<ProductoPedidoImagen?> ObtenerImagenPorProductoIdAsync(Guid productoId, TipoImagenProductoPedido tipo, CancellationToken ct) => Task.FromResult<ProductoPedidoImagen?>(null);
        public Task<IReadOnlyList<ProductoPedidoImagen>> ObtenerImagenesPorProductoIdAsync(Guid productoId, CancellationToken ct) => Task.FromResult<IReadOnlyList<ProductoPedidoImagen>>([]);
        public Task AgregarImagenAsync(ProductoPedidoImagen imagen, CancellationToken ct) => Task.CompletedTask;
        public void EliminarImagen(ProductoPedidoImagen imagen) { }
        public Task<IReadOnlyList<RegistroPrecioPedido>> ObtenerRegistrosPreciosAsync(string? busqueda, CancellationToken ct) => Task.FromResult<IReadOnlyList<RegistroPrecioPedido>>([]);
        public Task GuardarCambiosAsync(CancellationToken ct) { Guardado = true; return Task.CompletedTask; }
    }
}
