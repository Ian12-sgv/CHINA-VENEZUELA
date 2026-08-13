using System.Text.RegularExpressions;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Application.Usuarios.Contracts;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using ChinaVenezuela.Domain.Usuarios;

namespace ChinaVenezuela.Application.Usuarios.Services;

public sealed partial class UsuarioService(IUsuarioRepository repository, IContrasenaHasher hasher) : IUsuarioService
{
    private static readonly HashSet<string> CodigosProtegidos = new(StringComparer.OrdinalIgnoreCase) { "MS", "SIS" };

    public Task<UsuarioResponse> RegistrarAsync(RegistrarUsuarioRequest request, CancellationToken cancellationToken) =>
        CrearInternoAsync(request.CodigoUsuario, request.Nombre, request.Correo, request.Contrasena, true, [request.NombreGrupo], cancellationToken);

    public async Task<UsuarioResponse> CrearAdministrativoAsync(CrearUsuarioAdministrativoRequest request, CancellationToken cancellationToken) =>
        await CrearInternoAsync(request.CodigoUsuario, request.Nombre, request.Correo, request.Contrasena, request.Status, request.Grupos, cancellationToken);

    public async Task<UsuarioResponse> ValidarCredencialesAsync(IniciarSesionRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Contrasena)) throw new CredencialesInvalidasException();
        var nombre = request.Nombre.Trim();
        if (await repository.ContarPorNombreAsync(nombre, cancellationToken) > 1) throw new ConflictoException("Hay mas de un usuario con ese nombre. Debe corregirse antes de iniciar sesion por nombre.");
        var usuario = await repository.ObtenerPorNombreAsync(nombre, cancellationToken);
        if (usuario is null || !usuario.Status || !hasher.Verificar(request.Contrasena, usuario.ContrasenaHash)) throw new CredencialesInvalidasException();
        return Mapear(usuario);
    }

    public Task<IReadOnlyList<string>> ObtenerGruposAsync(CancellationToken cancellationToken) => repository.ObtenerNombresGruposAsync(cancellationToken);
    public async Task<IReadOnlyList<UsuarioResponse>> ObtenerTodosAsync(string codigoSolicitante, CancellationToken cancellationToken) =>
        (await repository.ObtenerTodosAsync(cancellationToken)).Where(usuario => PuedeAcceder(codigoSolicitante, usuario.CodigoUsuario)).Select(Mapear).ToArray();

    public async Task<IReadOnlyList<UsuarioResponse>> ObtenerReceptoresAsync(string codigoSolicitante, CancellationToken cancellationToken) =>
        (await repository.ObtenerTodosAsync(cancellationToken))
            .Where(usuario => usuario.Status && !string.Equals(usuario.CodigoUsuario, codigoSolicitante, StringComparison.OrdinalIgnoreCase))
            .OrderBy(usuario => usuario.Nombre)
            .Select(Mapear)
            .ToArray();
    public async Task<UsuarioResponse> ActualizarGruposAsync(string codigoSolicitante, string codigoUsuario, ActualizarGruposUsuarioRequest request, CancellationToken cancellationToken)
    {
        var usuario = await ObtenerUsuarioAsync(codigoUsuario, cancellationToken);
        ExigirAcceso(codigoSolicitante, usuario.CodigoUsuario);
        var grupos = await ValidarYNormalizarGruposAsync(request.Grupos, cancellationToken);
        await repository.ReemplazarGruposAsync(usuario.CodigoUsuario, grupos, cancellationToken);
        return new UsuarioResponse(usuario.CodigoUsuario, usuario.Nombre, usuario.Correo, usuario.Status, grupos.Order().ToArray());
    }

    public async Task<UsuarioResponse> ActualizarAdministrativoAsync(string codigoSolicitante, string codigoUsuario, ActualizarUsuarioAdministrativoRequest request, CancellationToken cancellationToken)
    {
        var usuario = await ObtenerUsuarioAsync(codigoUsuario, cancellationToken);
        ExigirAcceso(codigoSolicitante, usuario.CodigoUsuario);
        var nombre = ValidarNombre(request.Nombre);
        var correo = string.IsNullOrWhiteSpace(request.Correo) ? usuario.Correo : ValidarCorreo(request.Correo);
        if (!string.Equals(usuario.Nombre, nombre, StringComparison.OrdinalIgnoreCase) && await repository.ExisteNombreAsync(nombre, cancellationToken)) throw new ConflictoException("Ya existe un usuario con ese nombre.");
        if (!string.IsNullOrWhiteSpace(request.Contrasena) && request.Contrasena.Length < 8) throw new ValidacionException(new Dictionary<string, string[]> { ["contrasena"] = ["La contrasena debe tener al menos 8 caracteres."] });
        var grupos = await ValidarYNormalizarGruposAsync(request.Grupos, cancellationToken);
        if (!string.Equals(usuario.Correo, correo, StringComparison.OrdinalIgnoreCase) && correo is not null && await repository.ExisteCorreoAsync(correo, cancellationToken)) throw new ConflictoException("Ya existe un usuario con ese correo.");
        var hash = string.IsNullOrWhiteSpace(request.Contrasena) ? usuario.ContrasenaHash : hasher.Hash(request.Contrasena);
        await repository.ActualizarAsync(usuario.CodigoUsuario, nombre, correo, hash, request.Status, grupos, cancellationToken);
        return new UsuarioResponse(usuario.CodigoUsuario, nombre, correo, request.Status, grupos.Order().ToArray());
    }

    public async Task EliminarAsync(string codigoSolicitante, string codigoUsuario, CancellationToken cancellationToken)
    {
        var usuario = await ObtenerUsuarioAsync(codigoUsuario, cancellationToken);
        ExigirAcceso(codigoSolicitante, usuario.CodigoUsuario);
        if (CodigosProtegidos.Contains(usuario.CodigoUsuario)) throw new ConflictoException("Las cuentas Master y Sistemas no pueden eliminarse.");
        await repository.EliminarAsync(usuario.CodigoUsuario, cancellationToken);
    }

    private static bool PuedeAcceder(string codigoSolicitante, string codigoObjetivo) =>
        !string.Equals(codigoSolicitante, "MS", StringComparison.OrdinalIgnoreCase) || !string.Equals(codigoObjetivo, "SIS", StringComparison.OrdinalIgnoreCase);

    private static void ExigirAcceso(string codigoSolicitante, string codigoObjetivo)
    {
        if (!PuedeAcceder(codigoSolicitante, codigoObjetivo))
            throw new RecursoNoEncontradoPorNombreException("usuario", codigoObjetivo);
    }
    private async Task<UsuarioResponse> CrearInternoAsync(string codigoUsuario, string nombreUsuario, string correoUsuario, string contrasena, bool status, IReadOnlyList<string> gruposSolicitados, CancellationToken cancellationToken)
    {
        var codigo = ValidarCodigo(codigoUsuario);
        var nombre = ValidarNombre(nombreUsuario);
        var correo = ValidarCorreo(correoUsuario);
        if (string.IsNullOrWhiteSpace(contrasena) || contrasena.Length < 8) throw new ValidacionException(new Dictionary<string, string[]> { ["contrasena"] = ["La contrasena es obligatoria y debe tener al menos 8 caracteres."] });
        if (await repository.ExisteAsync(codigo, cancellationToken)) throw new ConflictoException("Ya existe un usuario con ese codigo.");
        if (await repository.ExisteNombreAsync(nombre, cancellationToken)) throw new ConflictoException("Ya existe un usuario con ese nombre.");
        if (await repository.ExisteCorreoAsync(correo, cancellationToken)) throw new ConflictoException("Ya existe un usuario con ese correo.");
        var grupos = await ValidarYNormalizarGruposAsync(gruposSolicitados, cancellationToken);
        var usuario = new Usuario(codigo, nombre, hasher.Hash(contrasena), status, correo);
        foreach (var grupo in grupos) usuario.Grupos.Add(new GrupoUsuario(codigo, grupo));
        await repository.AgregarAsync(usuario, cancellationToken);
        return Mapear(usuario);
    }

    private async Task<Usuario> ObtenerUsuarioAsync(string codigoUsuario, CancellationToken cancellationToken)
    {
        var codigo = ValidarCodigo(codigoUsuario);
        return await repository.ObtenerPorCodigoAsync(codigo, cancellationToken) ?? throw new RecursoNoEncontradoPorNombreException("usuario", codigo);
    }

    private async Task<string[]> ValidarYNormalizarGruposAsync(IReadOnlyList<string>? solicitados, CancellationToken cancellationToken)
    {
        var valores = solicitados?.Where(grupo => !string.IsNullOrWhiteSpace(grupo)).Select(grupo => grupo.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() ?? [];
        if (valores.Length == 0) throw new ValidacionException(new Dictionary<string, string[]> { ["grupos"] = ["Debes asignar al menos un grupo al usuario."] });
        var disponibles = await repository.ObtenerNombresGruposAsync(cancellationToken);
        if (valores.Any(grupo => !disponibles.Contains(grupo, StringComparer.OrdinalIgnoreCase))) throw new ValidacionException(new Dictionary<string, string[]> { ["grupos"] = ["Uno o mas grupos no existen."] });
        return valores.Select(valor => disponibles.Single(disponible => string.Equals(disponible, valor, StringComparison.OrdinalIgnoreCase))).ToArray();
    }

    private static UsuarioResponse Mapear(Usuario usuario) => new(usuario.CodigoUsuario, usuario.Nombre, usuario.Correo, usuario.Status, usuario.Grupos.Select(grupo => grupo.NombreGrupo).Order().ToArray());
    private static string ValidarCodigo(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo) || codigo.Trim().Length > 50 || !CodigoValido().IsMatch(codigo.Trim())) throw new ValidacionException(new Dictionary<string, string[]> { ["codigoUsuario"] = ["El codigo debe tener entre 1 y 50 caracteres alfanumericos, guiones o guion bajo."] });
        return codigo.Trim().ToUpperInvariant();
    }
    private static string ValidarNombre(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length > 200) throw new ValidacionException(new Dictionary<string, string[]> { ["nombre"] = ["El nombre es obligatorio y debe tener un maximo de 200 caracteres."] });
        return nombre.Trim();
    }

    private static string ValidarCorreo(string? correo)
    {
        var valor = correo?.Trim().ToLowerInvariant() ?? string.Empty;
        if (valor.Length > 254 || !CorreoValido().IsMatch(valor)) throw new ValidacionException(new Dictionary<string, string[]> { ["correo"] = ["El correo es obligatorio y debe tener un formato valido."] });
        return valor;
    }

    [GeneratedRegex("^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$")]
    private static partial Regex CorreoValido();
    [GeneratedRegex("^[A-Za-z0-9_-]+$")]
    private static partial Regex CodigoValido();
}


