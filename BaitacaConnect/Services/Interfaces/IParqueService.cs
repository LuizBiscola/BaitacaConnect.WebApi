using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Services.Interfaces
{
    public interface IParqueService
    {
        Task<IEnumerable<ParqueResumoDto>> GetParquesAsync(string? filtroNome, bool? ativo);
        Task<ParqueResponseDto?> GetParqueByIdAsync(int id);
        Task<ParqueResponseDto> CriarParqueAsync(CreateParqueDto createParqueDto);
        Task<bool> AtualizarParqueAsync(int id, UpdateParqueDto updateParqueDto);
        Task<bool> AtivarParqueAsync(int id);
        Task<bool> DesativarParqueAsync(int id);
        Task<bool> ParqueExisteAsync(int id);
        Task<bool> ParqueAtivoAsync(int id);
        Task<bool> NomeParqueExisteAsync(string nome);
        Task<int> GetCapacidadeParqueAsync(int id);
        Task<IEnumerable<ParqueComTrilhasDto>> GetParquesComTrilhasAsync();
        Task<EstatisticasParqueDto> GetEstatisticasParqueAsync(int id);
    }
}
