using System.ComponentModel.DataAnnotations;

namespace Proyecto_FUXA.Models;

public class TipoOperacion
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Preferencia { get; set; }
}
