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
        public async Task<List<Maquina>> GetAllAsync()
        {
            return await _db.Maquinas.ToListAsync();
        }

        public async Task<Maquina?> GetByIdAsync(int id)
        {
            return await _db.Maquinas.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Maquina?> ObtenerPorId(int id)
        {
            return await _db.Maquinas.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task GuardarMaquina(Maquina maquina)
        {
            var existe = await _db.Maquinas.AnyAsync(m => m.Id == maquina.Id);
            maquina.FechaActualizacion = DateTime.UtcNow;

            if (maquina.Id == 0)
            {
                maquina.FechaCreacion = DateTime.UtcNow;
                _db.Maquinas.Add(maquina);
            }
            else
            {
                maquina.FechaActualizacion = DateTime.UtcNow;
                _db.Maquinas.Update(maquina);
            }
            await _db.SaveChangesAsync();
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
        public async Task<bool> AddEmpleadoAsync(Empleado empleado)
        {
            try
            {
                _db.Empleados.Add(empleado);
                await _db.SaveChangesAsync();
                return true; 
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error: El código de empleado ya existe.");
                _db.Entry(empleado).State = EntityState.Detached;
                return false; 
            }
        }
        public async Task UpdateEmpleadoAsync(Empleado empleado)
        {
            _db.Empleados.Update(empleado);
            await _db.SaveChangesAsync();
        }

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

        public async Task DeleteEmpleadoAsync(Empleado empleado)
        {
            _db.Empleados.Remove(empleado);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Orden>> GetAllOrdenes()
        {
            try
            {
                return await _db.Ordenes.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.Write($"Error al carar las ordenes {ex.Message}");
                return new List<Orden>();
            }
        }

        public async Task CrearOrdenAsync(Orden nuevaOrden)
        {

        }
        public async Task<bool> InsertarOperacion(OperacionesOrden nuevaOp)
        {
            try
            {
                // añadimos el objeto a la tabla de operaciones
                _db.OperacionesOrden.Add(nuevaOp);

                // guardamos los cambios en SQL
                var resultado = await _db.SaveChangesAsync();

                // Si devuelve más de 0, es que se ha guardado bien
                return resultado > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar operación: {ex.Message}");
                return false;
            }
        }
    }
}