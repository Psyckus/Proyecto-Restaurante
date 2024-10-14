using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;
using Newtonsoft.Json;
using System.Text.Json;

namespace MVW_Proyecto_Mesas_Comida.Controllers
{
	public class UsuariosController : Controller
	{
		private readonly IUsuarioService _usuarioService;
		private readonly IAuthService _authService;
		private readonly IDistributedCache _cache;

		public UsuariosController(IUsuarioService usuarioService, IAuthService authService, IDistributedCache cache)
		{
			_usuarioService = usuarioService;
			_authService = authService;
			_cache = cache;
		}


		[HttpPost]
		public async Task<IActionResult> Register(string nombre, string correo, string contrasena, string confirmarContrasena)
		{
			var captchaResponse = Request.Form["g-recaptcha-response"];
			// Verificar contraseñas
			if (contrasena != confirmarContrasena)
			{

				return Json(new { success = false, message = "Las contraseñas no coinciden." });
			}

			// Verificar si el correo ya existe
			if (await _usuarioService.EmailExists(correo))
			{
				return Json(new { success = false, message = "Este correo ya está registrado." });
			}

			// Validar el token de reCAPTCHA
			var reCaptchaResult = await ValidarReCaptcha(captchaResponse);
			if (!reCaptchaResult)
			{
				return Json(new { success = false, message = "La verificación de reCAPTCHA falló. Inténtalo de nuevo." });
			}

			// Crear el usuario
			var resultado = await _usuarioService.CreateUser(new Usuario { nombre = nombre, correo = correo, contrasena = contrasena });

			if (resultado)
			{
				var usuario = await _usuarioService.GetUsuarioByEmail(correo);
				// Supongamos que el usuario se autentica correctamente
				var sessionKey = "UserSessionKey";
				// Generar un token único para la sesión del usuario
				var token = Guid.NewGuid().ToString();

				// Crear un objeto que almacene el usuario_id y el token
				var sessionData = new
				{
					nombre = usuario.nombre,
					UsuarioId = usuario.usuario_id,
					Token = token
				};

				// Guardar el nombre del usuario en Redis con un tiempo de expiración de 30 minutos
				var cacheOptions = new DistributedCacheEntryOptions()
					.SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Expira en 30 minutos de inactividad
																	// Serializa el objeto antes de guardarlo
				var sessionDataSerialized = System.Text.Json.JsonSerializer.Serialize(sessionData);


				await _cache.SetStringAsync(sessionKey, sessionDataSerialized, cacheOptions);

				return Json(new { success = true, message = "Usuario registrado exitosamente." });
			}

			// Si hubo un error al crear el usuario
			return Json(new { success = false, message = "Error al registrar el usuario." });
		}


		[HttpPost]
		public async Task<IActionResult> Login(Usuario model)
		{
			// Valida el token de reCAPTCHA
			var captchaResponse = Request.Form["g-recaptcha-response"];
			var reCaptchaResult = await ValidarReCaptcha(captchaResponse);
			if (!reCaptchaResult)
			{
				return Json(new { success = false, message = "La verificación de reCAPTCHA falló. Inténtalo de nuevo." });
			}

			// Intenta iniciar sesión
			var resultado = await _authService.Login(model);
			if (resultado)
			{
				// Obtener el usuario por correo
				var usuario = await _usuarioService.GetUsuarioByEmail(model.correo);

				if (usuario != null)
				{
					// Supongamos que el usuario se autentica correctamente
					var sessionKey = "UserSessionKey"; // Session Key única por usuario

					// Generar un token único para la sesión del usuario
					var token = Guid.NewGuid().ToString();

					// Crear un objeto que almacene el usuario_id y el token
					var sessionData = new
					{
						nombre = usuario.nombre,
						UsuarioId = usuario.usuario_id,
						Token = token
					};

					// Guardar el objeto serializado en Redis con un tiempo de expiración de 30 minutos
					var cacheOptions = new DistributedCacheEntryOptions()
						.SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Expira en 30 minutos de inactividad

					// Serializa el objeto antes de guardarlo
					var sessionDataSerialized = System.Text.Json.JsonSerializer.Serialize(sessionData);
					await _cache.SetStringAsync(sessionKey, sessionDataSerialized, cacheOptions);

					// Verifica el rol del usuario
					if (usuario.rol_id == 1) // 1 para Administrador
					{
						return Json(new
						{
							success = true,
							message = "Inicio de sesión exitoso Administrador.",
							redirectUrl = Url.Action("Index", "Dashboard"),
				
						});
					}
					else if (usuario.rol_id == 2) // 2 para Comprador
					{
						return Json(new
						{
							success = true,
							message = "Inicio de sesión exitoso Comprador.",
							redirectUrl = Url.Action("Index", "Home"),
					
						});
					}
				}
			}

			// Si la autenticación falla, devolver un mensaje de error.
			return Json(new { success = false, message = "Correo o contraseña incorrectos." });
		}



		private async Task<bool> ValidarReCaptcha(string g_recaptcha_response)
		{
			var secretKey = "6LeS8V4qAAAAAGr6jWkABhsLAB6kZcP5kZUE-ptx"; // Coloca tu secret key aquí
			var client = new HttpClient();
			var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={g_recaptcha_response}", null);
			var jsonResponse = await response.Content.ReadAsStringAsync();
			dynamic result = JsonConvert.DeserializeObject(jsonResponse);
			return result.success == "true";
		}


	}
}
