using ChinaVenezuela.Application.Catalogos.Interfaces;
using ChinaVenezuela.Domain.Catalogos;
using ChinaVenezuela.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChinaVenezuela.Infrastructure.Catalogos;

public sealed class CatalogoRepository(ChinaVenezuelaDbContext context) : ICatalogoRepository
{
    public Task<IReadOnlyList<Empresa>> ObtenerEmpresasAsync(CancellationToken ct) => ToReadOnly(context.Empresas.AsNoTracking().OrderBy(x => x.Nombre), ct);
    public Task<Empresa?> ObtenerEmpresaAsync(Guid id, CancellationToken ct) => context.Empresas.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<bool> ExisteEmpresaConNombreAsync(string nombre, Guid? excluirId, CancellationToken ct) => context.Empresas.AnyAsync(x => x.Nombre == nombre && (!excluirId.HasValue || x.Id != excluirId), ct);
    public Task<bool> ExisteEmpresaConRifAsync(string rif, Guid? excluirId, CancellationToken ct) => context.Empresas.AnyAsync(x => x.Rif == rif && (!excluirId.HasValue || x.Id != excluirId), ct);
    public Task AgregarEmpresaAsync(Empresa empresa, CancellationToken ct) => context.Empresas.AddAsync(empresa, ct).AsTask();
    public void EliminarEmpresa(Empresa empresa) => context.Empresas.Remove(empresa);
    public Task<bool> EmpresaEstaEnUsoAsync(Guid id, CancellationToken ct) => context.ComprasRecibidas.AnyAsync(x => x.EmpresaId == id, ct);
    public Task<IReadOnlyList<MarcaBulto>> ObtenerMarcasBultoAsync(CancellationToken ct) => ToReadOnly(context.MarcasBultos.AsNoTracking().OrderBy(x => x.Nombre), ct);
    public Task<MarcaBulto?> ObtenerMarcaBultoAsync(Guid id, CancellationToken ct) => context.MarcasBultos.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<bool> ExisteMarcaBultoConNombreAsync(string nombre, Guid? excluirId, CancellationToken ct) => context.MarcasBultos.AnyAsync(x => x.Nombre == nombre && (!excluirId.HasValue || x.Id != excluirId), ct);
    public Task AgregarMarcaBultoAsync(MarcaBulto marca, CancellationToken ct) => context.MarcasBultos.AddAsync(marca, ct).AsTask();
    public void EliminarMarcaBulto(MarcaBulto marca) => context.MarcasBultos.Remove(marca);
    public Task<bool> MarcaBultoEstaEnUsoAsync(Guid id, CancellationToken ct) => context.ComprasRecibidas.AnyAsync(x => x.MarcaBultoId == id, ct);
    public Task<IReadOnlyList<ContenedorCompartido>> ObtenerContenedoresCompartidosAsync(CancellationToken ct) => ToReadOnly(context.ContenedoresCompartidos.AsNoTracking().OrderBy(x => x.Nombre), ct);
    public Task<ContenedorCompartido?> ObtenerContenedorCompartidoAsync(Guid id, CancellationToken ct) => context.ContenedoresCompartidos.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<bool> ExisteContenedorCompartidoConNombreAsync(string nombre, Guid? excluirId, CancellationToken ct) => context.ContenedoresCompartidos.AnyAsync(x => x.Nombre == nombre && (!excluirId.HasValue || x.Id != excluirId), ct);
    public Task AgregarContenedorCompartidoAsync(ContenedorCompartido contenedor, CancellationToken ct) => context.ContenedoresCompartidos.AddAsync(contenedor, ct).AsTask();
    public void EliminarContenedorCompartido(ContenedorCompartido contenedor) => context.ContenedoresCompartidos.Remove(contenedor);
    public Task<bool> ContenedorCompartidoEstaEnUsoAsync(Guid id, CancellationToken ct) => context.ComprasRecibidas.AnyAsync(x => x.ContenedorCompartidoId == id, ct);
    public Task<IReadOnlyList<Aduana>> ObtenerAduanasAsync(CancellationToken ct) => ToReadOnly(context.Aduanas.AsNoTracking().OrderBy(x => x.Nombre), ct);
    public Task<Aduana?> ObtenerAduanaAsync(Guid id, CancellationToken ct) => context.Aduanas.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<bool> ExisteAduanaConNombreAsync(string nombre, Guid? excluirId, CancellationToken ct) => context.Aduanas.AnyAsync(x => x.Nombre == nombre && (!excluirId.HasValue || x.Id != excluirId), ct);
    public Task AgregarAduanaAsync(Aduana aduana, CancellationToken ct) => context.Aduanas.AddAsync(aduana, ct).AsTask();
    public void EliminarAduana(Aduana aduana) => context.Aduanas.Remove(aduana);
    public Task<IReadOnlyList<PuertoLlegada>> ObtenerPuertosLlegadaAsync(CancellationToken ct) => ToReadOnly(context.PuertosLlegada.AsNoTracking().OrderBy(x => x.Nombre), ct);
    public Task<PuertoLlegada?> ObtenerPuertoLlegadaAsync(Guid id, CancellationToken ct) => context.PuertosLlegada.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<bool> ExistePuertoLlegadaConNombreAsync(string nombre, Guid? excluirId, CancellationToken ct) => context.PuertosLlegada.AnyAsync(x => x.Nombre == nombre && (!excluirId.HasValue || x.Id != excluirId), ct);
    public Task AgregarPuertoLlegadaAsync(PuertoLlegada puerto, CancellationToken ct) => context.PuertosLlegada.AddAsync(puerto, ct).AsTask();
    public void EliminarPuertoLlegada(PuertoLlegada puerto) => context.PuertosLlegada.Remove(puerto);
    public Task GuardarCambiosAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
    private static async Task<IReadOnlyList<T>> ToReadOnly<T>(IQueryable<T> query, CancellationToken ct) where T : class => await query.ToListAsync(ct);
}