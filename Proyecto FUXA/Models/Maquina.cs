using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Proyecto_FUXA.Models;


namespace Proyecto_FUXA.Models
{
    [Table("Maquina")]
    public class Maquina
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? IdSeccion { get; set; }
        public int? NumeroOrden { get; set; }
        public string? NombreSeccion { get; set; } = string.Empty;
        public int? EmpleadoId { get; set; }
        public int? EstadoActualId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        [NotMapped]
        public string NombreOperario { get; set; } = string.Empty ;
        [NotMapped]
        public string CargoOperario { get; set; } = string.Empty;
        [NotMapped]
        public float PorcentajeCiclos { get; set;  }
        [NotMapped]
        public int CiclosObjetivo { get; set; }
        [NotMapped]
        public int CiclosReales { get; set; }
        [JsonIgnore]
        public Empleado? Empleado { get; set; }

        [ForeignKey("IdSeccion")]
        public virtual Seccion? Seccion { get; set; }

        public virtual ICollection<MaquinaProduccion> Producciones { get; set; } = new List<MaquinaProduccion>();

        [ForeignKey("EstadoActualId")]
        public virtual MaquinaEstatus? EstadoActual { get; set; }
    }
}