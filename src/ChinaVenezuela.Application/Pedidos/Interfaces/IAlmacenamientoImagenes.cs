namespace ChinaVenezuela.Application.Pedidos.Interfaces;
public interface IAlmacenamientoImagenes
{
    Task GuardarAsync(string clave, Stream contenido, CancellationToken ct);
    Task<Stream?> AbrirLecturaAsync(string clave, CancellationToken ct);
    Task EliminarAsync(string clave, CancellationToken ct);
}