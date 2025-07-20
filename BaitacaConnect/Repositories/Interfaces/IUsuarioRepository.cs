using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetWithFiltersAsync(string? filtroNome, string? filtroTipo, bool? ativo);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task<bool> IsActiveAsync(int id);
        Task<int> GetReservasCountAsync(int id);
        Task<int> GetReservasAtivasCountAsync(int id);
    }
}
