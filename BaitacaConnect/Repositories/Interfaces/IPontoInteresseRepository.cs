using BaitacaConnect.Models;

namespace BaitacaConnect.Repositories.Interfaces
{
    public interface IPontoInteresseRepository
    {
        Task<IEnumerable<PontoInteresse>> GetAllAsync();
        Task<PontoInteresse?> GetByIdAsync(int idParque, int idTrilha, string nomePonto);
        Task<IEnumerable<PontoInteresse>> GetByTrilhaAsync(int idTrilha);
        Task<IEnumerable<PontoInteresse>> GetByParqueAsync(int idParque);
        Task<IEnumerable<PontoInteresse>> GetByTipoAsync(string tipo);
        Task<IEnumerable<PontoInteresse>> GetWithFiltersAsync(int? idParque = null, int? idTrilha = null, 
            string? tipo = null, string? nome = null);
        Task<PontoInteresse> CreateAsync(PontoInteresse pontoInteresse);
        Task<PontoInteresse> UpdateAsync(PontoInteresse pontoInteresse);
        Task<bool> DeleteAsync(int idParque, int idTrilha, string nomePonto);
        Task<bool> ExistsAsync(int idParque, int idTrilha, string nomePonto);
        Task<IEnumerable<PontoInteresse>> GetMapaPontosAsync(int idTrilha);
        Task<int> GetProximaOrdemAsync(int idTrilha);
        Task<IEnumerable<string>> GetTiposAsync();
        Task<Dictionary<string, int>> GetEstatisticasPorTipoAsync();
        Task<IEnumerable<PontoInteresse>> GetPontosProximosAsync(string coordenadas, double raioKm);
    }
}
