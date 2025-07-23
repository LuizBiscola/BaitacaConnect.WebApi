using Microsoft.EntityFrameworkCore;
using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Repositories.Interfaces;

namespace BaitacaConnect.Repositories
{
    public class TrilhaRepository : ITrilhaRepository
    {
        private readonly BaitacaDbContext _context;

        public TrilhaRepository(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<Trilha?> GetByIdAsync(int id)
        {
            return await _context.Trilhas.FindAsync(id);
        }

        public async Task<Trilha?> GetTrilhaByIdAsync(int id)
        {
            return await _context.Trilhas.FindAsync(id);
        }

        public async Task<IEnumerable<Trilha>> GetAllAsync()
        {
            return await _context.Trilhas
                .Include(t => t.Parque)
                .OrderBy(t => t.NomeTrilha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trilha>> GetWithFiltersAsync(string? filtroNome, string? filtroDificuldade, bool? ativo, int? idParque)
        {
            var query = _context.Trilhas.AsQueryable();

            if (!string.IsNullOrEmpty(filtroNome))
            {
                query = query.Where(t => t.NomeTrilha.Contains(filtroNome));
            }

            if (!string.IsNullOrEmpty(filtroDificuldade))
            {
                query = query.Where(t => t.DificuldadeTrilha == filtroDificuldade);
            }

            if (ativo.HasValue)
            {
                query = query.Where(t => t.Ativo == ativo.Value);
            }

            if (idParque.HasValue)
            {
                query = query.Where(t => t.IdParque == idParque.Value);
            }

            return await query
                .OrderBy(t => t.NomeTrilha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trilha>> GetByParqueAsync(int idParque)
        {
            return await _context.Trilhas
                .Where(t => t.IdParque == idParque)
                .OrderBy(t => t.NomeTrilha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trilha>> GetByDificuldadeAsync(string dificuldade)
        {
            return await _context.Trilhas
                .Where(t => t.DificuldadeTrilha == dificuldade && t.Ativo)
                .OrderBy(t => t.NomeTrilha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trilha>> GetAtivasAsync()
        {
            return await _context.Trilhas
                .Include(t => t.PontosInteresse)
                .Where(t => t.Ativo)
                .OrderBy(t => t.NomeTrilha)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Trilhas.AnyAsync(t => t.IdTrilha == id);
        }

        public async Task<bool> IsActiveAsync(int id)
        {
            var trilha = await _context.Trilhas.FindAsync(id);
            return trilha?.Ativo ?? false;
        }

        public async Task<bool> NomeExistsInParqueAsync(string nome, int idParque)
        {
            return await _context.Trilhas.AnyAsync(t => t.NomeTrilha == nome && t.IdParque == idParque);
        }

        public async Task<Trilha> CreateAsync(Trilha trilha)
        {
            _context.Trilhas.Add(trilha);
            await _context.SaveChangesAsync();
            return trilha;
        }

        public async Task UpdateAsync(Trilha trilha)
        {
            _context.Trilhas.Update(trilha);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var trilha = await _context.Trilhas.FindAsync(id);
            if (trilha != null)
            {
                _context.Trilhas.Remove(trilha);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCapacidadeAsync(int id)
        {
            var trilha = await _context.Trilhas.FindAsync(id);
            return trilha?.CapacidadeMaxima ?? 0;
        }

        public async Task<IEnumerable<Trilha>> GetWithReservasByDateAsync(DateOnly data)
        {
            return await _context.Trilhas
                .Include(t => t.Reservas.Where(r => r.DataVisita == data && r.Status == "ativa"))
                .Where(t => t.Ativo)
                .ToListAsync();
        }
    }
}
