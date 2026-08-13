using ChinaVenezuela.Application.Recepciones.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChinaVenezuela.Api.ExceptionHandling;

public sealed class ApiExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Error no controlado al atender {Method} {Path}", context.Request.Method, context.Request.Path);
        ProblemDetails details = exception switch
        {
            ValidacionException validation => CreateValidationDetails(validation),
            RecursoNoEncontradoException notFound => new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Recurso no encontrado", Detail = notFound.Message },
            RecursoNoEncontradoPorNombreException notFound => new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Recurso no encontrado", Detail = notFound.Message },
            ConflictoException conflict => new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflicto de datos", Detail = conflict.Message },
            CredencialesInvalidasException invalidas => new ProblemDetails { Status = StatusCodes.Status401Unauthorized, Title = "Credenciales invalidas", Detail = invalidas.Message },
            _ => new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = "Error interno del servidor" }
        };
        context.Response.StatusCode = details.Status!.Value;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext { HttpContext = context, ProblemDetails = details });
    }

    private static ValidationProblemDetails CreateValidationDetails(ValidacionException validation)
    {
        var details = new ValidationProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Error de validacion" };
        foreach (var error in validation.Errores) details.Errors.Add(error.Key, error.Value);
        return details;
    }
}

