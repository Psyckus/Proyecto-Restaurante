using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
    public interface IPlatilloService
    {
        Task<IEnumerable<Platillos>> GetAllPlatillos();
		Task<IEnumerable<Platillos>> GetPlatillosPorRestauranteAsync(int restauranteId);
		Task<Platillos> AddPlatillo(Platillos platilloDto);

        Task<Platillos> GetPlatilloById(int id); // Método para obtener un platillo por su ID
        Task<Platillos> UpdatePlatillo(Platillos platilloDto); // Método para actualizar un platillo
        Task DeletePlatillo(int id);
    }
}
