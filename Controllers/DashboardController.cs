using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;

namespace MVW_Proyecto_Mesas_Comida.Controllers
{

    
    public class DashboardController : Controller
	{
        private readonly ILogger<DashboardController> _logger;
        private readonly IDistributedCache _cache;
        private readonly IRestauranteService _restauranteService;
        private readonly IPlatilloService _platilloService;
        private readonly IUsuarioService _usuarioService;
        private const string sessionKey = "UserSessionKey";
        public DashboardController(ILogger<DashboardController> logger, IDistributedCache cache, IRestauranteService restauranteService, IPlatilloService platilloService, IUsuarioService usuarioService)
        {
            _logger = logger;
            _cache = cache;
            _restauranteService = restauranteService;
            _platilloService = platilloService;
            _usuarioService = usuarioService;
        }
        public async Task<IActionResult> Index()
        {
            // Recupera el token de la sesión del usuario
            var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

            if (string.IsNullOrEmpty(sessionDataSerialized))
            {
                return RedirectToAction("Index", "Home");
            }

            // Opcional: Puedes deserializar los datos si necesitas usarlos
            //var sessionData = System.Text.Json.JsonSerializer.Deserialize<dynamic>(sessionDataSerialized);

            // Continuar con la lógica de la vista
            return View();
        }
        public async Task<IActionResult> Productos()
        {
            // Recupera el token de la sesión del usuario
            var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

            if (string.IsNullOrEmpty(sessionDataSerialized))
            {
                return RedirectToAction("Index", "Home");
            }
            var restaurantes = _restauranteService.ObtenerTodos();
            var platillos = await _platilloService.GetAllPlatillos();
       
            var viewModel = restaurantes.Select(r => new Restaurante
            {
                restaurante_id = r.restaurante_id,
                nombre = r.nombre,            

            });
            ViewBag.Platillos = platillos;
            return View(viewModel);
        }
        public async Task<IActionResult> Usuarios()
        {
            // Recupera el token de la sesión del usuario
            var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

            if (string.IsNullOrEmpty(sessionDataSerialized))
            {
                return RedirectToAction("Index", "Home");
            }
            var usuarios = await _usuarioService.GetAllUsers();
            ViewBag.usuarios = usuarios;
            return View();
        }
        public async Task<IActionResult> Restaurantes()
        {
            // Recupera el token de la sesión del usuario
            var sessionDataSerialized = await _cache.GetStringAsync(sessionKey);

            if (string.IsNullOrEmpty(sessionDataSerialized))
            {
                return RedirectToAction("Index", "Home");
            }
            var restaurantes = _restauranteService.ObtenerTodos();
            ViewBag.restaurantes = restaurantes;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuardarProducto(Platillos platilloDto, int restauranteId)
        {
            ModelState.Remove("imagen_url");
            ModelState.Remove("Restaurante");
            ModelState.Remove("DetalleCompras");
            ModelState.Remove("platillo_id");
            if(restauranteId == 0)
            {
                return Json(new { success = false, message = "Por favor, Seleccione un restaurante." });
            }
            // Verificar si el modelo es válido antes de continuar
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si se ha cargado una imagen
                    if (Request.Form.Files.Count == 0 || Request.Form.Files[0].Length == 0)
                    {
                        return Json(new { success = false, message = "Por favor, carga una imagen del producto." });
                    }

                    var file = Request.Form.Files[0];  // Tomar el primer archivo (la imagen del producto)

                    // Generar un nombre de archivo único
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "productos");

                    // Crear el directorio si no existe
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    // Ruta completa del archivo
                    var filePath = Path.Combine(uploads, fileName);

                    // Guardar el archivo en el servidor
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Asignar la URL relativa de la imagen
                    var imagenUrl = Path.Combine("/images/productos", fileName);

                    // Guardar el platillo en la base de datos con la URL de la imagen
                    await _platilloService.AddPlatillo(new Platillos
                    {
                        nombre = platilloDto.nombre,
                        descripcion = platilloDto.descripcion,
                        precio = platilloDto.precio,
                        imagen_url = imagenUrl,
                        restaurante_id = restauranteId
                    });

                    // Retornar éxito en formato JSON
                    return Json(new { success = true, message = "Producto guardado correctamente." });
                }
                catch (Exception ex)
                {
                    // Retornar error en formato JSON
                    return Json(new { success = false, message = "Error al guardar el producto: " + ex.Message });
                }
            }

