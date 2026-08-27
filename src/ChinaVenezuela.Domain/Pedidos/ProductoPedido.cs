namespace ChinaVenezuela.Domain.Pedidos;

public enum TipoImagenProductoPedido
{
    Fabrica = 1,
    ProductoTerminado = 2
}

public sealed class ProductoPedido
{
    private ProductoPedido() { }

    public ProductoPedido(
        string codigoBarraAsignado,
        decimal? precioRmb,
        decimal? totalRmb,
        int? cantidadDoz,
        string referenciaAsignada,
        string? tipoProducto,
        string? agente,
        string? fabrica,
        string? composicionTela,
        string? colorParaFabricar,
        string? marcaProducto,
        string? curvaTalla,
        int? packPorCaja,
        int? cantidadUnidades,
        string? marcaBulto,
        int? cantidadBulto,
        DateOnly fechaRegistroPedido,
        DateOnly? fechaInicioFabricacion,
        string creadoPorCodigoUsuario,
        DateTimeOffset ahoraUtc)
    {
        Id = Guid.NewGuid();
        CodigoBarraAsignado = codigoBarraAsignado;
        PrecioRmb = precioRmb;
        TotalRmb = totalRmb;
        CantidadDoz = cantidadDoz;
        ReferenciaAsignada = referenciaAsignada;
        TipoProducto = tipoProducto;
        Agente = agente;
        Fabrica = fabrica;
        ComposicionTela = composicionTela;
        ColorParaFabricar = colorParaFabricar;
        MarcaProducto = marcaProducto;
        CurvaTalla = curvaTalla;
        PackPorCaja = packPorCaja;
        CantidadUnidades = cantidadUnidades;
        MarcaBulto = marcaBulto;
        CantidadBulto = cantidadBulto;
        FechaRegistroPedido = fechaRegistroPedido;
        FechaInicioFabricacion = fechaInicioFabricacion;
        CreadoPorCodigoUsuario = creadoPorCodigoUsuario;
        FechaCreacionUtc = ahoraUtc;
        Activo = true;
    }

    public void Actualizar(
        string codigoBarraAsignado,
        decimal? precioRmb,
        decimal? totalRmb,
        int? cantidadDoz,
        string referenciaAsignada,
        string? tipoProducto,
        string? agente,
        string? fabrica,
        string? composicionTela,
        string? colorParaFabricar,
        string? marcaProducto,
        string? curvaTalla,
        int? packPorCaja,
        int? cantidadUnidades,
        string? marcaBulto,
        int? cantidadBulto,
        DateOnly fechaRegistroPedido,
        DateOnly? fechaInicioFabricacion)
    {
        CodigoBarraAsignado = codigoBarraAsignado;
        PrecioRmb = precioRmb;
        TotalRmb = totalRmb;
        CantidadDoz = cantidadDoz;
        ReferenciaAsignada = referenciaAsignada;
        TipoProducto = tipoProducto;
        Agente = agente;
        Fabrica = fabrica;
        ComposicionTela = composicionTela;
        ColorParaFabricar = colorParaFabricar;
        MarcaProducto = marcaProducto;
        CurvaTalla = curvaTalla;
        PackPorCaja = packPorCaja;
        CantidadUnidades = cantidadUnidades;
        MarcaBulto = marcaBulto;
        CantidadBulto = cantidadBulto;
    FechaRegistroPedido = fechaRegistroPedido;
        FechaInicioFabricacion = fechaInicioFabricacion;
    }

    public void MarcarComoEnviado(DateTimeOffset fechaEnvioUtc) { Enviado = true; FechaEnvioUtc = fechaEnvioUtc; }

