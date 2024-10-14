using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
	public interface IRestauranteService
	{
		IEnumerable<Restaurante> ObtenerTodos();
		Restaurante ObtenerPorId(int id);
		void Agregar(Restaurante restaurante);
		void Actualizar(Restaurante restaurante);
		void Eliminar(int id);
	}
}
