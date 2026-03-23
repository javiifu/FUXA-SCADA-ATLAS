using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("Incidencia")]
    public class Incidencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaquinaId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        [MaxLength(20)]
        public string Prioridad { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = string.Empty;

        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }

        [MaxLength(100)]
        public string? UsuarioApertura { get; set; }

        [MaxLength(100)]
        public string? UsuarioAsignado { get; set; }

        [ForeignKey(nameof(MaquinaId))]
        public virtual Maquina? Maquina { get; set; }
    }
}
