using ChinaVenezuela.Application.Grupos.Contracts;
using ChinaVenezuela.Application.Grupos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Domain.Usuarios;

namespace ChinaVenezuela.Application.Grupos.Services;

public sealed class GrupoService(IGrupoRepository repository) : IGrupoService
{
    private static readonly HashSet<string> GruposProtegidos = new(StringComparer.OrdinalIgnoreCase) { "Master", "Sistemas" };

    public async Task<IReadOnlyList<GrupoResponse>> ObtenerTodosAsync(CancellationToken cancellationToken) =>
        (await repository.ObtenerTodosAsync(cancellationToken)).Select(Mapear).ToArray();

    public async Task<GrupoResponse> CrearAsync(CrearGrupoRequest request, CancellationToken cancellationToken)
    {
        var nombre = ValidarNombre(request.Nombre);
        if (await repository.ExisteAsync(nombre, cancellationToken)) throw new ConflictoException("Ya existe un grupo con ese nombre.");
        var grupo = new Grupo(nombre);
        await repository.AgregarAsync(grupo, cancellationToken);
        return Mapear(grupo);
    }

    public async Task<GrupoResponse> ActualizarAsync(string nombreActual, ActualizarGrupoRequest request, CancellationToken cancellationToken)
    {
        var actual = ValidarNombre(nombreActual);
        Proteger(actual);
        if (!await repository.ExisteAsync(actual, cancellationToken)) throw new RecursoNoEncontradoPorNombreException("grupo", actual);
        var nuevo = ValidarNombre(request.Nombre);
        if (!string.Equals(actual, nuevo, StringComparison.OrdinalIgnoreCase) && await repository.ExisteAsync(nuevo, cancellationToken)) throw new ConflictoException("Ya existe un grupo con ese nombre.");
        await repository.RenombrarAsync(actual, nuevo, cancellationToken);
        return new GrupoResponse(nuevo, false);
    }

    public async Task EliminarAsync(string nombre, CancellationToken cancellationToken)
    {
        var valor = ValidarNombre(nombre);
        Proteger(valor);
        if (!await repository.ExisteAsync(valor, cancellationToken)) throw new RecursoNoEncontradoPorNombreException("grupo", valor);
        await repository.EliminarAsync(valor, cancellationToken);
    }

    public static bool EsProtegido(string nombre) => GruposProtegidos.Contains(nombre);
    private static GrupoResponse Mapear(Grupo grupo) => new(grupo.Nombre, EsProtegido(grupo.Nombre));
    private static string ValidarNombre(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length > 100)
            throw new ValidacionException(new Dictionary<string, string[]> { ["nombre"] = ["El nombre del grupo es obligatorio y debe tener un maximo de 100 caracteres."] });
        return nombre.Trim();
    }
    private static void Proteger(string nombre)
    {
        if (EsProtegido(nombre)) throw new ConflictoException("Este grupo de seguridad no puede modificarse ni eliminarse.");
    }
}