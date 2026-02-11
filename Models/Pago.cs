using System.ComponentModel.DataAnnotations;

namespace CarvaVault_API.Models;

public class Pago
{
    [Key]
    public int Id { get; set; }
    public string Remitente { get; set; } = string.Empty;
    public double Monto { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string ClienteId { get; set; } = string.Empty;
}