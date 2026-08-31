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
        Validar(request.CodigoBarraAsignado, request.ReferenciaAsignada, request.TipoProducto, request.PackPorCaja, request.CantidadUnidades, request.PrecioRmb, request.TotalRmb, request.CantidadDoz, request.Bultos, request.FechaRegistroPedido, request.FechaInicioFabricacion);
        await ValidarMarcasBultoAsync(request.Bultos, ct);
        var codigo = request.CodigoBarraAsignado.Trim();
        if (await repository.ObtenerPorCodigoBarraAsignadoAsync(codigo, ct) is not null) Duplicado();
        var producto = new ProductoPedido(codigo, request.PrecioRmb, request.TotalRmb, request.CantidadDoz, request.ReferenciaAsignada.Trim(), Limpiar(request.TipoProducto), Limpiar(request.Agente), Limpiar(request.Fabrica), Limpiar(request.ComposicionTela), Limpiar(request.ColorParaFabricar), Limpiar(request.MarcaProducto), Limpiar(request.CurvaTalla), request.PackPorCaja, request.CantidadUnidades, null, null, request.FechaRegistroPedido, request.FechaInicioFabricacion, codigoUsuario, timeProvider.GetUtcNow());
        foreach (var bulto in request.Bultos ?? []) producto.Bultos.Add(new ProductoPedidoBulto(producto.Id, bulto.MarcaBultoId, bulto.Cantidad));
        await repository.AgregarProductoAsync(producto, ct);
        var pedido = await ResolverPedidoAsync(request.PedidoId, request.NombreNuevoGrupo, codigoUsuario, ct);
        await repository.AgregarPedidoGrupoAsync(new PedidoGrupo(pedido.Id, producto.Id, timeProvider.GetUtcNow()), ct);
        await repository.GuardarCambiosAsync(ct);
        return await ObtenerProductoAsync(producto.Id, ct);
    }

    public async Task<ProductoPedidoResponse> ObtenerProductoAsync(Guid id, CancellationToken ct) => Map(await ObtenerEntidadAsync(id, ct));

    public async Task<ProductoPedidoResponse> ActualizarProductoAsync(Guid id, string codigoUsuario, ActualizarProductoPedidoRequest request, CancellationToken ct)
    {
        Validar(request.CodigoBarraAsignado, request.ReferenciaAsignada, request.TipoProducto, request.PackPorCaja, request.CantidadUnidades, request.PrecioRmb, request.TotalRmb, request.CantidadDoz, request.Bultos, request.FechaRegistroPedido, request.FechaInicioFabricacion);
        await ValidarMarcasBultoAsync(request.Bultos, ct);
        var producto = await ObtenerEntidadAsync(id, ct);
        if (producto.Enviado) Bloqueado();
        var codigo = request.CodigoBarraAsignado.Trim();
        var duplicado = await repository.ObtenerPorCodigoBarraAsignadoAsync(codigo, ct);
        if (duplicado is not null && duplicado.Id != id) Duplicado();
        producto.Actualizar(codigo, request.PrecioRmb, request.TotalRmb, request.CantidadDoz, request.ReferenciaAsignada.Trim(), Limpiar(request.TipoProducto), Limpiar(request.Agente), Limpiar(request.Fabrica), Limpiar(request.ComposicionTela), Limpiar(request.ColorParaFabricar), Limpiar(request.MarcaProducto), Limpiar(request.CurvaTalla), request.PackPorCaja, request.CantidadUnidades, null, null, request.FechaRegistroPedido, request.FechaInicioFabricacion);
        var bultosSolicitados = request.Bultos ?? [];
        var marcasSolicitadas = bultosSolicitados.Select(x => x.MarcaBultoId).ToHashSet();
        foreach (var bultoActual in producto.Bultos.Where(x => !marcasSolicitadas.Contains(x.MarcaBultoId)).ToArray())
        {
            producto.Bultos.Remove(bultoActual);
            repository.EliminarBulto(bultoActual);
        }
        foreach (var bultoSolicitado in bultosSolicitados)
        {
            var bultoActual = producto.Bultos.SingleOrDefault(x => x.MarcaBultoId == bultoSolicitado.MarcaBultoId);
            if (bultoActual is not null) bultoActual.ActualizarCantidad(bultoSolicitado.Cantidad);
            else
            {
                var nuevoBulto = new ProductoPedidoBulto(producto.Id, bultoSolicitado.MarcaBultoId, bultoSolicitado.Cantidad);
                producto.Bultos.Add(nuevoBulto);
                await repository.AgregarBultoAsync(nuevoBulto, ct);
            }
        }
        var pedido = await ResolverPedidoAsync(request.PedidoId, request.NombreNuevoGrupo, codigoUsuario, ct);
        var detalle = await repository.ObtenerGrupoPorProductoIdAsync(id, ct);
        if (detalle is null) await repository.AgregarPedidoGrupoAsync(new PedidoGrupo(pedido.Id, id, timeProvider.GetUtcNow()), ct);
        else if (detalle.PedidoId != pedido.Id) detalle.CambiarPedido(pedido.Id);
        await repository.GuardarCambiosAsync(ct);
        return await ObtenerProductoAsync(producto.Id, ct);
    }

    public async Task<ProductoPedidoResponse> DuplicarProductoAsync(Guid id, string codigoUsuario, DuplicarProductoPedidoRequest request, CancellationToken ct)
    {
        var productoOriginal = await ObtenerEntidadAsync(id, ct);
        var detalle = await repository.ObtenerGrupoPorProductoIdAsync(id, ct);
        if (detalle is null) throw new ValidacionException(new Dictionary<string, string[]> { ["producto"] = ["Solo se pueden duplicar subpedidos que pertenezcan a un grupo."] });

        var codigo = ValidarCodigoBarra(request.CodigoBarraAsignado);
        if (await repository.ObtenerPorCodigoBarraAsignadoAsync(codigo, ct) is not null) Duplicado();

        var duplicado = new ProductoPedido(
            codigo,
            productoOriginal.PrecioRmb,
            productoOriginal.TotalRmb,
            productoOriginal.CantidadDoz,
            productoOriginal.ReferenciaAsignada,
            productoOriginal.TipoProducto,
            productoOriginal.Agente,
            productoOriginal.Fabrica,
            productoOriginal.ComposicionTela,
            productoOriginal.ColorParaFabricar,
            productoOriginal.MarcaProducto,
            productoOriginal.CurvaTalla,
            productoOriginal.PackPorCaja,
            productoOriginal.CantidadUnidades,
            null,
            null,
            productoOriginal.FechaRegistroPedido,
            productoOriginal.FechaInicioFabricacion,
            codigoUsuario,
            timeProvider.GetUtcNow());

        foreach (var bulto in productoOriginal.Bultos) duplicado.Bultos.Add(new ProductoPedidoBulto(duplicado.Id, bulto.MarcaBultoId, bulto.Cantidad));

        var pedido = await repository.ObtenerPedidoPorIdAsync(detalle.PedidoId, ct) ?? throw new RecursoNoEncontradoException("Grupo de pedido", detalle.PedidoId);

        await repository.AgregarProductoAsync(duplicado, ct);
        await repository.AgregarPedidoGrupoAsync(new PedidoGrupo(pedido.Id, duplicado.Id, timeProvider.GetUtcNow()), ct);
        await repository.GuardarCambiosAsync(ct);
        var respuesta = await ObtenerProductoAsync(duplicado.Id, ct);
        return respuesta with { PedidoId = pedido.Id, GrupoPedidoNombre = pedido.Nombre };
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

        var existentes = await repository.ObtenerPedidosAsync(ct);
        var correlativo = existentes
            .Select(ExtraerCorrelativo)
            .DefaultIfEmpty(0)
            .Max() + 1;
        var pedido = new Pedido(correlativo.ToString("D3"), codigoUsuario, timeProvider.GetUtcNow());
        await repository.AgregarPedidoAsync(pedido, ct);
        return pedido;
    }
    private static int ExtraerCorrelativo(Pedido pedido)
    {
        var valor = pedido.Nombre.Trim();
        const string prefijoAnterior = "Pedido ";
        if (valor.StartsWith(prefijoAnterior, StringComparison.OrdinalIgnoreCase)) valor = valor[prefijoAnterior.Length..].Trim();
        return int.TryParse(valor, out var correlativo) && correlativo > 0 ? correlativo : 0;
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
    private static string ValidarCodigoBarra(string? codigoBarraAsignado)
    {
        if (string.IsNullOrWhiteSpace(codigoBarraAsignado)) throw new ValidacionException(new Dictionary<string, string[]> { ["codigoBarraAsignado"] = ["El código de barra asignado es obligatorio."] });
        var codigo = codigoBarraAsignado.Trim();
        if (codigo.Length > 100) throw new ValidacionException(new Dictionary<string, string[]> { ["codigoBarraAsignado"] = ["El código de barra asignado no puede superar 100 caracteres."] });
        return codigo;
    }
    private async Task ValidarMarcasBultoAsync(IReadOnlyList<BultoProductoPedidoRequest>? bultos, CancellationToken ct)
    {
        if (bultos is not { Count: > 0 }) return;
        if (!await repository.ExistenMarcasBultoAsync(bultos.Select(x => x.MarcaBultoId).ToArray(), ct))
            throw new ValidacionException(new Dictionary<string, string[]> { ["bultos"] = ["Una o más marcas de bulto no existen."] });
    }
    private static void Validar(string codigoBarraAsignado, string referenciaAsignada, string? tipoProducto, int? packPorCaja, int? cantidadUnidades, decimal? precioRmb, decimal? totalRmb, int? cantidadDoz, IReadOnlyList<BultoProductoPedidoRequest>? bultos, DateOnly fechaRegistroPedido, DateOnly? fechaInicioFabricacion)
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
        if (bultos is null) errores["bultos"] = ["Indica las marcas de bulto del pedido."];
        else
        {
            if (bultos.Any(x => x.MarcaBultoId == Guid.Empty)) errores["bultos"] = ["Selecciona una marca de bulto valida."];
            if (bultos.Any(x => x.Cantidad <= 0)) errores["bultos"] = ["La cantidad de cada marca de bulto debe ser mayor que cero."];
            if (bultos.GroupBy(x => x.MarcaBultoId).Any(x => x.Count() > 1)) errores["bultos"] = ["No repitas una marca de bulto en el mismo pedido."];
        }
        if (fechaRegistroPedido == default) errores["fechaRegistroPedido"] = ["La fecha de registro del pedido es obligatoria."];
        if (fechaInicioFabricacion is not null && fechaInicioFabricacion < fechaRegistroPedido) errores["fechaInicioFabricacion"] = ["La fecha de inicio de fabricación no puede ser anterior al registro del pedido."];
        if (errores.Count > 0) throw new ValidacionException(errores);
    }
    private static string? Limpiar(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static ProductoPedidoResponse Map(ProductoPedido x, Pedido? pedido = null) => new(x.Id, pedido?.Id ?? x.GrupoPedido?.PedidoId, pedido?.Nombre ?? x.GrupoPedido?.Pedido.Nombre, x.CodigoBarraAsignado, x.PrecioRmb, x.TotalRmb, x.CantidadDoz, x.ReferenciaAsignada, x.TipoProducto, x.Agente, x.Fabrica, x.ComposicionTela, x.ColorParaFabricar, x.MarcaProducto, x.CurvaTalla, x.PackPorCaja, x.CantidadUnidades, x.MarcaBulto, x.CantidadBulto, x.Bultos.OrderBy(bulto => bulto.MarcaBulto.Nombre).Select(bulto => new BultoProductoPedidoResponse(bulto.MarcaBultoId, bulto.MarcaBulto.Nombre, bulto.Cantidad)).ToArray(), x.FechaRegistroPedido, x.FechaInicioFabricacion, x.Activo, x.Enviado, x.FechaEnvioUtc, x.Imagenes.Any(i => i.Tipo == TipoImagenProductoPedido.Fabrica), x.Imagenes.Any(i => i.Tipo == TipoImagenProductoPedido.ProductoTerminado), x.CreadoPorCodigoUsuario, x.FechaCreacionUtc);
    private static ProductoPedidoImagenResponse? Map(ProductoPedidoImagen? x) => x is null ? null : new(x.Id, x.ProductoPedidoId, x.Tipo, x.ClaveAlmacenamiento, x.NombreOriginal, x.TipoContenido, x.TamanoBytes, x.FechaCreacionUtc, x.FechaActualizacionUtc);
}