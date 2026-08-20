namespace ChinaVenezuela.Api.Comprobantes;

public sealed record EnvioComprobanteRequest(
    string CorreoReceptor,
    string NombreReceptor,
    string CorreoRemitente,
    string NombreRemitente,
    string Asunto,
    string ContenidoHtml);

public sealed record ComprobanteEnviadoResponse(
    string Receptor,
    string Copia,
    DateTimeOffset EnviadoEnUtc);

public interface IComprobanteEmailService
{
    Task<ComprobanteEnviadoResponse> EnviarAsync(EnvioComprobanteRequest request, CancellationToken cancellationToken);
}