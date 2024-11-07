using Microsoft.AspNetCore.Mvc;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;
using System.Linq;
using System.Threading.Tasks;

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




        // Display payment success view
        public IActionResult PaymentSuccess()
        {
            return View();
        }

        // Display payment cancel view
        public IActionResult PaymentCancel()
        {
            return View();
        }

        public IActionResult Index(int? restauranteId)
        {
            var cart = new ShoppingCart
            {
                Items = _cartService.GetItems(),
                RestauranteId = restauranteId // Set the restaurant ID here
            };

            if (restauranteId.HasValue)
            {
                var availableDishes = _context.Platillos
                                              .Where(p => p.restaurante_id == restauranteId.Value)
                                              .ToList();

                ViewBag.AvailableDishes = availableDishes;
            }
            else
            {
                ViewBag.AvailableDishes = new List<Platillos>();
            }

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

        // Método para actualizar la base de datos con el contenido del carrito
        [HttpPost]
        public async Task<IActionResult> UpdateCart()
        {
            var items = _cartService.GetItems();

            if (!items.Any())
            {
                return RedirectToAction("Index"); // No hay artículos en el carrito
            }

            // Crear un nuevo registro de Compra
            var compra = new Compra
            {
                total = items.Sum(i => i.Price * i.Quantity),
                estado_compra = "Pendiente",
                usuario_id = GetCurrentUserId() // Usa `usuario_id` aquí
            };



            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            // Agregar los DetalleCompras relacionados con la Compra
            foreach (var item in items)
            {
                var detalleCompra = new DetalleCompras
                {
                    cantidad = item.Quantity,
                    precio_unitario = item.Price,
                    CompraId = compra.compra_id,
                    PlatilloId = item.PlatilloId
                };

                _context.DetalleCompras.Add(detalleCompra);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Redirigir a la vista del carrito
        }

        private int GetCurrentUserId()
        {
            // Implement logic to get the current user's ID
            return 1; // Placeholder value
        }
    
    }

}