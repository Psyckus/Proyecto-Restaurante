using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class Compra
    {
        [Key]
        public int compra_id { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal total { get; set; }

        public DateTime fecha_compra { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string estado_compra { get; set; } = "Pendiente";

        // Llave foránea
        public int UsuarioId { get; set; }

        // Propiedad de navegación
        [ForeignKey("usuario_id")]
        public Usuario Usuario { get; set; }

        // Propiedades de navegación adicionales
        public ICollection<DetalleCompras> DetalleCompras { get; set; }
        public ICollection<Pagos> Pagos { get; set; }
    }
}
