using AnotacionElectronica_Leon_VB.Infraestructure;
using AnotacionElectronica_Leon_VB.Infraestructure.Context;
using AnotacionElectronica_Leon_VB.Application.Services;
using AnotacionElectronica_Leon_VB.API.Hubs;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IPartidoService, PartidoService>();
builder.Services.AddScoped<ICalendarioService, CalendarioService>();
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IArbitroService, ArbitroService>();
builder.Services.AddScoped<ICompeticionService, CompeticionService>();
builder.Services.AddScoped<IReglamentoService, ReglamentoService>();

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
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Mapear ruta del Hub
app.MapHub<PartidoHub>("/hubs/partidos");

app.Run();
