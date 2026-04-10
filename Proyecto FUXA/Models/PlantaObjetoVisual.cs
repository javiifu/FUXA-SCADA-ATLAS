using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models;

[Table("PlantaObjetoVisual")]
public class PlantaObjetoVisual
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; } = "Rectangulo";

    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Width { get; set; } = 120;
    public int Height { get; set; } = 80;

    [Required]
    [MaxLength(20)]
    public string ColorHex { get; set; } = "#1f77b4";

    public int? MaquinaId { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }

    [ForeignKey(nameof(MaquinaId))]
    public Maquina? Maquina { get; set; }
}
