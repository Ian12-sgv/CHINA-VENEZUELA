namespace ChinaVenezuela.Domain.Usuarios;

public sealed class Usuario
{
    private Usuario() { }

    public Usuario(string codigoUsuario, string nombre, string contrasenaHash, bool status, string? correo = null)
    {
        CodigoUsuario = codigoUsuario;
        Nombre = nombre;
        ContrasenaHash = contrasenaHash;
        Status = status;
        Correo = correo;
    }

    public string CodigoUsuario { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string ContrasenaHash { get; private set; } = null!;
    public string? Correo { get; private set; }
    public bool Status { get; private set; }
    public ICollection<GrupoUsuario> Grupos { get; } = new List<GrupoUsuario>();
}
