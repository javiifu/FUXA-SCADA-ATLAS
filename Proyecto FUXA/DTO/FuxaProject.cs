using System.Text.Json.Serialization;
namespace Proyecto_FUXA.DTO
{
    public class FuxaProject
    {
        [JsonPropertyName("devices")]
        public Dictionary<string, FuxaDevice> Devices { get; set; } = new();

        [JsonPropertyName("hmi")]
        public HmiDto Hmi { get; set; } = new();
    }
}
