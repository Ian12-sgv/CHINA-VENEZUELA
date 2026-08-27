using ChinaVenezuela.Api.Pedidos;

namespace ChinaVenezuela.Application.Tests;

public sealed class ImagenesUploadLimitsTests
{
    [Fact]
    public void El_archivo_mantiene_el_limite_funcional_de_15_mb()
    {
        Assert.Equal(15L * 1024 * 1024, ImagenesUploadLimits.MaxFileBytes);
    }

    [Fact]
    public void El_cuerpo_multipart_tiene_margen_sobre_el_limite_del_archivo()
    {
        Assert.Equal(16L * 1024 * 1024, ImagenesUploadLimits.MaxRequestBodyBytes);
        Assert.True(ImagenesUploadLimits.MaxRequestBodyBytes > ImagenesUploadLimits.MaxFileBytes);
    }
}