using Microsoft.EntityFrameworkCore;
using promerica_backend.Models;

namespace promerica_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Puestos> Puestos { get; set; }
    }
}
