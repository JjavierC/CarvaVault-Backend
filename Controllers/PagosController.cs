using Microsoft.AspNetCore.Mvc;
using CarvaVault_API.Models;
using CarvaVault_API.Data;
using Microsoft.EntityFrameworkCore;

namespace CarvaVault_API.Controllers;

// --- DTO PARA EL LOGIN (DEFINIDO AQUÍ ARRIBA PARA QUE SE VEA CLARO) ---
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PagosController(AppDbContext context)
    {
        _context = context;
    }

    // --- 1. LOGIN ---
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        // Buscamos usuario comparando Username (Base de Datos) vs Username (Lo que envías)
        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Username.ToLower() == req.Username.ToLower() && u.Password == req.Password);

        if (user != null)
        {
            return Ok(new { 
                mensaje = "Bienvenido al sistema", 
                usuario = user.Username, 
                rol = user.Rol 
            });
        }
        return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario no encontrado" });
    }

    // --- 2. GUARDAR PAGO (DESDE ANDROID) ---
    [HttpPost("verificar")]
    public async Task<IActionResult> RecibirPago([FromBody] Pago pagoData)
    {
        // Validación de seguridad simple
        if (!Request.Headers.TryGetValue("X-Carva-Key", out var key) || key != "Tu_Clave_Secreta_Barranquilla_2026")
            return Unauthorized(new { mensaje = "Acceso denegado: Llave maestra incorrecta" });

        // Guardar en Base de Datos Real
        _context.Pagos.Add(pagoData);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Pago registrado exitosamente en PostgreSQL" });
    }

    // --- 3. HISTORIAL (PARA EL DASHBOARD) ---
    [HttpGet("historial/{clienteId}")]
    public async Task<IActionResult> ObtenerHistorial(string clienteId)
    {
        var historial = await _context.Pagos
            .Where(p => p.ClienteId.ToLower() == clienteId.ToLower())
            .OrderByDescending(p => p.Fecha) // Los más nuevos primero
            .ToListAsync();

        return Ok(historial);
    }
    
    // --- 4. CREAR USUARIO (SOLO PARA ADMIN/POSTMAN) ---
    [HttpPost("crear-usuario")]
    public async Task<IActionResult> CrearUsuario([FromBody] Usuario nuevoUser)
    {
        // Evitar duplicados
        var existe = await _context.Usuarios.AnyAsync(u => u.Username.ToLower() == nuevoUser.Username.ToLower());
        if (existe) return BadRequest(new { mensaje = "El usuario ya existe" });

        _context.Usuarios.Add(nuevoUser);
        await _context.SaveChangesAsync();
        
        return Ok(new { mensaje = $"¡Usuario {nuevoUser.Username} creado con éxito!" });
    }
}