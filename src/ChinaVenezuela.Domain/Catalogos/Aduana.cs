namespace ChinaVenezuela.Domain.Catalogos;

public sealed class Aduana
{
    private Aduana() { }
    public Aduana(string nombre) { Id = Guid.NewGuid(); Nombre = nombre; }
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public void ActualizarNombre(string nombre) => Nombre = nombre;
}