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

        public async Task<Restaurante> Actualizar(Restaurante restauranteDto)
        {
            // Buscar el restaurante en la base de datos
            var restaurante = await _context.Restaurantes.FindAsync(restauranteDto.restaurante_id);

            if (restaurante == null)
            {
                return null; // Si no se encuentra el restaurante, retornamos null o manejamos el error de otra forma
            }

            // Actualizar los campos del restaurante
            restaurante.nombre = restauranteDto.nombre;
            restaurante.direccion = restauranteDto.direccion;
            restaurante.descripcion = restauranteDto.descripcion;

            // Actualizar el restaurante en el contexto
            _context.Restaurantes.Update(restaurante);

            // Guardar los cambios
            await _context.SaveChangesAsync();

            return restaurante;
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
