using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
	public interface IAuthService
	{
		Task<bool> Login(Usuario model);
	}
}
