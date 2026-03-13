using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models;

[Table("MaquinaProduccion")]
public class MaquinaProduccion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MaquinaId { get; set; }

    [ForeignKey(nameof(MaquinaId))]
    public Maquina? Maquina { get; set; }

    [Required]
    public int CiclosReales { get; set; }

    public DateTime? FechaRegistro { get; set; }
}
