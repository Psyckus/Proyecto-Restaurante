using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlatilloId { get; set; }  // Foreign key to Platillos

        [ForeignKey("PlatilloId")]
        public Platillos Platillo { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [NotMapped]
        public decimal SubTotal => Price * Quantity;  // Calculate subtotal
    }
}
