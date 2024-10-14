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

		public HomeController(ILogger<HomeController> logger, IDistributedCache cache, IRestauranteService restauranteService)
        {
            _logger = logger;
			_cache = cache;
			_restauranteService = restauranteService;
		}

        public async Task<IActionResult> IndexAsync()
        {
		
			// Definir la clave de la cache
			var sessionKey = "UserSessionKey";// Puedes personalizar esta clave

	
			 // Recupera el objeto de sesión serializado desde Redis
            var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);
			// Deserializa el objeto de sesión
		

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

				// Verifica si el token es válido
				if (sessionData?.Token != null)
                {
                    ViewBag.TokenValido = true;
                }
                else
                {
                    ViewBag.TokenValido = false;
                }
            }
            else
            {
                ViewBag.TokenValido = false;
                ViewBag.NombreUsuario = "Invitado";
            }

		

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
