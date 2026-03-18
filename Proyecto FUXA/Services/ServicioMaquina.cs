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
                .Include(m => m.Producciones)
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
        public async Task AddAsync(Maquina maquina)
        {
            maquina.FechaCreacion = DateTime.UtcNow;
            maquina.FechaActualizacion = DateTime.UtcNow;
            _db.Maquinas.Add(maquina);
            await _db.SaveChangesAsync();
        }

        // Actualizar una máquina
        public async Task UpdateAsync(Maquina maquina)
        {
            maquina.FechaActualizacion = DateTime.UtcNow;
            _db.Maquinas.Update(maquina);
            await _db.SaveChangesAsync();
        }

        // Registrar un ciclo de producción
        public async Task AddCycleAsync(int maquinaId, int ciclosReales)
        {
            var log = new MaquinaProduccion
            {
                MaquinaId = maquinaId,
                CiclosReales = ciclosReales,
                FechaRegistro = DateTime.UtcNow
            };

            _db.Producciones.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}