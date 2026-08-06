using ChinaVenezuela.Application.Catalogos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Contracts;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Application.Recepciones.Interfaces;
using ChinaVenezuela.Application.Recepciones.Validation;
using ChinaVenezuela.Domain.Recepciones;

namespace ChinaVenezuela.Application.Recepciones.Services;

public sealed class CompraRecibidaService(ICompraRecibidaRepository repository, ICatalogoRepository catalogoRepository, TimeProvider timeProvider) : ICompraRecibidaService
{
    public async Task<CompraRecibidaResponse> CrearAsync(CrearCompraRecibidaRequest request, CancellationToken cancellationToken)
    {
        CompraRecibidaValidator.Validate(request);
        await ValidarRelacionesAsync(request.EmpresaId, request.ContenedorCompartidoId, request.MarcaBultoId, cancellationToken);
        var compra = new CompraRecibida(request.ContenedorCompartidoId, Required(request.NombreContenedor), Required(request.NumeroContenedor), request.EmpresaId, Optional(request.Descripcion), request.FechaSalida, request.FechaLlegada, Optional(request.Aduana), Required(request.PuertoLlegada), request.MarcaBultoId, timeProvider.GetUtcNow());
        await repository.AgregarAsync(compra, cancellationToken); await repository.GuardarCambiosAsync(cancellationToken); return Map(compra);
    }
    public async Task<IReadOnlyList<CompraRecibidaResponse>> ObtenerTodasAsync(CancellationToken cancellationToken) => (await repository.ObtenerTodasAsync(cancellationToken)).Select(Map).ToArray();
    public async Task<CompraRecibidaResponse> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) => Map(await FindAsync(id, cancellationToken));
    public async Task<CompraRecibidaResponse> ActualizarAsync(Guid id, ActualizarCompraRecibidaRequest request, CancellationToken cancellationToken)
    {
        CompraRecibidaValidator.Validate(request); await ValidarRelacionesAsync(request.EmpresaId, request.ContenedorCompartidoId, request.MarcaBultoId, cancellationToken);
        var compra = await FindAsync(id, cancellationToken);
        compra.Actualizar(request.ContenedorCompartidoId, Required(request.NombreContenedor), Required(request.NumeroContenedor), request.EmpresaId, Optional(request.Descripcion), request.FechaSalida, request.FechaLlegada, Optional(request.Aduana), Required(request.PuertoLlegada), request.MarcaBultoId, timeProvider.GetUtcNow());
        await repository.GuardarCambiosAsync(cancellationToken); return Map(compra);
    }
    public async Task EliminarAsync(Guid id, CancellationToken cancellationToken) { var compra = await FindAsync(id, cancellationToken); repository.Eliminar(compra); await repository.GuardarCambiosAsync(cancellationToken); }
    private async Task ValidarRelacionesAsync(Guid empresaId, Guid? contenedorId, Guid? marcaId, CancellationToken ct)
    {
        var errores = new Dictionary<string, string[]>();
        if (await catalogoRepository.ObtenerEmpresaAsync(empresaId, ct) is null) errores["empresaId"] = ["La empresa indicada no existe."];
        if (contenedorId.HasValue && await catalogoRepository.ObtenerContenedorCompartidoAsync(contenedorId.Value, ct) is null) errores["contenedorCompartidoId"] = ["El contenedor compartido indicado no existe."];
        if (marcaId.HasValue && await catalogoRepository.ObtenerMarcaBultoAsync(marcaId.Value, ct) is null) errores["marcaBultoId"] = ["La marca de bulto indicada no existe."];
        if (errores.Count > 0) throw new ValidacionException(errores);
    }
    private async Task<CompraRecibida> FindAsync(Guid id, CancellationToken ct) => await repository.ObtenerPorIdAsync(id, ct) ?? throw new RecursoNoEncontradoException("la compra recibida", id);
    private static string Required(string value) => value.Trim();
    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static CompraRecibidaResponse Map(CompraRecibida e) => new(e.Id, e.ContenedorCompartidoId, e.NombreContenedor, e.NumeroContenedor, e.EmpresaId, e.Descripcion, e.FechaSalida, e.FechaLlegada, e.Aduana, e.PuertoLlegada, e.MarcaBultoId, e.FechaCreacionUtc, e.FechaActualizacionUtc);
}