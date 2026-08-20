using ChinaVenezuela.Application.Catalogos.Interfaces;
using ChinaVenezuela.Application.Recepciones.Contracts;
using ChinaVenezuela.Application.Recepciones.Exceptions;
using ChinaVenezuela.Application.Recepciones.Interfaces;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using ChinaVenezuela.Domain.Usuarios;
using ChinaVenezuela.Application.Recepciones.Services;
using ChinaVenezuela.Domain.Catalogos;
using ChinaVenezuela.Domain.Recepciones;

namespace ChinaVenezuela.Application.Tests;

public sealed class CompraRecibidaServiceTests
{
    [Fact]
    public async Task CrearAsync_CuandoLaSolicitudEsValida_CreaLaCompra()
    {
        var repository = new RepositorioEnMemoria();
        var catalogos = new RepositorioCatalogosEnMemoria();
        var fecha = new DateTimeOffset(2026, 8, 6, 12, 0, 0, TimeSpan.Zero);
        var service = new CompraRecibidaService(repository, catalogos, new RepositorioUsuariosEnMemoria(), new RelojFijo(fecha));
        var request = new CrearCompraRecibidaRequest(catalogos.ContenedorId, "Contenedor principal", "MSCU-1234567", catalogos.EmpresaId, "MercancÃƒÂ­a recibida", new DateOnly(2026, 7, 1), new DateOnly(2026, 8, 1), "SENIAT", "La Guaira", catalogos.MarcaId, "RECEPTOR");

        var response = await service.CrearAsync("REMITE", request, CancellationToken.None);

        Assert.Equal(catalogos.EmpresaId, response.EmpresaId);
        Assert.Equal(fecha, response.FechaCreacionUtc);
        Assert.Single(repository.Items);
        Assert.True(repository.Guardado);
    }

    [Fact]
    public async Task CrearAsync_CuandoLlegadaEsAnteriorASalida_LanzaValidacion()
    {
        var service = new CompraRecibidaService(new RepositorioEnMemoria(), new RepositorioCatalogosEnMemoria(), new RepositorioUsuariosEnMemoria(), new RelojFijo(DateTimeOffset.UtcNow));
        var request = new CrearCompraRecibidaRequest(null, "Contenedor", "MSCU-1234567", Guid.NewGuid(), null, new DateOnly(2026, 8, 2), new DateOnly(2026, 8, 1), null, "La Guaira", null, "RECEPTOR");

        var exception = await Assert.ThrowsAsync<ValidacionException>(() => service.CrearAsync("REMITE", request, CancellationToken.None));

        Assert.Contains("fechaLlegada", exception.Errores.Keys);
    }

    private sealed class RelojFijo(DateTimeOffset fecha) : TimeProvider { public override DateTimeOffset GetUtcNow() => fecha; }

    private sealed class RepositorioEnMemoria : ICompraRecibidaRepository
    {
        public List<CompraRecibida> Items { get; } = [];
        public bool Guardado { get; private set; }
        public Task AgregarAsync(CompraRecibida compra, CancellationToken ct) { Items.Add(compra); return Task.CompletedTask; }
        public void Eliminar(CompraRecibida compra) => Items.Remove(compra);
        public Task GuardarCambiosAsync(CancellationToken ct) { Guardado = true; return Task.CompletedTask; }
        public Task<CompraRecibida?> ObtenerPorIdAsync(Guid id, CancellationToken ct) => Task.FromResult<CompraRecibida?>(Items.SingleOrDefault(x => x.Id == id));
        public Task<IReadOnlyList<CompraRecibida>> ObtenerTodasAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<CompraRecibida>>(Items);
    }

