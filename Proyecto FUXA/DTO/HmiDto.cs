using System.Text.Json.Serialization;
namespace Proyecto_FUXA.DTO
{
    public class HmiDto
    {
        [JsonPropertyName("views")]
        public List<ViewDto> Views { get; set; } = new();
    }
}
