using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class DetalleCompras
    {
        [Key]
        public int detalle_id { get; set; }

        [Required]
        public int cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal precio_unitario { get; set; }

        // Llaves foráneas
        public int CompraId { get; set; }
        public int PlatilloId { get; set; }

        // Propiedades de navegación
        [ForeignKey("compra_id")]
        public Compra Compra { get; set; }

        [ForeignKey("platillo_id")]
        public Platillos Platillo { get; set; }
    }
}
