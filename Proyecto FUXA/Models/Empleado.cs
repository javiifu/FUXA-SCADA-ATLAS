using System.ComponentModel.DataAnnotations;

namespace Proyecto_FUXA.Models
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string  Apellidos{ get; set; } = string.Empty;
        public string? CodigoEmpleado { get; set; }
        public bool EstaActivo { get; set; }
        public string Cargo { get; set; } = "Operario"; 

        public string BusquedaCombinada => $"{Nombre} - {CodigoEmpleado}";
    }
}
