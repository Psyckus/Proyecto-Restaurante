using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;
using System.Threading.Tasks;
using System.Linq;

namespace MVW_Proyecto_Mesas_Comida.Controllers
{
	public class RestauranteController : Controller
	{
		private readonly IRestauranteService _restauranteService;
		private readonly IDistributedCache _cache;
		private readonly ApplicationDbContext _context;
		private readonly IReservaService _reservaService;
		private readonly IPlatilloService _platilloService;

		public RestauranteController(IRestauranteService restauranteService, IDistributedCache cache, ApplicationDbContext context, IReservaService reservaService, IPlatilloService platilloService)
		{
			_restauranteService = restauranteService;
			_cache = cache;
			_context = context;
			_reservaService = reservaService;
			_platilloService = platilloService;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Catalogo()
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

		[HttpGet]
		public async Task<IActionResult> GetPlatillosPorRestaurante(int restauranteId)
		{
			try
			{
				var platillos = await _platilloService.GetPlatillosPorRestauranteAsync(restauranteId);

				if (platillos == null || !platillos.Any())
				{
					return Json(new { success = false, message = "No dishes found for the specified restaurant." });
				}

				return Json(platillos);
			}
			catch (Exception ex)
			{
				// Log the exception for debugging
				Console.WriteLine($"Error fetching dishes: {ex.Message}");
				return Json(new { success = false, message = "An error occurred while fetching dishes." });
			}
		}

		public async Task<IActionResult> Carrito()
		{
			var sessionKey = "UserSessionKey";
			var nombreUsuario = await _cache.GetStringAsync(sessionKey);

			ViewBag.NombreUsuario = !string.IsNullOrEmpty(nombreUsuario) ? nombreUsuario : "Invitado";
			return View();
		}

		public async Task<IActionResult> Mesas(int id)
		{
			var sessionKey = "UserSessionKey";
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

			var mesasDisponibles = _context.Mesas
										   .Where(m => m.restaurante_id == id && m.disponible)
										   .ToList();

			return View(mesasDisponibles);
		}

		[HttpPost]
		public async Task<JsonResult> AgendarReserva(Reservas model)
		{
			var sessionKey = "UserSessionKey";
			var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

			if (sessionDataSerialized == null)
			{
				return Json(new { success = false, message = "Session data not found. Please log in." });
			}

			var sessionData = System.Text.Json.JsonSerializer.Deserialize<SessionData>(sessionDataSerialized);
			if (sessionData == null || sessionData.UsuarioId == 0)
			{
				return Json(new { success = false, message = "Invalid session data." });
			}

			if (!int.TryParse(Request.Form["mesaId"].ToString(), out int mesaId))
			{
				return Json(new { success = false, message = "Invalid mesa ID." });
			}

			var reserva = new Reservas
			{
				fecha_reserva = model.fecha_reserva,
				mesa_id = mesaId,
				usuario_id = sessionData.UsuarioId
			};

			_context.Reservas.Add(reserva);
			await _context.SaveChangesAsync();

			var mesa = await _context.Mesas.FindAsync(mesaId);
			if (mesa != null)
			{
				mesa.disponible = false;
				_context.Mesas.Update(mesa);
				await _context.SaveChangesAsync();
			}

			return Json(new { success = true, message = "Reservation booked successfully.", redirectUrl = Url.Action("Catalogo", "Restaurante") });
		}

		public IActionResult Detalles(int id)
		{
			var restaurante = _restauranteService.ObtenerPorId(id);
			if (restaurante == null)
			{
				return NotFound();
			}
			return View(restaurante);
		}

		[HttpPost]
		public async Task<JsonResult> EliminarReserva(int reservaId)
		{
			var reserva = await _context.Reservas.FindAsync(reservaId);
			if (reserva != null)
			{
				_context.Reservas.Remove(reserva);
				var mesa = await _context.Mesas.FindAsync(reserva.mesa_id);
				if (mesa != null)
				{
					mesa.disponible = true;
					_context.Mesas.Update(mesa);
				}
				await _context.SaveChangesAsync();

				return Json(new { success = true, message = "Reservation deleted successfully." });
			}

			return Json(new { success = false, message = "Reservation not found." });
		}
	}
}
