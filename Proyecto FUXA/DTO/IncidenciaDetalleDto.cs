namespace Proyecto_FUXA.DTO
{
    public class IncidenciaDetalleDto
    {
        public int Id { get; set; }
        public int MaquinaId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Prioridad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? UsuarioApertura { get; set; }
        public string? UsuarioAsignado { get; set; }
    }
}
