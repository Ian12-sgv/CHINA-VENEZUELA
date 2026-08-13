namespace ChinaVenezuela.Application.Usuarios.Interfaces;

public interface IContrasenaHasher
{
    string Hash(string contrasena);
    bool Verificar(string contrasena, string hash);
}