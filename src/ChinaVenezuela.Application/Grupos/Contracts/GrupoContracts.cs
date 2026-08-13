namespace ChinaVenezuela.Application.Grupos.Contracts;

public sealed record GrupoResponse(string Nombre, bool Protegido);
public sealed record CrearGrupoRequest(string Nombre);
public sealed record ActualizarGrupoRequest(string Nombre);