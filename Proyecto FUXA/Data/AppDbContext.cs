using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Machine> Machines => Set<Machine>();
        public DbSet<LogCicloMaquina> LogsCicloMaquina => Set<LogCicloMaquina>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Machine>()
                .HasIndex(m => m.Codigo)
                .IsUnique();

            modelBuilder.Entity<Machine>()
                .Property(m => m.Estado)
                .HasConversion<string>();
        }
    }
}
