namespace ChinaVenezuela.Domain.Auditoria;

public sealed class RegistroAuditoria
{
    private RegistroAuditoria() { }

    public RegistroAuditoria(string tipoEntidad, Guid entidadId, string accion, string? valoresAntesJson, string? valoresDespuesJson, DateTimeOffset fechaUtc)
    {
        Id = Guid.NewGuid();
        TipoEntidad = tipoEntidad;
        EntidadId = entidadId;
        Accion = accion;
        ValoresAntesJson = valoresAntesJson;
        ValoresDespuesJson = valoresDespuesJson;
        FechaUtc = fechaUtc;
    }

    public Guid Id { get; private set; }
    public string TipoEntidad { get; private set; } = null!;
    public Guid EntidadId { get; private set; }
    public string Accion { get; private set; } = null!;
    public string? ValoresAntesJson { get; private set; }
    public string? ValoresDespuesJson { get; private set; }
    public DateTimeOffset FechaUtc { get; private set; }
}