using ChinaVenezuela.Application.Pedidos.Contracts;
using ChinaVenezuela.Application.Pedidos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Domain.Pedidos;

namespace ChinaVenezuela.Application.Pedidos.Services;

public sealed class PedidosService(IPedidosRepository repository, TimeProvider timeProvider) : IPedidosService
{
    public async Task<PaginaProductosPedidoResponse> ObtenerProductosAsync(string? busqueda, bool? enviado, Guid? pedidoId, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var resultado = await repository.ObtenerProductosAsync(busqueda, enviado, pedidoId, pagina, tamanoPagina, ct);
        return new PaginaProductosPedidoResponse(resultado.Items.Select(x => Map(x)).ToArray(), resultado.Total, pagina, tamanoPagina, (int)Math.Ceiling(resultado.Total / (double)tamanoPagina));
    }

    public async Task<IReadOnlyList<AgentePedidoResponse>> ObtenerAgentesAsync(CancellationToken ct) => (await repository.ObtenerAgentesAsync(ct)).Select(x => new AgentePedidoResponse(x.Id, x.Nombre)).ToArray();
    public async Task<AgentePedidoResponse> CrearAgenteAsync(CrearAgentePedidoRequest request, CancellationToken ct)
    {
        var nombre = ValidarNombreAgente(request.Nombre);
        if (await repository.ObtenerAgentePorNombreAsync(nombre, ct) is not null) throw AgenteDuplicado();
        var agente = new AgentePedido(nombre);
        await repository.AgregarAgenteAsync(agente, ct);
        await repository.GuardarCambiosAsync(ct);
        return new AgentePedidoResponse(agente.Id, agente.Nombre);
    }

    public async Task<AgentePedidoResponse> ActualizarAgenteAsync(Guid id, ActualizarAgentePedidoRequest request, CancellationToken ct)
    {
        var agente = await repository.ObtenerAgentePorIdAsync(id, ct) ?? throw new RecursoNoEncontradoException("Agente", id);
        var nombre = ValidarNombreAgente(request.Nombre);
        var duplicado = await repository.ObtenerAgentePorNombreAsync(nombre, ct);
        if (duplicado is not null && duplicado.Id != id) throw AgenteDuplicado();
        agente.Actualizar(nombre);
        await repository.GuardarCambiosAsync(ct);
        return new AgentePedidoResponse(agente.Id, agente.Nombre);
    }

    public async Task EliminarAgenteAsync(Guid id, CancellationToken ct)
    {
        var agente = await repository.ObtenerAgentePorIdAsync(id, ct) ?? throw new RecursoNoEncontradoException("Agente", id);
        repository.EliminarAgente(agente);
        await repository.GuardarCambiosAsync(ct);
    }
    public async Task<IReadOnlyList<PedidoResumenResponse>> ObtenerPedidosAsync(CancellationToken ct) => (await repository.ObtenerPedidosAsync(ct)).Select(x => new PedidoResumenResponse(x.Id, x.Nombre, x.Detalles.Count)).ToArray();

    public async Task<ProductoPedidoResponse> CrearProductoAsync(string codigoUsuario, CrearProductoPedidoRequest request, CancellationToken ct)
    {
        Validar(request.CodigoBarraAsignado, request.ReferenciaAsignada, request.TipoProducto, request.PackPorCaja, request.CantidadUnidades, request.PrecioRmb, request.TotalRmb, request.CantidadDoz, request.MarcaBulto, request.CantidadBulto);
        var codigo = request.CodigoBarraAsignado.Trim();
        if (await repository.ObtenerPorCodigoBarraAsignadoAsync(codigo, ct) is not null) Duplicado();
        var producto = new ProductoPedido(codigo, request.PrecioRmb, request.TotalRmb, request.CantidadDoz, request.ReferenciaAsignada.Trim(), Limpiar(request.TipoProducto), Limpiar(request.Agente), Limpiar(request.Fabrica), Limpiar(request.ComposicionTela), Limpiar(request.ColorParaFabricar), Limpiar(request.MarcaProducto), Limpiar(request.CurvaTalla), request.PackPorCaja, request.CantidadUnidades, Limpiar(request.MarcaBulto), request.CantidadBulto, codigoUsuario, timeProvider.GetUtcNow());
        await repository.AgregarProductoAsync(producto, ct);
        var pedido = await ResolverPedidoAsync(request.PedidoId, request.NombreNuevoGrupo, codigoUsuario, ct);
        await repository.AgregarPedidoGrupoAsync(new PedidoGrupo(pedido.Id, producto.Id, timeProvider.GetUtcNow()), ct);
        await repository.GuardarCambiosAsync(ct);
        return Map(producto, pedido);
    }

