using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Maquina> Maquinas { get; set; }
        public DbSet<MaquinaProduccion> Producciones { get; set; }
        public DbSet<MaquinaEstatus> Estatus { get; set; }
        public DbSet<PlantaObjetoVisual> ObjetosVisualesPlanta { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<Incidencia> Incidencias { get; set; }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Operacion> Operaciones { get; set; }
        public DbSet<MaquinaOrden> MaquinasOrdenes { get; set; }
        public DbSet<ImputacionMaquina> ImputacionesMaquina { get; set; }
        public DbSet<ImputacionOperario> ImputacionesOperario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Maquina>().ToTable("Maquina");
            modelBuilder.Entity<MaquinaProduccion>().ToTable("MaquinaProduccion");
            modelBuilder.Entity<MaquinaEstatus>().ToTable("MaquinaEstatus");
            modelBuilder.Entity<PlantaObjetoVisual>().ToTable("PlantaObjetoVisual");
            modelBuilder.Entity<Mantenimiento>().ToTable("Mantenimiento");
            modelBuilder.Entity<Incidencia>().ToTable("Incidencia");
            modelBuilder.Entity<Seccion>().ToTable("Secciones");
            modelBuilder.Entity<Empleado>().ToTable("Empleados");
            modelBuilder.Entity<Operacion>().ToTable("Operaciones");
            modelBuilder.Entity<MaquinaOrden>().ToTable("MaquinasOrdenes");
            modelBuilder.Entity<ImputacionMaquina>().ToTable("ImputacionMaquina");
            modelBuilder.Entity<ImputacionOperario>().ToTable("ImputacionOperarios");
            modelBuilder.Entity<JornadaOperario>().ToTable("JornadasOperario");
            modelBuilder.Entity<FichajeEvento>().ToTable("FichajeEventos");


            modelBuilder.Entity<PlantaObjetoVisual>()
                .HasOne(x => x.Maquina)
                .WithMany(m => m.ObjetosVisualesPlanta)
                .HasForeignKey(x => x.MaquinaId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Mantenimiento>()
                .HasOne(x => x.Maquina)
                .WithMany(m => m.Mantenimientos)
                .HasForeignKey(x => x.MaquinaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Incidencia>()
                .HasOne(x => x.Maquina)
                .WithMany(m => m.Incidencias)
                .HasForeignKey(x => x.MaquinaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Maquina>()
                .HasOne(x => x.Seccion)
                .WithMany(s => s.Maquinas)
                .HasForeignKey(x => x.IdSeccion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaquinaOrden>()
                .HasOne(x => x.Maquina)
                .WithMany(m => m.MaquinasOrdenes)
                .HasForeignKey(x => x.MaquinaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaquinaOrden>()
               .HasOne(x => x.Operacion)
               .WithMany(o => o.MaquinasOrdenes)
               .HasForeignKey(x => x.OperacionId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ImputacionMaquina>()
                .HasOne(x => x.MaquinaOrden)
                .WithMany(mo => mo.ImputacionesMaquina)
                .HasForeignKey(x => x.MaquinaOrdenId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ImputacionMaquina>()
                .HasOne(x => x.Operacion)
                .WithMany(op => op.ImputacionesMaquina)
                .HasForeignKey(x => x.OperacionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ImputacionOperario>()
                .HasOne(x => x.ImputacionMaquina)
                .WithMany(i => i.ImputacionesOperario)
                .HasForeignKey(x => x.ImputacionMaquinaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JornadaOperario>()
                .HasOne(x => x.Empleado)
                .WithMany(e => e.JornadasOperario)
                .HasForeignKey(x => x.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ImputacionOperario>()
                .HasIndex(i => i.EmpleadoId)
                .HasDatabaseName("UX_ImputacionOperarios_EmpleadoAbierto")
                .HasFilter("[FechaFin] IS NULL")
                .IsUnique();

            modelBuilder.Entity<FichajeEvento>()
                .HasOne(x => x.JornadaOperario)
                .WithMany(j => j.FichajeEventos)
                .HasForeignKey(x => x.JornadaOperarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
