using System.Security.Cryptography;
using ChinaVenezuela.Application.Usuarios.Interfaces;

namespace ChinaVenezuela.Infrastructure.Usuarios;

public sealed class Pbkdf2ContrasenaHasher : IContrasenaHasher
{
    private const int Iteraciones = 600_000;
    private const int TamanoSal = 16;
    private const int TamanoHash = 32;

    public string Hash(string contrasena)
    {
        var sal = RandomNumberGenerator.GetBytes(TamanoSal);
        var hash = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
        return $"PBKDF2-SHA256${Iteraciones}${Convert.ToBase64String(sal)}${Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string contrasena, string hash)
    {
        var partes = hash.Split('$');
        if (partes.Length != 4 || partes[0] != "PBKDF2-SHA256" || !int.TryParse(partes[1], out var iteraciones)) return false;
        try
        {
            var sal = Convert.FromBase64String(partes[2]);
            var esperado = Convert.FromBase64String(partes[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, iteraciones, HashAlgorithmName.SHA256, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(actual, esperado);
        }
        catch (FormatException) { return false; }
    }
}