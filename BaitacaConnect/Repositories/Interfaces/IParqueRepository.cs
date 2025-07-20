using BaitacaConnect.Models;

namespace BaitacaConnect.Repositories.Interfaces
{
    public interface IParqueRepository
    {
        Task<Parque?> GetByIdAsync(int id);
        Task<Parque?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Parque>> GetAllAsync();
        Task<IEnumerable<Parque>> GetWithFiltersAsync(string? filtroNome, bool? ativo);
        Task<IEnumerable<Parque>> GetAtivasWithTrilhasAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> IsActiveAsync(int id);
        Task<bool> NomeExistsAsync(string nome);
        Task<Parque> CreateAsync(Parque parque);
        Task UpdateAsync(Parque parque);
        Task DeleteAsync(int id);
        Task<int> GetCapacidadeAsync(int id);
    }
}
