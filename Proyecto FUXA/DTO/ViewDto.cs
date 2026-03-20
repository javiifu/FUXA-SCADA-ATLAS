using System.Text.Json.Serialization;

namespace Proyecto_FUXA.DTO
{
    public class ViewDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("items")]
        public Dictionary<string, FuxaItemDto> Items { get; set; } = new();

        [JsonPropertyName("svgcontent")]
        public string SvgContent { get; set; } = "";
    }
}
