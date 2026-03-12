using System.ComponentModel.DataAnnotations;

namespace Proyecto_FUXA.Models
{
    public class Machine
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Descripcion { get; set; }

        public int CiclosHoraObjetivo { get; set; }

        public EstadoMaquina Estado { get; set; } = EstadoMaquina.Activo;

        public bool EstaActivo { get; set; } = true;

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime ModificadoEn { get; set; } = DateTime.UtcNow;

        public List<LogCicloMaquina> CycleLogs { get; set; } = new();
    }
}
