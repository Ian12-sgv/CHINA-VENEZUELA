namespace ChinaVenezuela.Application.Recepciones.Exceptions;

public sealed class ValidacionException : Exception
{
    public ValidacionException(IReadOnlyDictionary<string, string[]> errores) : base("La solicitud contiene errores de validación.") => Errores = errores;
    public IReadOnlyDictionary<string, string[]> Errores { get; }
}

public sealed class RecursoNoEncontradoException(string recurso, Guid id) : Exception($"No se encontró {recurso} con identificador '{id}'.");
public sealed class ConflictoException(string detalle) : Exception(detalle);