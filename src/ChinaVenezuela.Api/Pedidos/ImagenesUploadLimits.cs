namespace ChinaVenezuela.Api.Pedidos;

/// <summary>
/// Límites para cargas multipart de imágenes de pedidos.
/// El cuerpo HTTP requiere un margen adicional para límites y cabeceras multipart;
/// el archivo en sí nunca puede superar <see cref="MaxFileBytes"/>.
/// </summary>
public static class ImagenesUploadLimits
{
    public const long MaxFileBytes = 15 * 1024 * 1024;
    public const long MaxRequestBodyBytes = 16 * 1024 * 1024;
}