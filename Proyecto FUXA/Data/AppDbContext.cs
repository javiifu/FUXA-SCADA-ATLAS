using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Maquina> Machines => Set<Maquina>();
    public DbSet<MaquinaEstatus> MachineStatuses => Set<MaquinaEstatus>();
    public DbSet<MaquinaProduccion> MachineProductions => Set<MaquinaProduccion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Maquina>()
            .HasOne(m => m.EstadoActual)
            .WithMany(e => e.Maquinas)
            .HasForeignKey(m => m.EstadoActualId)
            .HasConstraintName("FK_Machines_Status");

        modelBuilder.Entity<Maquina>()
            .Property(m => m.IdentificadorObjetoFuxa)
            .HasMaxLength(100);

        modelBuilder.Entity<Maquina>()
            .HasIndex(m => m.IdentificadorObjetoFuxa)
            .IsUnique()
            .HasFilter("[IdentificadorObjetoFuxa] IS NOT NULL")
            .HasDatabaseName("IX_Maquina_IdentificadorObjetoFuxa");

        modelBuilder.Entity<MaquinaProduccion>()
            .HasOne(p => p.Maquina)
            .WithMany(m => m.Producciones)
            .HasForeignKey(p => p.MaquinaId)
            .HasConstraintName("FK_Production_Machines");
    }
}
