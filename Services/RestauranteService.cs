using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
	public class RestauranteService : IRestauranteService
	{
		private readonly ApplicationDbContext _context;

		public RestauranteService(ApplicationDbContext context)
		{
			_context = context;
		}

		public IEnumerable<Restaurante> ObtenerTodos()
		{
			return _context.Restaurantes.ToList();
		}

		public Restaurante ObtenerPorId(int id)
		{
			return _context.Restaurantes.Find(id);
		}

		public void Agregar(Restaurante restaurante)
		{
			_context.Restaurantes.Add(restaurante);
			_context.SaveChanges();
		}

		public void Actualizar(Restaurante restaurante)
		{
			_context.Restaurantes.Update(restaurante);
			_context.SaveChanges();
		}

		public void Eliminar(int id)
		{
			var restaurante = _context.Restaurantes.Find(id);
			if (restaurante != null)
			{
				_context.Restaurantes.Remove(restaurante);
				_context.SaveChanges();
			}
		}
	}
}
