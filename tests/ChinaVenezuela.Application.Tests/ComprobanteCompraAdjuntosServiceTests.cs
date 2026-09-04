using ChinaVenezuela.Api.Comprobantes;
using ChinaVenezuela.Application.Recepciones.Contracts;
using System.IO.Compression;

namespace ChinaVenezuela.Application.Tests;

public sealed class ComprobanteCompraAdjuntosServiceTests
{
    [Fact]
    public void Generar_CreaExcelYPdfValidosConLosDatosDelComprobante()
    {
        var compra = new CompraRecibidaResponse(
            Guid.NewGuid(), null, "Contenedor 32", "MSKU-4429301", Guid.NewGuid(), "Mercancía de prueba",
            new DateOnly(2026, 8, 2), new DateOnly(2026, 8, 6), "Aduana principal", "Puerto Cabello", null,
            "MS", "Master", "master@example.com", "En proceso", DateTimeOffset.UtcNow, null, null, null, null, null, null);

        var adjuntos = new ComprobanteCompraAdjuntosService().Generar(compra, "Martha");

        Assert.Collection(adjuntos,
            excel =>
            {
                Assert.Equal("comprobante-compra-MSKU-4429301.xlsx", excel.NombreArchivo);
                using var paquete = new ZipArchive(new MemoryStream(excel.Contenido), ZipArchiveMode.Read);
                Assert.Contains(paquete.Entries, entrada => entrada.FullName == "xl/workbook.xml");
                Assert.Contains(paquete.Entries, entrada => entrada.FullName == "xl/worksheets/sheet1.xml");
            },
            pdf =>
            {
                Assert.Equal("comprobante-compra-MSKU-4429301.pdf", pdf.NombreArchivo);
                Assert.True(pdf.Contenido.Length > 500);
                Assert.Equal("%PDF-", System.Text.Encoding.ASCII.GetString(pdf.Contenido, 0, 5));
            });
    }
}