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
    Guid? MarcaBultoId);

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
    Guid? MarcaBultoId);

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
    DateTimeOffset FechaCreacionUtc,
    DateTimeOffset? FechaActualizacionUtc);