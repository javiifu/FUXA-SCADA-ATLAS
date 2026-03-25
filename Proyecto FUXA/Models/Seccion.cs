using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("Secciones")]
    public class Seccion
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        public bool Activa { get; set; }

        public virtual ICollection<Maquina> Maquinas { get; set; } = new List<Maquina>();
    }
}