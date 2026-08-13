using ChinaVenezuela.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChinaVenezuela.Infrastructure.Persistence.Configurations;

public sealed class GrupoConfiguration : IEntityTypeConfiguration<Grupo>
{
    public void Configure(EntityTypeBuilder<Grupo> builder)
    {
        builder.ToTable("grupo");
        builder.HasKey(x => x.Nombre);
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.HasData(new Grupo("Prueba"));
    }
}