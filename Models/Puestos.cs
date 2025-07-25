using System.ComponentModel.DataAnnotations;

namespace promerica_backend.Models
{
    public class Puestos
    {
        public int Id { get; set; }

        [Required]
        public int Codigo { get; set; }

        [Required]
        public string Puesto { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public int? CodigoJefe { get; set; }
    }
}
