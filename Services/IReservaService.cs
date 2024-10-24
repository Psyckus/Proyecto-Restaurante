using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
	public interface IReservaService
	{
		Task<IEnumerable<Reservas>> GetReservasPorUsuarioAsync(int usuarioId);
	}
}
