using System.Text.Json.Serialization;
using ChinaVenezuela.Domain.Pedidos;

namespace ChinaVenezuela.Application.Pedidos.Contracts;

public sealed record CrearProductoPedidoRequest(Guid? PedidoId, string? NombreNuevoGrupo, string CodigoBarraAsignado, decimal? PrecioRmb, decimal? TotalRmb, int? CantidadDoz, string ReferenciaAsignada, string? TipoProducto, string? Agente, string? Fabrica, string? ComposicionTela, string? ColorParaFabricar, string? MarcaProducto, string? CurvaTalla, int? PackPorCaja, int? CantidadUnidades, string? MarcaBulto, int? CantidadBulto, DateOnly FechaRegistroPedido, DateOnly? FechaInicioFabricacion);
public sealed record ActualizarProductoPedidoRequest(Guid? PedidoId, string? NombreNuevoGrupo, string CodigoBarraAsignado, decimal? PrecioRmb, decimal? TotalRmb, int? CantidadDoz, string ReferenciaAsignada, string? TipoProducto, string? Agente, string? Fabrica, string? ComposicionTela, string? ColorParaFabricar, string? MarcaProducto, string? CurvaTalla, int? PackPorCaja, int? CantidadUnidades, string? MarcaBulto, int? CantidadBulto, DateOnly FechaRegistroPedido, DateOnly? FechaInicioFabricacion);
public sealed record DuplicarProductoPedidoRequest(string CodigoBarraAsignado);
public sealed record EnviarProductoPedidoRequest(string ReceptorCodigoUsuario);
public sealed record GuardarImagenProductoPedidoRequest(string ClaveAlmacenamiento, string NombreOriginal, string TipoContenido, long TamanoBytes);
public sealed record ProductoPedidoImagenResponse(Guid Id, Guid ProductoPedidoId, TipoImagenProductoPedido Tipo, [property: JsonIgnore] string ClaveAlmacenamiento, string NombreOriginal, string TipoContenido, long TamanoBytes, DateTimeOffset FechaCreacionUtc, DateTimeOffset? FechaActualizacionUtc);
public sealed record AgentePedidoResponse(Guid Id, string Nombre);
public sealed record CrearAgentePedidoRequest(string Nombre);
public sealed record ActualizarAgentePedidoRequest(string Nombre);
public sealed record PedidoResumenResponse(Guid Id, string Nombre, int CantidadPedidos);
public sealed record ProductoPedidoResponse(Guid Id, Guid? PedidoId, string? GrupoPedidoNombre, string CodigoBarraAsignado, decimal? PrecioRmb, decimal? TotalRmb, int? CantidadDoz, string ReferenciaAsignada, string? TipoProducto, string? Agente, string? Fabrica, string? ComposicionTela, string? ColorParaFabricar, string? MarcaProducto, string? CurvaTalla, int? PackPorCaja, int? CantidadUnidades, string? MarcaBulto, int? CantidadBulto, DateOnly FechaRegistroPedido, DateOnly? FechaInicioFabricacion, bool Activo, bool Enviado, DateTimeOffset? FechaEnvioUtc, bool TieneImagenFabrica, bool TieneImagenProductoTerminado, string CreadoPorCodigoUsuario, DateTimeOffset FechaCreacionUtc);
public sealed record RegistroPrecioPedidoResponse(Guid Id, string CodigoBarra, string Producto, string Sucursal, decimal PrecioSistema, decimal PrecioVerificado);
public sealed record PaginaProductosPedidoResponse(IReadOnlyList<ProductoPedidoResponse> Items, int Total, int Pagina, int TamanoPagina, int TotalPaginas);