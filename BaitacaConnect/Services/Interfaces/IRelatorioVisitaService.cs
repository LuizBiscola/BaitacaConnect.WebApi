using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Services.Interfaces
{
    public interface IRelatorioVisitaService
    {
        Task<IEnumerable<RelatorioVisitaResponseDto>> GetRelatoriosAsync(int? idReserva = null, int? idUsuario = null,
            int? idParque = null, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<RelatorioVisitaResponseDto?> GetRelatorioByIdAsync(int idRelatorio);
        Task<RelatorioVisitaResponseDto?> GetRelatorioByReservaAsync(int idReserva);
        Task<RelatorioVisitaResponseDto> CreateRelatorioAsync(CreateRelatorioVisitaDto createRelatorioDto);
        Task<RelatorioVisitaResponseDto> UpdateRelatorioAsync(int idRelatorio, UpdateRelatorioVisitaDto updateRelatorioDto);
        Task<bool> DeleteRelatorioAsync(int idRelatorio);
        Task<IEnumerable<RelatorioVisitaResumoDto>> GetMeusRelatoriosAsync(int idUsuario);
        Task<IEnumerable<RelatorioVisitaResponseDto>> GetRelatoriosByParqueAsync(int idParque);
        Task<EstatisticasRelatorioDto> GetEstatisticasParqueAsync(int idParque);
        Task<IEnumerable<RelatorioVisitaResponseDto>> GetRelatoriosComProblemasAsync();
        Task<EstatisticasGeralDto> GetEstatisticasGeraisAsync();
        Task<bool> PodeCriarRelatorioAsync(int idReserva);
    }
}
