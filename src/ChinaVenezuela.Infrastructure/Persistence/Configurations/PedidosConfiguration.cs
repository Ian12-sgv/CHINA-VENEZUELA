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
        builder.Property(x => x.CodigoBarraAsignado).HasColumnName("codigo_barra_asignado").HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.CodigoBarraAsignado).IsUnique();
        builder.Property(x => x.PrecioRmb).HasColumnName("precio_rmb").HasPrecision(14, 2);
        builder.Property(x => x.TotalRmb).HasColumnName("total_rmb").HasPrecision(14, 2);
        builder.Property(x => x.CantidadDoz).HasColumnName("cantidad_doz");
        builder.Property(x => x.ReferenciaAsignada).HasColumnName("referencia_asignada").HasMaxLength(100).IsRequired();
        builder.Property(x => x.TipoProducto).HasColumnName("tipo_producto").HasMaxLength(100);
        builder.Property(x => x.Agente).HasColumnName("agente").HasMaxLength(150);
        builder.Property(x => x.Fabrica).HasColumnName("fabrica").HasMaxLength(150);
        builder.Property(x => x.ComposicionTela).HasColumnName("composicion_tela").HasMaxLength(255);
        builder.Property(x => x.ColorParaFabricar).HasColumnName("color_para_fabricar").HasMaxLength(100);
        builder.Property(x => x.MarcaProducto).HasColumnName("marca_producto").HasMaxLength(100);
        builder.Property(x => x.CurvaTalla).HasColumnName("curva_talla").HasMaxLength(50);
        builder.Property(x => x.PackPorCaja).HasColumnName("pack_por_caja");
        builder.Property(x => x.CantidadUnidades).HasColumnName("cantidad_unidades");
        builder.Property(x => x.MarcaBulto).HasColumnName("marca_bulto").HasMaxLength(100);
        builder.Property(x => x.CantidadBulto).HasColumnName("cantidad_bulto");
        builder.Property(x => x.Activo).HasColumnName("activo");
        builder.Property(x => x.Enviado).HasColumnName("enviado").HasDefaultValue(false);
        builder.Property(x => x.FechaEnvioUtc).HasColumnName("fecha_envio_utc");
        builder.Property(x => x.CreadoPorCodigoUsuario).HasColumnName("creado_por_codigo_usuario").HasMaxLength(50).IsRequired();
        builder.Property(x => x.FechaCreacionUtc).HasColumnName("fecha_creacion_utc").IsRequired();
        builder.HasMany(x => x.Imagenes).WithOne().HasForeignKey(x => x.ProductoPedidoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.GrupoPedido).WithOne().HasForeignKey<PedidoGrupo>(x => x.ProductoPedidoId).OnDelete(DeleteBehavior.Cascade);
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
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.HasIndex(x => new { x.ProductoPedidoId, x.Tipo }).IsUnique();
        builder.Property(x => x.ClaveAlmacenamiento).HasColumnName("clave_almacenamiento").HasMaxLength(500).IsRequired();
        builder.Property(x => x.NombreOriginal).HasColumnName("nombre_original").HasMaxLength(255).IsRequired();
        builder.Property(x => x.TipoContenido).HasColumnName("tipo_contenido").HasMaxLength(100).IsRequired();
        builder.Property(x => x.TamanoBytes).HasColumnName("tamano_bytes").IsRequired();
        builder.Property(x => x.FechaCreacionUtc).HasColumnName("fecha_creacion_utc").IsRequired();
        builder.Property(x => x.FechaActualizacionUtc).HasColumnName("fecha_actualizacion_utc");
    }
}

public sealed class AgentePedidoConfiguration : IEntityTypeConfiguration<AgentePedido>
{
    public void Configure(EntityTypeBuilder<AgentePedido> builder)
    {
        builder.ToTable("agente_pedido");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
        builder.HasIndex(x => x.Nombre).IsUnique();
    }
}

public sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("pedidos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
        builder.HasIndex(x => x.Nombre).IsUnique();
        builder.Property(x => x.CreadoPorCodigoUsuario).HasColumnName("creado_por_codigo_usuario").HasMaxLength(50).IsRequired();
        builder.Property(x => x.FechaCreacionUtc).HasColumnName("fecha_creacion_utc").IsRequired();
        builder.HasMany(x => x.Detalles).WithOne(x => x.Pedido).HasForeignKey(x => x.PedidoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class PedidoGrupoConfiguration : IEntityTypeConfiguration<PedidoGrupo>
{
    public void Configure(EntityTypeBuilder<PedidoGrupo> builder)
    {
        builder.ToTable("pedidos_grupos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PedidoId).HasColumnName("pedido_id").IsRequired();
        builder.Property(x => x.ProductoPedidoId).HasColumnName("producto_pedido_id").IsRequired();
        builder.HasIndex(x => x.ProductoPedidoId).IsUnique();
        builder.Property(x => x.FechaCreacionUtc).HasColumnName("fecha_creacion_utc").IsRequired();
    }
}

public sealed class RegistroPrecioPedidoConfiguration : IEntityTypeConfiguration<RegistroPrecioPedido>
{
    public void Configure(EntityTypeBuilder<RegistroPrecioPedido> builder)
    {
        builder.ToTable("registro_precio_pedido");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CodigoBarra).HasColumnName("codigo_barra").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Producto).HasColumnName("producto").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Sucursal).HasColumnName("sucursal").HasMaxLength(150).IsRequired();
        builder.Property(x => x.PrecioSistema).HasColumnName("precio_sistema").HasPrecision(12, 2);
        builder.Property(x => x.PrecioVerificado).HasColumnName("precio_verificado").HasPrecision(12, 2);
    }
}