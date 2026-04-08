namespace Proyecto_FUXA.Models;

public class ImputacionMaterial
{
    public int Id { get; set; }
    public int IdOperacion { get; set; }
    public int IdMaterial { get; set; }
    public double Cantidad { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaRegistro { get; set; }
}
