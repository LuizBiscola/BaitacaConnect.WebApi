using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaitacaConnect.Repositories
{
    public class RelatorioVisitaRepository : IRelatorioVisitaRepository
    {
        private readonly BaitacaDbContext _context;

        public RelatorioVisitaRepository(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RelatorioVisita>> GetRelatoriosAsync(int? idReserva = null, int? idUsuario = null,
            int? idParque = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var query = _context.RelatoriosVisita
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Usuario)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Parque)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Trilha)
                .AsQueryable();

            if (idReserva.HasValue)
                query = query.Where(r => r.IdReserva == idReserva.Value);

            if (idUsuario.HasValue)
                query = query.Where(r => r.Reserva.IdUsuario == idUsuario.Value);

            if (idParque.HasValue)
                query = query.Where(r => r.Reserva.IdParque == idParque.Value);

            if (dataInicio.HasValue)
                query = query.Where(r => r.DataRelatorio >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(r => r.DataRelatorio <= dataFim.Value);

            return await query.OrderByDescending(r => r.DataRelatorio).ToListAsync();
        }

        public async Task<RelatorioVisita?> GetRelatorioByIdAsync(int idRelatorio)
        {
            return await _context.RelatoriosVisita
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Usuario)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Parque)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Trilha)
                .FirstOrDefaultAsync(r => r.IdRelatorio == idRelatorio);
        }

        public async Task<RelatorioVisita?> GetRelatorioByReservaAsync(int idReserva)
        {
            return await _context.RelatoriosVisita
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Usuario)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Parque)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Trilha)
                .FirstOrDefaultAsync(r => r.IdReserva == idReserva);
        }

        public async Task<RelatorioVisita> CreateRelatorioAsync(RelatorioVisita relatorio)
        {
            relatorio.DataRelatorio = DateTime.Now;
            _context.RelatoriosVisita.Add(relatorio);
            await _context.SaveChangesAsync();
            return relatorio;
        }

        public async Task<RelatorioVisita> UpdateRelatorioAsync(RelatorioVisita relatorio)
        {
            _context.RelatoriosVisita.Update(relatorio);
            await _context.SaveChangesAsync();
            return relatorio;
        }

        public async Task<bool> DeleteRelatorioAsync(int idRelatorio)
        {
            var relatorio = await _context.RelatoriosVisita.FindAsync(idRelatorio);
            if (relatorio == null) return false;

            _context.RelatoriosVisita.Remove(relatorio);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteRelatorioParaReservaAsync(int idReserva)
        {
            return await _context.RelatoriosVisita.AnyAsync(r => r.IdReserva == idReserva);
        }

        public async Task<IEnumerable<RelatorioVisita>> GetRelatoriosByUsuarioAsync(int idUsuario)
        {
            return await _context.RelatoriosVisita
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Parque)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Trilha)
                .Where(r => r.Reserva.IdUsuario == idUsuario)
                .OrderByDescending(r => r.DataRelatorio)
                .ToListAsync();
        }

        public async Task<IEnumerable<RelatorioVisita>> GetRelatoriosByParqueAsync(int idParque)
        {
            return await _context.RelatoriosVisita
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Usuario)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Trilha)
                .Where(r => r.Reserva.IdParque == idParque)
                .OrderByDescending(r => r.DataRelatorio)
                .ToListAsync();
        }

        public async Task<double> GetAvaliacaoMediaParqueAsync(int idParque)
        {
            var avaliacoes = await _context.RelatoriosVisita
                .Where(r => r.Reserva.IdParque == idParque && r.Avaliacao.HasValue)
                .Select(r => r.Avaliacao!.Value)
                .ToListAsync();

            return avaliacoes.Any() ? avaliacoes.Average() : 0;
        }

        public async Task<IEnumerable<RelatorioVisita>> GetRelatoriosComProblemasAsync()
        {
            return await _context.RelatoriosVisita
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Usuario)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Parque)
                .Include(r => r.Reserva)
                    .ThenInclude(res => res.Trilha)
                .Where(r => !string.IsNullOrEmpty(r.ProblemasEncontrados))
                .OrderByDescending(r => r.DataRelatorio)
                .ToListAsync();
        }

        public async Task<int> GetTotalRelatoriosAsync()
        {
            return await _context.RelatoriosVisita.CountAsync();
        }

        public async Task<Dictionary<int, int>> GetEstatisticasAvaliacoesAsync(int? idParque = null)
        {
            var query = _context.RelatoriosVisita.AsQueryable();

            if (idParque.HasValue)
                query = query.Where(r => r.Reserva.IdParque == idParque.Value);

            var avaliacoes = await query
                .Where(r => r.Avaliacao.HasValue)
                .GroupBy(r => r.Avaliacao!.Value)
                .Select(g => new { Avaliacao = g.Key, Quantidade = g.Count() })
                .ToListAsync();

            return avaliacoes.ToDictionary(a => a.Avaliacao, a => a.Quantidade);
        }
    }
}
