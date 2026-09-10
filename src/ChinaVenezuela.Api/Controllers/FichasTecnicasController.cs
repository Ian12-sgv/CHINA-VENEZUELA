using ChinaVenezuela.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChinaVenezuela.Api.Controllers;

[ApiController, Authorize(Policy = "AccesoPedidos"), Route("api/fichas-tecnicas"), Produces("application/json")]
public sealed class FichasTecnicasController(ChinaVenezuelaDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FichaResponse>>> Listar(CancellationToken ct) => Ok((await context.FichasTecnicas.AsNoTracking().Include(x => x.Atributos).OrderBy(x => x.Codigo).ToListAsync(ct)).Select(Map).ToArray());

    [HttpGet("por-referencia/{referencia}")]
    public async Task<ActionResult<FichaResponse>> ObtenerPorReferencia(string referencia, CancellationToken ct) { var ficha = await context.FichasTecnicas.AsNoTracking().Include(x => x.Atributos).SingleOrDefaultAsync(x => x.Referencia == referencia.Trim(), ct); return ficha is null ? NotFound() : Ok(Map(ficha)); }

    [HttpPost]
    public async Task<ActionResult<FichaResponse>> Crear(FichaRequest request, CancellationToken ct)
    {
        var datos = Validar(request);
        if (await context.FichasTecnicas.AnyAsync(x => x.Codigo == datos.Codigo, ct)) return Conflict("Ya existe una ficha con ese código."); if (await context.FichasTecnicas.AnyAsync(x => x.Referencia == datos.Referencia, ct)) return Conflict("Ya existe una ficha con esa referencia.");
        var ficha = new ChinaVenezuela.Domain.FichasTecnicas.FichaTecnica(datos.Codigo, datos.Referencia, datos.Categoria, datos.Linea, datos.Estado, null, null, null, null);
        context.FichasTecnicas.Add(ficha); await context.SaveChangesAsync(ct);
        return Created($"api/fichas-tecnicas/{ficha.Id}", Map(ficha));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FichaResponse>> Actualizar(Guid id, FichaRequest request, CancellationToken ct)
    {
        var ficha = await context.FichasTecnicas.Include(x => x.Atributos).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (ficha is null) return NotFound();
        var datos = Validar(request);
        if (await context.FichasTecnicas.AnyAsync(x => x.Codigo == datos.Codigo && x.Id != id, ct)) return Conflict("Ya existe una ficha con ese código."); if (await context.FichasTecnicas.AnyAsync(x => x.Referencia == datos.Referencia && x.Id != id, ct)) return Conflict("Ya existe una ficha con esa referencia.");
        ficha.Actualizar(datos.Codigo, datos.Referencia, datos.Categoria, datos.Linea, datos.Estado, null, null, null, null); await context.SaveChangesAsync(ct); return Ok(Map(ficha));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    { var ficha = await context.FichasTecnicas.FindAsync([id], ct); if (ficha is null) return NotFound(); context.FichasTecnicas.Remove(ficha); await context.SaveChangesAsync(ct); return NoContent(); }

    [HttpPost("{id:guid}/atributos")]
    public async Task<ActionResult<AtributoResponse>> CrearAtributo(Guid id, AtributoRequest request, CancellationToken ct)
    {
        if (!await context.FichasTecnicas.AnyAsync(x => x.Id == id, ct)) return NotFound();
        var datos = ValidarAtributo(request);
        if (await context.FichasTecnicasAtributos.AnyAsync(x => x.FichaTecnicaId == id && x.Atributo == datos.Atributo, ct)) return Conflict("Ya existe un atributo con ese nombre.");
        var atributo = new ChinaVenezuela.Domain.FichasTecnicas.AtributoFichaTecnica(id, datos.Atributo, datos.Valor, datos.Observacion, datos.ComposicionTela, datos.ColorParaFabricar, datos.MarcaProducto, datos.CurvaTalla); context.FichasTecnicasAtributos.Add(atributo); await context.SaveChangesAsync(ct); return Created($"api/fichas-tecnicas/{id}/atributos/{atributo.Id}", Map(atributo));
    }

    [HttpPut("{id:guid}/atributos/{atributoId:guid}")]
    public async Task<ActionResult<AtributoResponse>> ActualizarAtributo(Guid id, Guid atributoId, AtributoRequest request, CancellationToken ct)
    {
        var atributo = await context.FichasTecnicasAtributos.SingleOrDefaultAsync(x => x.Id == atributoId && x.FichaTecnicaId == id, ct); if (atributo is null) return NotFound();
        var datos = ValidarAtributo(request);
        if (await context.FichasTecnicasAtributos.AnyAsync(x => x.FichaTecnicaId == id && x.Atributo == datos.Atributo && x.Id != atributoId, ct)) return Conflict("Ya existe un atributo con ese nombre.");
        atributo.Actualizar(datos.Atributo, datos.Valor, datos.Observacion, datos.ComposicionTela, datos.ColorParaFabricar, datos.MarcaProducto, datos.CurvaTalla); await context.SaveChangesAsync(ct); return Ok(Map(atributo));
    }

    [HttpDelete("{id:guid}/atributos/{atributoId:guid}")]
    public async Task<IActionResult> EliminarAtributo(Guid id, Guid atributoId, CancellationToken ct)
    { var atributo = await context.FichasTecnicasAtributos.SingleOrDefaultAsync(x => x.Id == atributoId && x.FichaTecnicaId == id, ct); if (atributo is null) return NotFound(); context.FichasTecnicasAtributos.Remove(atributo); await context.SaveChangesAsync(ct); return NoContent(); }

    [HttpPut("{id:guid}/imagen"), Consumes("multipart/form-data")]
    public async Task<ActionResult> GuardarImagen(Guid id, [FromForm] IFormFile? imagen, CancellationToken ct)
    {
        var ficha = await context.FichasTecnicas.FindAsync([id], ct); if (ficha is null) return NotFound();
        if (imagen is null || imagen.Length == 0 || imagen.Length > 2 * 1024 * 1024) return BadRequest("Selecciona una imagen JPEG, PNG o WebP de máximo 2 MB.");
        await using var memoria = new MemoryStream(); await imagen.CopyToAsync(memoria, ct); var datos = memoria.ToArray();
        var tipo = DetectarTipo(datos); if (tipo is null) return BadRequest("La imagen debe ser JPEG, PNG o WebP válida.");
        ficha.ActualizarImagen(datos, tipo); await context.SaveChangesAsync(ct); return NoContent();
    }

    [HttpGet("{id:guid}/imagen")]
    public async Task<IActionResult> ObtenerImagen(Guid id, CancellationToken ct)
    { var ficha = await context.FichasTecnicas.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct); return ficha?.ImagenDatos is null ? NotFound() : File(ficha.ImagenDatos, ficha.ImagenTipoContenido ?? "image/jpeg"); }

    private static (string Codigo, string Referencia, string Categoria, string Linea, string Estado) Validar(FichaRequest request)
    { var codigo = request.Codigo?.Trim(); var referencia = request.Referencia?.Trim(); var categoria = request.Categoria?.Trim(); var linea = request.Linea?.Trim(); var estado = request.Estado?.Trim(); if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(referencia) || string.IsNullOrWhiteSpace(categoria) || string.IsNullOrWhiteSpace(linea) || (estado is not "Activo" and not "Inactivo")) throw new BadHttpRequestException("Código, categoría, línea y estado son obligatorios."); return (codigo, referencia, categoria, linea, estado); }
    private static (string Atributo, string Valor, string? Observacion, string? ComposicionTela, string? ColorParaFabricar, string? MarcaProducto, string? CurvaTalla) ValidarAtributo(AtributoRequest request)
    { var atributo = request.Atributo?.Trim(); var valor = request.Valor?.Trim(); if (string.IsNullOrWhiteSpace(atributo) || string.IsNullOrWhiteSpace(valor)) throw new BadHttpRequestException("Atributo y valor son obligatorios."); return (atributo, valor, string.IsNullOrWhiteSpace(request.Observacion) ? null : request.Observacion.Trim(), request.ComposicionTela?.Trim(), request.ColorParaFabricar?.Trim(), request.MarcaProducto?.Trim(), request.CurvaTalla?.Trim()); }
    private static string? DetectarTipo(byte[] d) => d.Length >= 3 && d[0] == 0xFF && d[1] == 0xD8 && d[2] == 0xFF ? "image/jpeg" : d.Length >= 8 && d.Take(8).SequenceEqual(new byte[] { 137,80,78,71,13,10,26,10 }) ? "image/png" : d.Length >= 12 && d.Take(4).SequenceEqual("RIFF"u8.ToArray()) && d.Skip(8).Take(4).SequenceEqual("WEBP"u8.ToArray()) ? "image/webp" : null;
    private static FichaResponse Map(ChinaVenezuela.Domain.FichasTecnicas.FichaTecnica x) => new(x.Id, x.Codigo, x.Referencia, x.Categoria, x.Linea, x.Estado, x.ImagenDatos is not null, x.Atributos.OrderBy(a => a.Atributo).Select(Map).ToArray());
    private static AtributoResponse Map(ChinaVenezuela.Domain.FichasTecnicas.AtributoFichaTecnica x) => new(x.Id, x.Atributo, x.Valor, x.Observacion, x.ComposicionTela, x.ColorParaFabricar, x.MarcaProducto, x.CurvaTalla);
    public sealed record FichaRequest(string Codigo, string Referencia, string Categoria, string Linea, string Estado);
    public sealed record AtributoRequest(string Atributo, string Valor, string? Observacion, string? ComposicionTela, string? ColorParaFabricar, string? MarcaProducto, string? CurvaTalla);
    public sealed record AtributoResponse(Guid Id, string Atributo, string Valor, string? Observacion, string? ComposicionTela, string? ColorParaFabricar, string? MarcaProducto, string? CurvaTalla);
    public sealed record FichaResponse(Guid Id, string Codigo, string Referencia, string Categoria, string Linea, string Estado, bool TieneImagen, IReadOnlyList<AtributoResponse> Atributos);
}