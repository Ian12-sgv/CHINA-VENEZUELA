using ChinaVenezuela.Application.Recepciones.Contracts;
using ChinaVenezuela.Application.Recepciones.Exceptions;

namespace ChinaVenezuela.Application.Recepciones.Validation;

public static class CompraRecibidaValidator
{
    public static void Validate(CrearCompraRecibidaRequest request) => ValidateCore(request.NombreContenedor, request.NumeroContenedor, request.EmpresaId, request.Descripcion, request.FechaSalida, request.FechaLlegada, request.Aduana, request.PuertoLlegada, request.ReceptorCodigoUsuario);
    public static void Validate(ActualizarCompraRecibidaRequest request) => ValidateCore(request.NombreContenedor, request.NumeroContenedor, request.EmpresaId, request.Descripcion, request.FechaSalida, request.FechaLlegada, request.Aduana, request.PuertoLlegada, request.ReceptorCodigoUsuario);

    private static void ValidateCore(string nombreContenedor, string numeroContenedor, Guid empresaId, string? descripcion, DateOnly fechaSalida, DateOnly? fechaLlegada, string? aduana, string puertoLlegada, string receptorCodigoUsuario)
    {
        var errores = new Dictionary<string, string[]>();
        Required(nombreContenedor, nameof(nombreContenedor), 200, errores);
        Required(numeroContenedor, nameof(numeroContenedor), 100, errores);
        Required(puertoLlegada, nameof(puertoLlegada), 200, errores);
        Required(receptorCodigoUsuario, nameof(receptorCodigoUsuario), 50, errores);
        Optional(descripcion, nameof(descripcion), 2000, errores);
        Optional(aduana, nameof(aduana), 200, errores);
        if (empresaId == Guid.Empty) errores[nameof(empresaId)] = ["La empresa es obligatoria."];
        if (fechaLlegada is not null && fechaLlegada < fechaSalida) errores[nameof(fechaLlegada)] = ["La fecha de llegada no puede ser anterior a la fecha de salida."];
        if (errores.Count > 0) throw new ValidacionException(errores);
    }

    private static void Required(string? value, string property, int maxLength, IDictionary<string, string[]> errores)
    {
        if (string.IsNullOrWhiteSpace(value)) errores[property] = ["El campo es obligatorio."];
        else if (value.Trim().Length > maxLength) errores[property] = [$"El campo no puede exceder {maxLength} caracteres."];
    }

    private static void Optional(string? value, string property, int maxLength, IDictionary<string, string[]> errores)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Trim().Length > maxLength) errores[property] = [$"El campo no puede exceder {maxLength} caracteres."];
    }
}


