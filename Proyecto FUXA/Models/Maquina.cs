using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FUXA.Models
{
    [Table("Maquina")]

    public class Maquina
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int NumeroOrden { get; set; }
        public string NombreSeccion { get; set; }
        public int CiclosObjetivo { get; set;  }
        public int EstadoActualId { get; set;  }
        public bool EstaActivo { get; set;  }
        public DateTime FechaCreacion { get; set;  }
        public DateTime FechaActualizacion { get; set;  }
    }
}
