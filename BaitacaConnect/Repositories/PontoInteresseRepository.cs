using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaitacaConnect.Repositories
{
    public class PontoInteresseRepository : IPontoInteresseRepository
    {
        private readonly BaitacaDbContext _context;

        public PontoInteresseRepository(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PontoInteresse>> GetAllAsync()
        {
            return await _context.PontosInteresse
                .Include(p => p.Parque)
                .Include(p => p.Trilha)
                .OrderBy(p => p.IdTrilha)
                .ThenBy(p => p.OrdemNaTrilha)
                .ToListAsync();
        }

        public async Task<PontoInteresse?> GetByIdAsync(int idParque, int idTrilha, string nomePonto)
        {
            return await _context.PontosInteresse
                .Include(p => p.Parque)
                .Include(p => p.Trilha)
                .FirstOrDefaultAsync(p => p.IdParque == idParque && 
                                         p.IdTrilha == idTrilha && 
                                         p.NomePontoInteresse == nomePonto);
        }

        public async Task<IEnumerable<PontoInteresse>> GetByTrilhaAsync(int idTrilha)
        {
            return await _context.PontosInteresse
                .Include(p => p.Parque)
                .Where(p => p.IdTrilha == idTrilha)
                .OrderBy(p => p.OrdemNaTrilha)
                .ToListAsync();
        }

        public async Task<IEnumerable<PontoInteresse>> GetByParqueAsync(int idParque)
        {
            return await _context.PontosInteresse
                .Include(p => p.Trilha)
                .Where(p => p.IdParque == idParque)
                .OrderBy(p => p.IdTrilha)
                .ThenBy(p => p.OrdemNaTrilha)
                .ToListAsync();
        }

        public async Task<IEnumerable<PontoInteresse>> GetByTipoAsync(string tipo)
        {
            return await _context.PontosInteresse
                .Include(p => p.Parque)
                .Include(p => p.Trilha)
                .Where(p => p.Tipo == tipo)
                .OrderBy(p => p.IdTrilha)
                .ThenBy(p => p.OrdemNaTrilha)
                .ToListAsync();
        }

        public async Task<IEnumerable<PontoInteresse>> GetWithFiltersAsync(int? idParque = null, int? idTrilha = null, 
            string? tipo = null, string? nome = null)
        {
            var query = _context.PontosInteresse
                .Include(p => p.Parque)
                .Include(p => p.Trilha)
                .AsQueryable();

            if (idParque.HasValue)
                query = query.Where(p => p.IdParque == idParque.Value);

            if (idTrilha.HasValue)
                query = query.Where(p => p.IdTrilha == idTrilha.Value);

            if (!string.IsNullOrEmpty(tipo))
                query = query.Where(p => p.Tipo == tipo);

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(p => p.NomePontoInteresse.Contains(nome));

            return await query
                .OrderBy(p => p.IdTrilha)
                .ThenBy(p => p.OrdemNaTrilha)
                .ToListAsync();
        }

        public async Task<PontoInteresse> CreateAsync(PontoInteresse pontoInteresse)
        {
            // Se não tem ordem definida, calcular a próxima
            if (!pontoInteresse.OrdemNaTrilha.HasValue)
            {
                pontoInteresse.OrdemNaTrilha = await GetProximaOrdemAsync(pontoInteresse.IdTrilha);
            }

            _context.PontosInteresse.Add(pontoInteresse);
            await _context.SaveChangesAsync();
            return pontoInteresse;
        }

        public async Task<PontoInteresse> UpdateAsync(PontoInteresse pontoInteresse)
        {
            _context.PontosInteresse.Update(pontoInteresse);
            await _context.SaveChangesAsync();
            return pontoInteresse;
        }

        public async Task<bool> DeleteAsync(int idParque, int idTrilha, string nomePonto)
        {
            var ponto = await _context.PontosInteresse
                .FirstOrDefaultAsync(p => p.IdParque == idParque && 
                                         p.IdTrilha == idTrilha && 
                                         p.NomePontoInteresse == nomePonto);
            
            if (ponto == null) return false;

            _context.PontosInteresse.Remove(ponto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int idParque, int idTrilha, string nomePonto)
        {
            return await _context.PontosInteresse
                .AnyAsync(p => p.IdParque == idParque && 
                              p.IdTrilha == idTrilha && 
                              p.NomePontoInteresse == nomePonto);
        }

        public async Task<IEnumerable<PontoInteresse>> GetMapaPontosAsync(int idTrilha)
        {
            return await _context.PontosInteresse
                .Where(p => p.IdTrilha == idTrilha && !string.IsNullOrEmpty(p.Coordenadas))
                .OrderBy(p => p.OrdemNaTrilha)
                .ToListAsync();
        }

        public async Task<int> GetProximaOrdemAsync(int idTrilha)
        {
            var ultimaOrdem = await _context.PontosInteresse
                .Where(p => p.IdTrilha == idTrilha)
                .MaxAsync(p => (int?)p.OrdemNaTrilha) ?? 0;

            return ultimaOrdem + 1;
        }

        public async Task<IEnumerable<string>> GetTiposAsync()
        {
            return await _context.PontosInteresse
                .Where(p => !string.IsNullOrEmpty(p.Tipo))
                .Select(p => p.Tipo!)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetEstatisticasPorTipoAsync()
        {
            var estatisticas = await _context.PontosInteresse
                .Where(p => !string.IsNullOrEmpty(p.Tipo))
                .GroupBy(p => p.Tipo!)
                .Select(g => new { Tipo = g.Key, Quantidade = g.Count() })
                .ToListAsync();

            return estatisticas.ToDictionary(e => e.Tipo, e => e.Quantidade);
        }

        public async Task<IEnumerable<PontoInteresse>> GetPontosProximosAsync(string coordenadas, double raioKm)
        {
            // Implementação básica - em um cenário real usaria funções geoespaciais do PostgreSQL
            return await _context.PontosInteresse
                .Include(p => p.Parque)
                .Include(p => p.Trilha)
                .Where(p => !string.IsNullOrEmpty(p.Coordenadas))
                .ToListAsync();
        }
    }
}
