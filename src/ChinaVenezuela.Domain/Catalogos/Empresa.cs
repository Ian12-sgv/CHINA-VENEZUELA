namespace ChinaVenezuela.Domain.Catalogos;

public enum ClasificacionEmpresa { Oriente, Occidente, Aliada }

public sealed class Empresa
{
    private Empresa() { }
    public Empresa(string nombre, string rif, ClasificacionEmpresa clasificacion)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Rif = rif;
        Clasificacion = clasificacion;
    }

    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string? Rif { get; private set; }
    public ClasificacionEmpresa? Clasificacion { get; private set; }
    public void Actualizar(string nombre, string rif, ClasificacionEmpresa clasificacion)
    {
        Nombre = nombre;
        Rif = rif;
        Clasificacion = clasificacion;
    }
}