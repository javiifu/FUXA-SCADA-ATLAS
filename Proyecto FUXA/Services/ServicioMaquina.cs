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
        private static readonly string[] PrioridadesIncidenciaBloqueante = ["Alta", "Crítica", "Critica", "Critical"];
        private static readonly HashSet<string> EstadosImputacionPermitidos = ["Preparacion", "EnCurso", "Pausada", "Finalizada", "Cancelada"];

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

        public async Task<List<MaquinaGridDto>> GetMaquinasGridAsync()
        {
            var maquinas = await _db.Maquinas
                .AsNoTracking()
                .OrderBy(m => m.Nombre)
                .Select(m => new MaquinaGridDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Seccion = m.Seccion.Nombre,
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

        // Añadir una nueva máquina
        public async Task AddAsync(Maquina maquina)
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
                .FirstOrDefaultAsync(m => m.FuxaDeviceId == idFuxa);
        }

        public async Task GuardarMaquina(Maquina maquina)
        {

            var existe = maquina.Id > 0 || await _db.Maquinas.AnyAsync(m => m.FuxaDeviceId == maquina.FuxaDeviceId);

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


        public async Task<List<Maquina>> ObtenerMaquinasActivasAsync()
        {
            return await _db.Maquinas
                .AsNoTracking()
                .Where(m => m.EstaActivo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<List<Operacion>> ObtenerOperacionesActivasAsync()
        {
            return await _db.Operaciones
                .AsNoTracking()
                .Where(o => o.Activa)
                .OrderBy(o => o.Nombre)
                .ToListAsync();
        }

        public async Task<List<Empleado>> ObtenerEmpleadosActivosAsync()
        {
            return await _db.Empleados
                .AsNoTracking()
                .Where(e => e.EstaActivo)
                .OrderBy(e => e.Nombre)
                .ThenBy(e => e.Apellidos)
                .ToListAsync();
        }

        public async Task<EstadoImputacionMaquinaDto?> ObtenerEstadoImputacionMaquinaAsync(int maquinaId)
        {
            var maquina = await _db.Maquinas
                .AsNoTracking()
                .Include(m => m.Seccion)
                .Include(m => m.EstadoActual)
                .FirstOrDefaultAsync(m => m.Id == maquinaId);

            if (maquina is null)
            {
                return null;
            }

            var ordenActiva = await _db.MaquinasOrdenes
                .AsNoTracking()
                .Include(o => o.Operacion)
                .Where(mo => mo.MaquinaId == maquinaId && mo.FechaFin == null)
                .OrderByDescending(mo => mo.FechaInicio)
                .FirstOrDefaultAsync();

            var estado = new EstadoImputacionMaquinaDto
            {
                MaquinaId = maquina.Id,
                NombreMaquina = maquina.Nombre,
                Seccion = maquina.Seccion?.Nombre ?? "Sin sección",
                EstadoMaquinaFisica = maquina.EstadoActual?.Nombre ?? $"Estado #{maquina.EstadoActualId}",
                MaquinaOrdenId = ordenActiva?.Id,
                OrdenCodigo = ordenActiva?.CodigoOrden,
                OperacionId = ordenActiva?.OperacionId,
                OperacionNombre = ordenActiva?.Operacion?.Nombre
            };
            estado.MotivoBloqueoInicio = await ObtenerMotivoBloqueoInicioImputacionAsync(maquina, ordenActiva);
            estado.PuedeIniciarImputacion = string.IsNullOrEmpty(estado.MotivoBloqueoInicio);
            if (ordenActiva is null)
            {
                return estado;
            }

            var imputacionAbierta = await _db.ImputacionesMaquina
                .AsNoTracking()
                .Include(i => i.Operacion)
                .Where(i => i.MaquinaOrdenId == ordenActiva.Id && i.FechaFin == null)
                .OrderByDescending(i => i.FechaInicio)
                .FirstOrDefaultAsync();

            if (imputacionAbierta is null)
            {
                return estado;
            }

            estado.ImputacionMaquinaId = imputacionAbierta.Id;
            estado.OperacionId = imputacionAbierta.OperacionId;
            estado.OperacionNombre = imputacionAbierta.Operacion?.Nombre ?? estado.OperacionNombre;
            estado.FechaInicioMaquina = imputacionAbierta.FechaInicio;
            estado.FechaFinMaquina = imputacionAbierta.FechaFin;
            estado.EstadoImputacion = imputacionAbierta.Estado;
            estado.CantidadProducida = imputacionAbierta.CantidadProducida;
            estado.CantidadBuena = imputacionAbierta.CantidadBuena;
            estado.CantidadScrap = imputacionAbierta.CantidadScrap;
            estado.CantidadRetrabajo = imputacionAbierta.CantidadRetrabajo;
            estado.OperariosActivos = await _db.ImputacionesOperario
                .AsNoTracking()
                .Include(o => o.Empleado)
                .Where(o => o.ImputacionMaquinaId == imputacionAbierta.Id && o.FechaFin == null)
                .OrderBy(o => o.FechaInicio)
                .Select(o => new OperarioActivoDto
                {
                    ImputacionOperarioId = o.Id,
                    EmpleadoId = o.EmpleadoId,
                    EmpleadoNombre = (o.Empleado!.Nombre + " " + o.Empleado.Apellidos).Trim(),
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin
                })
                .ToListAsync();

            return estado;
        }

        public async Task<int> IniciarImputacionMaquinaAsync(IniciarImputacionMaquinaRequest request)
        {
            var maquina = await _db.Maquinas
                .Include(m => m.EstadoActual)
                .FirstOrDefaultAsync(m => m.Id == request.MaquinaId);

            if (maquina is null)
            {
                throw new InvalidOperationException("La máquina indicada no existe");
            }

            var ordenActiva = await _db.MaquinasOrdenes
                .Include(o => o.Operacion)
                .Where(mo => mo.MaquinaId == request.MaquinaId && mo.FechaFin == null)
                .OrderByDescending(mo => mo.FechaInicio)
                .FirstOrDefaultAsync();

            var motivoBloqueo = await ObtenerMotivoBloqueoInicioImputacionAsync(maquina, ordenActiva);
            if (!string.IsNullOrEmpty(motivoBloqueo))
            {
                throw new InvalidOperationException(motivoBloqueo);
            }

            var nueva = new ImputacionMaquina
            {
                MaquinaOrdenId = ordenActiva!.Id,
                OperacionId = ordenActiva.OperacionId!.Value,
                EmpleadoId = request.EmpleadoResponsableId,
                FechaInicio = DateTime.UtcNow,
                Observaciones = request.Observaciones,
                TipoImputacion = string.IsNullOrWhiteSpace(request.TipoImputacion) ? "Manual" : request.TipoImputacion.Trim(),
                Estado = "En Curso",
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };

            _db.ImputacionesMaquina.Add(nueva);
            await _db.SaveChangesAsync();
            return nueva.Id;
        }

        public async Task CambiarEstadoImputacionMaquinaAsync(CambiarEstadoImputacionMaquinaRequest request)
        {
            var imputacion = await _db.ImputacionesMaquina
                .FirstOrDefaultAsync(i => i.Id == request.ImputacionMaquinaId);

            if (imputacion is null)
            {
                throw new InvalidOperationException("No existe la imputación de máquina indicada.");
            }

            var nuevoEstado = (request.NuevoEstado ?? string.Empty).Trim();
            if (!EstadosImputacionPermitidos.Contains(nuevoEstado))
            {
                throw new InvalidOperationException("El estado de imputación solicitado no es válido.");
            }

            if (!EsTransicionValida(imputacion.Estado, nuevoEstado))
            {
                throw new InvalidOperationException($"No se puede cambiar la imputación de '{imputacion.Estado}' a '{nuevoEstado}'.");
            }

            if ((nuevoEstado == "Finalizada" || nuevoEstado == "Cancelada") && imputacion.FechaFin == null)
            {
                throw new InvalidOperationException("Use el cierre de imputación para finalizar o cancelar el trabajo.");
            }

            imputacion.Estado = nuevoEstado;
            imputacion.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }


        public async Task FinalizarImputacionMaquinaAsync(FinalizarImputacionMaquinaRequest request)
        {
            var imputacion = await _db.ImputacionesMaquina
                .FirstOrDefaultAsync(i => i.Id == request.ImputacionMaquinaId);

            if (imputacion is null)
            {
                throw new InvalidOperationException("No existe la imputación de máquina indicada.");
            }

            if (imputacion.FechaFin != null)
            {
                throw new InvalidOperationException("La imputación de máquina ya está cerrada.");
            }

            if (request.CantidadBuena < 0 || request.CantidadScrap < 0 || request.CantidadRetrabajo < 0)
            {
                throw new InvalidOperationException("Las cantidades de cierre no pueden ser negativas.");
            }

            if (string.IsNullOrWhiteSpace(request.MotivoCierre))
            {
                throw new InvalidOperationException("Debe indicar el motivo de cierre de la imputación.");
            }

            var operariosActivos = await _db.ImputacionesOperario
                .AnyAsync(i => i.ImputacionMaquinaId == imputacion.Id && i.FechaFin == null);

            if (operariosActivos)
            {
                throw new InvalidOperationException("No se puede finalizar la máquina mientras haya operarios activos.");
            }

            imputacion.CantidadBuena = request.CantidadBuena;
            imputacion.CantidadScrap = request.CantidadScrap;
            imputacion.CantidadRetrabajo = request.CantidadRetrabajo;
            imputacion.CantidadProducida = request.CantidadBuena + request.CantidadScrap + request.CantidadRetrabajo;
            imputacion.MotivoCierre = request.MotivoCierre.Trim();
            imputacion.ObservacionesCierre = string.IsNullOrWhiteSpace(request.ObservacionesCierre) ? null : request.ObservacionesCierre.Trim();
            imputacion.Estado = "Finalizada";
            imputacion.FechaActualizacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
        public async Task<int> IniciarImputacionOperarioAsync(IniciarImputacionOperarioRequest request)
        {
            var imputacion = await _db.ImputacionesMaquina
                .Include(i => i.MaquinaOrden)
                .FirstOrDefaultAsync(i => i.Id == request.ImputacionMaquinaId);

            if (imputacion is null || imputacion.FechaFin != null)
            {
                throw new InvalidOperationException("No existe una imputación de máquina abierta.");
            }

            if (imputacion.Estado == "Pausada" || imputacion.Estado == "Cancelada" || imputacion.Estado == "Finalizada")
            {
                throw new InvalidOperationException("No se puede iniciar imputación de operario con la imputación de máquina en estado no operativo.");
            }
            var existeAbiertaGlobal = await _db.ImputacionesOperario
                .AnyAsync(i => i.EmpleadoId == request.EmpleadoId && i.FechaFin == null);

            if (existeAbiertaGlobal)
            {
                throw new InvalidOperationException("El operario ya tiene una imputación activa en esta máquina u orden");
            }

            var item = new ImputacionOperario
            {
                EmpleadoId = request.EmpleadoId,
                MaquinaId = imputacion.MaquinaOrden!.MaquinaId,
                OrdenId = imputacion.MaquinaOrdenId,
                OperacionId = imputacion.OperacionId,
                ImputacionMaquinaId = imputacion.Id,
                FechaInicio = DateTime.UtcNow,
                Observaciones = request.Observaciones,
                FechaCreacion = DateTime.UtcNow
            };

            _db.ImputacionesOperario.Add(item);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FechaFin") == true || ex.InnerException?.Message.Contains("Empleado") == true)
            {
                throw new InvalidOperationException("El operario ya tiene una imputación activa en otra máquina u orden.");
            }
            return item.Id;
        }

        public async Task FinalizarImputacionOperarioAsync(FinalizarImputacionOperarioRequest request)
        {
            var imputacion = await _db.ImputacionesOperario
                .FirstOrDefaultAsync(i => i.Id == request.ImputacionOperarioId);

            if (imputacion is null)
            {
                throw new InvalidOperationException("No existe la imputación de operario indicada.");
            }

            if (imputacion.FechaFin != null)
            {
                return;
            }

            imputacion.FechaFin = DateTime.UtcNow;
            imputacion.Observaciones = string.IsNullOrWhiteSpace(request.Observaciones) ? imputacion.Observaciones : request.Observaciones;
            await _db.SaveChangesAsync();
        }

        public async Task<List<ImputacionMaquinaListadoDto>> ObtenerImputacionesAsync(int? maquinaId = null, bool soloAbiertas = false)
        {
            var query = _db.ImputacionesMaquina
                .AsNoTracking()
                .Include(i => i.MaquinaOrden)!.ThenInclude(mo => mo!.Maquina)
                .Include(i => i.Operacion)
                .AsQueryable();

            if (maquinaId.HasValue)
            {
                query = query.Where(i => i.MaquinaOrden!.MaquinaId == maquinaId.Value);
            }

            if (soloAbiertas)
            {
                query = query.Where(i => i.FechaFin == null);
            }

            return await query
                .OrderByDescending(i => i.FechaInicio)
                .Select(i => new ImputacionMaquinaListadoDto
                {
                    ImputacionMaquinaId = i.Id,
                    Maquina = i.MaquinaOrden!.Maquina!.Nombre,
                    Seccion = i.MaquinaOrden.Maquina.Seccion.Nombre,
                    Orden = i.MaquinaOrden.CodigoOrden,
                    Operacion = i.Operacion!.Nombre,
                    Estado = i.Estado,
                    FechaInicio = i.FechaInicio,
                    FechaFin = i.FechaFin,
                    CantidadProducida = i.CantidadProducida,
                    CantidadBuena = i.CantidadBuena,
                    CantidadScrap = i.CantidadScrap,
                    CantidadRetrabajo = i.CantidadRetrabajo,
                    MotivoCierre = i.MotivoCierre,
                    OperariosAsociados = i.ImputacionesOperario.Count()
                })
                .ToListAsync();
        }

        public async Task<ImputacionDetalleDto?> ObtenerDetalleImputacionAsync(int imputacionMaquinaId)
        {
            var cabecera = await _db.ImputacionesMaquina
                .AsNoTracking()
                .Include(i => i.MaquinaOrden)!.ThenInclude(mo => mo!.Maquina)
                .Include(i => i.Operacion)
                .Where(i => i.Id == imputacionMaquinaId)
                .Select(i => new ImputacionMaquinaListadoDto
                {
                    ImputacionMaquinaId = i.Id,
                    Maquina = i.MaquinaOrden!.Maquina!.Nombre,
                    Seccion = i.MaquinaOrden.Maquina.Seccion.Nombre,
                    Orden = i.MaquinaOrden.CodigoOrden,
                    Operacion = i.Operacion!.Nombre,
                    Estado = i.Estado,
                    FechaInicio = i.FechaInicio,
                    FechaFin = i.FechaFin,
                    CantidadProducida = i.CantidadProducida,
                    CantidadBuena = i.CantidadBuena,
                    CantidadScrap = i.CantidadScrap,
                    CantidadRetrabajo = i.CantidadRetrabajo,
                    MotivoCierre = i.MotivoCierre,
                    OperariosAsociados = i.ImputacionesOperario.Count()
                })
                .FirstOrDefaultAsync();

            if (cabecera is null)
            {
                return null;
            }

            var operarios = await _db.ImputacionesOperario
                .AsNoTracking()
                .Include(i => i.Empleado)
                .Where(i => i.ImputacionMaquinaId == imputacionMaquinaId)
                .OrderBy(i => i.FechaInicio)
                .Select(i => new ImputacionOperarioDto
                {
                    Id = i.Id,
                    EmpleadoId = i.EmpleadoId,
                    EmpleadoNombre = (i.Empleado!.Nombre + " " + i.Empleado.Apellidos).Trim(),
                    FechaInicio = i.FechaInicio,
                    FechaFin = i.FechaFin,
                    Observaciones = i.Observaciones
                })
                .ToListAsync();

            return new ImputacionDetalleDto
            {
                Cabecera = cabecera,
                Operarios = operarios
            };
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

        private async Task<string?> ObtenerMotivoBloqueoInicioImputacionAsync(Maquina maquina, MaquinaOrden? ordenActiva)
        {
            if (!maquina.EstaActivo)
            {
                return "La máquina está inactiva y no puede iniciar imputación.";
            }

            if (EsEstadoFisicoBloqueante(maquina.EstadoActualId, maquina.EstadoActual?.Nombre))
            {
                return "El estado físico actual de la máquina no permite iniciar producción.";
            }

            if (ordenActiva is null)
            {
                return "La máquina no tiene una orden activa.";
            }

            if (!ordenActiva.OperacionId.HasValue)
            {
                return "La orden activa no tiene operación asignada.";
            }

            var operacionValida = await _db.Operaciones
                .AnyAsync(o => o.Id == ordenActiva.OperacionId.Value && o.Activa);

            if (!operacionValida)
            {
                return "La operación de la orden activa no existe o no está activa.";
            }

            var existeAbierta = await _db.ImputacionesMaquina
                .AnyAsync(i => i.MaquinaOrdenId == ordenActiva.Id && i.FechaFin == null);

            if (existeAbierta)
            {
                return "Ya existe una imputación de máquina abierta para la orden activa.";
            }

            var incidenciaGraveAbierta = await _db.Incidencias
                .AnyAsync(i => i.MaquinaId == maquina.Id
                    && !EstadosIncidenciaCerrada.Contains(i.Estado)
                    && PrioridadesIncidenciaBloqueante.Contains(i.Prioridad));

            if (incidenciaGraveAbierta)
            {
                return "Existe una incidencia grave abierta que bloquea el inicio de imputación.";
            }

            return null;
        }

        private static bool EsEstadoFisicoBloqueante(int estadoActualId, string? nombreEstado)
        {
            if (estadoActualId == 3 || estadoActualId == 4)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(nombreEstado))
            {
                return false;
            }

            var normalizado = nombreEstado.Trim().ToLowerInvariant();
            return normalizado.Contains("parada") || normalizado.Contains("no disponible") || normalizado.Contains("mantenimiento");
        }

        private static bool EsTransicionValida(string estadoActual, string nuevoEstado)
        {
            if (estadoActual == nuevoEstado)
            {
                return true;
            }

            return estadoActual switch
            {
                "Preparacion" => nuevoEstado is "EnCurso" or "Cancelada",
                "EnCurso" => nuevoEstado is "Pausada",
                "Pausada" => nuevoEstado is "EnCurso",
                "Finalizada" => false,
                "Cancelada" => false,
                _ => false
            };
        }
    }
}
