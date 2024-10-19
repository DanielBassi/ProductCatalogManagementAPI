using System.Reflection;
using Api.ApiHandlers;
using Api.Filters;
using Api.Middleware;
using FluentValidation;
using Infrastructure.DataSource;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;
using Serilog.Debugging;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

/* Registrar todos los validadores de FluentValidation encontrados en el mismo ensamblado que la clase Program */
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

/* Obtener nombre de la DB en el archivo appsettings.json */
var connectionString = builder.Configuration.GetConnectionString("db");

/* Configurar el contexto de base de datos DataContext para que use MySQL con Entity Framework Core */
builder.Services.AddDbContext<DataContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 33))));

/* Configuración de verificaciones de salud (health checks) para monitorizar el estado de la base de datos y exponer esa información a Prometheus */
builder.Services.AddHealthChecks().AddDbContextCheck<DataContext>().ForwardToPrometheus();

builder.Services.AddDomainServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Configurar MediatR */
builder.Services.AddMediatR(Assembly.Load("Application"), typeof(Program).Assembly);

/* Configurar Serilog como el sistema de logging de la aplicación y le indica que los mensajes de log se envíen a la consola */
builder.Host.UseSerilog((_, loggerConfiguration) => loggerConfiguration.WriteTo.Console());

/* Habilitar el autolog de Serilog (SelfLog) y redirigir cualquier error interno de Serilog a la salida estándar de errores (Console.Error) */
SelfLog.Enable(Console.Error);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseHttpMetrics();

/* Configuración del Middleware para darle manejo a las excepciones de la aplicación */
app.UseMiddleware<AppExceptionHandlerMiddleware>();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.UseRouting().UseEndpoints(endpoint => { endpoint.MapMetrics(); });

app.MapGroup("/api/product").MapProduct().AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory);

app.Run();
