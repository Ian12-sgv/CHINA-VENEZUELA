using ChinaVenezuela.Application.Pedidos.Contracts;
using ChinaVenezuela.Application.Pedidos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Domain.Pedidos;

namespace ChinaVenezuela.Application.Pedidos.Services;

public sealed class PedidosService(IPedidosRepository repository, TimeProvider timeProvider) : IPedidosService
{
    public async Task<PaginaProductosPedidoResponse> ObtenerProductosAsync(string? busqueda, DateOnly? fechaPedido, bool? enviado, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var resultado = await repository.ObtenerProductosAsync(busqueda, fechaPedido, enviado, pagina, tamanoPagina, ct);
        return new PaginaProductosPedidoResponse(resultado.Items.Select(Map).ToArray(), resultado.Total, pagina, tamanoPagina, (int)Math.Ceiling(resultado.Total / (double)tamanoPagina));
    }

    public async Task<ProductoPedidoResponse> CrearProductoAsync(string codigoUsuario, CrearProductoPedidoRequest request, CancellationToken ct)
    {
        Validar(request.CodigoBarra, request.Referencia, request.Nombre, request.Categoria, request.PrecioDetal, request.Costo);
        var codigo = request.CodigoBarra.Trim();
        if (await repository.ObtenerPorCodigoBarraAsync(codigo, ct) is not null) Duplicado();
        var producto = new ProductoPedido(codigo, request.Referencia.Trim(), request.Nombre.Trim(), Limpiar(request.Marca), request.Categoria.Trim(), Limpiar(request.Talla), Limpiar(request.Color), Limpiar(request.Fabricante), request.PrecioDetal, request.Costo, request.FechaPedido, codigoUsuario, timeProvider.GetUtcNow());
        await repository.AgregarProductoAsync(producto, ct);
        await repository.GuardarCambiosAsync(ct);
        return Map(producto);
    }

    public async Task<ProductoPedidoResponse> ObtenerProductoAsync(Guid id, CancellationToken ct) => Map(await ObtenerEntidadAsync(id, ct));

    public async Task<ProductoPedidoResponse> ActualizarProductoAsync(Guid id, ActualizarProductoPedidoRequest request, CancellationToken ct)
    {
        Validar(request.CodigoBarra, request.Referencia, request.Nombre, request.Categoria, request.PrecioDetal, request.Costo);
        var producto = await ObtenerEntidadAsync(id, ct);
        if (producto.Enviado) Bloqueado();
        var codigo = request.CodigoBarra.Trim();
        var duplicado = await repository.ObtenerPorCodigoBarraAsync(codigo, ct);
        if (duplicado is not null && duplicado.Id != id) Duplicado();
        producto.Actualizar(codigo, request.Referencia.Trim(), request.Nombre.Trim(), Limpiar(request.Marca), request.Categoria.Trim(), Limpiar(request.Talla), Limpiar(request.Color), Limpiar(request.Fabricante), request.PrecioDetal, request.Costo, request.FechaPedido);
        await repository.GuardarCambiosAsync(ct);
        return Map(producto);
    }

    public async Task EliminarProductoAsync(Guid id, CancellationToken ct)
    {
        var producto = await ObtenerEntidadAsync(id, ct);
        repository.EliminarProducto(producto);
        await repository.GuardarCambiosAsync(ct);
    }

    public async Task MarcarComoEnviadoAsync(Guid id, CancellationToken ct)
    {
        var producto = await ObtenerEntidadAsync(id, ct);
        if (producto.Enviado) throw new ValidacionException(new Dictionary<string, string[]> { ["producto"] = ["Este pedido ya fue enviado."] });
        producto.MarcarComoEnviado(timeProvider.GetUtcNow());
        await repository.GuardarCambiosAsync(ct);
    }

    public async Task<ProductoPedidoImagenResponse?> ObtenerImagenAsync(Guid productoPedidoId, CancellationToken ct) => Map(await repository.ObtenerImagenPorProductoIdAsync(productoPedidoId, ct));

