using MVW_Proyecto_Mesas_Comida.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
    [Column("compra_id")]
    public int CompraId { get; set; }

    [Column("platillo_id")]
    public int PlatilloId { get; set; }

    // Propiedades de navegación
    [ForeignKey("CompraId")]
    public Compra Compra { get; set; }

    [ForeignKey("PlatilloId")]
    public Platillos Platillo { get; set; }
}
