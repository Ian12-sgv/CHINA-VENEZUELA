using ChinaVenezuela.Domain.Catalogos;

namespace ChinaVenezuela.Domain.Recepciones;

public sealed class CompraRecibida
{
    private CompraRecibida() { }

    public CompraRecibida(Guid? contenedorCompartidoId, string nombreContenedor, string numeroContenedor, Guid empresaId, string? descripcion, DateOnly fechaSalida, DateOnly? fechaLlegada, string? aduana, string puertoLlegada, Guid? marcaBultoId, DateTimeOffset fechaCreacionUtc)
    {
        Id = Guid.NewGuid();
        ContenedorCompartidoId = contenedorCompartidoId;
        NombreContenedor = nombreContenedor;
        NumeroContenedor = numeroContenedor;
        EmpresaId = empresaId;
        Descripcion = descripcion;
        FechaSalida = fechaSalida;
        FechaLlegada = fechaLlegada;
        Aduana = aduana;
        PuertoLlegada = puertoLlegada;
        MarcaBultoId = marcaBultoId;
        FechaCreacionUtc = fechaCreacionUtc;
    }

    public Guid Id { get; private set; }
    public Guid? ContenedorCompartidoId { get; private set; }
    public ContenedorCompartido? ContenedorCompartido { get; private set; }
    public string NombreContenedor { get; private set; } = null!;
    public string NumeroContenedor { get; private set; } = null!;
    public Guid EmpresaId { get; private set; }
    public Empresa Empresa { get; private set; } = null!;
    public string? Descripcion { get; private set; }
    public DateOnly FechaSalida { get; private set; }
    public DateOnly? FechaLlegada { get; private set; }
    public string? Aduana { get; private set; }
    public string PuertoLlegada { get; private set; } = null!;
    public Guid? MarcaBultoId { get; private set; }
    public MarcaBulto? MarcaBulto { get; private set; }
    public DateTimeOffset FechaCreacionUtc { get; private set; }
    public DateTimeOffset? FechaActualizacionUtc { get; private set; }

    public void Actualizar(Guid? contenedorCompartidoId, string nombreContenedor, string numeroContenedor, Guid empresaId, string? descripcion, DateOnly fechaSalida, DateOnly? fechaLlegada, string? aduana, string puertoLlegada, Guid? marcaBultoId, DateTimeOffset fechaActualizacionUtc)
    {
        ContenedorCompartidoId = contenedorCompartidoId;
        NombreContenedor = nombreContenedor;
        NumeroContenedor = numeroContenedor;
        EmpresaId = empresaId;
        Descripcion = descripcion;
        FechaSalida = fechaSalida;
        FechaLlegada = fechaLlegada;
        Aduana = aduana;
        PuertoLlegada = puertoLlegada;
        MarcaBultoId = marcaBultoId;
        FechaActualizacionUtc = fechaActualizacionUtc;
    }
}