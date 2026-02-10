var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN DE CORS ---
// Esto permite que tu React (localhost) lea los datos de Render
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

// Aplicar la política de seguridad antes de los controladores
app.UseCors("AllowReactApp"); 

app.UseAuthorization();
app.MapControllers();

app.Run();