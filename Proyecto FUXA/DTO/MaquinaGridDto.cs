namespace Proyecto_FUXA.DTO
{
    public class MaquinaGridDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public int EstadoActualId { get; set; }
        public int CiclosObjetivo { get; set; }
        public int CiclosReales { get; set; }
        public int IncidenciasActivas { get; set; }
        public List<IncidenciaActivaDto> IncidenciasActivasDetalle { get; set; } = new();
        public DateTime? UltimoMantenimiento { get; set; }
        public DateTime? ProximoMantenimiento { get; set; }
    }

    public class IncidenciaActivaDto
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
