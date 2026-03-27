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

        public class OperacionResumenDTO
        {
            public int Id { get; set; }
            public int IdMaquina { get; set; }
            public string CodigoOperacion { get; set; } = "";
            public string NombreOrden { get; set; } = "";   
            public string NombreMaquina { get; set; } = "";
            public string Producto { get; set; } = "";       
            public int CiclosObjetivo { get; set; }
            public int PiezasFabricadas { get; set; }
            public string Estado { get; set; } = "";
            public DateTime FechaInicio { get; set; }
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
                    .Where(o => o.Estado != "cerrada")
                    .OrderByDescending(o => o.FechaInicio)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Orden>();
            }
        }
        public async Task<List<OperacionResumenDTO>> ObtenerOperacionesActivasAsync()
        {
            try
            {
                return await _context.OperacionesOrden
                    .Include(o => o.Orden)   
                    .Include(o => o.Maquina) 
                    .Where(o => o.Estado == "Activa")
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
                        Estado = o.Estado,
                        FechaInicio = o.FechaCreacion 
                    }).ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return new List<OperacionResumenDTO>();
            }
        }

    }
}