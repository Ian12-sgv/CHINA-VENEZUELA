namespace ChinaVenezuela.Domain.Pedidos;

public sealed class ProductoPedido
{
    private ProductoPedido() { }
    public ProductoPedido(string codigoBarra, string referencia, string nombre, string? marca, string categoria, string? talla, string? color, string? fabricante, decimal precioDetal, decimal costo, DateOnly fechaPedido, string creadoPorCodigoUsuario, DateTimeOffset ahoraUtc)
    {
        Id = Guid.NewGuid(); CodigoBarra = codigoBarra; Referencia = referencia; Nombre = nombre; Marca = marca; Categoria = categoria; Talla = talla; Color = color; Fabricante = fabricante; PrecioDetal = precioDetal; Costo = costo; FechaPedido = fechaPedido; CreadoPorCodigoUsuario = creadoPorCodigoUsuario; FechaCreacionUtc = ahoraUtc; Activo = true;
    }
    public void Actualizar(string codigoBarra, string referencia, string nombre, string? marca, string categoria, string? talla, string? color, string? fabricante, decimal precioDetal, decimal costo, DateOnly fechaPedido)
    { CodigoBarra = codigoBarra; Referencia = referencia; Nombre = nombre; Marca = marca; Categoria = categoria; Talla = talla; Color = color; Fabricante = fabricante; PrecioDetal = precioDetal; Costo = costo; FechaPedido = fechaPedido; }
    public void MarcarComoEnviado(DateTimeOffset fechaEnvioUtc) { Enviado = true; FechaEnvioUtc = fechaEnvioUtc; }
    public Guid Id { get; private set; }
    public string CodigoBarra { get; private set; } = null!;
    public string Referencia { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string? Marca { get; private set; }
    public string Categoria { get; private set; } = null!;
    public string? Talla { get; private set; }
    public string? Color { get; private set; }
    public string? Fabricante { get; private set; }
    public decimal PrecioDetal { get; private set; }
    public decimal Costo { get; private set; }
    public DateOnly FechaPedido { get; private set; }
    public bool Activo { get; private set; }
    public bool Enviado { get; private set; }
    public DateTimeOffset? FechaEnvioUtc { get; private set; }
    public string CreadoPorCodigoUsuario { get; private set; } = null!;
    public DateTimeOffset FechaCreacionUtc { get; private set; }
    public ProductoPedidoImagen? Imagen { get; private set; }
}

public sealed class ProductoPedidoImagen
{
    private ProductoPedidoImagen() { }
    public ProductoPedidoImagen(Guid productoPedidoId, string claveAlmacenamiento, string nombreOriginal, string tipoContenido, long tamanoBytes, DateTimeOffset fechaCreacionUtc)
    { Id = Guid.NewGuid(); ProductoPedidoId = productoPedidoId; ClaveAlmacenamiento = claveAlmacenamiento; NombreOriginal = nombreOriginal; TipoContenido = tipoContenido; TamanoBytes = tamanoBytes; FechaCreacionUtc = fechaCreacionUtc; }
    public void Actualizar(string claveAlmacenamiento, string nombreOriginal, string tipoContenido, long tamanoBytes, DateTimeOffset fechaActualizacionUtc)
    { ClaveAlmacenamiento = claveAlmacenamiento; NombreOriginal = nombreOriginal; TipoContenido = tipoContenido; TamanoBytes = tamanoBytes; FechaActualizacionUtc = fechaActualizacionUtc; }
    public Guid Id { get; private set; }
    public Guid ProductoPedidoId { get; private set; }
    public string ClaveAlmacenamiento { get; private set; } = null!;
    public string NombreOriginal { get; private set; } = null!;
    public string TipoContenido { get; private set; } = null!;
    public long TamanoBytes { get; private set; }
    public DateTimeOffset FechaCreacionUtc { get; private set; }
    public DateTimeOffset? FechaActualizacionUtc { get; private set; }
}

public sealed class RegistroPrecioPedido
{
    private RegistroPrecioPedido() { }
    public Guid Id { get; private set; }
    public string CodigoBarra { get; private set; } = null!;
    public string Producto { get; private set; } = null!;
    public string Sucursal { get; private set; } = null!;
    public decimal PrecioSistema { get; private set; }
    public decimal PrecioVerificado { get; private set; }
}