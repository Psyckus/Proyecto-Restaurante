using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
	public class UsuarioService : IUsuarioService
	{

		private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

   
        public UsuarioService(ApplicationDbContext context, IConfiguration configuration)
		{
            // Aquí obtienes la cadena de conexión desde el archivo appsettings.json
            _connectionString = configuration.GetConnectionString("Connection");
            _context = context;
		}
     
        public async Task<IEnumerable<Usuario>> GetAllUsers()
        {
            return await _context.Usuarios.ToListAsync();
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



        // Aquí puedes usar ADO.NET si prefieres una consulta SQL directa
        public async Task<bool> UpdateUsuario(Usuario usuario)
        {
            var query = @"UPDATE Usuarios SET nombre = @nombre, correo = @correo, rol_id = @rol_id WHERE usuario_id = @usuario_id";
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", usuario.nombre);
                        command.Parameters.AddWithValue("@correo", usuario.correo);
         
                        command.Parameters.AddWithValue("@rol_id", usuario.rol_id);
                        command.Parameters.AddWithValue("@usuario_id", usuario.usuario_id);

                        int result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el usuario: {ex.Message}");
                return false;
            }
        }




        public async Task<bool> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario != null)
                {
                    // Check for related records in the Reservas table
                    var reservas = await _context.Reservas.Where(r => r.usuario_id == id).ToListAsync();
                    if (reservas.Any())
                    {
                        // Option 1: Return false indicating that deletion cannot occur
                        return false; // User has related reservas, handle this case

                        // Option 2: If you want to delete related reservas first, uncomment below
                        // _context.Reservas.RemoveRange(reservas);
                        // await _context.SaveChangesAsync();
                    }

                    _context.Usuarios.Remove(usuario);
                    await _context.SaveChangesAsync();
                    return true; // Deletion was successful
                }
                return false; // User not found
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                // _logger.LogError(ex, "Error while deleting user with ID {UserId}", id);
                return false; // Indicate that deletion failed
            }
        }

  

        public Usuario GetUsuarioById(int id)
        {
            return _context.Usuarios.Find(id);
        }


    }
}
