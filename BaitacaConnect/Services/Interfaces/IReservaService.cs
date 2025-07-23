using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Services.Interfaces
{
    public interface IReservaService
    {
        Task<IEnumerable<ReservaResponseDto>> GetReservasAsync(int? idUsuario = null, int? idParque = null, 
            int? idTrilha = null, DateOnly? dataInicio = null, DateOnly? dataFim = null, string? status = null);
        Task<ReservaResponseDto?> GetReservaByIdAsync(int idReserva);
        Task<ReservaResponseDto> CreateReservaAsync(int idUsuario, CreateReservaDto createReservaDto);
        Task<ReservaResponseDto> UpdateReservaAsync(int idReserva, UpdateReservaDto updateReservaDto);
        Task<bool> DeleteReservaAsync(int idReserva);
        Task<IEnumerable<ReservaResumoDto>> GetMinhasReservasAsync(int idUsuario);
        Task<ReservaResponseDto> CheckInAsync(int idReserva);
        Task<ReservaResponseDto> CheckOutAsync(int idReserva);
        Task<ValidarReservaResponseDto> ValidarReservaAsync(ValidarReservaDto validarReservaDto);
        Task<IEnumerable<ReservaCalendarioDto>> GetCalendarioReservasAsync(int idParque, DateOnly dataInicio, DateOnly dataFim);
        Task<ReservaResponseDto> CancelarReservaAsync(int idReserva, int idUsuario);
        Task<bool> PodeRealizarReservaAsync(int idUsuario, int idParque, DateOnly dataVisita);
    }
}
