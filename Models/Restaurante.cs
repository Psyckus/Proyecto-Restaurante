using System.ComponentModel.DataAnnotations;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class Restaurante
    {
        [Key]
        public int restaurante_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string nombre { get; set; }

        [MaxLength(255)]
        public string direccion { get; set; }

        public string descripcion { get; set; }

        public DateTime creado_en { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public ICollection<Platillos> Platillos { get; set; }
        public ICollection<Mesas> Mesas { get; set; }
    }
}
