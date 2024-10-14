using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class Mesas
    {
        [Key]
        public int mesa_id { get; set; }

        [Required]
        public int capacidad { get; set; }

        public bool disponible { get; set; } = true;

        public string nombre { get; set; }

        // Llave foránea
        public int restaurante_id { get; set; }

        // Propiedad de navegación
        [ForeignKey("restaurante_id")]
        public Restaurante Restaurante { get; set; }

        // Propiedades de navegación adicionales
        public ICollection<Reservas> Reservas { get; set; }
    }
}
