using AnotacionElectronica_Leon_VB.Infraestructure;
using AnotacionElectronica_Leon_VB.Infraestructure.Context;
using AnotacionElectronica_Leon_VB.Application.Services;
using AnotacionElectronica_Leon_VB.API.Hubs;
using AnotacionElectronica_Leon_VB.API.Middleware;
using AnotacionElectronica_Leon_VB.API.Logging;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddPhysicalFileLogging(builder.Environment.ContentRootPath, builder.Configuration.GetSection("FileLogging"));
var configuredUrls = builder.Configuration["Hosting:Urls"];
if (!string.IsNullOrWhiteSpace(configuredUrls))
{
    builder.WebHost.UseUrls(configuredUrls);
}
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Agregar SignalR y CORS
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true) // Permitir conexiones frontend (dev)
              .AllowCredentials();          // Requerido por WebSockets / SignalR
    });
});

// Registrar Controladores y Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyección de dependencias de la capa de Infraestructura
builder.Services.AddInfrastructure(builder.Configuration);

// Servicios de Aplicación
builder.Services.AddScoped<PartidoService>();
builder.Services.AddScoped<CalendarioService>();
builder.Services.AddScoped<EquipoService>();
builder.Services.AddScoped<ArbitroService>();
builder.Services.AddScoped<CompeticionService>();
builder.Services.AddScoped<ReglamentoService>();
builder.Services.AddScoped<IPartidoService>(sp => ServiceLoggingProxy<IPartidoService>.Create(sp.GetRequiredService<PartidoService>(), sp.GetRequiredService<ILoggerFactory>()));
builder.Services.AddScoped<ICalendarioService>(sp => ServiceLoggingProxy<ICalendarioService>.Create(sp.GetRequiredService<CalendarioService>(), sp.GetRequiredService<ILoggerFactory>()));
builder.Services.AddScoped<IEquipoService>(sp => ServiceLoggingProxy<IEquipoService>.Create(sp.GetRequiredService<EquipoService>(), sp.GetRequiredService<ILoggerFactory>()));
builder.Services.AddScoped<IArbitroService>(sp => ServiceLoggingProxy<IArbitroService>.Create(sp.GetRequiredService<ArbitroService>(), sp.GetRequiredService<ILoggerFactory>()));
builder.Services.AddScoped<ICompeticionService>(sp => ServiceLoggingProxy<ICompeticionService>.Create(sp.GetRequiredService<CompeticionService>(), sp.GetRequiredService<ILoggerFactory>()));
builder.Services.AddScoped<IReglamentoService>(sp => ServiceLoggingProxy<IReglamentoService>.Create(sp.GetRequiredService<ReglamentoService>(), sp.GetRequiredService<ILoggerFactory>()));
builder.Services.AddScoped<IPasswordHasher<UsuarioSistema>, PasswordHasher<UsuarioSistema>>();

var app = builder.Build();

// Inicialización de base de datos y presets
using (var scope = app.Services.CreateScope())
{
    var regService = scope.ServiceProvider.GetRequiredService<IReglamentoService>();
    try
    {
        await regService.AsegurarReglamentosPorDefectoAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Aviso durante la inicialización de presets de reglamentos.");
    }
}

// 2. Configurar el Middleware HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<RegistroSolicitudMiddleware>();
app.UseMiddleware<BitacoraAccionesMiddleware>();
app.MapControllers();

// Mapear ruta del Hub
app.MapHub<PartidoHub>("/hubs/partidos");

app.Run();
