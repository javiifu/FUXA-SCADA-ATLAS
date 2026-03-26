using Proyecto_FUXA.Data;
using Proyecto_FUXA.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Services
{
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

        public async Task<List<OperacionResumenDTO>> ObtenerOperacionesActivasAsync()
        {
            return await _context.OperacionesOrden
                .Where(op => op.Estado == "Activa" || op.Estado == "Pendiente")
                .Select(op => new OperacionResumenDTO
                {
                    Id = op.Id,
                    CodigoOperacion = op.CodigoOperacion,
                    NombreMaquina = op.Maquina.Nombre,
                    CodigoOrden = op.Orden.CodigoOrden,
                    Producto = op.Orden.Producto,
                    CiclosObjetivo = op.CiclosObjetivo,
                    PiezasFabricadas = 0,
                    PiezasRotas = 0,
                    FechaCreacion = op.FechaCreacion,
                    Estado = op.Estado
                })
                .OrderByDescending(op => op.FechaCreacion)
                .ToListAsync();
        }
    }

    public class OperacionResumenDTO
    {
        public int Id { get; internal set; }
        public string CodigoOperacion { get; internal set; }
        public string NombreMaquina { get; internal set; }
        public object CodigoOrden { get; internal set; }
        public object Producto { get; internal set; }
        public int CiclosObjetivo { get; internal set; }
        public int PiezasFabricadas { get; internal set; }
        public int PiezasRotas { get; internal set; }
        public DateTime FechaCreacion { get; internal set; }
        public string Estado { get; internal set; }
    }
}