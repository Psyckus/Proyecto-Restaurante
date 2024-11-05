

using Microsoft.AspNetCore.Mvc;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;
using System.Linq;
namespace MVW_Proyecto_Mesas_Comida.Controllers
{
	public class ShoppingCartController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ShoppingCartService _cartService;

		public ShoppingCartController(ApplicationDbContext context, ShoppingCartService cartService)
		{
			_context = context;
			_cartService = cartService;
		}

		public IActionResult Index()
		{
			var cart = new ShoppingCart
			{
				Items = _cartService.GetItems()
			};
			return View(cart);
		}

		[HttpPost]
		public IActionResult Add(int id)
		{
			var platillo = _context.Platillos.FirstOrDefault(p => p.platillo_id == id);
			if (platillo == null)
			{
				return NotFound();
			}

			_cartService.AddItem(platillo, 1);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult UpdateQuantity(int id, int quantity)
		{
			_cartService.UpdateItemQuantity(id, quantity);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Remove(int id)
		{
			_cartService.RemoveItem(id);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Clear()
		{
			_cartService.ClearCart();
			return RedirectToAction("Index");
		}
	}
}
