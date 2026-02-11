namespace CarvaVault_API.Models;

public class PagoRequest
{
    public string Remitente { get; set; } = string.Empty;
    public double Monto { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    
    public string ClienteId { get; set; } = "generico";
}

public class LoginRequest
{
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}