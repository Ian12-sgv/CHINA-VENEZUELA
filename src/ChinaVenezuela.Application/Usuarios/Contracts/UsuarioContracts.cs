namespace ChinaVenezuela.Application.Usuarios.Contracts;

public sealed record IniciarSesionRequest(string Nombre, string Contrasena);
public sealed record CuentaUsuarioResponse(string CodigoUsuario, string Nombre, string Correo);
public sealed record UsuarioResponse(string CodigoUsuario, string Nombre, string? Correo, bool Status, IReadOnlyList<string> Grupos);
public sealed record InicioSesionResponse(string Token, UsuarioResponse Usuario);
public sealed record ActualizarGruposUsuarioRequest(IReadOnlyList<string> Grupos);
public sealed record CrearUsuarioAdministrativoRequest(string CodigoUsuario, string Nombre, string Correo, string Contrasena, bool Status, IReadOnlyList<string> Grupos);
public sealed record ActualizarUsuarioAdministrativoRequest(string Nombre, string? Correo, string? Contrasena, bool Status, IReadOnlyList<string> Grupos);