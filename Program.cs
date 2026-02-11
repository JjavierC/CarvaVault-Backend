using Microsoft.EntityFrameworkCore;
using CarvaVault_API.Data;
using Npgsql; // Asegúrate de que este using no esté en gris/error

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONEXIÓN DIRECTA ---
// Tomamos la variable que acabas de poner en Render
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

try 
{
    if (string.IsNullOrEmpty(connectionString))
        throw new Exception("❌ LA VARIABLE DB_CONNECTION_STRING ESTÁ VACÍA.");

    // Imprimimos un pedacito de la cadena para verificar en los logs (sin mostrar la clave)
    Console.WriteLine($"✅ Conectando a Postgres: {connectionString.Split(';')[0]}");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ ERROR FATAL CONFIGURANDO DB: {ex.Message}");
}

// --- 2. CORS (REACT) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

// --- 3. AUTO-MIGRACIÓN ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated(); // ¡ESTO CREA LAS TABLAS!
        Console.WriteLine("✅ ¡ÉXITO! Base de Datos conectada y tablas listas.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ NO SE PUDO CONECTAR A LA BASE DE DATOS.");
    }
}

app.UseCors("AllowReactApp"); 
app.UseAuthorization();
app.MapControllers();

app.Run();