using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{

    [Table("FichajeEvento")]
    public class FichajeEvento
    {
        [Key]
        public int Id { get; set; }

        [Column("IdJornadaOperario")]
        public int JornadaOperarioId { get; set; }

        [MaxLength(20)]
        public string TipoEvento { get; set; } = string.Empty;

        public DateTime FechaHora { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; }

        [ForeignKey(nameof(JornadaOperarioId))]
        public virtual JornadaOperario? JornadaOperario { get; set; }
    }
}
