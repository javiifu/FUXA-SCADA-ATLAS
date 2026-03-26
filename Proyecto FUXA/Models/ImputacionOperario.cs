using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    public class ImputacionOperario
    {
        public int Id { get; set; }
        public int IdEmpleado {  get; set; }
        public int IdMaquina { get; set; }
        public int? IdOrden {  get; set; }
        public int IdOperacion { get; set; }
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public DateTime? FechaFin {  get; set; }


        [ForeignKey("IdEmpleado")]
        public virtual Empleado Empleado { get; set; } = null!;

        [ForeignKey("IdMaquina")]
        public virtual Maquina Maquina { get; set; } = null!;

        [ForeignKey("IdOrden")]
        public virtual Orden? Orden { get; set; }

        [ForeignKey("IdOperacion")]
        public virtual Operacion Operacion { get; set; } = null!;
    }
}
