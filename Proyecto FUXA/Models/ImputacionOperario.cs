using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("ImputacionOperarios")]
    public class ImputacionOperario
    {
        [Key]
        public int Id { get; set; }

        [Column("IdEmpleado")]
        public int EmpleadoId { get; set; }

        [Column("IdMaquina")]
        public int MaquinaId { get; set; }

        [Column("IdOrden")]
        public int OrdenId { get; set; }

        [Column("IdOperacion")]
        public int? OperacionId { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        [Column("IdImputacionMaquina")]
        public int ImputacionMaquinaId { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; }

        [ForeignKey(nameof(EmpleadoId))]
        public virtual Empleado? Empleado { get; set; }

        [ForeignKey(nameof(MaquinaId))]
        public virtual Maquina? Maquina { get; set; }

        [ForeignKey(nameof(OrdenId))]
        public virtual MaquinaOrden? Orden { get; set; }

        [ForeignKey(nameof(OperacionId))]
        public virtual Operacion? Operacion { get; set; }

        [ForeignKey(nameof(ImputacionMaquinaId))]
        public virtual ImputacionMaquina? ImputacionMaquina { get; set; }
    }
}