    public Guid Id { get; private set; }
    public string CodigoBarraAsignado { get; private set; } = null!;
    public decimal? PrecioRmb { get; private set; }
    public decimal? TotalRmb { get; private set; }
    public int? CantidadDoz { get; private set; }
    public string ReferenciaAsignada { get; private set; } = null!;
    public string? TipoProducto { get; private set; }
    public string? Agente { get; private set; }
    public string? Fabrica { get; private set; }
    public string? ComposicionTela { get; private set; }
    public string? ColorParaFabricar { get; private set; }
    public string? MarcaProducto { get; private set; }
    public string? CurvaTalla { get; private set; }
    public int? PackPorCaja { get; private set; }
    public int? CantidadUnidades { get; private set; }
    public string? MarcaBulto { get; private set; }
    public int? CantidadBulto { get; private set; }
    public DateOnly FechaRegistroPedido { get; private set; }
    public DateOnly? FechaInicioFabricacion { get; private set; }
    public bool Activo { get; private set; }
    public bool Enviado { get; private set; }
    public DateTimeOffset? FechaEnvioUtc { get; private set; }
    public string CreadoPorCodigoUsuario { get; private set; } = null!;
    public DateTimeOffset FechaCreacionUtc { get; private set; }
    public List<ProductoPedidoImagen> Imagenes { get; } = [];
    public PedidoGrupo? GrupoPedido { get; private set; }
}
public sealed class ProductoPedidoImagen
{
    private ProductoPedidoImagen() { }
    public ProductoPedidoImagen(Guid productoPedidoId, TipoImagenProductoPedido tipo, string claveAlmacenamiento, string nombreOriginal, string tipoContenido, long tamanoBytes, DateTimeOffset fechaCreacionUtc)
    { Id = Guid.NewGuid(); ProductoPedidoId = productoPedidoId; Tipo = tipo; ClaveAlmacenamiento = claveAlmacenamiento; NombreOriginal = nombreOriginal; TipoContenido = tipoContenido; TamanoBytes = tamanoBytes; FechaCreacionUtc = fechaCreacionUtc; }
    public void Actualizar(string claveAlmacenamiento, string nombreOriginal, string tipoContenido, long tamanoBytes, DateTimeOffset fechaActualizacionUtc)
    { ClaveAlmacenamiento = claveAlmacenamiento; NombreOriginal = nombreOriginal; TipoContenido = tipoContenido; TamanoBytes = tamanoBytes; FechaActualizacionUtc = fechaActualizacionUtc; }
    public Guid Id { get; private set; }
    public Guid ProductoPedidoId { get; private set; }
    public TipoImagenProductoPedido Tipo { get; private set; }
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
public sealed class AgentePedido
{
    private AgentePedido() { }
    public AgentePedido(string nombre) { Id = Guid.NewGuid(); Nombre = nombre; }
    public void Actualizar(string nombre) => Nombre = nombre;
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = null!;
}

public sealed class Pedido
{
    private Pedido() { }
    public Pedido(string nombre, string creadoPorCodigoUsuario, DateTimeOffset fechaCreacionUtc)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        CreadoPorCodigoUsuario = creadoPorCodigoUsuario;
        FechaCreacionUtc = fechaCreacionUtc;
    }
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string CreadoPorCodigoUsuario { get; private set; } = null!;
    public DateTimeOffset FechaCreacionUtc { get; private set; }
    public List<PedidoGrupo> Detalles { get; } = [];
}

public sealed class PedidoGrupo
{
    private PedidoGrupo() { }
    public PedidoGrupo(Guid pedidoId, Guid productoPedidoId, DateTimeOffset fechaCreacionUtc)
    {
        Id = Guid.NewGuid();
        PedidoId = pedidoId;
        ProductoPedidoId = productoPedidoId;
        FechaCreacionUtc = fechaCreacionUtc;
    }
    public void CambiarPedido(Guid pedidoId) => PedidoId = pedidoId;
    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public Guid ProductoPedidoId { get; private set; }
    public DateTimeOffset FechaCreacionUtc { get; private set; }
    public Pedido Pedido { get; private set; } = null!;
}