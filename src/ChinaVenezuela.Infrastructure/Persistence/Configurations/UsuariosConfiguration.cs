using ChinaVenezuela.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChinaVenezuela.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuario");
        builder.HasKey(x => x.CodigoUsuario);
        builder.HasIndex(x => x.Nombre).HasDatabaseName("ux_usuario_nombre").IsUnique();
        builder.HasIndex(x => x.Correo).HasDatabaseName("ux_usuario_correo").IsUnique().HasFilter("correo IS NOT NULL");
        builder.Property(x => x.CodigoUsuario).HasColumnName("codigo_usuario").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        builder.Property(x => x.ContrasenaHash).HasColumnName("contrasena_hash").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Correo).HasColumnName("correo").HasMaxLength(254);
        builder.Property(x => x.CorreoVerificado).HasColumnName("correo_verificado").HasDefaultValue(true).IsRequired();
        builder.Property(x => x.TokenVerificacionHash).HasColumnName("token_verificacion_hash").HasMaxLength(128);
        builder.Property(x => x.TokenVerificacionExpiraUtc).HasColumnName("token_verificacion_expira_utc");
        builder.Property(x => x.Status).HasColumnName("status").HasDefaultValue(true).IsRequired();

        builder.HasData(new Usuario(
            "MARTHA",
            "Martha",
            "PBKDF2-SHA256$600000$/ArqC9UVvmvFUXd6R3AnLw==$7LqpJF/sNGKbmYt60NGHe4RBjujsh04P8DkWFUvBVWI=",
            true));
    }
}

public sealed class GrupoUsuarioConfiguration : IEntityTypeConfiguration<GrupoUsuario>
{
    public void Configure(EntityTypeBuilder<GrupoUsuario> builder)
    {
        builder.ToTable("grupo_usuario");
        builder.HasKey(x => new { x.CodigoUsuario, x.NombreGrupo });
        builder.Property(x => x.CodigoUsuario).HasColumnName("codigo_usuario").HasMaxLength(50).IsRequired();
        builder.Property(x => x.NombreGrupo).HasColumnName("nombre_grupo").HasMaxLength(100).IsRequired();

        builder.HasOne(x => x.Usuario)
            .WithMany(x => x.Grupos)
            .HasForeignKey(x => x.CodigoUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(new GrupoUsuario("MARTHA", "Administradores"));
    }
}
