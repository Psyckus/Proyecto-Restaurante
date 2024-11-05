using System.Collections.Generic;
using System.Linq;

namespace MVW_Proyecto_Mesas_Comida.Models
{
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        // Add item to cart or update quantity if item exists
        public void AddItem(Platillos platillo, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.PlatilloId == platillo.platillo_id);
            if (item == null)
            {
                Items.Add(new ShoppingCartItem
                {
                    PlatilloId = platillo.platillo_id,
                    Platillo = platillo,
                    Quantity = quantity,
                    Price = platillo.precio
                });
            }
            else
            {
                item.Quantity += quantity;
            }
        }

        // Remove item from cart
        public void RemoveItem(int platilloId)
        {
            var item = Items.FirstOrDefault(i => i.PlatilloId == platilloId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        // Update item quantity
        public void UpdateItemQuantity(int platilloId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.PlatilloId == platilloId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }

        // Calculate total cost of cart
        public decimal GetTotalCost()
        {
            return Items.Sum(i => i.SubTotal);
        }

        // Clear all items from cart
        public void ClearCart()
        {
            Items.Clear();
        }
    }
}
