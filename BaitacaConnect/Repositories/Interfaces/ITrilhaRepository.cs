using BaitacaConnect.Models;

namespace BaitacaConnect.Repositories.Interfaces
{
    public interface ITrilhaRepository
    {
        Task<Trilha?> GetByIdAsync(int id);
        Task<IEnumerable<Trilha>> GetAllAsync();
        Task<IEnumerable<Trilha>> GetWithFiltersAsync(string? filtroNome, string? filtroDificuldade, bool? ativo, int? idParque);
        Task<IEnumerable<Trilha>> GetByParqueAsync(int idParque);
        Task<IEnumerable<Trilha>> GetByDificuldadeAsync(string dificuldade);
        Task<IEnumerable<Trilha>> GetAtivasAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> IsActiveAsync(int id);
        Task<bool> NomeExistsInParqueAsync(string nome, int idParque);
        Task<Trilha> CreateAsync(Trilha trilha);
        Task UpdateAsync(Trilha trilha);
        Task DeleteAsync(int id);
        Task<int> GetCapacidadeAsync(int id);
        Task<IEnumerable<Trilha>> GetWithReservasByDateAsync(DateOnly data);
    }
}
