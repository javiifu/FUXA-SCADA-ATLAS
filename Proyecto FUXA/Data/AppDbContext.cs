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
        public DbSet<Orden> Ordenes { get; set;  }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<ImputacionOperario> ImputacionesOperarios { get; set; }
        public DbSet<Operacion> Operaciones { get; set; }
        public DbSet<OperacionesOrden> OperacionesOrden{ get; set; }
        public DbSet<MaquinaOperario> MaquinasOperarios { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<ImputacionMaterial> ImputacionMateriales { get; set; }
        public DbSet<MaquinaMaterial> MaquinasMateriales { get; set; }
        public DbSet<EmpleadoMaquina> EmpleadoMaquinas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Maquina>().ToTable("Maquina");
            modelBuilder.Entity<MaquinaProduccion>().ToTable("MaquinaProduccion");
            modelBuilder.Entity<MaquinaEstatus>().ToTable("MaquinaEstatus");
            modelBuilder.Entity<Orden>().ToTable("Ordenes");
            modelBuilder.Entity<Seccion>().ToTable("Secciones");
            modelBuilder.Entity<Empleado>().ToTable("Empleados");
            modelBuilder.Entity<ImputacionOperario>().ToTable("ImputacionOperarios");
            modelBuilder.Entity<Operacion>().ToTable("Operaciones");
            modelBuilder.Entity<OperacionesOrden>().ToTable("OperacionesOrden");
            modelBuilder.Entity<MaquinaOperario>().ToTable("MaquinasOperarios");
            modelBuilder.Entity<Material>().ToTable("Materiales");
            modelBuilder.Entity<ImputacionMaterial>().ToTable("ImputacionMateriales");
            modelBuilder.Entity<MaquinaMaterial>().ToTable("MaquinasMateriales");
            modelBuilder.Entity<EmpleadoMaquina>().ToTable("MaquinasEmpleados");

            modelBuilder.Entity<ImputacionOperario>()
                .HasOne(i => i.Operacion)
                .WithMany()
                .HasForeignKey(i => i.IdOperacion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ImputacionOperario>()
                .HasOne(i => i.Empleado)
                .WithMany()
                .HasForeignKey(i => i.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaquinaMaterial>()
                .HasKey(mm => new { mm.IdMaquina, mm.IdMaterial });

            modelBuilder.Entity<MaquinaMaterial>()
                .HasOne(mm => mm.Maquina)
                .WithMany(m => m.MaquinasMateriales)
                .HasForeignKey(mm => mm.IdMaquina)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaquinaMaterial>()
                .HasOne(mm => mm.Material)
                .WithMany(mat => mat.MaquinasMateriales)
                .HasForeignKey(mm => mm.IdMaterial)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmpleadoMaquina>()
                .HasKey(em => new { em.IdMaquina, em.IdEmpleado });
        }
    }
}