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
        public DbSet<MaquinasOrdenes> Ordenes { get; set;  }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<ImputacionOperario> ImputacionesOperarios { get; set; }
        public DbSet<Operacion> Operaciones { get; set; }
        public DbSet<MaquinasOrdenes> MaquinasOrdenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Maquina>().ToTable("Maquina");
            modelBuilder.Entity<MaquinaProduccion>().ToTable("MaquinaProduccion");
            modelBuilder.Entity<MaquinaEstatus>().ToTable("MaquinaEstatus");
            modelBuilder.Entity<MaquinasOrdenes>().ToTable("MaquinasOrdenes");
            modelBuilder.Entity<Seccion>().ToTable("Secciones");
            modelBuilder.Entity<ImputacionOperario>().ToTable("ImputacionOperarios");
            modelBuilder.Entity<Operacion>().ToTable("Operaciones");

            modelBuilder.Entity<ImputacionOperario>()
                .HasOne(i => i.Empleado)
                .WithMany()
                .HasForeignKey(i => i.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ImputacionOperario>()
                .HasOne(i => i.Maquina)
                .WithMany()
                .HasForeignKey(i => i.IdMaquina)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ImputacionOperario>()
                .HasOne(i => i.Operacion)
                .WithMany()
                .HasForeignKey(i => i.IdOperacion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ImputacionOperario>()
                .HasOne(i => i.Orden)
                .WithMany()
                .HasForeignKey(i => i.IdOrden)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}