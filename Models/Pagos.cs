using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class Pagos
    {
        [Key]
        public int pago_id { get; set; }

        [MaxLength(100)]
        public string metodo_pago { get; set; }

        [MaxLength(50)]
        public string estado_pago { get; set; }

        public DateTime fecha_pago { get; set; } = DateTime.Now;

        // Llave foránea
        public int compra_id { get; set; }

        // Propiedad de navegación
        [ForeignKey("compra_id")]
        public Compra Compra { get; set; }
    }
}
