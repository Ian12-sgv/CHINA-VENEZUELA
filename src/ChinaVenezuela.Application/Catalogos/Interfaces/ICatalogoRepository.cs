using ChinaVenezuela.Domain.Catalogos;

namespace ChinaVenezuela.Application.Catalogos.Interfaces;

public interface ICatalogoRepository
{
    Task<IReadOnlyList<Empresa>> ObtenerEmpresasAsync(CancellationToken cancellationToken);
    Task<Empresa?> ObtenerEmpresaAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteEmpresaConNombreAsync(string nombre, Guid? excluirId, CancellationToken cancellationToken);
    Task AgregarEmpresaAsync(Empresa empresa, CancellationToken cancellationToken);
    void EliminarEmpresa(Empresa empresa);
    Task<bool> EmpresaEstaEnUsoAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<MarcaBulto>> ObtenerMarcasBultoAsync(CancellationToken cancellationToken);
    Task<MarcaBulto?> ObtenerMarcaBultoAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteMarcaBultoConNombreAsync(string nombre, Guid? excluirId, CancellationToken cancellationToken);
    Task AgregarMarcaBultoAsync(MarcaBulto marcaBulto, CancellationToken cancellationToken);
    void EliminarMarcaBulto(MarcaBulto marcaBulto);
    Task<bool> MarcaBultoEstaEnUsoAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ContenedorCompartido>> ObtenerContenedoresCompartidosAsync(CancellationToken cancellationToken);
    Task<ContenedorCompartido?> ObtenerContenedorCompartidoAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteContenedorCompartidoConNombreAsync(string nombre, Guid? excluirId, CancellationToken cancellationToken);
    Task AgregarContenedorCompartidoAsync(ContenedorCompartido contenedorCompartido, CancellationToken cancellationToken);
    void EliminarContenedorCompartido(ContenedorCompartido contenedorCompartido);
    Task<bool> ContenedorCompartidoEstaEnUsoAsync(Guid id, CancellationToken cancellationToken);

    Task GuardarCambiosAsync(CancellationToken cancellationToken);
}