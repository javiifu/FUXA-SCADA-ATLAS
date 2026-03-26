using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("JornadaOperario")]
    public class JornadaOperario
    {
        [Key]
        public int Id { get; set; }

        [Column("IdEmpleado")]
        public int EmpleadoId { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        [ForeignKey(nameof(EmpleadoId))]
        public virtual Empleado? Empleado { get; set; }

        public virtual ICollection<FichajeEvento> FichajeEventos { get; set; } = new List<FichajeEvento>();


    }
}
