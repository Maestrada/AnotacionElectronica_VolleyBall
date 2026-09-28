using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AnotacionElectronica_Leon_VB.API.Logging;

/// <summary>Proveedor liviano de archivos con rotación diaria para diagnósticos locales y de servidor.</summary>
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _directory;
    private readonly LogLevel _minimumLevel;
    private readonly int _retainedDays;
    private readonly object _sync = new();

    public FileLoggerProvider(string contentRootPath, IConfiguration configuration)
    {
        var relativePath = configuration["Path"] ?? "logs";
        _directory = Path.GetFullPath(Path.Combine(contentRootPath, relativePath));
        _minimumLevel = Enum.TryParse<LogLevel>(configuration["MinimumLevel"], true, out var level) ? level : LogLevel.Information;
        _retainedDays = int.TryParse(configuration["RetainedDays"], out var days) ? Math.Max(days, 1) : 30;
        Directory.CreateDirectory(_directory);
        RemoveExpiredFiles();
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, this);
    public void Dispose() { }

    private void Write(LogLevel level, string category, EventId eventId, string message, Exception? exception)
    {
        if (level < _minimumLevel || level == LogLevel.None) return;
        var line = $"{DateTime.UtcNow:O} [{level}] {category} ({eventId.Id}) {message}";
        if (exception is not null) line += Environment.NewLine + exception;
        var path = Path.Combine(_directory, $"aplicacion-{DateTime.UtcNow:yyyy-MM-dd}.log");
        lock (_sync)
        {
            File.AppendAllText(path, line + Environment.NewLine, System.Text.Encoding.UTF8);
        }
    }

    private void RemoveExpiredFiles()
    {
        var limit = DateTime.UtcNow.Date.AddDays(-_retainedDays);
        foreach (var file in Directory.EnumerateFiles(_directory, "aplicacion-*.log"))
        {
            if (File.GetLastWriteTimeUtc(file).Date < limit) File.Delete(file);
        }
    }

    private sealed class FileLogger : ILogger
    {
        private readonly string _category;
        private readonly FileLoggerProvider _provider;
        public FileLogger(string category, FileLoggerProvider provider) => (_category, _provider) = (category, provider);
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => logLevel >= _provider._minimumLevel && logLevel != LogLevel.None;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel)) _provider.Write(logLevel, _category, eventId, formatter(state, exception), exception);
        }
    }
}

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddPhysicalFileLogging(this ILoggingBuilder logging, string contentRootPath, IConfiguration configuration)
    {
        logging.AddProvider(new FileLoggerProvider(contentRootPath, configuration));
        return logging;
    }
}
