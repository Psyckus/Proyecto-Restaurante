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

		public RestauranteController(IRestauranteService restauranteService, IDistributedCache cache, ApplicationDbContext context)
		{
			_restauranteService = restauranteService;
			_cache = cache;
			_context = context;
		}

		public IActionResult Index()
		{
			

			return View();
	
		}
		public async Task<IActionResult> Catalogo()
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
			var sessionKey = "UserSessionKey";  // Puedes personalizar esta clave

			// Recuperar el nombre de usuario desde Redis
			var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);
			if (sessionDataSerialized != null)
			{
				var sessionData = System.Text.Json.JsonSerializer.Deserialize<SessionData>(sessionDataSerialized);
				if (!string.IsNullOrEmpty(sessionData.nombre))
				{
					ViewBag.NombreUsuario = sessionData.nombre;
				}
				else
				{
					ViewBag.NombreUsuario = "Invitado";
				}
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

			return Json(new { success = true, message = "Se agendo su reserva correctamente." });
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

		// Métodos para Crear, Editar, Eliminar...
	}
}
