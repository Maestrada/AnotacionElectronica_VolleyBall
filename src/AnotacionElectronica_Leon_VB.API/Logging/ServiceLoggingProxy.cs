using System.Diagnostics;
using System.Reflection;

namespace AnotacionElectronica_Leon_VB.API.Logging;

/// <summary>Intercepción centralizada de servicios de aplicación sin registrar argumentos sensibles.</summary>
public class ServiceLoggingProxy<TService> : DispatchProxy where TService : class
{
    private TService _service = null!;
    private ILogger _logger = null!;

    public static TService Create(TService service, ILoggerFactory loggerFactory)
    {
        var proxy = DispatchProxy.Create<TService, ServiceLoggingProxy<TService>>();
        var typedProxy = (ServiceLoggingProxy<TService>)(object)proxy;
        typedProxy._service = service;
        typedProxy._logger = loggerFactory.CreateLogger($"Servicio.{service.GetType().Name}");
        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod is null) throw new ArgumentNullException(nameof(targetMethod));
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Servicio {Service}.{Method} iniciado", _service.GetType().Name, targetMethod.Name);
        try
        {
            var result = targetMethod.Invoke(_service, args);
            if (result is Task task)
            {
                if (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var resultType = targetMethod.ReturnType.GenericTypeArguments[0];
                    return typeof(ServiceLoggingProxy<TService>).GetMethod(nameof(TrackGenericTask), BindingFlags.Instance | BindingFlags.NonPublic)!
                        .MakeGenericMethod(resultType).Invoke(this, new object?[] { task, targetMethod, stopwatch });
                }
                return TrackTask(task, targetMethod, stopwatch);
            }

            LogSuccess(targetMethod, stopwatch);
            return result;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            LogError(targetMethod, stopwatch, ex.InnerException);
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
        catch (Exception ex) { LogError(targetMethod, stopwatch, ex); throw; }
    }

    private async Task TrackTask(Task task, MethodInfo method, Stopwatch stopwatch)
    {
        try { await task.ConfigureAwait(false); LogSuccess(method, stopwatch); }
        catch (Exception ex) { LogError(method, stopwatch, ex); throw; }
    }

    private async Task<TResult> TrackGenericTask<TResult>(Task<TResult> task, MethodInfo method, Stopwatch stopwatch)
    {
        try { var result = await task.ConfigureAwait(false); LogSuccess(method, stopwatch); return result; }
        catch (Exception ex) { LogError(method, stopwatch, ex); throw; }
    }

    private void LogSuccess(MethodInfo method, Stopwatch stopwatch) => _logger.LogInformation(
        "Servicio {Service}.{Method} completado en {ElapsedMs} ms", _service.GetType().Name, method.Name, stopwatch.ElapsedMilliseconds);
    private void LogError(MethodInfo method, Stopwatch stopwatch, Exception exception) => _logger.LogError(exception,
        "Servicio {Service}.{Method} falló en {ElapsedMs} ms", _service.GetType().Name, method.Name, stopwatch.ElapsedMilliseconds);
}
