using ChinaVenezuela.Application.Recepciones.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChinaVenezuela.Api.ExceptionHandling;

public sealed class ApiExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        ProblemDetails details = exception switch
        {
            ValidacionException validation => CreateValidationDetails(validation),
            RecursoNoEncontradoException notFound => new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Recurso no encontrado", Detail = notFound.Message },
            ConflictoException conflict => new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflicto de datos", Detail = conflict.Message },
            _ => new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = "Error interno del servidor" }
        };
        context.Response.StatusCode = details.Status!.Value;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext { HttpContext = context, ProblemDetails = details });
    }
    private static ValidationProblemDetails CreateValidationDetails(ValidacionException validation)
    {
        var details = new ValidationProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Error de validación" };
        foreach (var error in validation.Errores) details.Errors.Add(error.Key, error.Value);
        return details;
    }
}