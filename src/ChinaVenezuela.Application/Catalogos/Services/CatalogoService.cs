using ChinaVenezuela.Application.Catalogos.Contracts;
using ChinaVenezuela.Application.Catalogos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Domain.Catalogos;

namespace ChinaVenezuela.Application.Catalogos.Services;

public sealed class CatalogoService(ICatalogoRepository repository) : ICatalogoService
{
    public async Task<IReadOnlyList<CatalogoResponse>> ObtenerEmpresasAsync(CancellationToken cancellationToken) => (await repository.ObtenerEmpresasAsync(cancellationToken)).Select(x => new CatalogoResponse(x.Id, x.Nombre)).ToArray();
    public async Task<CatalogoResponse> CrearEmpresaAsync(CrearCatalogoRequest request, CancellationToken cancellationToken)
    {
        var nombre = ValidarNombre(request.Nombre);
        if (await repository.ExisteEmpresaConNombreAsync(nombre, null, cancellationToken)) throw new ConflictoException("Ya existe una empresa con ese nombre.");
        var empresa = new Empresa(nombre); await repository.AgregarEmpresaAsync(empresa, cancellationToken); await repository.GuardarCambiosAsync(cancellationToken); return new(empresa.Id, empresa.Nombre);
    }
    public async Task<CatalogoResponse> ActualizarEmpresaAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken)
    {
        var empresa = await ObtenerEmpresaRequeridaAsync(id, cancellationToken); var nombre = ValidarNombre(request.Nombre);
        if (await repository.ExisteEmpresaConNombreAsync(nombre, id, cancellationToken)) throw new ConflictoException("Ya existe una empresa con ese nombre.");
        empresa.ActualizarNombre(nombre); await repository.GuardarCambiosAsync(cancellationToken); return new(empresa.Id, empresa.Nombre);
    }
    public async Task EliminarEmpresaAsync(Guid id, CancellationToken cancellationToken)
    {
        var empresa = await ObtenerEmpresaRequeridaAsync(id, cancellationToken);
        if (await repository.EmpresaEstaEnUsoAsync(id, cancellationToken)) throw new ConflictoException("No se puede eliminar la empresa porque está asociada a compras recibidas.");
        repository.EliminarEmpresa(empresa); await repository.GuardarCambiosAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CatalogoResponse>> ObtenerMarcasBultoAsync(CancellationToken cancellationToken) => (await repository.ObtenerMarcasBultoAsync(cancellationToken)).Select(x => new CatalogoResponse(x.Id, x.Nombre)).ToArray();
    public async Task<CatalogoResponse> CrearMarcaBultoAsync(CrearCatalogoRequest request, CancellationToken cancellationToken)
    {
        var nombre = ValidarNombre(request.Nombre);
        if (await repository.ExisteMarcaBultoConNombreAsync(nombre, null, cancellationToken)) throw new ConflictoException("Ya existe una marca de bulto con ese nombre.");
        var marca = new MarcaBulto(nombre); await repository.AgregarMarcaBultoAsync(marca, cancellationToken); await repository.GuardarCambiosAsync(cancellationToken); return new(marca.Id, marca.Nombre);
    }
    public async Task<CatalogoResponse> ActualizarMarcaBultoAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken)
    {
        var marca = await ObtenerMarcaRequeridaAsync(id, cancellationToken); var nombre = ValidarNombre(request.Nombre);
        if (await repository.ExisteMarcaBultoConNombreAsync(nombre, id, cancellationToken)) throw new ConflictoException("Ya existe una marca de bulto con ese nombre.");
        marca.ActualizarNombre(nombre); await repository.GuardarCambiosAsync(cancellationToken); return new(marca.Id, marca.Nombre);
    }
    public async Task EliminarMarcaBultoAsync(Guid id, CancellationToken cancellationToken)
    {
        var marca = await ObtenerMarcaRequeridaAsync(id, cancellationToken);
        if (await repository.MarcaBultoEstaEnUsoAsync(id, cancellationToken)) throw new ConflictoException("No se puede eliminar la marca porque está asociada a compras recibidas.");
        repository.EliminarMarcaBulto(marca); await repository.GuardarCambiosAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CatalogoResponse>> ObtenerContenedoresCompartidosAsync(CancellationToken cancellationToken) => (await repository.ObtenerContenedoresCompartidosAsync(cancellationToken)).Select(x => new CatalogoResponse(x.Id, x.Nombre)).ToArray();
    public async Task<CatalogoResponse> CrearContenedorCompartidoAsync(CrearCatalogoRequest request, CancellationToken cancellationToken)
    {
        var nombre = ValidarNombre(request.Nombre);
        if (await repository.ExisteContenedorCompartidoConNombreAsync(nombre, null, cancellationToken)) throw new ConflictoException("Ya existe un contenedor compartido con ese nombre.");
        var contenedor = new ContenedorCompartido(nombre); await repository.AgregarContenedorCompartidoAsync(contenedor, cancellationToken); await repository.GuardarCambiosAsync(cancellationToken); return new(contenedor.Id, contenedor.Nombre);
    }
    public async Task<CatalogoResponse> ActualizarContenedorCompartidoAsync(Guid id, ActualizarCatalogoRequest request, CancellationToken cancellationToken)
    {
        var contenedor = await ObtenerContenedorRequeridoAsync(id, cancellationToken); var nombre = ValidarNombre(request.Nombre);
        if (await repository.ExisteContenedorCompartidoConNombreAsync(nombre, id, cancellationToken)) throw new ConflictoException("Ya existe un contenedor compartido con ese nombre.");
        contenedor.ActualizarNombre(nombre); await repository.GuardarCambiosAsync(cancellationToken); return new(contenedor.Id, contenedor.Nombre);
    }
    public async Task EliminarContenedorCompartidoAsync(Guid id, CancellationToken cancellationToken)
    {
        var contenedor = await ObtenerContenedorRequeridoAsync(id, cancellationToken);
        if (await repository.ContenedorCompartidoEstaEnUsoAsync(id, cancellationToken)) throw new ConflictoException("No se puede eliminar el contenedor porque está asociado a compras recibidas.");
        repository.EliminarContenedorCompartido(contenedor); await repository.GuardarCambiosAsync(cancellationToken);
    }

    private static string ValidarNombre(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ValidacionException(new Dictionary<string, string[]> { ["nombre"] = ["El nombre es obligatorio."] });
        nombre = nombre.Trim();
        if (nombre.Length > 200) throw new ValidacionException(new Dictionary<string, string[]> { ["nombre"] = ["El nombre no puede exceder 200 caracteres."] });
        return nombre;
    }
    private async Task<Empresa> ObtenerEmpresaRequeridaAsync(Guid id, CancellationToken ct) => await repository.ObtenerEmpresaAsync(id, ct) ?? throw new RecursoNoEncontradoException("la empresa", id);
    private async Task<MarcaBulto> ObtenerMarcaRequeridaAsync(Guid id, CancellationToken ct) => await repository.ObtenerMarcaBultoAsync(id, ct) ?? throw new RecursoNoEncontradoException("la marca de bulto", id);
    private async Task<ContenedorCompartido> ObtenerContenedorRequeridoAsync(Guid id, CancellationToken ct) => await repository.ObtenerContenedorCompartidoAsync(id, ct) ?? throw new RecursoNoEncontradoException("el contenedor compartido", id);
}