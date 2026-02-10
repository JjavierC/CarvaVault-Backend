using Microsoft.AspNetCore.Mvc;

namespace CarvaVault_API.Controllers;

[ApiController]
[Route("api/[controller]")] // Esto crea la ruta /api/Pagos
public class PagosController : ControllerBase
{
    private static readonly List<object> PagosRecibidos = new();

    [HttpPost("verificar")] // Esto crea /api/Pagos/verificar
    public IActionResult RecibirPago([FromBody] object pago)
    {
        PagosRecibidos.Add(pago);
        return Ok(new { mensaje = "Pago registrado en la bóveda" });
    }

    [HttpGet("historial")]
    public IActionResult ObtenerPagos() => Ok(PagosRecibidos);
}