namespace ChinaVenezuela.Domain.Usuarios;

public sealed class Grupo
{
    private Grupo() { }

    public Grupo(string nombre)
    {
        Nombre = nombre;
    }

    public string Nombre { get; private set; } = null!;
}