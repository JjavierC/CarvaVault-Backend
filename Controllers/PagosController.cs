using Microsoft.AspNetCore.Mvc;
using CarvaVault_API.Models;

namespace CarvaVault_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private static readonly List<PagoRequest> PagosRecibidos = new();

    // POST: api/Pagos/verificar
    [HttpPost("verificar")]
    public IActionResult RecibirPago([FromBody] PagoRequest pago)
    {
        // 1. SEGURIDAD: Validar la llave secreta
        // Si el Header no trae "X-Carva-Key" o la clave no coincide, rechaza la entrada.
        if (!Request.Headers.TryGetValue("X-Carva-Key", out var extractedKey) || 
            extractedKey != "Tu_Clave_Secreta_Barranquilla_2026")
        {
            return Unauthorized(new { mensaje = "Acceso denegado: Llave incorrecta" });
        }

        // 2. VALIDACIÓN DE DATOS
        if (pago == null || pago.Monto <= 0)
            return BadRequest(new { mensaje = "Datos de pago inválidos" });

        // 3. REGISTRO EXITOSO
        PagosRecibidos.Add(pago);
        return Ok(new { mensaje = "Pago registrado en la bóveda", fecha = DateTime.Now });
    }

    // GET: api/Pagos/historial
    [HttpGet("historial")]
    public IActionResult ObtenerPagos()
    {
        return Ok(PagosRecibidos);
    }
}