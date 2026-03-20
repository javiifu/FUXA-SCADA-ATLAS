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
            return await _db.Maquinas.ToListAsync();
        }

        public async Task<List<Empleado>> GetAllEmpleadosAsync()
        {
            try
            {
                return await _db.Empleados.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando empleados: {ex.Message}");
                return new List<Empleado>();
            }
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

        public async Task<Maquina?> ObtenerPorIdFuxa(string idFuxa)
        {
            return await _db.Maquinas
                .FirstOrDefaultAsync(m => m.IdFuxa == idFuxa);
        }

        public async Task GuardarMaquina(Maquina maquina)
        {

            var existe = await _db.Maquinas.AnyAsync(m => m.IdFuxa == maquina.IdFuxa);

            maquina.FechaActualizacion = DateTime.UtcNow;

            if (!existe)
            {
                maquina.FechaCreacion = DateTime.UtcNow;
                _db.Maquinas.Add(maquina);
            }
            else
            {
                _db.Maquinas.Update(maquina);
            }

            await _db.SaveChangesAsync();
        }
    }
}