    public async Task<ProductoPedidoImagenResponse> GuardarImagenAsync(Guid productoPedidoId, GuardarImagenProductoPedidoRequest request, CancellationToken ct)
    {
        var producto = await ObtenerEntidadAsync(productoPedidoId, ct);
        if (producto.Enviado) Bloqueado();
        var imagen = await repository.ObtenerImagenPorProductoIdAsync(productoPedidoId, ct);
        if (imagen is null)
        {
            imagen = new ProductoPedidoImagen(productoPedidoId, request.ClaveAlmacenamiento, request.NombreOriginal, request.TipoContenido, request.TamanoBytes, timeProvider.GetUtcNow());
            await repository.AgregarImagenAsync(imagen, ct);
        }
        else
            imagen.Actualizar(request.ClaveAlmacenamiento, request.NombreOriginal, request.TipoContenido, request.TamanoBytes, timeProvider.GetUtcNow());
        await repository.GuardarCambiosAsync(ct);
        return Map(imagen)!;
    }

    public async Task<ProductoPedidoImagenResponse?> EliminarImagenAsync(Guid productoPedidoId, CancellationToken ct)
    {
        var producto = await ObtenerEntidadAsync(productoPedidoId, ct);
        if (producto.Enviado) Bloqueado();
        var imagen = await repository.ObtenerImagenPorProductoIdAsync(productoPedidoId, ct);
        if (imagen is null) return null;
        var respuesta = Map(imagen);
        repository.EliminarImagen(imagen);
        await repository.GuardarCambiosAsync(ct);
        return respuesta;
    }

    public async Task<IReadOnlyList<RegistroPrecioPedidoResponse>> ObtenerRegistrosPreciosAsync(string? busqueda, CancellationToken ct) => (await repository.ObtenerRegistrosPreciosAsync(busqueda, ct)).Select(x => new RegistroPrecioPedidoResponse(x.Id, x.CodigoBarra, x.Producto, x.Sucursal, x.PrecioSistema, x.PrecioVerificado)).ToArray();

    private async Task<ProductoPedido> ObtenerEntidadAsync(Guid id, CancellationToken ct) => await repository.ObtenerPorIdAsync(id, ct) ?? throw new RecursoNoEncontradoException("Producto", id);
    private static void Bloqueado() => throw new ValidacionException(new Dictionary<string, string[]> { ["producto"] = ["Un pedido enviado no puede editarse."] });
    private static void Duplicado() => throw new ValidacionException(new Dictionary<string, string[]> { ["codigoBarra"] = ["Ya existe un producto con este código de barra."] });
    private static void Validar(string codigoBarra, string referencia, string nombre, string categoria, decimal precioDetal, decimal costo)
    {
        var errores = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(codigoBarra)) errores["codigoBarra"] = ["El código de barra es obligatorio."];
        if (string.IsNullOrWhiteSpace(referencia)) errores["referencia"] = ["La referencia es obligatoria."];
        if (string.IsNullOrWhiteSpace(nombre)) errores["nombre"] = ["El nombre es obligatorio."];
        if (string.IsNullOrWhiteSpace(categoria)) errores["categoria"] = ["La categoría es obligatoria."];
        if (precioDetal < 0 || costo < 0) errores["precios"] = ["Los precios no pueden ser negativos."];
        if (errores.Count > 0) throw new ValidacionException(errores);
    }
    private static string? Limpiar(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static ProductoPedidoResponse Map(ProductoPedido x) => new(x.Id, x.CodigoBarra, x.Referencia, x.Nombre, x.Marca, x.Categoria, x.Talla, x.Color, x.Fabricante, x.PrecioDetal, x.Costo, x.FechaPedido, x.Activo, x.Enviado, x.FechaEnvioUtc, x.Imagen is not null, x.CreadoPorCodigoUsuario, x.FechaCreacionUtc);
    private static ProductoPedidoImagenResponse? Map(ProductoPedidoImagen? x) => x is null ? null : new(x.Id, x.ProductoPedidoId, x.ClaveAlmacenamiento, x.NombreOriginal, x.TipoContenido, x.TamanoBytes, x.FechaCreacionUtc, x.FechaActualizacionUtc);
}