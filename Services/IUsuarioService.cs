using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllUsers();
        Task<bool> EmailExists(string email);
        Task<bool> CreateUser(Usuario model);
        Task<Usuario> GetUsuarioByEmail(string email);
        Usuario GetUsuarioById(int id);
        Task<bool> UpdateUsuario(Usuario model);
        Task<bool> DeleteUsuario(int id);
    }
}