    public async Task<ProductoPedidoResponse> ObtenerProductoAsync(Guid id, CancellationToken ct) => Map(await ObtenerEntidadAsync(id, ct));

    public async Task<ProductoPedidoResponse> ActualizarProductoAsync(Guid id, string codigoUsuario, ActualizarProductoPedidoRequest request, CancellationToken ct)
    {
        Validar(request.CodigoBarraAsignado, request.ReferenciaAsignada, request.TipoProducto, request.PackPorCaja, request.CantidadUnidades, request.PrecioRmb, request.TotalRmb, request.CantidadDoz, request.MarcaBulto, request.CantidadBulto);
        var producto = await ObtenerEntidadAsync(id, ct);
        if (producto.Enviado) Bloqueado();
        var codigo = request.CodigoBarraAsignado.Trim();
        var duplicado = await repository.ObtenerPorCodigoBarraAsignadoAsync(codigo, ct);
        if (duplicado is not null && duplicado.Id != id) Duplicado();
        producto.Actualizar(codigo, request.PrecioRmb, request.TotalRmb, request.CantidadDoz, request.ReferenciaAsignada.Trim(), Limpiar(request.TipoProducto), Limpiar(request.Agente), Limpiar(request.Fabrica), Limpiar(request.ComposicionTela), Limpiar(request.ColorParaFabricar), Limpiar(request.MarcaProducto), Limpiar(request.CurvaTalla), request.PackPorCaja, request.CantidadUnidades, Limpiar(request.MarcaBulto), request.CantidadBulto);
        var pedido = await ResolverPedidoAsync(request.PedidoId, request.NombreNuevoGrupo, codigoUsuario, ct);
        var detalle = await repository.ObtenerGrupoPorProductoIdAsync(id, ct);
        if (detalle is null) await repository.AgregarPedidoGrupoAsync(new PedidoGrupo(pedido.Id, id, timeProvider.GetUtcNow()), ct);
        else if (detalle.PedidoId != pedido.Id) detalle.CambiarPedido(pedido.Id);
        await repository.GuardarCambiosAsync(ct);
        return Map(producto, pedido);
    }

