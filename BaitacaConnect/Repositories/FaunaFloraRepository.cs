using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaitacaConnect.Repositories
{
    public class FaunaFloraRepository : IFaunaFloraRepository
    {
        private readonly BaitacaDbContext _context;

        public FaunaFloraRepository(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FaunaFlora>> GetAllAsync()
        {
            return await _context.FaunaFlora
                .OrderBy(f => f.NomePopular)
                .ToListAsync();
        }

        public async Task<FaunaFlora?> GetByIdAsync(int id)
        {
            return await _context.FaunaFlora.FindAsync(id);
        }

        public async Task<IEnumerable<FaunaFlora>> GetWithFiltersAsync(string? nome = null, string? tipo = null,
            string? categoria = null, int? idTrilha = null)
        {
            var query = _context.FaunaFlora.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(f => f.NomePopular.Contains(nome) || 
                                        (f.NomeCientifico != null && f.NomeCientifico.Contains(nome)));
            }

            if (!string.IsNullOrEmpty(tipo))
            {
                query = query.Where(f => f.Tipo == tipo);
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(f => f.Categoria == categoria);
            }

            if (idTrilha.HasValue)
            {
                query = query.Where(f => f.TrilhasOndeEncontra != null && 
                                        f.TrilhasOndeEncontra.Contains(idTrilha.Value));
            }

            return await query.OrderBy(f => f.NomePopular).ToListAsync();
        }

        public async Task<IEnumerable<FaunaFlora>> GetByTipoAsync(string tipo)
        {
            return await _context.FaunaFlora
                .Where(f => f.Tipo == tipo)
                .OrderBy(f => f.NomePopular)
                .ToListAsync();
        }

        public async Task<IEnumerable<FaunaFlora>> GetByCategoriaAsync(string categoria)
        {
            return await _context.FaunaFlora
                .Where(f => f.Categoria == categoria)
                .OrderBy(f => f.NomePopular)
                .ToListAsync();
        }

        public async Task<IEnumerable<FaunaFlora>> GetByTrilhaAsync(int idTrilha)
        {
            return await _context.FaunaFlora
                .Where(f => f.TrilhasOndeEncontra != null && f.TrilhasOndeEncontra.Contains(idTrilha))
                .OrderBy(f => f.NomePopular)
                .ToListAsync();
        }

        public async Task<FaunaFlora> CreateAsync(FaunaFlora faunaFlora)
        {
            _context.FaunaFlora.Add(faunaFlora);
            await _context.SaveChangesAsync();
            return faunaFlora;
        }

        public async Task<FaunaFlora> UpdateAsync(FaunaFlora faunaFlora)
        {
            _context.FaunaFlora.Update(faunaFlora);
            await _context.SaveChangesAsync();
            return faunaFlora;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var faunaFlora = await _context.FaunaFlora.FindAsync(id);
            if (faunaFlora == null) return false;

            _context.FaunaFlora.Remove(faunaFlora);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.FaunaFlora.AnyAsync(f => f.IdFaunaFlora == id);
        }

        public async Task<bool> NomeExistsAsync(string nomePopular, string? nomeCientifico = null)
        {
            var query = _context.FaunaFlora.Where(f => f.NomePopular == nomePopular);
            
            if (!string.IsNullOrEmpty(nomeCientifico))
            {
                query = query.Where(f => f.NomeCientifico == nomeCientifico);
            }

            return await query.AnyAsync();
        }

        public async Task<int> GetTotalByTipoAsync(string tipo)
        {
            return await _context.FaunaFlora.CountAsync(f => f.Tipo == tipo);
        }

        public async Task<IEnumerable<string>> GetCategoriasAsync()
        {
            return await _context.FaunaFlora
                .Where(f => !string.IsNullOrEmpty(f.Categoria))
                .Select(f => f.Categoria!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetEstatisticasPorTipoAsync()
        {
            var estatisticas = await _context.FaunaFlora
                .GroupBy(f => f.Tipo)
                .Select(g => new { Tipo = g.Key, Quantidade = g.Count() })
                .ToListAsync();

            return estatisticas.ToDictionary(e => e.Tipo, e => e.Quantidade);
        }

        public async Task<IEnumerable<FaunaFlora>> SearchAsync(string termo)
        {
            return await _context.FaunaFlora
                .Where(f => f.NomePopular.Contains(termo) ||
                           (f.NomeCientifico != null && f.NomeCientifico.Contains(termo)) ||
                           (f.Descricao != null && f.Descricao.Contains(termo)) ||
                           (f.Categoria != null && f.Categoria.Contains(termo)))
                .OrderBy(f => f.NomePopular)
                .ToListAsync();
        }
    }
}
