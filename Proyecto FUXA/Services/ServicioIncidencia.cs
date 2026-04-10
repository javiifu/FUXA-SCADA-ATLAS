
using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.DTO;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Services
{
    public class ServicioIncidencia
    {
        private static readonly HashSet<string> EstadosPermitidos = new(StringComparer.OrdinalIgnoreCase)
        {
            "Abierta",
            "EnCurso",
            "Resuelta",
            "Cerrada"
        };

        private readonly AppDbContext _db;

        public ServicioIncidencia(AppDbContext db)
        {
            _db = db;
        }

        public async Task ActualizarEstadoAsync(int incidenciaId, string nuevoEstado)
        {
            nuevoEstado = (nuevoEstado ?? string.Empty).Trim();

            if (!EstadosPermitidos.Contains(nuevoEstado))
            {
                throw new InvalidOperationException("El estado de la incidencia no es válido.");
            }

            var incidencia = await _db.Incidencias
                .FirstOrDefaultAsync(i => i.Id == incidenciaId);

            if (incidencia is null)
            {
                throw new InvalidOperationException("La incidencia no existe.");
            }

            incidencia.Estado = nuevoEstado;
            incidencia.FechaCierre = nuevoEstado is "Resuelta" or "Cerrada"
                ? DateTime.Now
                : null;

            await _db.SaveChangesAsync();
        }

        public async Task<List<IncidenciaDetalleDto>> ObtenerHistoricoIncidenciasPorMaquinaAsync(int maquinaId)
        {
            return await _db.Incidencias
                .AsNoTracking()
                .Where(i => i.MaquinaId == maquinaId)
                .OrderByDescending(i => i.FechaApertura)
                .ThenByDescending(i => i.Id)
                .Select(i => new IncidenciaDetalleDto
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
        }
    }
}