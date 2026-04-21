using Proyecto_FUXA.Data;
using Proyecto_FUXA.Models;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;
using Proyecto_FUXA.Components.Pages;


namespace Proyecto_FUXA.Services;

public class ImputacionService
{
    private readonly AppDbContext _context;

    public ImputacionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Seccion>> GetSeccionesAsync()
    {
        return await _context.Secciones.OrderBy(s => s.Nombre).ToListAsync();
    }

    public async Task<List<Maquina>> GetMaquinasBySeccionAsync(int idSeccion)
    {
        return await _context.Maquinas
            .Where(m => m.IdSeccion == idSeccion)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<List<Empleado>> GetEmpleadosAsync()
    {
        return await _context.Empleados.OrderBy(e => e.Nombre).ToListAsync();
    }

    public async Task<List<Operacion>> GetOperacionesAsync()
    {
        return await _context.Operaciones.OrderBy(o => o.Nombre).ToListAsync();
    }

    public async Task<List<Orden>> GetOrdenesActivasAsync()
    {
        return await _context.Ordenes.ToListAsync();
    }

    public async Task<bool> GuardarImputacionAsync(ImputacionOperario imputacion)
    {
        try
        {
            _context.ImputacionesOperarios.Add(imputacion);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CrearNuevaOrdenAsync(Orden orden)
    {
        try
        {
            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<string> GenerarProximoCodigoOrdenAsync()
    {
        var fecha = DateTime.Now;
        string prefijo = $"ORD-{fecha:yyMM}-";

        // buscamos cuantas ordenes hay ya este mes para seguir el contador
        var conteoMes = await _context.Ordenes
            .CountAsync(o => o.CodigoOrden.StartsWith(prefijo));

        return $"{prefijo}{(conteoMes + 1).ToString("D3")}";
    }

    public async Task<bool> InsertarOrdenMadreAsync(Orden orden)
    {
        try
        {
            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<List<Orden>> ObtenerOrdenesActivasParaAsignarAsync()
    {
        //ordenes que no estén cerradas y ordenadas por la más reciente
        return await _context.Ordenes
            .Where(o => o.Estado != "Cerrada")
            .OrderByDescending(o => o.Id)
            .ToListAsync();
    }

    public async Task<bool> CrearNuevaOperacionAsync(int idOrden, int idMaquina, int ciclos)
    {
        try
        {
            var ordenPadre = await _context.Ordenes.FindAsync(idOrden);

            //cuenta cuántas operaciones tiene ya esta orden para calcular el sufijo (-1, -2...)
            var totalOperaciones = await _context.OperacionesOrden.CountAsync(op => op.IdOrden == idOrden);
            string nuevoCodigo = $"{ordenPadre.CodigoOrden}-{totalOperaciones + 1}";

            var nuevaOp = new OperacionesOrden
            {
                IdOrden = idOrden,
                CodigoOperacion = nuevoCodigo,
                IdMaquina = idMaquina,
                CiclosObjetivo = ciclos,
                Estado = "Activa",
                FechaCreacion = DateTime.Now
            };

            _context.OperacionesOrden.Add(nuevaOp);
            await _context.SaveChangesAsync();
            return true;
        }
        catch { return false; }
    }

    public class OperacionResumenDTO
    {
        public int Id { get; set; }
        public int IdMaquina { get; set; }
        public string CodigoOperacion { get; set; } = "";
        public string NombreOrden { get; set; } = "";   
        public string NombreMaquina { get; set; } = "";
        public string Producto { get; set; } = "";       
        public int CiclosObjetivo { get; set; }
        public int PiezasFabricadas { get; set; } = 0;
        public int PiezasRotas { get; set; } = 0;
        public string Estado { get; set; } = "";
        public int EstadoMaquinaId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    public async Task<Orden?> GetOrdenById(string codigo)
    {
        return await _context.Ordenes.FirstOrDefaultAsync(o => o.CodigoOrden == codigo);
    }

    public async Task<bool> AsignarOrdenAMaquinaAsync(int idOrden, int idMaquina, int ciclos)
    {
        var ordenPadre = await _context.Ordenes.FindAsync(idOrden);
        if (ordenPadre == null) return false;

        // Contamos cuántas operaciones tiene ya esa orden para el sufijo
        var conteo = await _context.OperacionesOrden.CountAsync(op => op.IdOrden == idOrden);

        var nuevaOp = new OperacionesOrden
        {
            IdOrden = idOrden,
            IdMaquina = idMaquina,
            CodigoOperacion = $"{ordenPadre.CodigoOrden}-{conteo + 1}",
            CiclosObjetivo = ciclos,
            PiezasFabricadas = 0,
            PiezasRotas = 0,
            Estado = "Activa",
            FechaCreacion = DateTime.Now,
            IdSeccion = 1,
            IdOperacionMaestra = 1
        };

        _context.OperacionesOrden.Add(nuevaOp);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<List<Orden>> ObtenerOrdenesActivasAsync()
    {
        try
        {
            return await _context.Ordenes
                .Where(o => o.Estado == "Activa")
                .OrderByDescending(o => o.FechaInicio)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            return new List<Orden>();
        }
    }

    public async Task<List<Orden>> ObtenerOrdenesAsync()
    {
        return await _context.Ordenes.ToListAsync();
    }
    public async Task<List<OperacionResumenDTO>> ObtenerOperacionesActivasAsync()
    {
        try
        {
            return await _context.OperacionesOrden
                .Include(o => o.Orden)
                .Include(o => o.Maquina)
                .Where(o => o.Estado == "Activa" || o.Estado == "Pendiente")
                .Select(o => new OperacionResumenDTO
                {
                    Id = o.Id,
                    IdMaquina = o.IdMaquina,
                    CodigoOperacion = o.CodigoOperacion,
                    NombreOrden = o.Orden != null ? o.Orden.CodigoOrden : "Sin Código",
                    Producto = o.Orden != null ? o.Orden.Producto : "Sin Producto",
                    NombreMaquina = o.Maquina != null ? o.Maquina.Nombre : "Sin Máquina",
                    CiclosObjetivo = o.CiclosObjetivo,
                    PiezasFabricadas = o.PiezasFabricadas,
                    PiezasRotas = o.PiezasRotas,
                    Estado = o.Estado,
                    FechaInicio = o.FechaCreacion,
                    FechaFin = o.FechaFin
                })
                .OrderByDescending(o => o.FechaInicio)
                .ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            return new List<OperacionResumenDTO>();
        }
    }

    public async Task<string> GenerarSiguienteCodigoOperacionAsync(string codigoOrdenBase)
    {
        var cantidad = await _context.OperacionesOrden.CountAsync(o => o.CodigoOperacion.StartsWith(codigoOrdenBase));

        return $"{codigoOrdenBase}-{cantidad + 1}";
    }
    public async Task<List<OperacionResumenDTO>> ObtenerTodasLasOperacionesResumenAsync()
    {
        try
        {
            return await _context.OperacionesOrden
                .Include(o => o.Orden)
                .Include(o => o.Maquina)
                .Select(o => new OperacionResumenDTO
                {
                    Id = o.Id,
                    IdMaquina = o.IdMaquina,
                    CodigoOperacion = o.CodigoOperacion,
                    NombreOrden = o.Orden != null ? o.Orden.CodigoOrden : "Sin Orden",
                    Producto = o.Orden != null ? o.Orden.Producto : "Sin Producto",
                    NombreMaquina = o.Maquina != null ? o.Maquina.Nombre : "Sin Máquina",
                    CiclosObjetivo = o.CiclosObjetivo,
                    PiezasFabricadas = o.PiezasFabricadas,
                    PiezasRotas = o.PiezasRotas,
                    Estado = o.Estado,
                    FechaInicio = o.FechaCreacion,
                    FechaFin = o.FechaFin
                }).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            return new List<OperacionResumenDTO>();
        }
    }
    public async Task<bool> ActualizarOrdenAsync(Orden orden)
    {
        try
        {
            _context.Ordenes.Update(orden);
            await _context.SaveChangesAsync();
            return true;
        }
        catch { return false; }
    }

    public async Task<bool> InsertarImputacionOperario(ImputacionOperario nuevaImp)
    {
        try
        {

            nuevaImp.Operacion = null;
            nuevaImp.Empleado = null;

            _context.ImputacionesOperarios.Add(nuevaImp);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }catch(Exception ex){
            Console.WriteLine($"Error al guardarl la imputacion: {ex.Message}");
            return false;
        }
    }

    public async Task ActualizarCierreOperacion(dynamic operacion)
    {
        try
        {
            int idOperacion = (int)operacion.Id;
            int idMaquina = (int)operacion.IdMaquina;

            var op = await _context.OperacionesOrden.FindAsync(idOperacion);

            if (op != null)
            {
                op.Estado = "Finalizado";
                op.FechaFin = (DateTime)operacion.FechaFin;
                op.PiezasFabricadas = (int)operacion.PiezasFabricadas;
                op.PiezasRotas = (int)operacion.PiezasRotas;

                await _context.SaveChangesAsync();
            }

            var maquina = await _context.Maquinas.FindAsync(idMaquina);
            if(maquina != null)
            {
                if(maquina.EstadoActualId != 4)
                {
                    maquina.EstadoActualId = 3;
                }
                maquina.CiclosReales = 0;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en ActualizarCierreOperacion: {ex.Message}");
            throw;
        }
    }

    public async Task<List<Material>> ObtenerMaterialesAsync()
    {
        return await _context.Materiales.OrderByDescending(e => e.Nombre).ToListAsync();
    }

    public async Task<bool> GuardarSeccionAsync(Seccion nuevaSeccion)
    {
        try
        {
            _context.Secciones.Add(nuevaSeccion);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> GuardarMaterialAsync(Material material)
    {
        try
        {
            if(material.Id == 0)
            {
                _context.Materiales.Add(material);
            }
            else
            {
                _context.Materiales.Update(material);
            }

            int filasAfectadas = await _context.SaveChangesAsync();
            if(filasAfectadas > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar material: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> GuardarMultiplesMaterialesAsync(List<ImputacionMaterial> listaMateriales)
    {
        try
        {
            _context.ImputacionMateriales.AddRange(listaMateriales);

            var filasAfectadas = await _context.SaveChangesAsync();

            return filasAfectadas > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar múltiples materiales: {ex.Message}");
            return false;
        }
    }

    public async Task<List<OperarioHorasDTO>> ObtenerHorasPorOperarioAsync(int idOperacion)
    {
        return await _context.ImputacionesOperarios
            .Where(i => i.IdOperacion == idOperacion)
            .GroupBy(i => new {
                i.Empleado.CodigoEmpleado,
                i.Empleado.Nombre,
                i.Empleado.Apellidos
            })
            .Select(grupo => new OperarioHorasDTO
            {
                NombreCompleto = grupo.Key.Nombre + " " + grupo.Key.Apellidos,
                CodigoOperario = grupo.Key.CodigoEmpleado,
                TotalHoras = grupo.Sum(i => i.Horas)
            })
            .ToListAsync();
    }

    private async Task<string> GenerarCodigoMaterial(string nombreMaterial)
    {

        string limpio = new string((nombreMaterial ?? "MAT")
            .Trim()
            .ToUpper()
            .Where(c => char.IsLetter(c)) 
            .ToArray());

        string prefijo = limpio.Length >= 3 ? limpio.Substring(0, 3) : limpio.PadRight(3, 'X');

        int contador = await _context.Materiales
            .Where(m => m.CodigoMaterial.StartsWith(prefijo + "-"))
            .CountAsync();

        return $"{prefijo}-{(contador + 1):D3}";
    }

    public async Task<bool> ImputarTrabajoDesdeTerminalAsync(int idOperacion, int idMaquina, int idEmpleado, DateTime inicio, DateTime fin, int pHechasTurno, int pRotasTurno)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            decimal horasTotales = (decimal)(fin - inicio).TotalHours;

            // sumamos al acumulado de la operacion
            var opActiva = await _context.OperacionesOrden.FindAsync(idOperacion);
            if (opActiva != null)
            {
                opActiva.PiezasFabricadas += pHechasTurno; 
                opActiva.PiezasRotas += pRotasTurno;     
            }

            // Sumamos a los ciclos de la máquina
            var dbMaquina = await _context.Maquinas.FindAsync(idMaquina);
            if (dbMaquina != null)
            {
                dbMaquina.CiclosReales += pHechasTurno;
                dbMaquina.FechaActualizacion = DateTime.Now;
            }

            var nuevaImp = new ImputacionOperario
            {
                IdOperacion = idOperacion,
                IdEmpleado = idEmpleado,
                FechaRegistro = DateTime.Now,
                FechaInicio = inicio,
                FechaFin = fin,
                Horas = Math.Round(horasTotales, 2),
                PiezasFabricadas = pHechasTurno, 
                PiezasRotas = pRotasTurno       
            };

            _context.ImputacionesOperarios.Add(nuevaImp);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> CerrarOperacionAsync(int idOperacion)
    {
        try
        {
            var operacion = await _context.OperacionesOrden.FindAsync(idOperacion);

            if(operacion != null)
            {
                operacion.Estado = "Finalizada";

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }catch(Exception ex)
        {
            
            Console.WriteLine($"Error: { ex.Message}");
            return false;
        }
    }

    public async Task<bool> VerificarOperacionCerrada(int idOperacion)
    {
        try
        {
            var operacion = await _context.OperacionesOrden.FindAsync(idOperacion);

            return operacion != null && operacion.Estado == "Finalizado";
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Material>> ObtenerMaterialesPorMaquinaAsync(int idMaquina)
    {
        try
        {
            var idsMateriales = await _context.MaquinasMateriales
                .Where(mm => mm.IdMaquina == idMaquina)
                .Select(mm => mm.IdMaterial)
                .ToListAsync();

            if (!idsMateriales.Any())
            {
                return new List<Material>();
            }

            return await _context.Materiales
                .Where(m => idsMateriales.Contains(m.Id))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo materiales: {ex.Message}");
            return new List<Material>();
        }
    }

    public async Task<bool> RegistrarConsumoMaterialAsync(int idOperacion, int idMaterial, decimal cantidad, int idEmpleado , string? observaciones = null)
    {
        try
        {
            var nuevoConsumo = new ImputacionMaterial
            {
                IdOperacion = idOperacion,
                IdMaterial = idMaterial,
                IdEmpleado = idEmpleado,
                Cantidad = cantidad,
                Observaciones = observaciones,
                FechaRegistro = DateTime.Now
            };

            _context.ImputacionMateriales.Add(nuevoConsumo);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al registrar el material {ex.Message}");
            return false;
        }
    }

    public async Task<List<ImputacionMaterial>> ObtenerConsumosPorOperacionAsync(int idOperacion)
    {
        try
        {
            var listaConsumos = await _context.ImputacionMateriales
                .AsNoTracking()
                .Where(im => im.IdOperacion == idOperacion)
                .OrderByDescending(im => im.FechaRegistro)
                .ToListAsync();

            var todosLosMateriales = await _context.Materiales.AsNoTracking().ToListAsync();

            foreach (var consumo in listaConsumos)
            {
                consumo.Material = todosLosMateriales.FirstOrDefault(m => m.Id == consumo.IdMaterial);
            }

            return listaConsumos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fatal leyendo BBDD: {ex.Message}");
            return new List<ImputacionMaterial>();
        }
    }

    public async Task<bool> RestadoStockAsync(int idMaterial, decimal cantidadConsumida)
    {
        try
        {
            var material = await _context.Materiales.FindAsync(idMaterial);

            if (material != null)
            {
                if (material.Stock < cantidadConsumida)
                {
                    return false;
                }

                material.Stock -= cantidadConsumida;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }catch (Exception ex)
        {
            Console.WriteLine($"Error al restar stock {ex.Message}");
            return false;
        }
    }
    public async Task<bool> StockMinimoAsync(int idMaquina, decimal cantidadConsumida)
    {
        try
        {
            var material = await _context.Materiales.FindAsync(idMaquina);

            if(material != null)
            {
                decimal stockMinimoAviso = material.Stock - cantidadConsumida;
                if(material.StockMinimo > stockMinimoAviso)
                {
                    return true;
                }
            }
            return false;
        }catch(Exception ex)
        {
            Console.WriteLine($"Error con el stock minimo {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EliminarConsumoAsync(int idConsumo)
    {
        using var transaccion = await _context.Database.BeginTransactionAsync();
        try
        {
            var consumo = await _context.ImputacionMateriales.FindAsync(idConsumo);

            if (consumo == null) return false;

            var material = await _context.Materiales.FindAsync(consumo.IdMaterial);
            if(material != null)
            {
                material.Stock += consumo.Cantidad;
            }
            _context.ImputacionMateriales.Remove(consumo);
            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return true;
        }
        catch (Exception ex)
        {
            await transaccion.RollbackAsync();
            Console.WriteLine($"Error al borrar el consumo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RegistrarConsumoMaterialAsync(int idOperacion, int idMaterial, decimal cantidad, int idEmpleado, bool esMerma)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var nuevoConsumo = new ImputacionMaterial
            {
                IdOperacion = idOperacion,
                IdMaterial = idMaterial,
                Cantidad = cantidad,
                IdEmpleado = idEmpleado,
                EsMerma = esMerma, 
                FechaRegistro = DateTime.Now
            };

            _context.ImputacionMateriales.Add(nuevoConsumo);

            var material = await _context.Materiales.FindAsync(idMaterial);
            if (material != null)
            {
                material.Stock -= cantidad;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> PuedeIniciarOperacionAsync(int idOperacion, int idOrden)
    {
        var opActual = await _context.OperacionesOrden.FindAsync(idOperacion);
        if (opActual == null) return false;

        var hayBloqueo = await _context.OperacionesOrden
            .AnyAsync(o => o.IdOrden == idOrden &&
                           o.Preferencia > opActual.Preferencia &&
                           o.Estado != "Finalizado");

        return !hayBloqueo;
    }

    public async Task IniciarFichajeAsync(int idOperacion, int idEmpleado)
    {
        var operacion = await _context.OperacionesOrden.FindAsync(idOperacion);

        if (operacion != null)
        {
            operacion.Estado = "Activa";

            var nuevaImputacion = new ImputacionOperario
            {
                IdOperacion = idOperacion,
                IdEmpleado = idEmpleado,
                FechaInicio = DateTime.Now
            };

            _context.ImputacionesOperarios.Add(nuevaImputacion);
            await _context.SaveChangesAsync();
        }
    }
    public async Task GenerarHojaRutaAsync(int idOrden)
    {
        var ordenPadre = await _context.Ordenes.FindAsync(idOrden);
        if (ordenPadre == null) return;

        var secciones = await _context.Secciones.ToListAsync();

        var opMaestra = await _context.Operaciones.FirstOrDefaultAsync();

        int contadorSufijo = 1;

        foreach (var seccion in secciones)
        {
            var maquinas = await _context.Maquinas
                .Where(m => m.IdSeccion == seccion.Id)
                .ToListAsync();

            foreach (var maquina in maquinas)
            {
                var nuevaOperacion = new OperacionesOrden
                {
                    IdOrden = idOrden,
                    IdSeccion = seccion.Id,
                    IdMaquina = maquina.Id,
                    Preferencia = seccion.Preferencia,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Now,

                    CodigoOperacion = $"{ordenPadre.CodigoOrden}-{contadorSufijo}",

                    IdOperacionMaestra = opMaestra?.Id ?? 1,

                    CiclosObjetivo = 0 
                };

                _context.OperacionesOrden.Add(nuevaOperacion);
                contadorSufijo++; 
            }
        }

        await _context.SaveChangesAsync();
    }


}