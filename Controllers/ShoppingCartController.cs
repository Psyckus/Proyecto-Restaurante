using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _cache;
        private readonly IReservaService _reservaService;
        public ShoppingCartController(ApplicationDbContext context, ShoppingCartService cartService, IDistributedCache cache, IReservaService reservaService)
        {
            _cache = cache;
            _context = context;
            _cartService = cartService;
            _reservaService = reservaService;
        }




        // Display payment success view
        public async Task<IActionResult> PaymentSuccess()
        {
            var sessionKey = "UserSessionKey"; // Custom session key
            var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

            if (sessionDataSerialized != null)
            {
                var sessionData = System.Text.Json.JsonSerializer.Deserialize<SessionData>(sessionDataSerialized);
                int usuarioId = sessionData.UsuarioId;

                var reservas = await _reservaService.GetReservasPorUsuarioAsync(usuarioId);

                ViewBag.NombreUsuario = !string.IsNullOrEmpty(sessionData.nombre) ? sessionData.nombre : "Invitado";
                ViewBag.TieneReservas = reservas != null && reservas.Any();
                ViewBag.Reservas = reservas;
            }
            else
            {
                ViewBag.NombreUsuario = "Invitado";
                ViewBag.TieneReservas = false;
                ViewBag.Reservas = null;
            }
            return View();
        }

        // Display payment cancel view
        public IActionResult PaymentCancel()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var sessionKey = "UserSessionKey"; // Custom session key
            var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

            if (sessionDataSerialized != null)
            {
                var sessionData = System.Text.Json.JsonSerializer.Deserialize<SessionData>(sessionDataSerialized);
                int usuarioId = sessionData.UsuarioId;

                var reservas = await _reservaService.GetReservasPorUsuarioAsync(usuarioId);

                ViewBag.NombreUsuario = !string.IsNullOrEmpty(sessionData.nombre) ? sessionData.nombre : "Invitado";
                ViewBag.TieneReservas = reservas != null && reservas.Any();
                ViewBag.Reservas = reservas;
            }
            else
            {
                ViewBag.NombreUsuario = "Invitado";
                ViewBag.TieneReservas = false;
                ViewBag.Reservas = null;
            }
            // Obtención de la sesión del restaurante
            var sessionKeyRes = "RestauranteIdKey";
            var sessionDataSerializedRes = await _cache.GetStringAsync(sessionKeyRes);
            int? restauranteId = null;

            if (sessionDataSerializedRes != null)
            {
                var sessionDataRes = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(sessionDataSerializedRes);
                restauranteId = sessionDataRes.ContainsKey("RestauranteId") ? sessionDataRes["RestauranteId"] : (int?)null;
            }

            ViewBag.RestauranteId = restauranteId;

            // Inicialización del carrito de compras
            var cart = new ShoppingCart
            {
                Items = _cartService.GetItems(),
                RestauranteId = restauranteId ?? 0 // Se asegura de no tener un valor nulo
            };

            // Consultar platillos disponibles para el restaurante
            ViewBag.AvailableDishes = restauranteId.HasValue
                ? _context.Platillos.Where(p => p.restaurante_id == restauranteId.Value).ToList()
                : new List<Platillos>();

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            var platillo = _context.Platillos.FirstOrDefault(p => p.platillo_id == id);
            if (platillo == null)
            {
                return NotFound();
            }
            // Guardar el restaurante_id en Redis
            var restauranteId = platillo.restaurante_id;
            var sessionKey = "RestauranteIdKey"; // Llave personalizada para Redis
            var sessionData = new { RestauranteId = restauranteId };

            // Serializamos el objeto para almacenarlo en Redis
            var sessionDataSerialized = System.Text.Json.JsonSerializer.Serialize(sessionData);
            await _cache.SetStringAsync(sessionKey, sessionDataSerialized);

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