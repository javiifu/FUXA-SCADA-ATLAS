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
}
