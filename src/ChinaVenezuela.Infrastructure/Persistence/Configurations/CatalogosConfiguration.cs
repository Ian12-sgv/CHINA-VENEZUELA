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
    public void Configure(EntityTypeBuilder<Empresa> builder) { builder.ToTable("empresa"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired(); builder.HasIndex(x => x.Nombre).IsUnique(); }
}

public sealed class MarcaBultoConfiguration : IEntityTypeConfiguration<MarcaBulto>
{
    public void Configure(EntityTypeBuilder<MarcaBulto> builder) { builder.ToTable("marca_bulto"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired(); builder.HasIndex(x => x.Nombre).IsUnique(); }
}