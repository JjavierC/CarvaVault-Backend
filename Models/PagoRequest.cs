namespace CarvaVault_API.Models;

public class PagoRequest
{
    public string Remitente { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
}