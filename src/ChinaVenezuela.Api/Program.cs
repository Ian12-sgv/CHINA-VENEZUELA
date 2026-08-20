using ChinaVenezuela.Api.Auth;
using ChinaVenezuela.Api.ExceptionHandling;
using ChinaVenezuela.Api.Hubs;
using ChinaVenezuela.Api.Comprobantes;
using ChinaVenezuela.Application.Catalogos.Interfaces;
using ChinaVenezuela.Application.Catalogos.Services;
using ChinaVenezuela.Application.Grupos.Interfaces;
using ChinaVenezuela.Application.Grupos.Services;
using ChinaVenezuela.Application.Recepciones.Interfaces;
using ChinaVenezuela.Application.Pedidos.Interfaces;
using ChinaVenezuela.Application.Pedidos.Services;
using ChinaVenezuela.Application.Recepciones.Services;
using ChinaVenezuela.Application.Usuarios.Interfaces;
using ChinaVenezuela.Application.Usuarios.Services;
using ChinaVenezuela.Infrastructure.Catalogos;
using ChinaVenezuela.Infrastructure.Grupos;
using ChinaVenezuela.Infrastructure.Persistence;
using ChinaVenezuela.Infrastructure.Recepciones;
using ChinaVenezuela.Infrastructure.Pedidos;
using ChinaVenezuela.Infrastructure.Usuarios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostgreSql") ?? throw new InvalidOperationException("La cadena de conexion 'PostgreSql' es obligatoria.");
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSignalR();
builder.Services.AddCors(options => options.AddPolicy("NetlifyFrontend", policy =>
    policy.WithOrigins("https://tracking-china.netlify.app")
        .AllowAnyHeader()
        .AllowAnyMethod()));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtTokenService.CrearParametrosValidacion(builder.Configuration);
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Query["access_token"];
                if (!string.IsNullOrWhiteSpace(token) && context.HttpContext.Request.Path.StartsWithSegments("/hub/actualizaciones"))
                    context.Token = token;
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GestionGrupos", policy => policy.RequireAuthenticatedUser().RequireAssertion(context =>
        context.User.HasClaim("codigo_usuario", "MS") || context.User.HasClaim("codigo_usuario", "SIS")));
    options.AddPolicy("AccesoCompras", policy => policy.RequireAuthenticatedUser().RequireAssertion(context =>
        context.User.HasClaim("codigo_usuario", "MS") || context.User.HasClaim("codigo_usuario", "SIS") ||
        context.User.FindAll(ClaimTypes.Role).Any(claim => string.Equals(claim.Value, "oficina", StringComparison.OrdinalIgnoreCase))));
    options.AddPolicy("AccesoPedidos", policy => policy.RequireAuthenticatedUser().RequireAssertion(context =>
        context.User.HasClaim("codigo_usuario", "MS") || context.User.HasClaim("codigo_usuario", "SIS") ||
        context.User.FindAll(ClaimTypes.Role).Any(claim => string.Equals(claim.Value, "Pedidos", StringComparison.OrdinalIgnoreCase))));
    options.AddPolicy("AccesoOperativo", policy => policy.RequireAuthenticatedUser().RequireAssertion(context =>
        context.User.HasClaim("codigo_usuario", "MS") || context.User.HasClaim("codigo_usuario", "SIS") ||
        context.User.FindAll(ClaimTypes.Role).Any(claim => string.Equals(claim.Value, "oficina", StringComparison.OrdinalIgnoreCase) || string.Equals(claim.Value, "Pedidos", StringComparison.OrdinalIgnoreCase))));
});
builder.Services.Configure<ResendOptions>(builder.Configuration.GetSection(ResendOptions.SectionName));
builder.Services.Configure<ImagenesOptions>(builder.Configuration.GetSection(ImagenesOptions.SectionName));
builder.Services.AddHttpClient<IComprobanteEmailService, ResendComprobanteEmailService>(client => client.BaseAddress = new Uri("https://api.resend.com/"));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddDbContext<ChinaVenezuelaDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<ICompraRecibidaRepository, CompraRecibidaRepository>();
builder.Services.AddScoped<IPedidosRepository, PedidosRepository>();
builder.Services.AddSingleton<IAlmacenamientoImagenes, AlmacenamientoImagenesLocal>();
builder.Services.AddScoped<IPedidosService, PedidosService>();
builder.Services.AddScoped<ICompraRecibidaService, CompraRecibidaService>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IGrupoRepository, GrupoRepository>();
builder.Services.AddScoped<IGrupoService, GrupoService>();
builder.Services.AddSingleton<IContrasenaHasher, Pbkdf2ContrasenaHasher>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var app = builder.Build();
app.UseExceptionHandler();
app.UseCors("NetlifyFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapControllers();
app.MapHub<ActualizacionesHub>("/hub/actualizaciones").RequireAuthorization();
app.Run();

public partial class Program;

