using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Proyecto_FUXA.Models
{
    [Table("Mantenimiento")]
    public class Mantenimiento
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public int MaquinaId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = String.Empty;

        public DateTime? FechaProgramada { get; set; }
        public DateTime? FechaRealizada { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; }

        [ForeignKey(nameof(MaquinaId))]
        public virtual Maquina? Maquina { get; set; }
    }
}
