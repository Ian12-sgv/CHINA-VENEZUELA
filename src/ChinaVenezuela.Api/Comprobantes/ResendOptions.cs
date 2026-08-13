namespace ChinaVenezuela.Api.Comprobantes;

public sealed class ResendOptions
{
    public const string SectionName = "Resend";
    public string? ApiKey { get; init; }
    public string? RemitenteCorreo { get; init; }
    public string? RemitenteNombre { get; init; }
}
