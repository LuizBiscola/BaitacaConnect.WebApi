using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Services.Interfaces
{
    public interface IPontoInteresseService
    {
        Task<IEnumerable<PontoInteresseResponseDto>> GetAllAsync();
        Task<PontoInteresseResponseDto?> GetByIdAsync(int idParque, int idTrilha, string nomePonto);
        Task<IEnumerable<PontoInteresseResponseDto>> GetByTrilhaAsync(int idTrilha);
        Task<IEnumerable<PontoInteresseResponseDto>> GetByParqueAsync(int idParque);
        Task<IEnumerable<PontoInteresseResponseDto>> GetWithFiltersAsync(int? idParque = null, int? idTrilha = null,
            string? tipo = null, string? nome = null);
        Task<PontoInteresseResponseDto> CreateAsync(CreatePontoInteresseDto createDto);
        Task<PontoInteresseResponseDto> UpdateAsync(int idParque, int idTrilha, string nomePonto, UpdatePontoInteresseDto updateDto);
        Task<bool> DeleteAsync(int idParque, int idTrilha, string nomePonto);
        Task<MapaTrilhaDto> GetMapaTrilhaAsync(int idTrilha);
        Task<IEnumerable<string>> GetTiposAsync();
        Task<EstatisticasPontosDto> GetEstatisticasAsync();
        Task<IEnumerable<PontoInteresseResponseDto>> GetPontosProximosAsync(string coordenadas, double raioKm);
        Task<bool> ReordenarPontosAsync(int idTrilha, List<ReordenarPontoDto> novaOrdem);
    }
}
