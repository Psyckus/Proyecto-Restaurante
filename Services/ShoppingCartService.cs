using MVW_Proyecto_Mesas_Comida.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVW_Proyecto_Mesas_Comida.Services
{
	public class ShoppingCartService
	{
		private readonly List<ShoppingCartItem> _items;

		public ShoppingCartService()
		{
			_items = new List<ShoppingCartItem>();
		}

		// Agregar un elemento al carrito
		public void AddItem(Platillos platillo, int quantity)
		{
			var existingItem = _items.FirstOrDefault(i => i.PlatilloId == platillo.platillo_id);
			if (existingItem != null)
			{
				existingItem.Quantity += quantity;
			}
			else
			{
				_items.Add(new ShoppingCartItem
				{
					PlatilloId = platillo.platillo_id,
					Platillo = platillo,
					Quantity = quantity,
					Price = platillo.precio
				});
			}
		}

		// Actualizar la cantidad de un elemento
		public void UpdateItemQuantity(int platilloId, int quantity)
		{
			var existingItem = _items.FirstOrDefault(i => i.PlatilloId == platilloId);
			if (existingItem != null)
			{
				existingItem.Quantity = quantity;
			}
		}

		// Eliminar un elemento del carrito
		public void RemoveItem(int platilloId)
		{
			var item = _items.FirstOrDefault(i => i.PlatilloId == platilloId);
			if (item != null)
			{
				_items.Remove(item);
			}
		}

		// Limpiar el carrito
		public void ClearCart()
		{
			_items.Clear();
		}

		// Obtener los elementos del carrito
		public List<ShoppingCartItem> GetItems()
		{
			return _items;
		}

		// Obtener el costo total del carrito
		public decimal GetTotalCost()
		{
			return _items.Sum(i => i.SubTotal);
		}
	}
}
