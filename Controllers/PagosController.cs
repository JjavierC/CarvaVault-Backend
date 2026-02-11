using Microsoft.AspNetCore.Mvc;
using CarvaVault_API.Models;
using CarvaVault_API.Data;
using Microsoft.EntityFrameworkCore;

namespace CarvaVault_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PagosController(AppDbContext context)
    {
        _context = context;
    }

    // --- LOGIN ---
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        // CAMBIO CLAVE: Usamos req.Username en vez de req.Usuario
        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Username.ToLower() == req.Username.ToLower() && u.Password == req.Password);

        if (user != null)
        {
            return Ok(new { 
                mensaje = "Bienvenido", 
                usuario = user.Username, // Devolvemos el nombre real
                rol = user.Rol 
            });
        }
        return Unauthorized(new { mensaje = "Credenciales incorrectas" });
    }

    // --- GUARDAR PAGO ---
    [HttpPost("verificar")]
    public async Task<IActionResult> RecibirPago([FromBody] Pago pagoData)
    {
        if (!Request.Headers.TryGetValue("X-Carva-Key", out var key) || key != "Tu_Clave_Secreta_Barranquilla_2026")
            return Unauthorized(new { mensaje = "Llave maestra incorrecta" });

        _context.Pagos.Add(pagoData);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Pago registrado en DB" });
    }

    // --- HISTORIAL ---
    [HttpGet("historial/{clienteId}")]
    public async Task<IActionResult> ObtenerHistorial(string clienteId)
    {
        var historial = await _context.Pagos
            .Where(p => p.ClienteId.ToLower() == clienteId.ToLower())
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();

        return Ok(historial);
    }
    
    // --- CREAR USUARIO (SOLO PARA PRUEBAS INICIALES) ---
    [HttpPost("crear-usuario")]
    public async Task<IActionResult> CrearUsuario([FromBody] Usuario nuevoUser)
    {
        // Verificar si ya existe
        var existe = await _context.Usuarios.AnyAsync(u => u.Username == nuevoUser.Username);
        if (existe) return BadRequest("El usuario ya existe");

        _context.Usuarios.Add(nuevoUser);
        await _context.SaveChangesAsync();
        return Ok(new { mensaje = $"Usuario {nuevoUser.Username} creado!" });
    }
}

// --- DTO PARA EL LOGIN (DEFINIDO AQUÍ MISMO) ---
public class LoginRequest
{
    // LE CAMBIAMOS EL NOMBRE AQUI PARA QUE NO CHOQUE
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}