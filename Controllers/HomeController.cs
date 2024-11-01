using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;
using System.Diagnostics;

namespace MVW_Proyecto_Mesas_Comida.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IDistributedCache _cache;
		private readonly IRestauranteService _restauranteService;
		private readonly IReservaService _reservaService;

		public HomeController(ILogger<HomeController> logger, IDistributedCache cache, IRestauranteService restauranteService, IReservaService reservaService)
        {
            _logger = logger;
			_cache = cache;
			_restauranteService = restauranteService;
			_reservaService = reservaService;
		}

		public async Task<IActionResult> Index()
		{
			// Definir la clave de la cache
			var sessionKey = "UserSessionKey"; // Personaliza esta clave si es necesario

			// Recupera el objeto de sesión serializado desde Redis
			var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

			// Deserializa el objeto de sesión si existe
			if (sessionDataSerialized != null)
			{
				var sessionData = System.Text.Json.JsonSerializer.Deserialize<SessionData>(sessionDataSerialized);

				// Obtener usuarioId desde la sesión
				int usuarioId = sessionData.UsuarioId;

				// Obtener reservas del usuario
				var reservas = await _reservaService.GetReservasPorUsuarioAsync(usuarioId);

				// Guardar en ViewBag si el usuario tiene reservas
				ViewBag.TieneReservas = reservas != null && reservas.Any();

				// Establecer el nombre del usuario para mostrar en la navegación
				ViewBag.NombreUsuario = !string.IsNullOrEmpty(sessionData.nombre) ? sessionData.nombre : "Invitado";

				// Verificar si el token es válido
				ViewBag.TokenValido = sessionData?.Token != null;

				// Si el usuario es administrador (rol_id == 1), redirigir al Dashboard
				if (sessionData?.rol_id == 1)
				{
					return RedirectToAction("Index", "Dashboard");
				}
			}
			else
			{
				// Si no hay sesión, configurar valores por defecto en ViewBag
				ViewBag.TokenValido = false;
				ViewBag.NombreUsuario = "Invitado";
				ViewBag.TieneReservas = false;
			}

			// Obtener la lista de restaurantes
			var restaurantes = _restauranteService.ObtenerTodos();
			var viewModel = restaurantes.Select(r => new Restaurante
			{
				restaurante_id = r.restaurante_id,
				nombre = r.nombre,
				descripcion = r.descripcion,
			});

			return View(viewModel);
		}


		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
