using BaitacaConnect.Models;

namespace BaitacaConnect.Repositories.Interfaces
{
    public interface IFaunaFloraRepository
    {
        Task<IEnumerable<FaunaFlora>> GetAllAsync();
        Task<FaunaFlora?> GetByIdAsync(int id);
        Task<IEnumerable<FaunaFlora>> GetWithFiltersAsync(string? nome = null, string? tipo = null, 
            string? categoria = null, int? idTrilha = null);
        Task<IEnumerable<FaunaFlora>> GetByTipoAsync(string tipo);
        Task<IEnumerable<FaunaFlora>> GetByCategoriaAsync(string categoria);
        Task<IEnumerable<FaunaFlora>> GetByTrilhaAsync(int idTrilha);
        Task<FaunaFlora> CreateAsync(FaunaFlora faunaFlora);
        Task<FaunaFlora> UpdateAsync(FaunaFlora faunaFlora);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> NomeExistsAsync(string nomePopular, string? nomeCientifico = null);
        Task<int> GetTotalByTipoAsync(string tipo);
        Task<IEnumerable<string>> GetCategoriasAsync();
        Task<Dictionary<string, int>> GetEstatisticasPorTipoAsync();
        Task<IEnumerable<FaunaFlora>> SearchAsync(string termo);
    }
}
