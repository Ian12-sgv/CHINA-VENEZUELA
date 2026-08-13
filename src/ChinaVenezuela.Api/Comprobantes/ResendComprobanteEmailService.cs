using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using Microsoft.Extensions.Options;

namespace ChinaVenezuela.Api.Comprobantes;

public sealed class ResendComprobanteEmailService(
    HttpClient httpClient,
    IOptions<ResendOptions> options,
    TimeProvider timeProvider) : IComprobanteEmailService
{
    public async Task<ComprobanteEnviadoResponse> EnviarAsync(EnvioComprobanteRequest request, CancellationToken cancellationToken)
    {
        var configuration = options.Value;
        if (string.IsNullOrWhiteSpace(configuration.ApiKey) || string.IsNullOrWhiteSpace(configuration.RemitenteCorreo))
            throw new ValidacionException(new Dictionary<string, string[]>
            {
                ["resend"] = ["El envio automatico no esta configurado. Faltan las variables Resend__ApiKey y Resend__RemitenteCorreo en el servidor."]
            });

        using var message = new HttpRequestMessage(HttpMethod.Post, "emails");
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", configuration.ApiKey);
        message.Content = JsonContent.Create(new ResendEmailRequest(
            $"{configuration.RemitenteNombre ?? "China - Venezuela"} <{configuration.RemitenteCorreo}>",
            [request.CorreoReceptor],
            [request.CorreoRemitente],
            request.Asunto,
            request.ContenidoHtml));

        using var response = await httpClient.SendAsync(message, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new ValidacionException(new Dictionary<string, string[]>
            {
                ["resend"] = ["Resend no pudo enviar el comprobante. Verifica la clave API y el remitente de dominio verificado."]
            });

        return new ComprobanteEnviadoResponse(request.CorreoReceptor, request.CorreoRemitente, timeProvider.GetUtcNow());
    }

    private sealed record ResendEmailRequest(
        [property: JsonPropertyName("from")] string From,
        [property: JsonPropertyName("to")] IReadOnlyList<string> To,
        [property: JsonPropertyName("cc")] IReadOnlyList<string> Cc,
        [property: JsonPropertyName("subject")] string Subject,
        [property: JsonPropertyName("html")] string Html);
}
