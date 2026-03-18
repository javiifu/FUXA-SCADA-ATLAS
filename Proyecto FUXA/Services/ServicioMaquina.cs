using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Services;

public class ServicioMaquina
{
    private readonly AppDbContext _db;

    public ServicioMaquina(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Maquina>> GetAllAsync()
    {
        return await _db.Machines
            .Include(m => m.EstadoActual)
            .Include(m => m.Producciones)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public Task<List<Maquina>> GetMachinesAsync() => GetAllAsync();

    public async Task<Maquina?> GetByIdAsync(int id)
    {
        return await _db.Machines
            .Include(m => m.EstadoActual)
            .Include(m => m.Producciones)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Maquina?> GetByIdentificadorObjetoFuxaAsync(string identificadorObjetoFuxa)
    {
        var objetoNormalizado = identificadorObjetoFuxa.Trim();
        if (string.IsNullOrWhiteSpace(objetoNormalizado))
        {
            return null;
        }

        return await _db.Machines
            .Include(m => m.EstadoActual)
            .Include(m => m.Producciones)
            .FirstOrDefaultAsync(m => m.IdentificadorObjetoFuxa == objetoNormalizado);
    }

    public async Task AddAsync(Maquina machine)
    {
        var now = DateTime.UtcNow;
        machine.FechaCreacion = now;
        machine.FechaActualizacion = now;
        _db.Machines.Add(machine);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Maquina machine)
    {
        machine.FechaActualizacion = DateTime.UtcNow;
        _db.Machines.Update(machine);
        await _db.SaveChangesAsync();
    }

    public async Task VincularObjetoFuxaAsync(int maquinaId, string identificadorObjetoFuxa)
    {
        var maquina = await _db.Machines.FirstOrDefaultAsync(m => m.Id == maquinaId);
        if (maquina is null)
        {
            return;
        }

        var objetoNormalizado = identificadorObjetoFuxa.Trim();
        if (string.IsNullOrWhiteSpace(objetoNormalizado))
        {
            return;
        }

        var maquinaConVinculo = await _db.Machines
            .Where(m => m.Id != maquinaId && m.IdentificadorObjetoFuxa == objetoNormalizado)
            .FirstOrDefaultAsync();

        if (maquinaConVinculo is not null)
        {
            maquinaConVinculo.IdentificadorObjetoFuxa = null;
            maquinaConVinculo.FechaActualizacion = DateTime.UtcNow;
        }

        maquina.IdentificadorObjetoFuxa = objetoNormalizado;
        maquina.FechaActualizacion = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task AddCycleAsync(int machineId, int realCycles)
    {
        var produccion = new MaquinaProduccion
        {
            MaquinaId = machineId,
            CiclosReales = realCycles,
            FechaRegistro = DateTime.UtcNow
        };

        _db.MachineProductions.Add(produccion);
        await _db.SaveChangesAsync();
    }
}
