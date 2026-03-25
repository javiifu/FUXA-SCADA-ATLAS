using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;

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
        public bool EstaActivo { get; set; }
        public int? IdSeccion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        

        public virtual ICollection<MaquinaProduccion> Producciones { get; set; } = new List<MaquinaProduccion>();

        public virtual ICollection<PlantaObjetoVisual> ObjetosVisualesPlanta { get; set; } = new List<PlantaObjetoVisual>();

        public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
        public virtual ICollection<Incidencia> Incidencias { get; set; } = new List<Incidencia>();
        public virtual ICollection<MaquinaOrden> MaquinasOrdenes { get; set; } = new List<MaquinaOrden>();
        public virtual ICollection<ImputacionOperario> ImputacionesOperario { get; set; } = new List<ImputacionOperario>();

        [ForeignKey(nameof(IdSeccion))]
        public virtual Seccion? Seccion { get; set; }

        [ForeignKey("EstadoActualId")]
        public virtual MaquinaEstatus? EstadoActual { get; set; }

        [NotMapped]
        public int? PosX { get; set; }
        [NotMapped]
        public int? PosY { get; set; }
    }
}
