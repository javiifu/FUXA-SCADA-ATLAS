using Proyecto_FUXA.Data;
using Proyecto_FUXA.Models;

using Microsoft.EntityFrameworkCore;

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

        public async Task<List<MaquinasOrdenes>> GetOrdenesActivasAsync()
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

        public async Task<bool> CrearNuevaOrdenAsync(MaquinasOrdenes orden)
        {
            try
            {
                _context.MaquinasOrdenes.Add(orden);
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
            var conteoMes = await _context.MaquinasOrdenes
                .CountAsync(o => o.CodigoOrden.StartsWith(prefijo));

            return $"{prefijo}{(conteoMes + 1).ToString("D3")}";
        }
    }
}