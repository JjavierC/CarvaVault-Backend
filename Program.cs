using Microsoft.EntityFrameworkCore;
using CarvaVault_API.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE BASE DE DATOS (CON TRADUCTOR INTELIGENTE) ---
var rawConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

// TRADUCTOR: Si Render nos da una URL (postgres://), la convertimos a Connection String de C#
string connectionString = rawConnectionString;

if (!string.IsNullOrEmpty(rawConnectionString) && rawConnectionString.StartsWith("postgres://"))
{
    try 
    {
        var uri = new Uri(rawConnectionString);
        var db = uri.AbsolutePath.Trim('/');
        var user = uri.UserInfo.Split(':')[0];
        var passwd = uri.UserInfo.Split(':')[1];
        var port = uri.Port > 0 ? uri.Port : 5432;
        var host = uri.Host;
        
        // Armamos el formato que le gusta a C#
        connectionString = $"Host={host};Database={db};Username={user};Password={passwd};Port={port}";
    }
    catch 
    { 
        // Si falla la traducción, usamos la original (por si acaso)
        connectionString = rawConnectionString; 
    }
}

// Conectamos usando la cadena ya traducida
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
// -----------------------------------------------------------------------

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

// --- 3. AUTO-CREACIÓN DE TABLAS (MIGRACIÓN) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated(); // ¡MAGIA! Crea la DB si no existe
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error fatal al conectar la Base de Datos.");
    }
}

app.UseCors("AllowReactApp"); 
app.UseAuthorization();
app.MapControllers();

app.Run();