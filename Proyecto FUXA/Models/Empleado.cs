using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("Empleados")]
    public class Empleado
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? CodigoEmpleado { get; set; }

        public bool EstaActivo { get; set; }

        [NotMapped]
        public string NombreCompleto => string.IsNullOrWhiteSpace(Apellidos) ? Nombre : $"{Nombre} {Apellidos}";

        public virtual ICollection<MaquinaOrden> MaquinasOrdenes { get; set; } = new List<MaquinaOrden>();
        public virtual ICollection<ImputacionOperario> ImputacionesOperario { get; set; } = new List<ImputacionOperario>();
        public virtual ICollection<ImputacionMaquina> ImputacionesMaquina { get; set; } = new List<ImputacionMaquina>();
        public virtual ICollection<JornadaOperario> JornadasOperario { get; set; } = new List<JornadaOperario>();
    }
}