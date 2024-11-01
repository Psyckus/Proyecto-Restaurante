using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;

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
			// Definir la clave de la cache
			var sessionKey = "UserSessionKey";  // Personaliza esta clave si es necesario

			// Recuperar el objeto de sesión serializado desde Redis
			var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

			// Deserializar el objeto de sesión si existe
			if (sessionDataSerialized != null)
			{
				var sessionData = System.Text.Json.JsonSerializer.Deserialize<SessionData>(sessionDataSerialized);

				// Obtener el ID del usuario desde la sesión
				int usuarioId = sessionData.UsuarioId;

				// Obtener las reservas del usuario si está logueado
				var reservas = await _reservaService.GetReservasPorUsuarioAsync(usuarioId);

				// Guardar el nombre del usuario en ViewBag
				ViewBag.NombreUsuario = !string.IsNullOrEmpty(sessionData.nombre) ? sessionData.nombre : "Invitado";

				// Guardar las reservas en ViewBag para mostrarlas en la vista si es necesario
				ViewBag.TieneReservas = reservas != null && reservas.Any();
				ViewBag.Reservas = reservas;
			}
			else
			{
				// Si no hay sesión, configurar valores por defecto en ViewBag
				ViewBag.NombreUsuario = "Invitado";
				ViewBag.TieneReservas = false;
				ViewBag.Reservas = null;
			}

			return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetPlatillosPorRestaurante(int restauranteId)
		{


		
			var platillos = await _platilloService.GetPlatillosPorRestauranteAsync(restauranteId);
			return Json(platillos); // Retornar los platillos como JSON
		}


		public async Task<IActionResult> Carrito()
		{
			// Definir la clave de la cache
			var sessionKey = "UserSessionKey";  // Puedes personalizar esta clave

			// Recuperar el nombre de usuario desde Redis
			var nombreUsuario = await _cache.GetStringAsync(sessionKey);

			if (!string.IsNullOrEmpty(nombreUsuario))
			{
				ViewBag.NombreUsuario = nombreUsuario;
			}
			else
			{
				ViewBag.NombreUsuario = "Invitado";
			}

			return View();

		}
		public async Task<IActionResult> Mesas(int id)
		{
			// Definir la clave de la cache
			var sessionKey = "UserSessionKey";  // Personaliza esta clave si es necesario

			// Recuperar el objeto de sesión serializado desde Redis
			var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

			// Deserializar el objeto de sesión si existe
			if (sessionDataSerialized != null)
			{
				var sessionData = System.Text.Json.JsonSerializer.Deserialize<SessionData>(sessionDataSerialized);

				// Obtener el ID del usuario desde la sesión
				int usuarioId = sessionData.UsuarioId;

				// Obtener las reservas del usuario si está logueado
				var reservas = await _reservaService.GetReservasPorUsuarioAsync(usuarioId);

				// Guardar el nombre del usuario en ViewBag
				ViewBag.NombreUsuario = !string.IsNullOrEmpty(sessionData.nombre) ? sessionData.nombre : "Invitado";

				// Guardar las reservas en ViewBag para mostrarlas en la vista si es necesario
				ViewBag.TieneReservas = reservas != null && reservas.Any();
				ViewBag.Reservas = reservas;
			}
			else
			{
				// Si no hay sesión, configurar valores por defecto en ViewBag
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
			//if (!ModelState.IsValid)
			//{
			//	return View("Error"); // Puedes redirigir a una vista de error o a la misma vista con errores.
			//}
			var mesaIdString = Request.Form["mesaId"].ToString(); // Asegúrate de convertirlo a string
			int mesaId;
			// Obtener el usuario_id desde la sesión
			var sessionKey = "UserSessionKey";
			var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);
			if (sessionDataSerialized == null)
			{
				return Json(new { success = false, message = "Error." });
			}

			var sessionData = System.Text.Json.JsonSerializer.Deserialize<SessionData>(sessionDataSerialized);
			if (sessionData == null || sessionData.UsuarioId == 0)
			{
				return Json(new { success = false, message = "Error." });
			}
			if (int.TryParse(mesaIdString, out mesaId))
			{
				// Crear el objeto de reserva solo si la conversión fue exitosa
				var reserva = new Reservas
				{
					fecha_reserva = model.fecha_reserva, // Usa el nombre correcto para el modelo
					mesa_id = mesaId,
					usuario_id = sessionData.UsuarioId
				};

				// Guardar la reserva en la base de datos
				_context.Reservas.Add(reserva);
				await _context.SaveChangesAsync();
			}
	


			// Actualizar el estado de la mesa a no disponible
			var mesa = await _context.Mesas.FindAsync(mesaId);
			if (mesa != null)
			{
				mesa.disponible = false; // Marcamos la mesa como no disponible
				_context.Mesas.Update(mesa);
				await _context.SaveChangesAsync();
			}

			return Json(new { success = true, message = "Se agendo su reserva correctamente.", redirectUrl = Url.Action("Catalogo", "Restaurante") });
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

				return Json(new { success = true, message = "Reserva eliminada correctamente." });
			}

			return Json(new { success = false, message = "La reserva no existe." });
		}




	}
}
