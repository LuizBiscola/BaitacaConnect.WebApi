using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Services.Interfaces
{
    public interface ITrilhaService
    {
        Task<IEnumerable<TrilhaResumoDto>> GetTrilhasAsync(string? filtroNome, string? filtroDificuldade, bool? ativo, int? idParque);
        Task<TrilhaResponseDto?> GetTrilhaByIdAsync(int id);
        Task<TrilhaResponseDto> CriarTrilhaAsync(CreateTrilhaDto createTrilhaDto);
        Task<bool> AtualizarTrilhaAsync(int id, UpdateTrilhaDto updateTrilhaDto);
        Task<bool> AtivarTrilhaAsync(int id);
        Task<bool> DesativarTrilhaAsync(int id);
        Task<bool> TrilhaExisteAsync(int id);
        Task<bool> TrilhaAtivaAsync(int id);
        Task<bool> NomeTrilhaExisteNoParqueAsync(string nome, int idParque);
        Task<int> GetCapacidadeTrilhaAsync(int id);
        Task<IEnumerable<TrilhaResumoDto>> GetTrilhasPorParqueAsync(int idParque);
        Task<IEnumerable<TrilhaComMapaDto>> GetTrilhasComMapaAsync(int? idParque = null);
        Task<bool> ValidarCapacidadeTrilhaAsync(int id, int numeroVisitantes);
        Task<EstatisticasTrilhaDto> GetEstatisticasTrilhaAsync(int id);
        Task<IEnumerable<TrilhaResumoDto>> GetTrilhasPorDificuldadeAsync(string dificuldade);
        Task<IEnumerable<TrilhaResumoDto>> GetTrilhasDisponiveisAsync(DateOnly data, int numeroVisitantes);
    }
}
