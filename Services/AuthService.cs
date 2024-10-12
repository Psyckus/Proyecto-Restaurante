using Microsoft.EntityFrameworkCore;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
	public class AuthService : IAuthService
	{


		private readonly ApplicationDbContext _context;
		public AuthService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> Login(Usuario model)
		{
			var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.correo == model.correo);
			if (user != null && BCrypt.Net.BCrypt.Verify(model.contrasena, user.contrasena))
			{
				// Lógica para manejar el inicio de sesión, como generar un token
				return true;
			}
			return false;
		}

	
	}
}
