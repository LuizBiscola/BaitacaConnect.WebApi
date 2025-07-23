using Microsoft.EntityFrameworkCore;
using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Repositories.Interfaces;

namespace BaitacaConnect.Repositories
{
    public class ParqueRepository : IParqueRepository
    {
        private readonly BaitacaDbContext _context;

        public ParqueRepository(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<Parque?> GetByIdAsync(int id)
        {
            return await _context.Parques.FindAsync(id);
        }

        public async Task<Parque?> GetParqueByIdAsync(int id)
        {
            return await _context.Parques.FindAsync(id);
        }

        public async Task<Parque?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Parques
                .Include(p => p.Trilhas)
                .Include(p => p.Reservas)
                .Include(p => p.PontosInteresse)
                .FirstOrDefaultAsync(p => p.IdParque == id);
        }

        public async Task<IEnumerable<Parque>> GetAllAsync()
        {
            return await _context.Parques
                .OrderBy(p => p.NomeParque)
                .ToListAsync();
        }

        public async Task<IEnumerable<Parque>> GetWithFiltersAsync(string? filtroNome, bool? ativo)
        {
            var query = _context.Parques.AsQueryable();

            if (!string.IsNullOrEmpty(filtroNome))
            {
                query = query.Where(p => p.NomeParque.Contains(filtroNome));
            }

            if (ativo.HasValue)
            {
                query = query.Where(p => p.Ativo == ativo.Value);
            }

            return await query
                .OrderBy(p => p.NomeParque)
                .ToListAsync();
        }

        public async Task<IEnumerable<Parque>> GetAtivasWithTrilhasAsync()
        {
            return await _context.Parques
                .Include(p => p.Trilhas.Where(t => t.Ativo))
                .Where(p => p.Ativo)
                .OrderBy(p => p.NomeParque)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Parques.AnyAsync(p => p.IdParque == id);
        }

        public async Task<bool> IsActiveAsync(int id)
        {
            var parque = await _context.Parques.FindAsync(id);
            return parque?.Ativo ?? false;
        }

        public async Task<bool> NomeExistsAsync(string nome)
        {
            return await _context.Parques.AnyAsync(p => p.NomeParque == nome);
        }

        public async Task<Parque> CreateAsync(Parque parque)
        {
            _context.Parques.Add(parque);
            await _context.SaveChangesAsync();
            return parque;
        }

        public async Task UpdateAsync(Parque parque)
        {
            _context.Parques.Update(parque);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var parque = await _context.Parques.FindAsync(id);
            if (parque != null)
            {
                _context.Parques.Remove(parque);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCapacidadeAsync(int id)
        {
            var parque = await _context.Parques.FindAsync(id);
            return parque?.CapacidadeMaxima ?? 0;
        }
    }
}
