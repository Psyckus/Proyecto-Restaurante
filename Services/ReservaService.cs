using Microsoft.EntityFrameworkCore;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Services
{
	public class ReservaService : IReservaService
	{

		private readonly ApplicationDbContext _context;
		public ReservaService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Reservas>> GetReservasPorUsuarioAsync(int usuarioId)
		{
			// Consulta para obtener las reservas del usuario por su ID, incluyendo la Mesa y el Restaurante
			return await _context.Reservas
				.Where(r => r.usuario_id == usuarioId)
				.Include(r => r.Mesa)  // Incluir detalles de la Mesa
				.ThenInclude(m => m.Restaurante)  // Incluir detalles del Restaurante a través de la Mesa
				.Include(r => r.Usuario)  // Incluir detalles del Usuario
				.ToListAsync();
		}

	}

}
