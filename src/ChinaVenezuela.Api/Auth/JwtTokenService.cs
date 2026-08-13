using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChinaVenezuela.Application.Usuarios.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace ChinaVenezuela.Api.Auth;

public sealed class JwtTokenService(IConfiguration configuration)
{
    private const string Issuer = "ChinaVenezuela.Api";
    private const string Audience = "ChinaVenezuela.Web";

    public string Crear(UsuarioResponse usuario)
    {
        var clave = ObtenerClave(configuration);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.CodigoUsuario),
            new(ClaimTypes.Name, usuario.Nombre),
            new("codigo_usuario", usuario.CodigoUsuario)
        };
        claims.AddRange(usuario.Grupos.Select(grupo => new Claim(ClaimTypes.Role, grupo)));
        var credenciales = new SigningCredentials(new SymmetricSecurityKey(clave), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(Issuer, Audience, claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: credenciales);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static TokenValidationParameters CrearParametrosValidacion(IConfiguration configuration) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = Issuer,
        ValidateAudience = true,
        ValidAudience = Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(ObtenerClave(configuration)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };

    private static byte[] ObtenerClave(IConfiguration configuration)
    {
        var valor = configuration["Jwt:Key"] ?? throw new InvalidOperationException("La variable Jwt__Key es obligatoria.");
        var clave = Encoding.UTF8.GetBytes(valor);
        if (clave.Length < 32) throw new InvalidOperationException("Jwt__Key debe tener al menos 32 caracteres.");
        return clave;
    }
}