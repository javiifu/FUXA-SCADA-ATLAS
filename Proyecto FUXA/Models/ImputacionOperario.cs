using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models;

public class ImputacionOperario
{
    [Key]
    public int Id { get; set; }
    public int IdOperacion { get; set; }
    public int IdEmpleado { get; set; }
    public decimal Horas { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [ForeignKey("IdOperacion")]
    public virtual OperacionesOrden Operacion { get; set; } = null!;

    [ForeignKey("IdEmpleado")]
    public virtual Empleado Empleado { get; set; } = null!;
}
