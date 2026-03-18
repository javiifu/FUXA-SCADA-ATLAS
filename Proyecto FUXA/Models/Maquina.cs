using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models;

[Table("Maquina")]
public class Maquina
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public int NumeroOrden { get; set; }

    [Required, MaxLength(100)]
    public string NombreSeccion { get; set; } = string.Empty;

    public int CiclosObjetivo { get; set; }

    public int EstadoActualId { get; set; }

    public bool EstaActivo { get; set; }

    [MaxLength(100)]
    public string? IdentificadorObjetoFuxa { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    [ForeignKey(nameof(EstadoActualId))]
    public MaquinaEstatus? EstadoActual { get; set; }

    public ICollection<MaquinaProduccion> Producciones { get; set; } = new List<MaquinaProduccion>();
}
