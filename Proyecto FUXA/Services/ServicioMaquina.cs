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
            return await _db.Maquinas
                .AsNoTracking()
                .Include(m => m.Seccion)
                .Include(m => m.MaquinasEmpleados)
                .ThenInclude(me => me.Empleado)
                .ToListAsync();
        }
        public async Task<List<Maquina>> GetAllBySeccionAsync()
        {
            return await _db.Maquinas
                .Include(m => m.Seccion)
                .ToListAsync();
        }

        public async Task<Maquina?> GetByIdAsync(int id)
        {
            return await _db.Maquinas.FirstOrDefaultAsync(m => m.Id == id);
        }


        public async Task GuardarMaquina(Maquina maquina)
        {
            if (maquina.Id == 0)
            {
                maquina.FechaCreacion = DateTime.UtcNow;
                maquina.FechaActualizacion = DateTime.UtcNow;
                _db.Maquinas.Add(maquina);
                await _db.SaveChangesAsync();
            }
            else
            {
                var dbMaquina = await _db.Maquinas.FindAsync(maquina.Id);
                if (dbMaquina != null)
                {
                    dbMaquina.Nombre = maquina.Nombre;

                    if (maquina.IdSeccion.HasValue && maquina.IdSeccion > 0)
                    {
                        dbMaquina.IdSeccion = maquina.IdSeccion;
                    }

                    dbMaquina.CiclosReales = maquina.CiclosReales;
                    dbMaquina.EstadoActualId = maquina.EstadoActualId;
                    dbMaquina.FechaActualizacion = DateTime.UtcNow;

                    dbMaquina.CiclosObjetivo = maquina.CiclosObjetivo;

                    var opActiva = await _db.OperacionesOrden
                        .FirstOrDefaultAsync(o => o.IdMaquina == maquina.Id && o.Estado == "Activa");

                    if (opActiva != null)
                    {
                        opActiva.CiclosObjetivo = maquina.CiclosObjetivo;
                        opActiva.PiezasFabricadas = maquina.PiezasFabricadas;
                        opActiva.PiezasRotas = maquina.PiezasRotas;
                    }

                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task<List<Maquina>> GetAllMaquinas()
        {
            return await _db.Maquinas.ToListAsync();
        }

        public async Task<List<Empleado>> GetAllEmpleadosAsync()
        {
            return await _db.Empleados.AsNoTracking().ToListAsync();
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
            var dbEmpleado = await _db.Empleados.FindAsync(empleado.Id);
            if (dbEmpleado != null)
            {
                dbEmpleado.Nombre = empleado.Nombre;
                dbEmpleado.Apellidos = empleado.Apellidos;
                dbEmpleado.Cargo = empleado.Cargo;

                await _db.SaveChangesAsync();
            }
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

        public async Task<bool> InsertarOperacion(OperacionesOrden nuevaOp)
        {
            try
            {
                _db.OperacionesOrden.Add(nuevaOp);
                var resultado = await _db.SaveChangesAsync();
                return resultado > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar operación: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> InsertarMaquina(Maquina nuevaMaquina)
        {
            try
            {
                nuevaMaquina.FechaCreacion = DateTime.UtcNow;
                nuevaMaquina.FechaActualizacion = DateTime.UtcNow;

                _db.Maquinas.Add(nuevaMaquina);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar máquina: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarMaquina(Maquina maquinaEditada)
        {
            try
            {
                maquinaEditada.FechaActualizacion = DateTime.UtcNow;

                _db.Maquinas.Update(maquinaEditada);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar máquina: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Empleado>> GetAllEmpleadosConMaquinaAsync()
        {
            try
            {
                return await _db.Empleados
                    .Include(e => e.MaquinasEmpleados)
                        .ThenInclude(me => me.Maquina)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Empleado>();
            }
        }

        public async Task<List<Empleado>> ObtenerOperariosDeUnaMaquina(int maquinaId)
        {
            try
            {
                return await _db.EmpleadoMaquinas
                    .Where(me => me.IdMaquina == maquinaId)
                    .Include(me => me.Empleado)
                    .Select(me => me.Empleado)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Empleado>();
            }
        }

        public async Task<List<Seccion>> ObtenerSecciones()
        {
            try
            {
                return await _db.Secciones
                    .OrderBy(s => s.Nombre)
                    .ToListAsync();
            }
            catch
            {
                return new List<Seccion>();
            }
        }

        public async Task<bool> ComprobarOperacionActiva(int maquinaId)
        {
            return await _db.OperacionesOrden.AnyAsync(op => op.IdMaquina == maquinaId && op.Estado == "Activa");
        }

        public async Task<List<Material>> ObtenerTodosLosMaterialesAsync()
        {
            return await _db.Materiales
                .AsNoTracking()
                .OrderBy(n => n.Nombre)
                .ToListAsync();
        }

        public async Task<List<Material>> ObtenerMaterialesDeMaquinaAsync(int idMaquina)
        {
            var materiales = await _db.MaquinasMateriales
                .AsNoTracking()
                .Where(mm => mm.IdMaquina == idMaquina)
                .Include(mm => mm.Material)
                .Select(mm => mm.Material)
                .ToListAsync();

            return materiales ?? new List<Material>();
        }

        public async Task AsignarMaterialAMaquinaAsync(int idMaquina, int idMaterial)
        {
            var existe = await _db.MaquinasMateriales
                .AnyAsync(mm => mm.IdMaquina == idMaquina && mm.IdMaterial == idMaterial);

            if (!existe)
            {
                var nuevaRelacion = new MaquinaMaterial
                {
                    IdMaquina = idMaquina,
                    IdMaterial = idMaterial
                };

                _db.MaquinasMateriales.Add(nuevaRelacion);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DesvincularMaterialDeMaquinaAsync(int idMaquina, int idMaterial)
        {
            var relacion = await _db.MaquinasMateriales
                .FirstOrDefaultAsync(mm => mm.IdMaquina == idMaquina && mm.IdMaterial == idMaterial);

            if (relacion != null)
            {
                _db.MaquinasMateriales.Remove(relacion);
                await _db.SaveChangesAsync();
            }
        }

        public async Task AsignarEmpleadoAMaquinaAsync(int idMaquina, int idEmpleado)
        {
            var existe = await _db.EmpleadoMaquinas
                .AnyAsync(em => em.IdMaquina == idMaquina && em.IdEmpleado == idEmpleado);

            if (!existe)
            {
                var nuevaRelacion = new EmpleadoMaquina
                {
                    IdMaquina = idMaquina,
                    IdEmpleado = idEmpleado
                };
                _db.EmpleadoMaquinas.Add(nuevaRelacion);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DesvincularEmpleadoDeMaquinaAsync(int idMaquina, int idEmpleado)
        {
            var relacion = await _db.EmpleadoMaquinas
                .FirstOrDefaultAsync(em => em.IdMaquina == idMaquina && em.IdEmpleado == idEmpleado);

            if (relacion != null)
            {
                _db.EmpleadoMaquinas.Remove(relacion);
                await _db.SaveChangesAsync();
            }
        }

        //public async Task <int?> PreferenciaPorMaquinaId(int idMaquina)
        //{
        //    var maquina = await _db.Maquinas
        //        .FirstOrDefaultAsync(m => m.Id == idMaquina);

        //    if (maquina == null) return null;

        //    var seccion = await _db.Secciones
        //        .FirstOrDefaultAsync(s => s.Id == maquina.IdSeccion);

        //    return seccion?.Preferencia;
        //}
    }
}