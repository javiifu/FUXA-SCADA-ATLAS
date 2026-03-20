using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Maquina> Maquinas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<MaquinaProduccion> Producciones { get; set; }
        public DbSet<MaquinaEstatus> Estatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Maquina>().ToTable("Maquina");
            modelBuilder.Entity<MaquinaProduccion>().ToTable("MaquinaProduccion");
            modelBuilder.Entity<MaquinaEstatus>().ToTable("MaquinaEstatus");
        }
    }
}