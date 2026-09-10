using ChinaVenezuela.Domain.FichasTecnicas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChinaVenezuela.Infrastructure.Persistence.Configurations;

public sealed class FichaTecnicaConfiguration : IEntityTypeConfiguration<FichaTecnica>
{
    public void Configure(EntityTypeBuilder<FichaTecnica> builder)
    {
        builder.ToTable("ficha_tecnica");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(80).IsRequired();
        builder.HasIndex(x => x.Codigo).IsUnique();
        builder.Property(x => x.Referencia).HasColumnName("referencia").HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.Referencia).IsUnique();
        builder.Property(x => x.Categoria).HasColumnName("categoria").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Linea).HasColumnName("linea").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(20).IsRequired();
        builder.Property(x => x.ComposicionTela).HasColumnName("composicion_tela").HasMaxLength(255);
        builder.Property(x => x.ColorParaFabricar).HasColumnName("color_para_fabricar").HasMaxLength(100);
        builder.Property(x => x.MarcaProducto).HasColumnName("marca_producto").HasMaxLength(100);
        builder.Property(x => x.CurvaTalla).HasColumnName("curva_talla").HasMaxLength(100);
        builder.Property(x => x.ImagenDatos).HasColumnName("imagen_datos");
        builder.Property(x => x.ImagenTipoContenido).HasColumnName("imagen_tipo_contenido").HasMaxLength(100);
        builder.HasMany(x => x.Atributos).WithOne(x => x.FichaTecnica).HasForeignKey(x => x.FichaTecnicaId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class AtributoFichaTecnicaConfiguration : IEntityTypeConfiguration<AtributoFichaTecnica>
{
    public void Configure(EntityTypeBuilder<AtributoFichaTecnica> builder)
    {
        builder.ToTable("ficha_tecnica_atributo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.FichaTecnicaId).HasColumnName("ficha_tecnica_id").IsRequired();
        builder.Property(x => x.Atributo).HasColumnName("atributo").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Valor).HasColumnName("valor").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Observacion).HasColumnName("observacion").HasMaxLength(500);
        builder.HasIndex(x => new { x.FichaTecnicaId, x.Atributo }).IsUnique();
        builder.Property(x => x.ComposicionTela).HasColumnName("composicion_tela").HasMaxLength(255);
        builder.Property(x => x.ColorParaFabricar).HasColumnName("color_para_fabricar").HasMaxLength(100);
        builder.Property(x => x.MarcaProducto).HasColumnName("marca_producto").HasMaxLength(100);
        builder.Property(x => x.CurvaTalla).HasColumnName("curva_talla").HasMaxLength(100);
    }
}