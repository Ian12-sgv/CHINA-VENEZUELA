using ChinaVenezuela.Domain.Auditoria;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChinaVenezuela.Infrastructure.Persistence.Configurations;

public sealed class RegistroAuditoriaConfiguration : IEntityTypeConfiguration<RegistroAuditoria>
{
    public void Configure(EntityTypeBuilder<RegistroAuditoria> builder)
    {
        builder.ToTable("registro_auditoria");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TipoEntidad).HasColumnName("tipo_entidad").HasMaxLength(100).IsRequired();
        builder.Property(x => x.EntidadId).HasColumnName("entidad_id").IsRequired();
        builder.Property(x => x.Accion).HasColumnName("accion").HasMaxLength(30).IsRequired();
        builder.Property(x => x.ValoresAntesJson).HasColumnName("valores_antes_json").HasColumnType("jsonb");
        builder.Property(x => x.ValoresDespuesJson).HasColumnName("valores_despues_json").HasColumnType("jsonb");
        builder.Property(x => x.FechaUtc).HasColumnName("fecha_utc").HasColumnType("timestamp with time zone").IsRequired();
        builder.HasIndex(x => new { x.TipoEntidad, x.EntidadId }).HasDatabaseName("ix_registro_auditoria_tipo_entidad_entidad_id");
    }
}