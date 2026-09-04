using System.Text.Json.Serialization;

namespace ChinaVenezuela.Application.Recepciones.Contracts;

public sealed record CrearCompraRecibidaRequest(
    Guid? ContenedorCompartidoId,
    string NombreContenedor,
    string NumeroContenedor,
    Guid EmpresaId,
    string? Descripcion,
    DateOnly FechaSalida,
    DateOnly? FechaLlegada,
    string? Aduana,
    string PuertoLlegada,
    Guid? MarcaBultoId,    string ReceptorCodigoUsuario);

public sealed record ActualizarCompraRecibidaRequest(
    Guid? ContenedorCompartidoId,
    string NombreContenedor,
    string NumeroContenedor,
    Guid EmpresaId,
    string? Descripcion,
    DateOnly FechaSalida,
    DateOnly? FechaLlegada,
    string? Aduana,
    string PuertoLlegada,
    Guid? MarcaBultoId,    string ReceptorCodigoUsuario);

public sealed record ActualizarStatusCompraRecibidaRequest(string Status);

public sealed record GuardarArchivoComprobanteCompraRequest(string ClaveAlmacenamiento, string NombreOriginal, string TipoContenido, long TamanoBytes);

public sealed record CompraRecibidaResponse(
    Guid Id,
    Guid? ContenedorCompartidoId,
    string NombreContenedor,
    string NumeroContenedor,
    Guid EmpresaId,
    string? Descripcion,
    DateOnly FechaSalida,
    DateOnly? FechaLlegada,
    string? Aduana,
    string PuertoLlegada,
    Guid? MarcaBultoId,    string? ReceptorCodigoUsuario,
    string? ReceptorNombre,
    string? ReceptorCorreo,
    string Status,
    DateTimeOffset FechaCreacionUtc,
    DateTimeOffset? FechaActualizacionUtc,
    DateTimeOffset? FechaComprobanteEnviadoUtc,
    [property: JsonIgnore] string? ClaveArchivoComprobante,
    string? NombreArchivoComprobante,
    string? TipoContenidoArchivoComprobante,
    DateTimeOffset? FechaCargaArchivoComprobanteUtc);


