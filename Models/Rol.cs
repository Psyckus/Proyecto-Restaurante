using System.ComponentModel.DataAnnotations;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class Rol
    {

        [Key]
        public int rol_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string nombre { get; set; }

        // Propiedad de navegación
        public ICollection<Usuario> Usuarios { get; set; }
    }
}
