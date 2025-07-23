using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Services.Interfaces
{
    public interface IFaunaFloraService
    {
        Task<IEnumerable<FaunaFloraResponseDto>> GetAllAsync();
        Task<FaunaFloraResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<FaunaFloraResponseDto>> GetWithFiltersAsync(string? nome = null, string? tipo = null,
            string? categoria = null, int? idTrilha = null);
        Task<IEnumerable<FaunaFloraResponseDto>> GetByTipoAsync(string tipo);
        Task<IEnumerable<FaunaFloraResponseDto>> GetByCategoriaAsync(string categoria);
        Task<IEnumerable<FaunaFloraResponseDto>> GetByTrilhaAsync(int idTrilha);
        Task<FaunaFloraResponseDto> CreateAsync(CreateFaunaFloraDto createDto);
        Task<FaunaFloraResponseDto> UpdateAsync(int id, UpdateFaunaFloraDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<string>> GetCategoriasAsync();
        Task<EstatisticasFaunaFloraDto> GetEstatisticasAsync();
        Task<IEnumerable<FaunaFloraResponseDto>> SearchAsync(string termo);
        Task<GuiaEspeciesDto> GetGuiaEspeciesAsync(int idTrilha);
    }
}
