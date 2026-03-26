namespace Proyecto_FUXA.DTO
{
    public class EstadoImputacionMaquinaDto
    {
        public int MaquinaId { get; set; }
        public string NombreMaquina { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public string EstadoMaquinaFisica { get; set;  } = "Desconocido";
        public int? MaquinaOrdenId { get; set; }
        public string? OrdenCodigo { get; set; }
        public int? OperacionId { get; set; }
        public string? OperacionNombre { get; set; }
        public int? ImputacionMaquinaId { get; set; }
        public DateTime? FechaInicioMaquina { get; set; }
        public DateTime? FechaFinMaquina { get; set; }
        public string EstadoImputacion { get; set; } = "Sin imputación";
        public int? CantidadProducida { get; set; }
        public int? CantidadBuena { get; set; }
        public int? CantidadScrap { get; set; }
        public int? CantidadRetrabajo { get; set; }
        public bool PuedeIniciarImputacion { get; set; }
        public string? MotivoBloqueoInicio { get; set; }
        public List<OperarioActivoDto> OperariosActivos { get; set; } = new();
    }

    public class OperarioActivoDto
    {
        public int ImputacionOperarioId { get; set; }
        public int EmpleadoId { get; set; }
        public string EmpleadoNombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activo => !FechaFin.HasValue;
    }

    public class ImputacionMaquinaListadoDto
    {
        public int ImputacionMaquinaId { get; set; }
        public string Maquina { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public string Orden { get; set; } = string.Empty;
        public string Operacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? CantidadProducida { get; set; }
        public int? CantidadBuena { get; set; }
        public int? CantidadScrap { get; set; }
        public int? CantidadRetrabajo { get; set; }
        public string? MotivoCierre { get; set; }
        public int OperariosAsociados { get; set; }
    }

    public class ImputacionOperarioDto
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public string EmpleadoNombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Observaciones { get; set; }
    }

    public class IniciarImputacionMaquinaRequest
    {
        public int MaquinaId { get; set; }
        public int OperacionId { get; set; }
        public int? EmpleadoResponsableId { get; set; }
        public string TipoImputacion { get; set; } = "Manual";
        public string? Observaciones { get; set; }
    }

    public class FinalizarImputacionMaquinaRequest
    {
        public int ImputacionMaquinaId { get; set; }
        public int CantidadBuena { get; set; }
        public int CantidadScrap { get; set; }
        public int CantidadRetrabajo { get; set; }
        public string MotivoCierre { get; set; } = string.Empty;
        public string? ObservacionesCierre { get; set; }
    }

    public class CambiarEstadoImputacionMaquinaRequest
    {
        public int ImputacionMaquinaId { get; set; }
        public string NuevoEstado { get; set; } = string.Empty;
    }

    public class IniciarImputacionOperarioRequest
    {
        public int ImputacionMaquinaId { get; set; }
        public int EmpleadoId { get; set; }
        public string? Observaciones { get; set; }
    }

    public class FinalizarImputacionOperarioRequest
    {
        public int ImputacionOperarioId { get; set; }
        public string? Observaciones { get; set; }
    }

    public class ImputacionDetalleDto
    {
        public ImputacionMaquinaListadoDto Cabecera { get; set; } = new();
        public List<ImputacionOperarioDto> Operarios { get; set; } = new();
    }
}