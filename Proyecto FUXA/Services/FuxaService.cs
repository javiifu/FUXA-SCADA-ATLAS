using Proyecto_FUXA.DTO;
using Proyecto_FUXA.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace Proyecto_FUXA.Services
{
    public class FuxaService
    {
        private readonly HttpClient _http;

        public FuxaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Maquina>> GetMaquinasDesdeFuxa()
        {
            //var project = await _http.GetFromJsonAsync<FuxaProject>("/api/project", new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true
            //});

            //Console.WriteLine(project == null ? "PROJECT NULL" : "PROJECT OK");
            //Console.WriteLine(project?.Devices?.Count ?? 0);

            var project = await _http.GetFromJsonAsync<FuxaProject>("/api/project",new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            Console.WriteLine(project == null ? "PROJECT NULL" : "PROJECT OK");

            Console.WriteLine(project?.Devices?.Count ?? 0);

            //var items = project.Hmi.Views.SelectMany(v => v.Items.Values).ToList();


            if (project?.Hmi?.Views == null || !project.Hmi.Views.Any())
                return new List<Maquina>();

            //.Where(i => i.Type.StartsWith("svg-ext-proceng"))
            var maquinas = new List<Maquina>();
            int numeroOrden = 1;

            foreach (var view in project.Hmi.Views)
            {
                var itemsValidos = ObtenerItemsValidos(view);

                foreach (var item in itemsValidos.Values)
                {
                    
                    maquinas.Add(new Maquina
                    {
                        Nombre = ObtenerNombreVisible(item),
                        NumeroOrden = numeroOrden++,
                        NombreSeccion = item.Type,
                        CiclosObjetivo = 100,
                        EstadoActualId = 1,
                        EstaActivo = !item.Hide,
                        FechaCreacion = DateTime.Now,
                        FechaActualizacion = DateTime.Now,
                        FuxaDeviceId = item.Id
                    });
                }
            }

            return maquinas;
        }
        //Obtiene los items válidos de todas las vistas que hay en fuxa
        //También ignora los que se quedan "huerfanos" en el apartado items.
        public async Task<List<FuxaItemDto>> GetItemsValidosDesdeFuxa()
        {
            var project = await _http.GetFromJsonAsync<FuxaProject>(
                "/api/project",
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (project?.Hmi?.Views == null || !project.Hmi.Views.Any())
                return new List<FuxaItemDto>();

            var resultado = new List<FuxaItemDto>();

            foreach (var view in project.Hmi.Views)
            {
                var itemsValidos = ObtenerItemsValidos(view);
                resultado.AddRange(itemsValidos.Values);
            }

            return resultado;
        }

        //Filtra y devuelve SOLO las tuberías que hay en el svgContent. 
        public async Task<List<FuxaItemDto>> GetTuberiasDesdeFuxa()
        {
            var items = await GetItemsValidosDesdeFuxa();

            return items
                .Where(i => i.Type.Equals("svg-ext-pipe", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        //Devuelve una serie de items que existen tanto en el diccionario Items como en el SVGContent.
        private Dictionary<string, FuxaItemDto> ObtenerItemsValidos(ViewDto view)
        {
            if (view.Items == null || view.Items.Count == 0 || string.IsNullOrWhiteSpace(view.SvgContent))
                return new Dictionary<string, FuxaItemDto>();

            var idsEnSvg = ExtraerIdsDelSvg(view.SvgContent);

            var itemsValidos = view.Items
                .Where(i => idsEnSvg.Contains(i.Key))
                .ToDictionary(i => i.Key, i => i.Value);

            var itemsHuerfanos = view.Items
                .Where(i => !idsEnSvg.Contains(i.Key))
                .ToList();

            foreach (var item in itemsHuerfanos)
            {
                Console.WriteLine($"[FUXA] Item huérfano ignorado: {item.Key} ({item.Value.Type})");
            }

            return itemsValidos;
        }

        //Extrae todos los identificadores id=".." dentro del contenido SVG
        private HashSet<string> ExtraerIdsDelSvg(string svgContent)
        {
            var ids = new HashSet<string>();

            if (string.IsNullOrWhiteSpace(svgContent))
                return ids;

            var matches = Regex.Matches(svgContent, "id=\"([^\"]+)\"");

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                    ids.Add(match.Groups[1].Value);
            }

            return ids;
        }

        /*private bool EsMaquinaIndustrial(FuxaItemDto item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Type))
                return false;

            return item.Type.StartsWith("svg-ext-proceng-", StringComparison.OrdinalIgnoreCase)
                   && !item.Type.Equals("svg-ext-pipe", StringComparison.OrdinalIgnoreCase);
        }*/
        /*
        private string ObtenerNombreMaquina(FuxaItemDto item)
        {
            if (!string.IsNullOrWhiteSpace(item.Name))
                return item.Name;

            if (!string.IsNullOrWhiteSpace(item.Label))
                return item.Label;

            return item.Type;
        }*/

        //Devuelve un nombre visible para el item, usando su Name si está informado. 
        private string ObtenerNombreVisible(FuxaItemDto item)
        {
            if (!string.IsNullOrWhiteSpace(item.Name))
                return item.Name;

            // Si no tiene nombre, generamos uno a partir del tipo
            return item.Name;
        }

        /*
        private string ObtenerTipoVisible(FuxaItemDto item)
        {
            if (string.IsNullOrWhiteSpace(item.Type))
                return "Desconocido";

            var prefijo = "svg-ext-proceng-";

            var tipo = item.Type.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase)
                ? item.Type.Substring(prefijo.Length)
                : item.Type;

            return tipo switch
            {
                "pumphidra" => "Bomba hidráulica",
                "compair" => "Compresor",
                "pumpgear" => "Bomba de engranajes",
                _ => tipo
            };
        }*/
    }
}
