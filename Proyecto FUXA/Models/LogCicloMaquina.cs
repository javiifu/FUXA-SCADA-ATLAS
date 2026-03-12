using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    public class LogCicloMaquina
    {
        public int Id { get; set; }

        [Required]
        public int IdMaquina { get; set; }

        [ForeignKey(nameof(IdMaquina))]
        public Machine? Maquina { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Range(0, int.MaxValue)]
        public int CiclosReales { get; set; }
    }
}
