using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("Operaciones")]
    public class Operacion
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        public bool EsProductiva { get; set; }
        public bool Activa { get; set; }

        public virtual ICollection<ImputacionMaquina> ImputacionesMaquina { get; set; } = new List<ImputacionMaquina>();
        public virtual ICollection<ImputacionOperario> ImputacionesOperario { get; set; } = new List<ImputacionOperario>();
    }
}