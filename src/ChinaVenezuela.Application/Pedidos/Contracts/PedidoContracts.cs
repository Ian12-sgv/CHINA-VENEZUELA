using System.Text.Json.Serialization;
namespace ChinaVenezuela.Application.Pedidos.Contracts;

public sealed record CrearProductoPedidoRequest(string CodigoBarra, string Referencia, string Nombre, string? Marca, string Categoria, string? Talla, string? Color, string? Fabricante, decimal PrecioDetal, decimal Costo, DateOnly FechaPedido);
public sealed record ActualizarProductoPedidoRequest(string CodigoBarra, string Referencia, string Nombre, string? Marca, string Categoria, string? Talla, string? Color, string? Fabricante, decimal PrecioDetal, decimal Costo, DateOnly FechaPedido);
public sealed record EnviarProductoPedidoRequest(string ReceptorCodigoUsuario);
public sealed record GuardarImagenProductoPedidoRequest(string ClaveAlmacenamiento, string NombreOriginal, string TipoContenido, long TamanoBytes);
public sealed record ProductoPedidoImagenResponse(Guid Id, Guid ProductoPedidoId, [property: JsonIgnore] string ClaveAlmacenamiento, string NombreOriginal, string TipoContenido, long TamanoBytes, DateTimeOffset FechaCreacionUtc, DateTimeOffset? FechaActualizacionUtc);
public sealed record ProductoPedidoResponse(Guid Id, string CodigoBarra, string Referencia, string Nombre, string? Marca, string Categoria, string? Talla, string? Color, string? Fabricante, decimal PrecioDetal, decimal Costo, DateOnly FechaPedido, bool Activo, bool Enviado, DateTimeOffset? FechaEnvioUtc, bool TieneImagen, string CreadoPorCodigoUsuario, DateTimeOffset FechaCreacionUtc);
public sealed record RegistroPrecioPedidoResponse(Guid Id, string CodigoBarra, string Producto, string Sucursal, decimal PrecioSistema, decimal PrecioVerificado);
public sealed record PaginaProductosPedidoResponse(IReadOnlyList<ProductoPedidoResponse> Items, int Total, int Pagina, int TamanoPagina, int TotalPaginas);