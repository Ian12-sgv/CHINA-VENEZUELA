using ChinaVenezuela.Domain.Pedidos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChinaVenezuela.Infrastructure.Persistence.Configurations;

public sealed class ProductoPedidoConfiguration : IEntityTypeConfiguration<ProductoPedido>
{
    public void Configure(EntityTypeBuilder<ProductoPedido> builder)
    {
        builder.ToTable("producto_pedido");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CodigoBarra).HasColumnName("codigo_barra").HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.CodigoBarra).IsUnique();
        builder.Property(x => x.Referencia).HasColumnName("referencia").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Marca).HasColumnName("marca").HasMaxLength(100);
        builder.Property(x => x.Categoria).HasColumnName("categoria").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Talla).HasColumnName("talla").HasMaxLength(50);
        builder.Property(x => x.Color).HasColumnName("color").HasMaxLength(100);
        builder.Property(x => x.Fabricante).HasColumnName("fabricante").HasMaxLength(150);
        builder.Property(x => x.PrecioDetal).HasColumnName("precio_detal").HasPrecision(12, 2);
        builder.Property(x => x.Costo).HasColumnName("costo").HasPrecision(12, 2);
        builder.Property(x => x.FechaPedido).HasColumnName("fecha_pedido").HasColumnType("date").HasDefaultValueSql("CURRENT_DATE");
        builder.Property(x => x.Activo).HasColumnName("activo");
        builder.Property(x => x.Enviado).HasColumnName("enviado").HasDefaultValue(false);
        builder.Property(x => x.FechaEnvioUtc).HasColumnName("fecha_envio_utc");
        builder.Property(x => x.CreadoPorCodigoUsuario).HasColumnName("creado_por_codigo_usuario").HasMaxLength(50).IsRequired();
        builder.Property(x => x.FechaCreacionUtc).HasColumnName("fecha_creacion_utc").IsRequired();
        builder.HasOne(x => x.Imagen).WithOne().HasForeignKey<ProductoPedidoImagen>(x => x.ProductoPedidoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ProductoPedidoImagenConfiguration : IEntityTypeConfiguration<ProductoPedidoImagen>
{
    public void Configure(EntityTypeBuilder<ProductoPedidoImagen> builder)
    {
        builder.ToTable("producto_pedido_imagen");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ProductoPedidoId).HasColumnName("producto_pedido_id").IsRequired();
        builder.HasIndex(x => x.ProductoPedidoId).IsUnique();
        builder.Property(x => x.ClaveAlmacenamiento).HasColumnName("clave_almacenamiento").HasMaxLength(100).IsRequired();
        builder.Property(x => x.NombreOriginal).HasColumnName("nombre_original").HasMaxLength(255).IsRequired();
        builder.Property(x => x.TipoContenido).HasColumnName("tipo_contenido").HasMaxLength(100).IsRequired();
        builder.Property(x => x.TamanoBytes).HasColumnName("tamano_bytes").IsRequired();
        builder.Property(x => x.FechaCreacionUtc).HasColumnName("fecha_creacion_utc").IsRequired();
        builder.Property(x => x.FechaActualizacionUtc).HasColumnName("fecha_actualizacion_utc");
    }
}

public sealed class RegistroPrecioPedidoConfiguration : IEntityTypeConfiguration<RegistroPrecioPedido>
{
    public void Configure(EntityTypeBuilder<RegistroPrecioPedido> builder)
    {
        builder.ToTable("registro_precio_pedido"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.CodigoBarra).HasColumnName("codigo_barra").HasMaxLength(50).IsRequired(); builder.Property(x => x.Producto).HasColumnName("producto").HasMaxLength(255).IsRequired(); builder.Property(x => x.Sucursal).HasColumnName("sucursal").HasMaxLength(150).IsRequired(); builder.Property(x => x.PrecioSistema).HasColumnName("precio_sistema").HasPrecision(12, 2); builder.Property(x => x.PrecioVerificado).HasColumnName("precio_verificado").HasPrecision(12, 2);
    }
}