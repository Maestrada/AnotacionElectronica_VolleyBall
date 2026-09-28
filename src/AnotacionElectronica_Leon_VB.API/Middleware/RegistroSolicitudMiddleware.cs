namespace AnotacionElectronica_Leon_VB.API.Middleware;

public class RegistroSolicitudMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RegistroSolicitudMiddleware> _logger;
    public RegistroSolicitudMiddleware(RequestDelegate next, ILogger<RegistroSolicitudMiddleware> logger) => (_next, _logger) = (next, logger);

    public async Task InvokeAsync(HttpContext context)
    {
        var started = DateTime.UtcNow;
        try
        {
            await _next(context);
            _logger.LogInformation("HTTP {Method} {Path} respondió {StatusCode} en {ElapsedMs} ms desde {RemoteIp}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode,
                (DateTime.UtcNow - started).TotalMilliseconds, context.Connection.RemoteIpAddress?.ToString() ?? "sin-ip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en HTTP {Method} {Path} desde {RemoteIp}",
                context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress?.ToString() ?? "sin-ip");
            throw;
        }
    }
}
