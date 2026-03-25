using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("MaquinasOrdenes")]
    public class MaquinaOrden
    {
        [Key]
        public int Id { get; set; }

        [Column("IdMaquina")]
        public int MaquinaId { get; set; }

        [MaxLength(50)]
        public string CodigoOrden { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Producto { get; set; } = string.Empty;

        public int CiclosObjetivo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        [MaxLength(30)]
        public string Estado { get; set; } = string.Empty;

        public int? EmpleadoId { get; set; }

        [ForeignKey(nameof(MaquinaId))]
        public virtual Maquina? Maquina { get; set; }

        [ForeignKey(nameof(EmpleadoId))]
        public virtual Empleado? Empleado { get; set; }

        public virtual ICollection<ImputacionMaquina> ImputacionesMaquina { get; set; } = new List<ImputacionMaquina>();
    }
}