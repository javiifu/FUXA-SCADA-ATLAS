using System.Text.Json.Serialization;

namespace Proyecto_FUXA.DTO
{
    public class FuxaDevice
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
    }
}

