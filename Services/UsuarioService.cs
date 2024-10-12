using Microsoft.EntityFrameworkCore;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
	public class UsuarioService : IUsuarioService
	{

		private readonly ApplicationDbContext _context;
		public UsuarioService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> EmailExists(string email)
		{
			return await _context.Usuarios.AnyAsync(u => u.correo == email);
		}

		public async Task<bool> CreateUser(Usuario model)
		{
			var newUser = new Usuario
			{
				nombre = model.nombre,
				correo = model.correo,
				contrasena = BCrypt.Net.BCrypt.HashPassword(model.contrasena),
				token_autenticacion = Guid.NewGuid().ToString(),
				rol_id = 2
			};

			try
			{
				_context.Usuarios.Add(newUser);
				await _context.SaveChangesAsync();
				return true; // Devuelve verdadero si el usuario se creó correctamente
			}
			catch (Exception)
			{
				// Aquí podrías manejar el error según sea necesario
				return false; // Devuelve falso si hubo un error
			}
		}
		public async Task<Usuario> GetUsuarioByEmail(string email)
		{
			return await _context.Usuarios.FirstOrDefaultAsync(u => u.correo == email);
		}

	}
}
