using ChinaVenezuela.Domain.Catalogos;

namespace ChinaVenezuela.Application.Catalogos.Contracts;

public sealed record CrearCatalogoRequest(string Nombre);
public sealed record ActualizarCatalogoRequest(string Nombre);
public sealed record CatalogoResponse(Guid Id, string Nombre);
public sealed record EmpresaResponse(Guid Id, string Nombre, string? Rif, ClasificacionEmpresa? Clasificacion);
public sealed record CrearEmpresaRequest(string Nombre, string Rif, ClasificacionEmpresa Clasificacion);
public sealed record ActualizarEmpresaRequest(string Nombre, string Rif, ClasificacionEmpresa Clasificacion);