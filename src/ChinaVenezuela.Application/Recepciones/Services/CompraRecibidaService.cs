using ChinaVenezuela.Application.Catalogos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Contracts;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Application.Recepciones.Interfaces;
using ChinaVenezuela.Application.Recepciones.Validation;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using ChinaVenezuela.Domain.Recepciones;

namespace ChinaVenezuela.Application.Recepciones.Services;

public sealed class CompraRecibidaService(
    ICompraRecibidaRepository repository,
    ICatalogoRepository catalogoRepository,
    IUsuarioRepository usuarioRepository,
    TimeProvider timeProvider) : ICompraRecibidaService
{
    public async Task<CompraRecibidaResponse> CrearAsync(string codigoRemitente, CrearCompraRecibidaRequest request, CancellationToken cancellationToken)
    {
        CompraRecibidaValidator.Validate(request);
        var receptor = await ValidarRelacionesAsync(codigoRemitente, request.EmpresaId, request.ContenedorCompartidoId, request.MarcaBultoId, request.ReceptorCodigoUsuario, cancellationToken);
        var compra = new CompraRecibida(request.ContenedorCompartidoId, Required(request.NombreContenedor), Required(request.NumeroContenedor), request.EmpresaId, Optional(request.Descripcion), request.FechaSalida, request.FechaLlegada, Optional(request.Aduana), Required(request.PuertoLlegada), request.MarcaBultoId, receptor, timeProvider.GetUtcNow());
        await repository.AgregarAsync(compra, cancellationToken);
        await repository.GuardarCambiosAsync(cancellationToken);
        return Map(compra);
    }

    public async Task<IReadOnlyList<CompraRecibidaResponse>> ObtenerTodasAsync(CancellationToken cancellationToken) => (await repository.ObtenerTodasAsync(cancellationToken)).Select(Map).ToArray();
    public async Task<CompraRecibidaResponse> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) => Map(await FindAsync(id, cancellationToken));

    public async Task<CompraRecibidaResponse> ActualizarAsync(string codigoRemitente, Guid id, ActualizarCompraRecibidaRequest request, CancellationToken cancellationToken)
    {
        CompraRecibidaValidator.Validate(request);
        var receptor = await ValidarRelacionesAsync(codigoRemitente, request.EmpresaId, request.ContenedorCompartidoId, request.MarcaBultoId, request.ReceptorCodigoUsuario, cancellationToken);
        var compra = await FindAsync(id, cancellationToken);
        compra.Actualizar(request.ContenedorCompartidoId, Required(request.NombreContenedor), Required(request.NumeroContenedor), request.EmpresaId, Optional(request.Descripcion), request.FechaSalida, request.FechaLlegada, Optional(request.Aduana), Required(request.PuertoLlegada), request.MarcaBultoId, receptor, timeProvider.GetUtcNow());
        await repository.GuardarCambiosAsync(cancellationToken);
        return Map(compra);
    }

    public async Task EliminarAsync(Guid id, CancellationToken cancellationToken)
    {
        var compra = await FindAsync(id, cancellationToken);
        repository.Eliminar(compra);
        await repository.GuardarCambiosAsync(cancellationToken);
    }

    private async Task<string> ValidarRelacionesAsync(string codigoRemitente, Guid empresaId, Guid? contenedorId, Guid? marcaId, string receptorCodigoUsuario, CancellationToken ct)
    {
        var errores = new Dictionary<string, string[]>();
        if (await catalogoRepository.ObtenerEmpresaAsync(empresaId, ct) is null) errores["empresaId"] = ["La empresa indicada no existe."];
        if (contenedorId.HasValue && await catalogoRepository.ObtenerContenedorCompartidoAsync(contenedorId.Value, ct) is null) errores["contenedorCompartidoId"] = ["El contenedor compartido indicado no existe."];
        if (marcaId.HasValue && await catalogoRepository.ObtenerMarcaBultoAsync(marcaId.Value, ct) is null) errores["marcaBultoId"] = ["La marca de bulto indicada no existe."];

        var receptor = receptorCodigoUsuario.Trim().ToUpperInvariant();
        if (string.Equals(codigoRemitente, receptor, StringComparison.OrdinalIgnoreCase)) errores["receptorCodigoUsuario"] = ["El receptor debe ser otro usuario."];
        else
        {
            var usuario = await usuarioRepository.ObtenerPorCodigoAsync(receptor, ct);
            if (usuario is null || !usuario.Status) errores["receptorCodigoUsuario"] = ["El receptor debe ser un usuario activo del sistema."];
        }

        if (errores.Count > 0) throw new ValidacionException(errores);
        return receptor;
    }

    private async Task<CompraRecibida> FindAsync(Guid id, CancellationToken ct) => await repository.ObtenerPorIdAsync(id, ct) ?? throw new RecursoNoEncontradoException("la compra recibida", id);
    private static string Required(string value) => value.Trim();
    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static CompraRecibidaResponse Map(CompraRecibida entity) => new(entity.Id, entity.ContenedorCompartidoId, entity.NombreContenedor, entity.NumeroContenedor, entity.EmpresaId, entity.Descripcion, entity.FechaSalida, entity.FechaLlegada, entity.Aduana, entity.PuertoLlegada, entity.MarcaBultoId, entity.ReceptorCodigoUsuario, entity.Receptor?.Nombre, entity.Receptor?.Correo, entity.FechaCreacionUtc, entity.FechaActualizacionUtc);
}


