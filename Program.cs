using Microsoft.EntityFrameworkCore;
using CarvaVault_API.Data;
using Npgsql; 

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONEXIÓN A BASE DE DATOS ---
// Leemos la variable. Si seguiste el paso 1, esto funcionará directo.
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

try 
{
    // Validación de seguridad básica
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("❌ CRÍTICO: No se encontró la variable DB_CONNECTION_STRING");
    }

    Console.WriteLine($"✅ Intentando conectar a la base de datos: {connectionString.Split(';')[0]}..."); // Solo mostramos el Host por seguridad

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Error configurando la DB: {ex.Message}");
}
// -----------------------------------

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

// --- 3. AUTO-MIGRACIÓN (Crea la DB si no existe) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        Console.WriteLine("✅ ¡ÉXITO! Base de datos conectada y lista.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ ERROR FATAL: No se pudo conectar a la Base de Datos.");
    }
}

app.UseCors("AllowReactApp"); 
app.UseAuthorization();
app.MapControllers();

app.Run();