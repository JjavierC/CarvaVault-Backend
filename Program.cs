using Microsoft.EntityFrameworkCore;
using CarvaVault_API.Data; // Asegúrate de que este namespace coincida con tu carpeta Data

var builder = WebApplication.CreateBuilder(args);


var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

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

// --- 3. SERVICIOS DE CONTROLADORES ---
builder.Services.AddControllers();

var app = builder.Build();

// --- 4. AUTO-CREACIÓN DE TABLAS (MIGRACIÓN) ---
// Esto es vital: Cada vez que Render inicia, revisa si la DB existe.
// Si es nueva, crea las tablas automáticamente.
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
        // Si falla, lo imprime en los logs de Render para que sepamos qué pasó
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error fatal al conectar la Base de Datos.");
    }
}
// ----------------------------------------------

// --- 5. MIDDLEWARE (TUBERÍA) ---
// Aplicar la política de seguridad antes de los controladores
app.UseCors("AllowReactApp"); 

app.UseAuthorization();
app.MapControllers();

app.Run();