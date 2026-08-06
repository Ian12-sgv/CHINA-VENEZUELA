using System.Text.Json;
using ChinaVenezuela.Domain.Auditoria;
using ChinaVenezuela.Domain.Catalogos;
using ChinaVenezuela.Domain.Recepciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ChinaVenezuela.Infrastructure.Persistence;

public sealed class ChinaVenezuelaDbContext(DbContextOptions<ChinaVenezuelaDbContext> options, TimeProvider timeProvider) : DbContext(options)
{
    public DbSet<CompraRecibida> ComprasRecibidas => Set<CompraRecibida>();
    public DbSet<ContenedorCompartido> ContenedoresCompartidos => Set<ContenedorCompartido>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<MarcaBulto> MarcasBultos => Set<MarcaBulto>();
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChinaVenezuelaDbContext).Assembly);

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        var auditorias = ChangeTracker.Entries<CompraRecibida>().Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted).Select(CrearAuditoria).ToArray();
        RegistrosAuditoria.AddRange(auditorias);
        return base.SaveChangesAsync(cancellationToken);
    }

    private RegistroAuditoria CrearAuditoria(EntityEntry<CompraRecibida> entry)
    {
        var antes = entry.State is EntityState.Modified or EntityState.Deleted ? JsonSerializer.Serialize(Snapshot(entry, true)) : null;
        var despues = entry.State is EntityState.Added or EntityState.Modified ? JsonSerializer.Serialize(Snapshot(entry, false)) : null;
        var accion = entry.State switch { EntityState.Added => "Creada", EntityState.Modified => "Actualizada", EntityState.Deleted => "Eliminada", _ => throw new InvalidOperationException() };
        return new RegistroAuditoria(nameof(CompraRecibida), entry.Entity.Id, accion, antes, despues, timeProvider.GetUtcNow());
    }

    private static Dictionary<string, object?> Snapshot(EntityEntry<CompraRecibida> entry, bool original) => entry.Properties.ToDictionary(property => property.Metadata.Name, property => original ? property.OriginalValue : property.CurrentValue);
}