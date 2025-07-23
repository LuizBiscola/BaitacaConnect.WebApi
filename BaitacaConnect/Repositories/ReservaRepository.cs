using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaitacaConnect.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly BaitacaDbContext _context;

        public ReservaRepository(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reserva>> GetReservasAsync(int? idUsuario = null, int? idParque = null, 
            int? idTrilha = null, DateOnly? dataInicio = null, DateOnly? dataFim = null, string? status = null)
        {
            var query = _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Parque)
                .Include(r => r.Trilha)
                .AsQueryable();

            if (idUsuario.HasValue)
                query = query.Where(r => r.IdUsuario == idUsuario.Value);

            if (idParque.HasValue)
                query = query.Where(r => r.IdParque == idParque.Value);

            if (idTrilha.HasValue)
                query = query.Where(r => r.IdTrilha == idTrilha.Value);

            if (dataInicio.HasValue)
                query = query.Where(r => r.DataVisita >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(r => r.DataVisita <= dataFim.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            return await query.OrderBy(r => r.DataVisita).ToListAsync();
        }

        public async Task<Reserva?> GetReservaByIdAsync(int idReserva)
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Parque)
                .Include(r => r.Trilha)
                .FirstOrDefaultAsync(r => r.IdReserva == idReserva);
        }

        public async Task<Reserva> CreateReservaAsync(Reserva reserva)
        {
            reserva.DataCriacao = DateTime.Now; // Removido SpecifyKind UTC
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
            return reserva;
        }

        public async Task<Reserva> UpdateReservaAsync(Reserva reserva)
        {
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
            return reserva;
        }

        public async Task<bool> DeleteReservaAsync(int idReserva)
        {
            var reserva = await _context.Reservas.FindAsync(idReserva);
            if (reserva == null) return false;

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Reserva>> GetReservasByUsuarioAsync(int idUsuario)
        {
            return await _context.Reservas
                .Include(r => r.Parque)
                .Include(r => r.Trilha)
                .Where(r => r.IdUsuario == idUsuario)
                .OrderByDescending(r => r.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> GetReservasAtivasAsync()
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Parque)
                .Include(r => r.Trilha)
                .Where(r => r.Status == "ativa")
                .OrderBy(r => r.DataVisita)
                .ToListAsync();
        }

        public async Task<int> GetNumeroVisitantesPorDataAsync(int idParque, DateOnly data, int? idTrilha = null)
        {
            var query = _context.Reservas
                .Where(r => r.IdParque == idParque && 
                           r.DataVisita == data && 
                           r.Status == "ativa");

            if (idTrilha.HasValue)
                query = query.Where(r => r.IdTrilha == idTrilha.Value);

            return await query.SumAsync(r => r.NumeroVisitantes);
        }

        public async Task<bool> ExisteReservaAtivaAsync(int idUsuario, int idParque, DateOnly data)
        {
            return await _context.Reservas
                .AnyAsync(r => r.IdUsuario == idUsuario && 
                              r.IdParque == idParque && 
                              r.DataVisita == data && 
                              r.Status == "ativa");
        }

        public async Task<IEnumerable<Reserva>> GetReservasPorCalendarioAsync(int idParque, DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Trilha)
                .Where(r => r.IdParque == idParque && 
                           r.DataVisita >= dataInicio && 
                           r.DataVisita <= dataFim &&
                           r.Status == "ativa")
                .OrderBy(r => r.DataVisita)
                .ToListAsync();
        }

        public async Task<bool> ReservaExisteAsync(int idReserva)
        {
            return await _context.Reservas.AnyAsync(r => r.IdReserva == idReserva);
        }

        public async Task<Reserva?> GetReservaComDetalhesAsync(int idReserva)
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Parque)
                .Include(r => r.Trilha)
                .FirstOrDefaultAsync(r => r.IdReserva == idReserva);
        }
    }
}
