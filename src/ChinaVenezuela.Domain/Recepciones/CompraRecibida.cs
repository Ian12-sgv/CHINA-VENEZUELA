using ChinaVenezuela.Domain.Catalogos;
using ChinaVenezuela.Domain.Usuarios;

namespace ChinaVenezuela.Domain.Recepciones;

public sealed class CompraRecibida
{
    public const string StatusEnProceso = "En proceso";
    public const string StatusAprobado = "Aprobado";
    public const string StatusSinTerminar = "Sin terminar";
    private CompraRecibida() { }

    public CompraRecibida(
        Guid? contenedorCompartidoId,
        string nombreContenedor,
        string numeroContenedor,
        Guid empresaId,
        string? descripcion,
        DateOnly fechaSalida,
        DateOnly? fechaLlegada,
        string? aduana,
        string puertoLlegada,
        Guid? marcaBultoId,        string receptorCodigoUsuario,
        DateTimeOffset fechaCreacionUtc)
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
        MarcaBultoId = marcaBultoId;        ReceptorCodigoUsuario = receptorCodigoUsuario;
        Status = StatusEnProceso;
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
    public MarcaBulto? MarcaBulto { get; private set; }    public string? ReceptorCodigoUsuario { get; private set; }
    public Usuario? Receptor { get; private set; }
    public string Status { get; private set; } = StatusEnProceso;
    public DateTimeOffset FechaCreacionUtc { get; private set; }
    public DateTimeOffset? FechaActualizacionUtc { get; private set; }
    public DateTimeOffset? FechaComprobanteEnviadoUtc { get; private set; }
    public string? ClaveArchivoComprobante { get; private set; }
    public string? NombreArchivoComprobante { get; private set; }
    public string? TipoContenidoArchivoComprobante { get; private set; }
    public long? TamanoBytesArchivoComprobante { get; private set; }
    public DateTimeOffset? FechaCargaArchivoComprobanteUtc { get; private set; }

    public void MarcarComprobanteEnviado(DateTimeOffset fechaEnvioUtc) => FechaComprobanteEnviadoUtc = fechaEnvioUtc;
    public void ActualizarStatus(string status, DateTimeOffset fechaActualizacionUtc) { Status = status; FechaActualizacionUtc = fechaActualizacionUtc; }

    public void AsignarArchivoComprobante(string clave, string nombreOriginal, string tipoContenido, long tamanoBytes, DateTimeOffset fechaCargaUtc)
    {
        ClaveArchivoComprobante = clave;
        NombreArchivoComprobante = nombreOriginal;
        TipoContenidoArchivoComprobante = tipoContenido;
        TamanoBytesArchivoComprobante = tamanoBytes;
        FechaCargaArchivoComprobanteUtc = fechaCargaUtc;
    }

    public void EliminarArchivoComprobante()
    {
        ClaveArchivoComprobante = null;
        NombreArchivoComprobante = null;
        TipoContenidoArchivoComprobante = null;
        TamanoBytesArchivoComprobante = null;
        FechaCargaArchivoComprobanteUtc = null;
    }

    public void Actualizar(
        Guid? contenedorCompartidoId,
        string nombreContenedor,
        string numeroContenedor,
        Guid empresaId,
        string? descripcion,
        DateOnly fechaSalida,
        DateOnly? fechaLlegada,
        string? aduana,
        string puertoLlegada,
        Guid? marcaBultoId,        string receptorCodigoUsuario,
        DateTimeOffset fechaActualizacionUtc)
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
        MarcaBultoId = marcaBultoId;        ReceptorCodigoUsuario = receptorCodigoUsuario;
        FechaActualizacionUtc = fechaActualizacionUtc;
    }
}
