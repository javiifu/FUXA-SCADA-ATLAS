using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("ProduccionMaquinas")]
    public class MaquinaProduccion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MaquinaId { get; set; }

        public int CiclosReales { get; set; }
        public DateTime FechaRegistro { get; set; }


    public DateTime? FechaRegistro { get; set; }
}
