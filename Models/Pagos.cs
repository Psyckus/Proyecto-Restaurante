using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class Pagos
    {
        [Key]
        public int pago_id { get; set; }

        [MaxLength(100)]
        public string metodo_pago { get; set; } = "PayPal"; // Por defecto, será PayPal

        [MaxLength(50)]
        public string estado_pago { get; set; } = "Pendiente";

        public DateTime fecha_pago { get; set; } = DateTime.Now;

        // Llave foránea
        public int compra_id { get; set; }

        // Relación con el modelo de Compra
        [ForeignKey("compra_id")]
        public Compra Compra { get; set; }
    }
}
