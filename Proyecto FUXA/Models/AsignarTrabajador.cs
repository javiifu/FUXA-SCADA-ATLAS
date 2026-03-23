namespace Proyecto_FUXA.Models
{
    public class AsignarTrabajador
    {
        public string MaquinaId { get; set; } = string.Empty;
        public string NombreMaquina { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public int? EmpleadoId { get; set; }
        public string Cargo { get; set; } = "Operario";
    }
}
