using ChinaVenezuela.Domain.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChinaVenezuela.Infrastructure.Persistence.Configurations;

public sealed class ContenedorCompartidoConfiguration : IEntityTypeConfiguration<ContenedorCompartido>
{
    public void Configure(EntityTypeBuilder<ContenedorCompartido> builder) => ConfigureBase(builder, "contenedor_compartido");
    private static void ConfigureBase(EntityTypeBuilder<ContenedorCompartido> builder, string table) { builder.ToTable(table); builder.HasKey(x => x.Id); builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired(); builder.HasIndex(x => x.Nombre).IsUnique(); }
}

public sealed class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("empresa");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Rif).HasColumnName("rif").HasMaxLength(20);
        builder.Property(x => x.Clasificacion).HasColumnName("clasificacion").HasConversion<string>().HasMaxLength(12);
        builder.HasIndex(x => x.Nombre).IsUnique();
        builder.HasIndex(x => x.Rif).HasDatabaseName("ux_empresa_rif").IsUnique().HasFilter("rif IS NOT NULL");
    }
}

public sealed class MarcaBultoConfiguration : IEntityTypeConfiguration<MarcaBulto>
{
    public void Configure(EntityTypeBuilder<MarcaBulto> builder) { builder.ToTable("marca_bulto"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired(); builder.HasIndex(x => x.Nombre).IsUnique(); }
}
public sealed class AduanaConfiguration : IEntityTypeConfiguration<Aduana>
{
    public void Configure(EntityTypeBuilder<Aduana> builder) { builder.ToTable("aduana"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired(); builder.HasIndex(x => x.Nombre).IsUnique(); }
}

public sealed class PuertoLlegadaConfiguration : IEntityTypeConfiguration<PuertoLlegada>
{
    public void Configure(EntityTypeBuilder<PuertoLlegada> builder) { builder.ToTable("puerto_llegada"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired(); builder.HasIndex(x => x.Nombre).IsUnique(); }
}