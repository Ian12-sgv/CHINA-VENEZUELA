using ChinaVenezuela.Application.Pedidos.Interfaces;
using Microsoft.Extensions.Options;

namespace ChinaVenezuela.Infrastructure.Pedidos;

public sealed class ImagenesOptions
{
    public const string SectionName = "Imagenes";
    public string? Directorio { get; init; }
}

public sealed class AlmacenamientoImagenesLocal(IOptions<ImagenesOptions> options) : IAlmacenamientoImagenes
{
    private readonly string directorioRaiz = Path.GetFullPath(options.Value.Directorio ?? Path.Combine(AppContext.BaseDirectory, "datos", "imagenes"));

    public async Task GuardarAsync(string clave, Stream contenido, CancellationToken ct)
    {
        var ruta = RutaSegura(clave);
        Directory.CreateDirectory(directorioRaiz);
        await using var destino = new FileStream(ruta, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await contenido.CopyToAsync(destino, ct);
    }

    public Task<Stream?> AbrirLecturaAsync(string clave, CancellationToken ct)
    {
        var ruta = RutaSegura(clave);
        Stream? contenido = File.Exists(ruta) ? new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true) : null;
        return Task.FromResult(contenido);
    }

    public Task EliminarAsync(string clave, CancellationToken ct)
    {
        var ruta = RutaSegura(clave);
        if (File.Exists(ruta)) File.Delete(ruta);
        return Task.CompletedTask;
    }

    private string RutaSegura(string clave)
    {
        if (string.IsNullOrWhiteSpace(clave) || !string.Equals(Path.GetFileName(clave), clave, StringComparison.Ordinal)) throw new InvalidOperationException("La clave de almacenamiento no es válida.");
        return Path.Combine(directorioRaiz, clave);
    }
}