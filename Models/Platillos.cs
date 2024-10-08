using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class Platillos
    {
        [Key]
        public int platillo_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string nombre { get; set; }

        public string descripcion { get; set; }

        [MaxLength(255)]
        public string imagen_url { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal precio { get; set; }

        // Llave foránea
        public int RestauranteId { get; set; }

        // Propiedad de navegación
        [ForeignKey("restaurante_id")]
        public Restaurante Restaurante { get; set; }

        // Propiedades de navegación adicionales
        public ICollection<DetalleCompras> DetalleCompras { get; set; }
    }
}
