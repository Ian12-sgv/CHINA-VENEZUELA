using ChinaVenezuela.Domain.Recepciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChinaVenezuela.Infrastructure.Persistence.Configurations;

public sealed class CompraRecibidaConfiguration : IEntityTypeConfiguration<CompraRecibida>
{
    public void Configure(EntityTypeBuilder<CompraRecibida> builder)
    {
        builder.ToTable("compra_recibida");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ContenedorCompartidoId).HasColumnName("contenedor_compartido_id");
        builder.Property(x => x.NombreContenedor).HasColumnName("nombre_contenedor").HasMaxLength(200).IsRequired();
        builder.Property(x => x.NumeroContenedor).HasColumnName("numero_contenedor").HasMaxLength(100).IsRequired();
        builder.Property(x => x.EmpresaId).HasColumnName("empresa_id").IsRequired();
        builder.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(2000);
        builder.Property(x => x.FechaSalida).HasColumnName("fecha_salida").HasColumnType("date").IsRequired();
        builder.Property(x => x.FechaLlegada).HasColumnName("fecha_llegada").HasColumnType("date");
        builder.Property(x => x.Aduana).HasColumnName("aduana").HasMaxLength(200);
        builder.Property(x => x.PuertoLlegada).HasColumnName("puerto_llegada").HasMaxLength(200).IsRequired();
        builder.Property(x => x.MarcaBultoId).HasColumnName("marca_bulto_id");
        builder.Property(x => x.ReceptorCodigoUsuario).HasColumnName("receptor_codigo_usuario").HasMaxLength(50);
        builder.Property(x => x.FechaCreacionUtc).HasColumnName("fecha_creacion_utc").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.FechaActualizacionUtc).HasColumnName("fecha_actualizacion_utc").HasColumnType("timestamp with time zone");
        builder.HasOne(x => x.ContenedorCompartido).WithMany().HasForeignKey(x => x.ContenedorCompartidoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MarcaBulto).WithMany().HasForeignKey(x => x.MarcaBultoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Receptor).WithMany().HasForeignKey(x => x.ReceptorCodigoUsuario).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.NumeroContenedor).HasDatabaseName("ix_compra_recibida_numero_contenedor");
        builder.HasIndex(x => x.ContenedorCompartidoId).HasDatabaseName("ix_compra_recibida_contenedor_compartido_id");
        builder.HasIndex(x => x.EmpresaId).HasDatabaseName("ix_compra_recibida_empresa_id");
        builder.HasIndex(x => x.MarcaBultoId).HasDatabaseName("ix_compra_recibida_marca_bulto_id");
        builder.HasIndex(x => x.ReceptorCodigoUsuario).HasDatabaseName("ix_compra_recibida_receptor_codigo_usuario");
    }
}
