using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Services;

public class ServicioPlantaVisual
{
    private readonly AppDbContext _db;

    public ServicioPlantaVisual(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PlantaObjetoVisual>> GetAllAsync()
    {
        return await _db.ObjetosVisualesPlanta
            .Include(x => x.Maquina)
            .ThenInclude(m => m!.Producciones)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<PlantaObjetoVisual?> GetByIdAsync(int id)
    {
        return await _db.ObjetosVisualesPlanta
            .Include(x => x.Maquina)
            .ThenInclude(m => m!.Producciones)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PlantaObjetoVisual> AddRectanguloAsync(string nombre)
    {
        var objeto = new PlantaObjetoVisual
        {
            Nombre = nombre,
            Tipo = "Rectangulo",
            PosX = 20,
            PosY = 20,
            Width = 120,
            Height = 80,
            ColorHex = "#2f80ed",
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        _db.ObjetosVisualesPlanta.Add(objeto);
        await _db.SaveChangesAsync();

        return objeto;
    }

    public async Task UpdateAsync(PlantaObjetoVisual objeto)
    {
        objeto.FechaActualizacion = DateTime.UtcNow;
        _db.ObjetosVisualesPlanta.Update(objeto);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var objeto = await _db.ObjetosVisualesPlanta.FindAsync(id);
        if (objeto is null)
        {
            return;
        }

        _db.ObjetosVisualesPlanta.Remove(objeto);
        await _db.SaveChangesAsync();
    }

    public async Task VincularMaquinaAsync(int objetoId, int? maquinaId)
    {
        var objeto = await _db.ObjetosVisualesPlanta.FindAsync(objetoId);
        if (objeto is null)
        {
            return;
        }

        objeto.MaquinaId = maquinaId;
        objeto.FechaActualizacion = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
