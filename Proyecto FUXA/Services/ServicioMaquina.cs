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

        // Obtener todas las máquinas ordenadas por nombre
        public async Task<List<Maquina>> GetAllAsync()
        {
            return await _db.Maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        // Obtener una máquina por su ID
        public async Task<Maquina?> GetByIdAsync(int id)
        {
            return await _db.Maquinas
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // Añadir una nueva máquina
        public async Task AddAsync(Maquina machine)
        {
            machine.FechaCreacion = DateTime.UtcNow;
            machine.FechaActualizacion = DateTime.UtcNow;
            _db.Maquinas.Add(machine);
            await _db.SaveChangesAsync();
        }

        // Actualizar una máquina
        public async Task UpdateAsync(Maquina machine)
        {
            machine.FechaActualizacion = DateTime.UtcNow;
            _db.Maquinas.Update(machine);
            await _db.SaveChangesAsync();
        }

        // Registrar un ciclo de producción
        public async Task AddCycleAsync(int machineId, int realCycles)
        {
            var log = new MaquinaProduccion
            {
                MaquinaId = machineId,
                CiclosReales = realCycles,
                FechaRegistro = DateTime.UtcNow
            };

            _db.Producciones.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}