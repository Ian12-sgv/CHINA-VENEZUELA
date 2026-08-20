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
    Guid? MarcaBultoId,
    string ReceptorCodigoUsuario);

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
    Guid? MarcaBultoId,
    string ReceptorCodigoUsuario);

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
    Guid? MarcaBultoId,
    string? ReceptorCodigoUsuario,
    string? ReceptorNombre,
    string? ReceptorCorreo,
    DateTimeOffset FechaCreacionUtc,
    DateTimeOffset? FechaActualizacionUtc,
    DateTimeOffset? FechaComprobanteEnviadoUtc);


