using Microsoft.EntityFrameworkCore;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
    public class PlatilloService : IPlatilloService
    {
        private readonly ApplicationDbContext _context;

        public PlatilloService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Platillos>> GetAllPlatillos()
        {
            return await _context.Platillos.ToListAsync();
        }

        public async Task<Platillos> AddPlatillo(Platillos platilloDto)
        {
            var platillo = new Platillos
            {
                nombre = platilloDto.nombre,
                descripcion = platilloDto.descripcion,
                imagen_url = platilloDto.imagen_url,
                precio = platilloDto.precio,
                restaurante_id = platilloDto.restaurante_id
            };

            _context.Platillos.Add(platillo);
            await _context.SaveChangesAsync();
            return platillo;
        }

        // Obtener un platillo por su ID
        public async Task<Platillos> GetPlatilloById(int id)
        {
            return await _context.Platillos.FindAsync(id);
        }

        // Actualizar un platillo
        public async Task<Platillos> UpdatePlatillo(Platillos platilloDto)
        {
            var platillo = await _context.Platillos.FindAsync(platilloDto.platillo_id);

            if (platillo == null)
            {
                return null; // Si el platillo no se encuentra, retornamos null o puedes manejarlo de otra manera.
            }

            // Actualizamos los campos del platillo
            platillo.nombre = platilloDto.nombre;
            platillo.descripcion = platilloDto.descripcion;
            platillo.imagen_url = platilloDto.imagen_url;
            platillo.precio = platilloDto.precio;
            platillo.restaurante_id = platilloDto.restaurante_id;


            _context.Platillos.Update(platillo);
            await _context.SaveChangesAsync();
            return platillo;
        }

        // Eliminar un platillo por su ID
        public async Task DeletePlatillo(int id)
        {
            var platillo = await _context.Platillos.FindAsync(id);
            if (platillo != null)
            {
                _context.Platillos.Remove(platillo);
                await _context.SaveChangesAsync();
            }
        }




		public async Task<IEnumerable<Platillos>> GetPlatillosPorRestauranteAsync(int restauranteId)
		{
			return await _context.Platillos
				.Where(p => p.restaurante_id == restauranteId)
				.ToListAsync();
		}

	}
}
