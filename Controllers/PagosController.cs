using Microsoft.AspNetCore.Mvc;
using CarvaVault_API.Models;
using System.Collections.Concurrent; // Usamos Concurrent para evitar choques de hilos

namespace CarvaVault_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    // 1. GESTIÓN DE USUARIOS (Simulando base de datos)
    // Aquí agregas a tus clientes cuando te paguen la suscripción
    private static readonly Dictionary<string, string> Usuarios = new()
    {
        { "admin", "admin123" },       // Tu acceso maestro
        { "ruta66", "hamburguesa" },   // Cliente 1
        { "donjediondo", "sopita" },   // Cliente 2
        { "wil", "millonario" }        // Tus pruebas
    };

    // 2. LA BÓVEDA MULTI-TENANT
    // Estructura: "ID_CLIENTE" -> [Lista de Pagos]
    private static readonly ConcurrentDictionary<string, List<PagoRequest>> Bovedas = new();

    // LOGIN: El Frontend llama a esto para entrar
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        if (Usuarios.TryGetValue(req.Usuario.ToLower(), out var passReal) && passReal == req.Password)
        {
            // Si entra, le devolvemos su ID para que el Front sepa qué pedir
            return Ok(new { 
                mensaje = "Acceso Concedido", 
                token = Guid.NewGuid().ToString(), // Simulamos seguridad pro
                usuario = req.Usuario.ToLower() 
            });
        }
        return Unauthorized(new { mensaje = "Credenciales Inválidas" });
    }

    // VERIFICAR: La App Android llama a esto
    [HttpPost("verificar")]
    public IActionResult RecibirPago([FromBody] PagoRequest pago)
    {
        // 1. Seguridad Global (Tu llave maestra)
        if (!Request.Headers.TryGetValue("X-Carva-Key", out var key) || key != "Tu_Clave_Secreta_Barranquilla_2026")
            return Unauthorized(new { mensaje = "Intruso detectado: Llave incorrecta" });

        // 2. Normalizar ID del cliente
        string clienteId = string.IsNullOrEmpty(pago.ClienteId) ? "generico" : pago.ClienteId.ToLower();

        // 3. Crear la caja fuerte si es el primer pago de este cliente
        if (!Bovedas.ContainsKey(clienteId))
        {
            Bovedas[clienteId] = new List<PagoRequest>();
        }

        // 4. Guardar el pago en SU caja
        Bovedas[clienteId].Add(pago);

        Console.WriteLine($"[SaaS] Pago de {pago.Monto:C} recibido para el cliente: {clienteId}");
        return Ok(new { mensaje = $"Pago asegurado en la bóveda de {clienteId}" });
    }

    // HISTORIAL: El Frontend pide SUS datos
    [HttpGet("historial/{clienteId}")]
    public IActionResult ObtenerHistorial(string clienteId)
    {
        string id = clienteId.ToLower();
        
        // Si la bóveda existe, devolvemos sus pagos. Si no, lista vacía.
        if (Bovedas.TryGetValue(id, out var pagos))
        {
            // Devolvemos los más recientes primero
            return Ok(pagos.OrderByDescending(p => p.Fecha));
        }
        
        return Ok(new List<PagoRequest>());
    }
}