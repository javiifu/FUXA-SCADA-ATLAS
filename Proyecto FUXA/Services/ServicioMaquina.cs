using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Services
{
    public class ServicioMaquina
    {
        private readonly AppDbContext _db;

        public ServicioMaquina(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Machine>> GetAllAsync()
        {
            return await _db.Machines
                .Include(m => m.CycleLogs)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<Machine?> GetByIdAsync(int id)
        {
            return await _db.Machines
                .Include(m => m.CycleLogs)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Machine machine)
        {
            machine.CreadoEn = DateTime.UtcNow;
            machine.ModificadoEn = DateTime.UtcNow;
            _db.Machines.Add(machine);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Machine machine)
        {
            machine.ModificadoEn = DateTime.UtcNow;
            _db.Machines.Update(machine);
            await _db.SaveChangesAsync();
        }

        public async Task AddCycleAsync(int machineId, int realCycles)
        {
            var log = new LogCicloMaquina
            {
                IdMaquina = machineId,
                CiclosReales = realCycles,
                Timestamp = DateTime.UtcNow
            };

            _db.LogsCicloMaquina.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
