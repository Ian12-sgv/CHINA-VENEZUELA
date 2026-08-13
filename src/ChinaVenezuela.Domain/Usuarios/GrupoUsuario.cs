namespace ChinaVenezuela.Domain.Usuarios;

public sealed class GrupoUsuario
{
    private GrupoUsuario() { }

    public GrupoUsuario(string codigoUsuario, string nombreGrupo)
    {
        CodigoUsuario = codigoUsuario;
        NombreGrupo = nombreGrupo;
    }

    public string CodigoUsuario { get; private set; } = null!;
    public string NombreGrupo { get; private set; } = null!;
    public Usuario Usuario { get; private set; } = null!;
}