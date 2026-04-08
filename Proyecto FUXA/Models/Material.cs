using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models;

[Table("Materiales")]
public class Material
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string CodigoMaterial { get; set; }
    public string? Descripcion { get; set; }
    public double Stock { get; set; }
    public double StockMinimo { get; set; }
}
