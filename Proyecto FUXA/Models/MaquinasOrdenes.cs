using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("MaquinasOrdenes")]
    public class MaquinasOrdenes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdMaquina { get; set; }
        public string CodigoOrden { get; set; } = string.Empty;
        public string Producto {  get; set; } = string.Empty;
        public int CiclosObjetivo { get; set;  }
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } = "Abierta";
        public int EmpleadoId { get; set; }

    }
}
