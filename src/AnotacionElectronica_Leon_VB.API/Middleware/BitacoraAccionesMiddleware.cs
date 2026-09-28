using System.Text.Json;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Infraestructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AnotacionElectronica_Leon_VB.API.Middleware;

public class BitacoraAccionesMiddleware
{
    private readonly RequestDelegate _next;
    public BitacoraAccionesMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext db, ILogger<BitacoraAccionesMiddleware> logger)
    {
        await _next(context);
        if (!HttpMethods.IsPost(context.Request.Method) && !HttpMethods.IsPut(context.Request.Method) && !HttpMethods.IsPatch(context.Request.Method) && !HttpMethods.IsDelete(context.Request.Method)) return;
        if (context.Request.Path.StartsWithSegments("/api/seguridad/bitacora")) return;

        try
        {
            Guid? usuarioId = Guid.TryParse(context.Request.Headers["X-Usuario-Id"], out var id) && await db.UsuariosSistema.AnyAsync(x => x.Id == id) ? id : null;
            var usuario = context.User.Identity?.Name ?? context.Request.Headers["X-Usuario-Nombre"].FirstOrDefault() ?? "Sistema";
            var detalle = JsonSerializer.Serialize(new { consulta = context.Request.QueryString.Value, longitudContenido = context.Request.ContentLength });
            db.BitacoraAcciones.Add(new BitacoraAccion(usuarioId, usuario, $"{context.Request.Method} {context.Request.Path}", context.Request.Method, context.Request.Path, context.Response.StatusCode, context.Connection.RemoteIpAddress?.ToString(), detalle));
            await db.SaveChangesAsync();
        }
        catch (Exception ex) { logger.LogWarning(ex, "No fue posible guardar una acción en la bitácora."); }
    }
}
