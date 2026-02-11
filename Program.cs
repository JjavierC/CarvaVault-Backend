using Microsoft.EntityFrameworkCore;
using CarvaVault_API.Data;
using Npgsql;

// --- PARCHE CRÍTICO PARA POSTGRES ---
// Esta línea soluciona el error 500 al guardar fechas (timestamps)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE LA BASE DE DATOS ---
// Priorizamos la variable de Render, si no, usa el appsettings local
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

try 
{
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("❌ CRÍTICO: La cadena de conexión está vacía.");
    }

    // Log para verificar el Host en Render (ocultando la contraseña)
    Console.WriteLine($"🚀 Iniciando conexión hacia: {connectionString.Split(';')[0]}...");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ ERROR DE CONFIGURACIÓN: {ex.Message}");
}

// --- 2. CONFIGURACIÓN DE CORS (REACT / ANDROID) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

// --- 3. AUTO-MIGRACIÓN (Crea las tablas si no existen) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Esto asegura que las tablas 'Usuarios' y 'Pagos' se creen en Render
        context.Database.EnsureCreated();
        Console.WriteLine("✅ ¡ESTADO VERDE! Base de datos sincronizada correctamente.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ ERROR EN DB: Verifica las credenciales en Render.");
    }
}

// --- 4. MIDDLEWARES ---
app.UseCors("AllowReactApp"); 
app.UseAuthorization();
app.MapControllers();

app.Run();