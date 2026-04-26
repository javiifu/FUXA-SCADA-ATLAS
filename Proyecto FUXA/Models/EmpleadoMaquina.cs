using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models;

[Table("MaquinasEmpleados")]
public class EmpleadoMaquina
{
    public int IdMaquina { get; set; }
    public int IdEmpleado { get; set; }

    [ForeignKey("IdMaquina")]
    public virtual Maquina Maquina { get; set; } = null!;

    [ForeignKey("IdEmpleado")]
    public virtual Empleado Empleado { get; set; } = null!;
}