namespace ChinaVenezuela.Domain.FichasTecnicas;

public sealed class FichaTecnica
{
    private FichaTecnica() { }
    public FichaTecnica(string codigo, string referencia, string categoria, string linea, string estado, string? composicionTela, string? colorParaFabricar, string? marcaProducto, string? curvaTalla)
    { Id = Guid.NewGuid(); Actualizar(codigo, referencia, categoria, linea, estado, composicionTela, colorParaFabricar, marcaProducto, curvaTalla); }
    public void Actualizar(string codigo, string referencia, string categoria, string linea, string estado, string? composicionTela, string? colorParaFabricar, string? marcaProducto, string? curvaTalla)
    { Codigo = codigo; Referencia = referencia; Categoria = categoria; Linea = linea; Estado = estado; ComposicionTela = composicionTela; ColorParaFabricar = colorParaFabricar; MarcaProducto = marcaProducto; CurvaTalla = curvaTalla; }
    public void ActualizarImagen(byte[] datos, string tipoContenido) { ImagenDatos = datos; ImagenTipoContenido = tipoContenido; }
    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = null!;
    public string Referencia { get; private set; } = null!;
    public string Categoria { get; private set; } = null!;
    public string Linea { get; private set; } = null!;
    public string Estado { get; private set; } = null!;
    public string? ComposicionTela { get; private set; }
    public string? ColorParaFabricar { get; private set; }
    public string? MarcaProducto { get; private set; }
    public string? CurvaTalla { get; private set; }
    public byte[]? ImagenDatos { get; private set; }
    public string? ImagenTipoContenido { get; private set; }
    public List<AtributoFichaTecnica> Atributos { get; } = [];
}

public sealed class AtributoFichaTecnica
{
    private AtributoFichaTecnica() { }
    public AtributoFichaTecnica(Guid fichaTecnicaId, string atributo, string valor, string? observacion, string? composicionTela, string? colorParaFabricar, string? marcaProducto, string? curvaTalla)
    { Id = Guid.NewGuid(); FichaTecnicaId = fichaTecnicaId; Actualizar(atributo, valor, observacion, composicionTela, colorParaFabricar, marcaProducto, curvaTalla); }
    public void Actualizar(string atributo, string valor, string? observacion, string? composicionTela, string? colorParaFabricar, string? marcaProducto, string? curvaTalla)
    { Atributo = atributo; Valor = valor; Observacion = observacion; ComposicionTela = composicionTela; ColorParaFabricar = colorParaFabricar; MarcaProducto = marcaProducto; CurvaTalla = curvaTalla; }
    public Guid Id { get; private set; }
    public Guid FichaTecnicaId { get; private set; }
    public string Atributo { get; private set; } = null!;
    public string Valor { get; private set; } = null!;
    public string? Observacion { get; private set; }
    public string? ComposicionTela { get; private set; }
    public string? ColorParaFabricar { get; private set; }
    public string? MarcaProducto { get; private set; }
    public string? CurvaTalla { get; private set; }
    public FichaTecnica FichaTecnica { get; private set; } = null!;
}