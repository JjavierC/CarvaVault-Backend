using Microsoft.EntityFrameworkCore;
using CarvaVault_API.Models;

namespace CarvaVault_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Pago> Pagos { get; set; }
}