    public async Task EliminarProductoAsync(Guid id, CancellationToken ct)
    {
        var producto = await ObtenerEntidadAsync(id, ct);
        if (producto.Enviado) Bloqueado();
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

    public async Task<ProductoPedidoImagenResponse?> ObtenerImagenAsync(Guid productoPedidoId, TipoImagenProductoPedido tipo, CancellationToken ct) => Map(await repository.ObtenerImagenPorProductoIdAsync(productoPedidoId, tipo, ct));
    public async Task<IReadOnlyList<ProductoPedidoImagenResponse>> ObtenerImagenesAsync(Guid productoPedidoId, CancellationToken ct) => (await repository.ObtenerImagenesPorProductoIdAsync(productoPedidoId, ct)).Select(Map).OfType<ProductoPedidoImagenResponse>().ToArray();
    public async Task<ProductoPedidoImagenResponse> GuardarImagenAsync(Guid productoPedidoId, TipoImagenProductoPedido tipo, GuardarImagenProductoPedidoRequest request, CancellationToken ct)
    {
        var producto = await ObtenerEntidadAsync(productoPedidoId, ct);
        if (producto.Enviado) Bloqueado();
        var imagen = await repository.ObtenerImagenPorProductoIdAsync(productoPedidoId, tipo, ct);
        if (imagen is null) { imagen = new ProductoPedidoImagen(productoPedidoId, tipo, request.ClaveAlmacenamiento, request.NombreOriginal, request.TipoContenido, request.TamanoBytes, timeProvider.GetUtcNow()); await repository.AgregarImagenAsync(imagen, ct); }
        else imagen.Actualizar(request.ClaveAlmacenamiento, request.NombreOriginal, request.TipoContenido, request.TamanoBytes, timeProvider.GetUtcNow());
        await repository.GuardarCambiosAsync(ct);
        return Map(imagen)!;
    }
    public async Task<ProductoPedidoImagenResponse?> EliminarImagenAsync(Guid productoPedidoId, TipoImagenProductoPedido tipo, CancellationToken ct)
    {
        var producto = await ObtenerEntidadAsync(productoPedidoId, ct);
        if (producto.Enviado) Bloqueado();
        var imagen = await repository.ObtenerImagenPorProductoIdAsync(productoPedidoId, tipo, ct);
        if (imagen is null) return null;
        var respuesta = Map(imagen); repository.EliminarImagen(imagen); await repository.GuardarCambiosAsync(ct); return respuesta;
    }
    public async Task<IReadOnlyList<RegistroPrecioPedidoResponse>> ObtenerRegistrosPreciosAsync(string? busqueda, CancellationToken ct) => (await repository.ObtenerRegistrosPreciosAsync(busqueda, ct)).Select(x => new RegistroPrecioPedidoResponse(x.Id, x.CodigoBarra, x.Producto, x.Sucursal, x.PrecioSistema, x.PrecioVerificado)).ToArray();

    private async Task<Pedido> ResolverPedidoAsync(Guid? pedidoId, string? nombreNuevoGrupo, string codigoUsuario, CancellationToken ct)
    {
        if (pedidoId is not null && !string.IsNullOrWhiteSpace(nombreNuevoGrupo)) throw GrupoInvalido();
        if (pedidoId is not null) return await repository.ObtenerPedidoPorIdAsync(pedidoId.Value, ct) ?? throw new RecursoNoEncontradoException("Grupo de pedido", pedidoId.Value);
        if (string.IsNullOrWhiteSpace(nombreNuevoGrupo)) throw GrupoInvalido();
        var nombre = nombreNuevoGrupo.Trim();
        if ((await repository.ObtenerPedidosAsync(ct)).Any(x => string.Equals(x.Nombre, nombre, StringComparison.OrdinalIgnoreCase))) throw new ValidacionException(new Dictionary<string, string[]> { ["nombreNuevoGrupo"] = ["Ya existe un grupo con ese nombre. Selecciónalo como grupo existente."] });
        var pedido = new Pedido(nombre, codigoUsuario, timeProvider.GetUtcNow());
        await repository.AgregarPedidoAsync(pedido, ct);
        return pedido;
    }
    private async Task<ProductoPedido> ObtenerEntidadAsync(Guid id, CancellationToken ct) => await repository.ObtenerPorIdAsync(id, ct) ?? throw new RecursoNoEncontradoException("Producto", id);
    private static ValidacionException GrupoInvalido() => new(new Dictionary<string, string[]> { ["grupoPedido"] = ["Selecciona un grupo existente o escribe el nombre de un grupo nuevo."] });
    private static string ValidarNombreAgente(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ValidacionException(new Dictionary<string, string[]> { ["nombre"] = ["El nombre del agente es obligatorio."] });
        var limpio = nombre.Trim();
        if (limpio.Length > 150) throw new ValidacionException(new Dictionary<string, string[]> { ["nombre"] = ["El nombre del agente no puede superar 150 caracteres."] });
        return limpio;
    }
    private static ValidacionException AgenteDuplicado() => new(new Dictionary<string, string[]> { ["nombre"] = ["Ya existe un agente con ese nombre."] });
    private static void Bloqueado() => throw new ValidacionException(new Dictionary<string, string[]> { ["producto"] = ["Un pedido enviado no puede editarse ni eliminarse."] });
    private static void Duplicado() => throw new ValidacionException(new Dictionary<string, string[]> { ["codigoBarraAsignado"] = ["Ya existe un producto con este código de barra asignado."] });
    private static void Validar(string codigoBarraAsignado, string referenciaAsignada, string? tipoProducto, int? packPorCaja, int? cantidadUnidades, decimal? precioRmb, decimal? totalRmb, int? cantidadDoz, string? marcaBulto, int? cantidadBulto)
    {
        var errores = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(codigoBarraAsignado)) errores["codigoBarraAsignado"] = ["El código de barra asignado es obligatorio."];
        if (string.IsNullOrWhiteSpace(referenciaAsignada)) errores["referenciaAsignada"] = ["La referencia asignada es obligatoria."];
        if (!string.Equals(tipoProducto, "Nuevo", StringComparison.OrdinalIgnoreCase) && !string.Equals(tipoProducto, "Repetido", StringComparison.OrdinalIgnoreCase)) errores["tipoProducto"] = ["Selecciona Nuevo o Repetido."];
        if (packPorCaja is <= 0) errores["packPorCaja"] = ["El pack por caja debe ser mayor que cero."];
        if (cantidadUnidades is <= 0) errores["cantidadUnidades"] = ["La cantidad de unidades debe ser mayor que cero."];
        if (precioRmb is < 0) errores["precioRmb"] = ["El precio RMB no puede ser negativo."];
        if (totalRmb is < 0) errores["totalRmb"] = ["El total RMB no puede ser negativo."];
        if (cantidadDoz is <= 0) errores["cantidadDoz"] = ["La cantidad DOZ debe ser mayor que cero."];
        if (!string.IsNullOrWhiteSpace(marcaBulto) && cantidadBulto is not > 0) errores["cantidadBulto"] = ["Indica una cantidad de bultos mayor que cero para la marca seleccionada."];
        if (string.IsNullOrWhiteSpace(marcaBulto) && cantidadBulto.HasValue) errores["marcaBulto"] = ["Selecciona una marca de bulto para indicar su cantidad."];
        if (errores.Count > 0) throw new ValidacionException(errores);
    }
    private static string? Limpiar(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static ProductoPedidoResponse Map(ProductoPedido x, Pedido? pedido = null) => new(x.Id, pedido?.Id ?? x.GrupoPedido?.PedidoId, pedido?.Nombre ?? x.GrupoPedido?.Pedido.Nombre, x.CodigoBarraAsignado, x.PrecioRmb, x.TotalRmb, x.CantidadDoz, x.ReferenciaAsignada, x.TipoProducto, x.Agente, x.Fabrica, x.ComposicionTela, x.ColorParaFabricar, x.MarcaProducto, x.CurvaTalla, x.PackPorCaja, x.CantidadUnidades, x.MarcaBulto, x.CantidadBulto, x.Activo, x.Enviado, x.FechaEnvioUtc, x.Imagenes.Any(i => i.Tipo == TipoImagenProductoPedido.Fabrica), x.Imagenes.Any(i => i.Tipo == TipoImagenProductoPedido.ProductoTerminado), x.CreadoPorCodigoUsuario, x.FechaCreacionUtc);
    private static ProductoPedidoImagenResponse? Map(ProductoPedidoImagen? x) => x is null ? null : new(x.Id, x.ProductoPedidoId, x.Tipo, x.ClaveAlmacenamiento, x.NombreOriginal, x.TipoContenido, x.TamanoBytes, x.FechaCreacionUtc, x.FechaActualizacionUtc);
}