using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    public class OperacionOrden
    {
        public int Id { get; set; }
        public int IdOrden { get; set; }
        public string CodigoOperacion { get; set; } = string.Empty;
        public int IdSeccion { get; set; }
        public int IdMaquina { get; set; }
        public int IdOperacionMaestra { get; set; }
        public string Estado { get; set; } = "Pendiente";

        [ForeignKey("IdOrden")]
        public virtual MaquinasOrdenes? OrdenPadre { get; set; }

        [ForeignKey("IdSeccion")]
        public virtual Seccion? Seccion { get; set; }

        [ForeignKey("IdMaquina")]
        public virtual Maquina? Maquina { get; set; }

        [ForeignKey("IdOperacionMaestra")]
        public virtual Operacion? DetalleOperacion { get; set; }
    }
}
