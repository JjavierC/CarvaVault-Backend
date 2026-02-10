using Microsoft.AspNetCore.Mvc;
using CarvaVault_API.Models;

namespace CarvaVault_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    // Lista en memoria (se reinicia si Render apaga el servicio)
    private static readonly List<PagoRequest> PagosRecibidos = new();

    // POST: api/Pagos/verificar
    [HttpPost("verificar")]
    public IActionResult RecibirPago([FromBody] PagoRequest pago)
    {
        if (pago == null || pago.Monto <= 0)
            return BadRequest(new { mensaje = "Datos de pago inválidos" });

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