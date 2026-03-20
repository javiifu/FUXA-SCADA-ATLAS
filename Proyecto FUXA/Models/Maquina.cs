using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("Maquina")]
    public class Maquina
    {
        [Key]
        public int Id { get; set; }
        public string? FuxaDeviceId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int NumeroOrden { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int CiclosObjetivo { get; set; }
        public int EstadoActualId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        

        public virtual ICollection<MaquinaProduccion> Producciones { get; set; } = new List<MaquinaProduccion>();

        public virtual ICollection<PlantaObjetoVisual> ObjetosVisualesPlanta { get; set; } = new List<PlantaObjetoVisual>();

        [ForeignKey("EstadoActualId")]
        public virtual MaquinaEstatus? EstadoActual { get; set; }

        [NotMapped]
        public int? PosX { get; set; }
        [NotMapped]
        public int? PosY { get; set; }
    }
}
