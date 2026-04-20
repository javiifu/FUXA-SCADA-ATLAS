using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Services;

public class ServicioEmpleado
{
    private readonly AppDbContext _db;

    public ServicioEmpleado(AppDbContext db)
    {
        _db = db;
    }
    public async Task <List<Empleado>> GetAllAsync()
    {
        return await _db.Empleados.AsNoTracking().ToListAsync();
    }

    public async Task<List<Empleado>> ObtenerOperariosPorMaquinaAsync(int idMaquina)
    {
        var empleados = await _db.EmpleadoMaquinas
            .Where(me => me.IdMaquina == idMaquina)
            .Include(me => me.Empleado) 
            .Select(me => me.Empleado)
            .ToListAsync();

        return empleados ?? new List<Empleado>();
    }

    public async Task<List<Maquina>> ObtenerMaquinasPorEmpleadoAsync(int idEmpleado)
    {
        var maquinas = await _db.EmpleadoMaquinas
            .Where(me => me.IdEmpleado == idEmpleado)
            .Include(me => me.Maquina)
            .Select(me => me.Maquina)
            .ToListAsync();

        return maquinas ?? new List<Maquina>();
    }

    public async Task AsignarEmpleadoAMaquinaAsync(int idMaquina, int idEmpleado)
    {
        var existe = await _db.EmpleadoMaquinas
            .AnyAsync(me => me.IdMaquina == idMaquina && me.IdEmpleado == idEmpleado);

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
            .FirstOrDefaultAsync(me => me.IdMaquina == idMaquina && me.IdEmpleado == idEmpleado);

        if(relacion != null)
        {
            _db.EmpleadoMaquinas.Remove(relacion);
            await _db.SaveChangesAsync();
        }
    }
}
