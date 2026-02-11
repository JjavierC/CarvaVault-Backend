using Microsoft.EntityFrameworkCore;
using CarvaVault_API.Data;
using Npgsql; 

var builder = WebApplication.CreateBuilder(args);

// --- 1. DETECCIÓN Y TRADUCCIÓN DE LA CONEXIÓN ---
var rawConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

string connectionString = "";

try 
{
    if (string.IsNullOrEmpty(rawConnectionString))
    {
        throw new Exception("❌ LA VARIABLE DB_CONNECTION_STRING ESTÁ VACÍA EN RENDER.");
    }

    // Limpiamos espacios basura
    rawConnectionString = rawConnectionString.Trim();

    // ¿Viene en formato Render (postgres://)? -> Lo traducimos
    if (rawConnectionString.StartsWith("postgres://"))
    {
        var uri = new Uri(rawConnectionString);
        var db = uri.AbsolutePath.Trim('/');
        var user = uri.UserInfo.Split(':')[0];
        var passwd = uri.UserInfo.Split(':')[1];
        var port = uri.Port > 0 ? uri.Port : 5432;
        var host = uri.Host;

        // Armamos el formato C# (Host=...;Database=...)
        connectionString = $"Host={host};Database={db};Username={user};Password={passwd};Port={port}";
        Console.WriteLine("✅ URL de Render detectada y traducida correctamente.");
    }
    else 
    {
        // Si ya viene bien (Local), la usamos tal cual
        connectionString = rawConnectionString;
    }
}
catch (Exception ex)
{
    // Si falla algo, mostramos el error en los logs de Render
    Console.WriteLine($"⚠️ ERROR CRÍTICO ARMANDO LA CONEXIÓN: {ex.Message}");
    // Intentamos usarla como venga por si acaso
    connectionString = rawConnectionString;
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
// ------------------------------------------------------

// --- 2. CONFIGURACIÓN DE CORS (REACT) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

// --- 3. AUTO-CREACIÓN DE TABLAS (MIGRACIÓN AL INICIAR) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated(); // Esto crea las tablas si no existen
        Console.WriteLine("✅ BASE DE DATOS CONECTADA Y TABLAS VERIFICADAS.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ ERROR FATAL INTENTANDO CREAR LA BASE DE DATOS.");
    }
}

// --- 4. MIDDLEWARES ---
app.UseCors("AllowReactApp"); 
app.UseAuthorization();
app.MapControllers();

app.Run();