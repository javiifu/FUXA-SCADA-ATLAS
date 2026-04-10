using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models;

[Table("Imputaciones")]
public class Imputacion
{
    [Key]
    private int Id { get; set; }

}
