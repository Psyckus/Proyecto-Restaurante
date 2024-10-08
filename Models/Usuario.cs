using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class Usuario
    {

        [Key]
        public int usuario_id { get; set; }

        [Required]
        [MaxLength(100)]
        public string nombre { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string correo { get; set; }

        [Required]
        [MaxLength(255)]
        public string contrasena { get; set; }

        [Required]
        [MaxLength(255)]
        public string token_autenticacion { get; set; }

        public DateTime creado_en { get; set; } = DateTime.Now;

        public DateTime actualizado_en { get; set; } = DateTime.Now;

        // Llave foránea
        public int rol_id { get; set; }

        // Propiedad de navegación
        [ForeignKey("rol_id")]
        public Rol Rol { get; set; }

        // Propiedades de navegación adicionales
        public ICollection<Reservas> Reservas { get; set; }
        public ICollection<Compra> Compras { get; set; }
    }
}
