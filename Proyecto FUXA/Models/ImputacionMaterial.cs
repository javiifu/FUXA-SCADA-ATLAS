using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models;

[Table("ImputacionMateriales")]
public class ImputacionMaterial
{
    public int Id { get; set; }
    public int IdOperacion { get; set; }
    public int IdMaterial { get; set; }
    public double Cantidad { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaRegistro { get; set; }

    [ForeignKey("IdOperacion")]
    public virtual OperacionesOrden OperacionesOrden{ get; set; }
    [ForeignKey("IdMaterial")]
    public virtual Material Material { get; set; }

}
