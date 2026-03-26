using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("ImputacionMaquina")]
    public class ImputacionMaquina
    {
        [Key]
        public int Id { get; set; }

        [Column("IdMaquinaOrden")]
        public int MaquinaOrdenId { get; set; }

        [Column("IdOperacion")]
        public int OperacionId { get; set; }

        [Column("IdEmpleado")]
        public int? EmpleadoId { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? CantidadProducida { get; set; }
        public int? CantidadBuena { get; set; }
        public int? CantidadScrap { get; set; }
        public int? CantidadRetrabajo { get; set; }

        [MaxLength(200)]
        public string? MotivoCierre { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        [MaxLength(500)]
        public string? ObservacionesCierre { get; set; }

        [MaxLength(20)]
        public string TipoImputacion { get; set; } = "Manual";

        [MaxLength(20)]
        public string Estado { get; set; } = "Preparacion";

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        [ForeignKey(nameof(MaquinaOrdenId))]
        public virtual MaquinaOrden? MaquinaOrden { get; set; }

        [ForeignKey(nameof(OperacionId))]
        public virtual Operacion? Operacion { get; set; }

        [ForeignKey(nameof(EmpleadoId))]
        public virtual Empleado? Empleado { get; set; }

        public virtual ICollection<ImputacionOperario> ImputacionesOperario { get; set; } = new List<ImputacionOperario>();
    }
}