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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Maquina>().ToTable("Maquina");
            modelBuilder.Entity<MaquinaProduccion>().ToTable("MaquinaProduccion");
            modelBuilder.Entity<MaquinaEstatus>().ToTable("MaquinaEstatus");
            modelBuilder.Entity<PlantaObjetoVisual>().ToTable("PlantaObjetoVisual");
            modelBuilder.Entity<Mantenimiento>().ToTable("Mantenimiento");
            modelBuilder.Entity<Incidencia>().ToTable("Incidencia");

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
        }
    }
}
