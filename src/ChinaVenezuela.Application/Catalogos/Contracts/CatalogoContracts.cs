namespace ChinaVenezuela.Application.Catalogos.Contracts;

public sealed record CrearCatalogoRequest(string Nombre);
public sealed record ActualizarCatalogoRequest(string Nombre);
public sealed record CatalogoResponse(Guid Id, string Nombre);