            // Si el modelo no es válido
            return Json(new { success = false, message = "Datos inválidos, revise la información del formulario." });
        }


        [HttpPost]
        public async Task<IActionResult> EliminarPlatillo(int id)
        {
            try
            {
                await _platilloService.DeletePlatillo(id);
                return Json(new { success = true, message = "Producto eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el producto: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPlatillo(int id)
        {
            var platillo = await _platilloService.GetPlatilloById(id);
            if (platillo == null)
            {
                return Json(new { success = false, message = "Platillo no encontrado." });
            }
            return Json(new { success = true, data = platillo });
        }


       
        [HttpPost]
        public async Task<IActionResult> EditarPlatillo(Platillos platilloDto)
        {
            ModelState.Remove("imagen_url");
            ModelState.Remove("Restaurante");
            ModelState.Remove("DetalleCompras");
            int restauranteId = Convert.ToInt32(Request.Form["restauranteId"]);

            platilloDto.restaurante_id = restauranteId;

            if (ModelState.IsValid)
            {
                // Verificar si la imagen ha cambiado
                var imagenCambiada = Request.Form["imagenCambiada"];
                bool.TryParse(imagenCambiada, out bool seCambioImagen);

                if (seCambioImagen)
                {
                    // Verificar que se haya subido una nueva imagen
                    if (Request.Form.Files.Count == 0 || Request.Form.Files[0].Length == 0)
                    {
                        return Json(new { success = false, message = "Error: No se ha subido ninguna imagen." });
                    }

                    var file = Request.Form.Files[0];

                    // Generar un nombre único para la nueva imagen
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "productos");

                    // Crear el directorio si no existe
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    var filePath = Path.Combine(uploads, fileName);

                    // Guardar el archivo
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Asignar la nueva URL de la imagen
                    platilloDto.imagen_url = Path.Combine("/images/productos", fileName);
                }
                else
                {
                    // Mantener la URL de la imagen original
                    platilloDto.imagen_url = Request.Form["imagenOriginal"];
                }

                var actualizado = await _platilloService.UpdatePlatillo(platilloDto);
                if (actualizado != null)
                {
                    return Json(new { success = true, message = "Platillo actualizado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "Error al actualizar el platillo." });
                }
            }

            return Json(new { success = false, message = "Datos inválidos." });
        }

        [HttpPost]
        public IActionResult GuardarRestaurante(Restaurante model)
        {
            ModelState.Remove("Platillos");
            ModelState.Remove("Mesas");
            if (ModelState.IsValid)
            {
         
                _restauranteService.Agregar(model);
               
                return Json(new { success = true, message = "Restaurante guardado con éxito" });
            }

            return Json(new { success = false, message = "Error al guardar el restaurante" });
        }

        // Obtener los datos de un restaurante para editar
        [HttpGet]
        public IActionResult ObtenerRestaurante(int id)
        {
            var restaurante = _restauranteService.ObtenerPorId(id);
            if (restaurante != null)
            {
                return Json(new { success = true, data = restaurante });
            }
            return Json(new { success = false, message = "Restaurante no encontrado" });
        }

        // Actualizar un restaurante
        [HttpPost]
        public async Task<IActionResult> EditarRestaurante(Restaurante model)
        {
            ModelState.Remove("Platillos");
            ModelState.Remove("Mesas");
            if (ModelState.IsValid)
            {

                int restauranteId = Convert.ToInt32(Request.Form["restauranteId"]);

                model.restaurante_id = restauranteId;
                var actualizado = await _restauranteService.Actualizar(model); ;
                if (actualizado != null)
                {
                    return Json(new { success = true, message = "Restaurante actualizado con éxito" });
                }
                else
                {
                    return Json(new { success = false, message = "Error al actualizar el restaurante" });
                }
            
               
            }
            return Json(new { success = false, message = "Error al actualizar el restaurante" });
        }

        // Eliminar un restaurante
        [HttpPost]
        public IActionResult EliminarRestaurante(int id)
        {
            var restaurante = _restauranteService.ObtenerPorId(id);
            if (restaurante != null)
            {
                _restauranteService.Eliminar(id);
                return Json(new { success = true, message = "Restaurante eliminado con éxito" });
            }
            return Json(new { success = false, message = "Restaurante no encontrado" });
        }


        [HttpGet]
        public IActionResult ObtenerUsuario(int id)
        {
            var usuario = _usuarioService.GetUsuarioById(id);
            if (usuario != null)
            {
                return Json(new { success = true, data = usuario });
            }
            return Json(new { success = false, message = "Usuario no encontrado" });
        }

        [HttpPost]
        public IActionResult EditarUsuario(Usuario model)
        {
            ModelState.Remove("Rol");
            ModelState.Remove("Compras");
            ModelState.Remove("Reservas");
            ModelState.Remove("contrasena");
            ModelState.Remove("token_autenticacion");
            int usuarioId = Convert.ToInt32(Request.Form["usuarioId"]);

            model.usuario_id = usuarioId;
            if (ModelState.IsValid)
            {
                _usuarioService.UpdateUsuario(model);
                return Json(new { success = true, message = "Usuario actualizado con éxito" });
            }
            return Json(new { success = false, message = "Error al actualizar el usuario" });
        }
        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var success = await _usuarioService.DeleteUsuario(id); // Await the async method
            if (success)
            {
                return Json(new { success = true, message = "Usuario eliminado con éxito" });
            }
            return Json(new { success = false, message = "Error al eliminar el usuario" });
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Obtener el usuario actual para construir la clave de sesión
        

            // Eliminar la sesión de Redis
            await _cache.RemoveAsync(sessionKey);

            // Opcional: si estás usando autenticación basada en cookies
            // HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redireccionar al inicio de sesión o a la página principal
            return RedirectToAction("Index", "Home");
        }

    }
}