    private sealed class RepositorioUsuariosEnMemoria : IUsuarioRepository
    {
        private readonly Usuario receptor = new("RECEPTOR", "Receptor", "hash", true);
        public Task<bool> ExisteAsync(string codigoUsuario, CancellationToken ct) => Task.FromResult(codigoUsuario == receptor.CodigoUsuario);
        public Task<bool> ExisteNombreAsync(string nombre, CancellationToken ct) => Task.FromResult(nombre == receptor.Nombre);
        public Task<bool> ExisteCorreoAsync(string correo, CancellationToken ct) => Task.FromResult(false);
        public Task<int> ContarPorNombreAsync(string nombre, CancellationToken ct) => Task.FromResult(nombre == receptor.Nombre ? 1 : 0);
        public Task<Usuario?> ObtenerPorNombreAsync(string nombre, CancellationToken ct) => Task.FromResult<Usuario?>(nombre == receptor.Nombre ? receptor : null);
        public Task<Usuario?> ObtenerPorCodigoAsync(string codigoUsuario, CancellationToken ct) => Task.FromResult<Usuario?>(codigoUsuario == receptor.CodigoUsuario ? receptor : null);
        public Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct) => Task.FromResult<Usuario?>(null);
        public Task<Usuario?> ObtenerPorTokenVerificacionHashAsync(string tokenHash, CancellationToken ct) => Task.FromResult<Usuario?>(null);
        public Task<IReadOnlyList<Usuario>> ObtenerTodosAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<Usuario>>([receptor]);
        public Task<IReadOnlyList<string>> ObtenerNombresGruposAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<string>>([]);
        public Task AgregarAsync(Usuario usuario, CancellationToken ct) => Task.CompletedTask;
        public Task ReemplazarGruposAsync(string codigoUsuario, IReadOnlyCollection<string> grupos, CancellationToken ct) => Task.CompletedTask;
        public Task ActualizarAsync(string codigoUsuario, string nombre, string? correo, string contrasenaHash, bool status, IReadOnlyCollection<string> grupos, CancellationToken ct) => Task.CompletedTask;
        public Task EliminarAsync(string codigoUsuario, CancellationToken ct) => Task.CompletedTask;
        public Task GuardarAsync(Usuario usuario, CancellationToken ct) => Task.CompletedTask;
        public Task MarcarCorreoPendienteAsync(string codigoUsuario, string tokenHash, DateTimeOffset expiraUtc, CancellationToken ct) => Task.CompletedTask;
    }
    private sealed class RepositorioCatalogosEnMemoria : ICatalogoRepository
    {
        private readonly Empresa empresa = new("Empresa prueba", "J123456789", ClasificacionEmpresa.Oriente);
        private readonly MarcaBulto marca = new("Marca prueba");
        private readonly ContenedorCompartido contenedor = new("Compartido");
        public Guid EmpresaId => empresa.Id;
        public Guid MarcaId => marca.Id;
        public Guid ContenedorId => contenedor.Id;
        public Task<IReadOnlyList<Empresa>> ObtenerEmpresasAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<Empresa>>([empresa]);
        public Task<Empresa?> ObtenerEmpresaAsync(Guid id, CancellationToken ct) => Task.FromResult<Empresa?>(id == empresa.Id ? empresa : null);
        public Task<bool> ExisteEmpresaConNombreAsync(string nombre, Guid? excluir, CancellationToken ct) => Task.FromResult(empresa.Nombre == nombre && empresa.Id != excluir);
        public Task<bool> ExisteEmpresaConRifAsync(string rif, Guid? excluir, CancellationToken ct) => Task.FromResult(empresa.Rif == rif && empresa.Id != excluir);
        public Task AgregarEmpresaAsync(Empresa entity, CancellationToken ct) => Task.CompletedTask;
        public void EliminarEmpresa(Empresa entity) { }
        public Task<bool> EmpresaEstaEnUsoAsync(Guid id, CancellationToken ct) => Task.FromResult(false);
        public Task<IReadOnlyList<MarcaBulto>> ObtenerMarcasBultoAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<MarcaBulto>>([marca]);
        public Task<MarcaBulto?> ObtenerMarcaBultoAsync(Guid id, CancellationToken ct) => Task.FromResult<MarcaBulto?>(id == marca.Id ? marca : null);
        public Task<bool> ExisteMarcaBultoConNombreAsync(string nombre, Guid? excluir, CancellationToken ct) => Task.FromResult(marca.Nombre == nombre && marca.Id != excluir);
        public Task AgregarMarcaBultoAsync(MarcaBulto entity, CancellationToken ct) => Task.CompletedTask;
        public void EliminarMarcaBulto(MarcaBulto entity) { }
        public Task<bool> MarcaBultoEstaEnUsoAsync(Guid id, CancellationToken ct) => Task.FromResult(false);
        public Task<IReadOnlyList<ContenedorCompartido>> ObtenerContenedoresCompartidosAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<ContenedorCompartido>>([contenedor]);
        public Task<ContenedorCompartido?> ObtenerContenedorCompartidoAsync(Guid id, CancellationToken ct) => Task.FromResult<ContenedorCompartido?>(id == contenedor.Id ? contenedor : null);
        public Task<bool> ExisteContenedorCompartidoConNombreAsync(string nombre, Guid? excluir, CancellationToken ct) => Task.FromResult(contenedor.Nombre == nombre && contenedor.Id != excluir);
        public Task AgregarContenedorCompartidoAsync(ContenedorCompartido entity, CancellationToken ct) => Task.CompletedTask;
        public void EliminarContenedorCompartido(ContenedorCompartido entity) { }
        public Task<bool> ContenedorCompartidoEstaEnUsoAsync(Guid id, CancellationToken ct) => Task.FromResult(false);
        public Task GuardarCambiosAsync(CancellationToken ct) => Task.CompletedTask;
    }
}



