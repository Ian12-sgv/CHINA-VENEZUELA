using ClosedXML.Excel;
using ChinaVenezuela.Application.Recepciones.Contracts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ChinaVenezuela.Api.Comprobantes;

public interface IComprobanteCompraAdjuntosService
{
    IReadOnlyList<ArchivoAdjuntoCorreo> Generar(CompraRecibidaResponse compra, string origen);
}

public sealed class ComprobanteCompraAdjuntosService : IComprobanteCompraAdjuntosService
{
    private const string ColorPrincipal = "12345B";
    private const string ColorBorde = "D9E4F2";

    public IReadOnlyList<ArchivoAdjuntoCorreo> Generar(CompraRecibidaResponse compra, string origen)
    {
        var nombreBase = $"comprobante-compra-{NombreSeguro(compra.NumeroContenedor)}";
        var filas = CrearFilas(compra, origen);

        return
        [
            new ArchivoAdjuntoCorreo($"{nombreBase}.xlsx", CrearExcel(filas)),
            new ArchivoAdjuntoCorreo($"{nombreBase}.pdf", CrearPdf(filas))
        ];
    }

    private static IReadOnlyList<FilaComprobante> CrearFilas(CompraRecibidaResponse compra, string origen) =>
    [
        new("Origen", origen),
        new("Receptor", compra.ReceptorNombre ?? compra.ReceptorCodigoUsuario ?? "Sin asignar"),
        new("Contenedor", compra.NombreContenedor),
        new("Número", compra.NumeroContenedor),
        new("Fecha de salida", compra.FechaSalida.ToString("dd/MM/yyyy")),
        new("Fecha de llegada", compra.FechaLlegada?.ToString("dd/MM/yyyy") ?? "No aplica"),
        new("Puerto", compra.PuertoLlegada),
        new("Aduana", compra.Aduana ?? "No aplica"),
        new("Descripción", compra.Descripcion ?? "No aplica"),
        new("Status", compra.Status)
    ];

    private static byte[] CrearExcel(IReadOnlyList<FilaComprobante> filas)
    {
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add("Comprobante");
        hoja.Range("A1:B1").Merge();
        hoja.Cell("A1").Value = "Comprobante de compra";
        hoja.Cell("A1").Style.Font.Bold = true;
        hoja.Cell("A1").Style.Font.FontSize = 16;
        hoja.Cell("A1").Style.Font.FontColor = XLColor.FromHtml("#" + ColorPrincipal);
        hoja.Cell("A2").Value = "Resumen del recibo de mercancía China - Venezuela";
        hoja.Range("A2:B2").Merge();
        hoja.Cell("A2").Style.Font.FontColor = XLColor.FromHtml("#4B6584");

        for (var indice = 0; indice < filas.Count; indice++)
        {
            var fila = indice + 4;
            hoja.Cell(fila, 1).Value = filas[indice].Etiqueta;
            hoja.Cell(fila, 2).Value = filas[indice].Valor;
            hoja.Range(fila, 1, fila, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            hoja.Range(fila, 1, fila, 2).Style.Border.OutsideBorderColor = XLColor.FromHtml("#" + ColorBorde);
            hoja.Cell(fila, 1).Style.Font.Bold = true;
            hoja.Cell(fila, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F6FB");
        }

        hoja.Column(1).Width = 25;
        hoja.Column(2).Width = 42;
        using var contenido = new MemoryStream();
        libro.SaveAs(contenido);
        return contenido.ToArray();
    }

    private static byte[] CrearPdf(IReadOnlyList<FilaComprobante> filas)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.DefaultTextStyle(style => style.FontSize(10).FontColor("263B5A"));
                page.Header().Column(column =>
                {
                    column.Item().Text("Comprobante de compra").FontSize(20).SemiBold().FontColor(ColorPrincipal);
                    column.Item().PaddingTop(3).Text("Resumen del recibo de mercancía China - Venezuela.").FontColor("597095");
                });
                page.Content().PaddingVertical(18).Table(tabla =>
                {
                    tabla.ColumnsDefinition(columnas =>
                    {
                        columnas.ConstantColumn(170);
                        columnas.RelativeColumn();
                    });

                    foreach (var fila in filas)
                    {
                        tabla.Cell().Border(1).BorderColor(ColorBorde).Background("F2F6FB").Padding(8).Text(fila.Etiqueta).SemiBold();
                        tabla.Cell().Border(1).BorderColor(ColorBorde).Padding(8).Text(fila.Valor);
                    }
                });
                page.Footer().AlignCenter().Text(texto =>
                {
                    texto.Span("Generado por China - Venezuela el ");
                    texto.Span(DateTimeOffset.UtcNow.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                    texto.Span(" UTC");
                });
            });
        }).GeneratePdf();
    }

    private static string NombreSeguro(string valor)
    {
        var caracteresInvalidos = Path.GetInvalidFileNameChars();
        var seguro = new string(valor.Select(caracter => caracteresInvalidos.Contains(caracter) ? '-' : caracter).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(seguro) ? "sin-numero" : seguro;
    }

    private sealed record FilaComprobante(string Etiqueta, string Valor);
}