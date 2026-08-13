namespace ChinaVenezuela.Domain.Catalogos;

public sealed class PuertoLlegada
{
    private PuertoLlegada() { }
    public PuertoLlegada(string nombre) { Id = Guid.NewGuid(); Nombre = nombre; }
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public void ActualizarNombre(string nombre) => Nombre = nombre;
}