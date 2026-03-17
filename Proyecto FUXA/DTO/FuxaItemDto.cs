using System.Text.Json.Serialization;

namespace Proyecto_FUXA.DTO
{
    public class FuxaItemDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("label")]
        public string Label { get; set; } = "";

        [JsonPropertyName("hide")]
        public bool Hide { get; set; }

        [JsonPropertyName("lock")]
        public bool Lock { get; set; }

        [JsonPropertyName("property")]
        public object? Property { get; set; }
    }
}
