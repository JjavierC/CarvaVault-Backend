using System.ComponentModel.DataAnnotations;

namespace CarvaVault_API.Models;

public class Usuario
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = "Cliente";
    public string? CreadoPor { get; set; }
}