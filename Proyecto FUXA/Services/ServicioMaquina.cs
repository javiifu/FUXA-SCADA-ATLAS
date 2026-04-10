using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.Models;
using Proyecto_FUXA.DTO;

namespace Proyecto_FUXA.Services
{
    public class ServicioMaquina
    {
        private readonly AppDbContext _db;
        private static readonly string[] EstadosIncidenciaCerrada = ["Resuelta", "Cerrada"];

        public ServicioMaquina(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Maquina>> GetAllAsync()
        {
            return await _db.Maquinas
                .Include(m => m.Producciones)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<List<MaquinaGridDto>> GetMaquinasGridAsync()
        {
            var maquinas = await _db.Maquinas
                .AsNoTracking()
                .OrderBy(m => m.Nombre)
                .Select(m => new MaquinaGridDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Seccion = m.NombreSeccion,
                    EstadoActualId = m.EstadoActualId,
                    CiclosObjetivo = m.CiclosObjetivo,
                    CiclosReales = m.Producciones
                        .OrderByDescending(p => p.FechaRegistro)
                        .Select(p => p.CiclosReales)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var incidenciasActivas = await _db.Incidencias
                .AsNoTracking()
                .Where(i => !EstadosIncidenciaCerrada.Contains(i.Estado))
                .OrderByDescending(i => i.FechaApertura)
                .Select(i => new IncidenciaActivaDto
                {
                    Id = i.Id,
                    MaquinaId = i.MaquinaId,
                    Titulo = i.Titulo,
                    Descripcion = i.Descripcion,
                    Prioridad = i.Prioridad,
                    Estado = i.Estado,
                    FechaApertura = i.FechaApertura,
                    FechaCierre = i.FechaCierre,
                    UsuarioApertura = i.UsuarioApertura,
                    UsuarioAsignado = i.UsuarioAsignado
                })
                .ToListAsync();

            var incidenciasPorMaquina = incidenciasActivas
                .GroupBy(i => i.MaquinaId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var ultimosMantenimientos = await _db.Mantenimientos
                .AsNoTracking()
                .Where(m => m.FechaRealizada != null)
                .Select(m => new
                {
                    m.MaquinaId,
                    m.Estado,
                    m.FechaRealizada
                })
                .ToListAsync();

            var ultimoMantenimientoPorMaquina = ultimosMantenimientos
                .GroupBy(m => m.MaquinaId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Where(m => m.Estado == "Realizado")
                            .Select(m => m.FechaRealizada)
                            .DefaultIfEmpty(null)
                            .Max()
                        ?? g.Select(m => m.FechaRealizada).Max());

            var proximosMantenimientos = await _db.Mantenimientos
                .AsNoTracking()
                .Where(m => m.FechaProgramada != null)
                .Select(m => new
                {
                    m.MaquinaId,
                    m.FechaProgramada,
                    m.Id
                })
                .ToListAsync();

            var proximoMantenimientoPorMaquina = proximosMantenimientos
                .GroupBy(m => m.MaquinaId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.FechaProgramada)
                          .ThenByDescending(x => x.Id)
                          .Select(x => x.FechaProgramada)
                          .FirstOrDefault());

            foreach (var maquina in maquinas)
            {
                if (incidenciasPorMaquina.TryGetValue(maquina.Id, out var incidencias))
                {
                    maquina.IncidenciasActivas = incidencias.Count;
                    maquina.IncidenciasActivasDetalle = incidencias;
                }

                if (ultimoMantenimientoPorMaquina.TryGetValue(maquina.Id, out var ultimoMantenimiento))
                {
                    maquina.UltimoMantenimiento = ultimoMantenimiento;
                }

                if (proximoMantenimientoPorMaquina.TryGetValue(maquina.Id, out var proximoMantenimiento))
                {
                    maquina.ProximoMantenimiento = proximoMantenimiento;
                }
            }

            return maquinas;
        }

        // Obtener una máquina por su ID
        public async Task<Maquina?> GetByIdAsync(int id)
        {
            return await _db.Maquinas
                .Include(m => m.Producciones)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Maquina?> ObtenerPorId(int id)
        {
            maquina.Nombre = (maquina.Nombre ?? string.Empty).Trim();
            maquina.FechaCreacion = DateTime.UtcNow;
            maquina.FechaActualizacion = DateTime.UtcNow;
            _db.Maquinas.Add(maquina);
            await _db.SaveChangesAsync();

            var yaExisteObjetoVisual = await _db.ObjetosVisualesPlanta
                .AnyAsync(x => x.MaquinaId == maquina.Id);

            if (!yaExisteObjetoVisual)
            {
                var totalObjetos = await _db.ObjetosVisualesPlanta.CountAsync();
                var columna = totalObjetos % 5;
                var fila = totalObjetos / 5;

                _db.ObjetosVisualesPlanta.Add(new PlantaObjetoVisual
                {
                    Nombre = maquina.Nombre,
                    Tipo = "Rectangulo",
                    PosX = 20 + (columna * 150),
                    PosY = 20 + (fila * 110),
                    Width = 120,
                    Height = 80,
                    ColorHex = "#2f80ed",
                    MaquinaId = maquina.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
            }
        }

        public async Task GuardarMaquina(Maquina maquina)
        {
            var dbMaquina = await _db.Maquinas.FindAsync(maquina.Id);


            if (maquina.Id == 0)
            {
                maquina.FechaCreacion = DateTime.UtcNow;
                maquina.FechaActualizacion = DateTime.UtcNow;

                _db.Maquinas.Add(maquina);
                await _db.SaveChangesAsync();
            }
            else
            {
                if (dbMaquina != null)
                {
                    dbMaquina.Nombre = maquina.Nombre;
                    dbMaquina.IdSeccion = maquina.IdSeccion;
                    dbMaquina.CiclosReales = maquina.CiclosReales;
                    dbMaquina.EmpleadoId = maquina.EmpleadoId;
                    dbMaquina.EstadoActualId = maquina.EstadoActualId;
                    dbMaquina.FechaActualizacion = DateTime.UtcNow;
                    dbMaquina.PiezasFabricadas = maquina.PiezasFabricadas;
                    dbMaquina.PiezasRotas = maquina.PiezasRotas;

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
                dbEmpleado.IdMaquina = empleado.IdMaquina;

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
            return await _db.Maquinas
                .FirstOrDefaultAsync(m => m.FuxaDeviceId == idFuxa);
        }

        public async Task<List<Orden>> GetAllOrdenes()
        {

            var existe = maquina.Id > 0 || await _db.Maquinas.AnyAsync(m => m.FuxaDeviceId == maquina.FuxaDeviceId);

            maquina.FechaActualizacion = DateTime.UtcNow;

            if (!existe)
            {
                maquina.FechaCreacion = DateTime.UtcNow;

                _db.Maquinas.Add(maquina);
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

        public async Task <List<Empleado>> ObtenerEmpleadosConMaquinaAsync()
        {
            return await _db.Empleados.Include(e => e.Maquina).ToListAsync();
        }

        public async Task<List<Empleado>> ObtenerOperariosDeMaquina(int maquinaId)
        {
            return await _db.MaquinasOperarios
                .Where(mo => mo.MaquinaId == maquinaId)
                .Select(mo => mo.Empleado)
                .ToListAsync();
        }

        public async Task<List<Empleado>> GetAllEmpleadosConMaquinaAsync()
        {
            try
            {
                return await _db.Empleados
                    .Include(e => e.Maquina)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Empleado>();
            }
        }

        public async Task<List<Empleado>> ObtenerOperariosDeUnaMaquina (int maquinaId)
        {
            try
            {
                return await _db.Empleados
                    .Where(e => e.IdMaquina == maquinaId)
                    .AsNoTracking()
                    .ToListAsync();
                        
            }
            catch(Exception ex)
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


        public async Task<DateTime?> GuardarProximoMantenimientoAsync(int maquinaId, DateTime? fechaProgramada)
        {
            var mantenimiento = await _db.Mantenimientos
                .Where(m => m.MaquinaId == maquinaId && m.FechaProgramada != null)
                .OrderByDescending(m => m.FechaProgramada)
                .ThenByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            if (mantenimiento is null)
            {
                if (fechaProgramada is null)
                {
                    return null;
                }
                mantenimiento = new Mantenimiento
                {
                    MaquinaId = maquinaId,
                    Estado = "Programado",
                    Tipo = "Preventivo",
                    FechaProgramada = fechaProgramada,
                    FechaCreacion = DateTime.UtcNow,
                    Observaciones = null
                };

                _db.Mantenimientos.Add(mantenimiento);
            }
            else
            {
                mantenimiento.FechaProgramada = fechaProgramada;
            }

            await _db.SaveChangesAsync();
            return mantenimiento.FechaProgramada;
        }
    

    public async Task<int> CrearIncidenciaAsync(IncidenciaActivaDto dto)
        {
            var maquina = await _db.Maquinas.FirstOrDefaultAsync(m => m.Id == dto.MaquinaId);

            if (maquina is null)
                throw new InvalidOperationException($"No existe la máquina con Id {dto.MaquinaId}.");

            if (string.IsNullOrWhiteSpace(dto.Titulo))
                throw new InvalidOperationException("El título es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Prioridad))
                throw new InvalidOperationException("La prioridad es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.Estado))
                throw new InvalidOperationException("El estado es obligatorio.");

            var incidencia = new Incidencia
            {
                MaquinaId = dto.MaquinaId,
                Titulo = dto.Titulo.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
                Prioridad = dto.Prioridad.Trim(),
                Estado = dto.Estado.Trim(),
                FechaApertura = dto.FechaApertura == default ? DateTime.Now : dto.FechaApertura,
                FechaCierre = dto.FechaCierre,
                UsuarioApertura = string.IsNullOrWhiteSpace(dto.UsuarioApertura) ? null : dto.UsuarioApertura.Trim(),
                UsuarioAsignado = string.IsNullOrWhiteSpace(dto.UsuarioAsignado) ? null : dto.UsuarioAsignado.Trim()
            };

            _db.Incidencias.Add(incidencia);

            await _db.SaveChangesAsync();

            return incidencia.Id;
        }
    }
}
