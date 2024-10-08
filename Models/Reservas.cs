using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class Reservas
    {
        [Key]
        public int reserva_id { get; set; }

        [Required]
        public DateTime fecha_reserva { get; set; }

        // Llaves foráneas
        public int usuario_id { get; set; }
        public int mesa_id { get; set; }

        // Propiedades de navegación
        [ForeignKey("usuario_id")]
        public Usuario Usuario { get; set; }

        [ForeignKey("mesa_id")]
        public Mesas Mesa { get; set; }
    }
}
