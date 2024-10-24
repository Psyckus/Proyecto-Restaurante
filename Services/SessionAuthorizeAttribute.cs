using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using MVW_Proyecto_Mesas_Comida.Models;
using System.Text.Json;

namespace MVW_Proyecto_Mesas_Comida.Services
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly IDistributedCache _cache;
        private const string sessionKey = "UserSessionKey";

        public SessionAuthorizeAttribute(IDistributedCache cache)
        {
            _cache = cache;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Obtener la sesión de Redis de manera síncrona
            var sessionDataSerialized = _cache.GetString(sessionKey); // Usa GetString para operaciones síncronas

            if (string.IsNullOrEmpty(sessionDataSerialized))
            {
                // Si no hay sesión activa, devolver una respuesta de no autorizado (401)
                context.Result = new UnauthorizedResult();
                return; // Finalizar la ejecución del filtro
            }

            // Opcional: deserializar los datos de sesión si es necesario
            var sessionData = JsonSerializer.Deserialize<SessionData>(sessionDataSerialized);
            // Puedes hacer validaciones adicionales aquí, como verificar la vigencia del token

            base.OnActionExecuting(context);
        }
    }
}
