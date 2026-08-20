namespace ChinaVenezuela.Application.Recepciones.Exceptions;

public sealed class ValidacionException : Exception
{
    public ValidacionException(IReadOnlyDictionary<string, string[]> errores) : base("La solicitud contiene errores de validaciÃ³n.") => Errores = errores;
    public IReadOnlyDictionary<string, string[]> Errores { get; }
}

public sealed class RecursoNoEncontradoException(string recurso, Guid id) : Exception($"No se encontrÃ³ {recurso} con identificador '{id}'.");
public sealed class ConflictoException(string detalle) : Exception(detalle);
public sealed class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException() : base("Codigo de usuario o contrasena invalidos.") { }
}
public sealed class CorreoNoVerificadoException : Exception
{
    public CorreoNoVerificadoException() : base("Debes verificar tu correo antes de iniciar sesion. Revisa tu bandeja de entrada o solicita un nuevo enlace.") { }
}
public sealed class RecursoNoEncontradoPorNombreException(string recurso, string nombre) : Exception($"No se encontro {recurso} con nombre '{nombre}'.");