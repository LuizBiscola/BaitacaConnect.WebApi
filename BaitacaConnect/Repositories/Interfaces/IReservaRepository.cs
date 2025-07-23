using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Repositories.Interfaces
{
    public interface IReservaRepository
    {
        Task<IEnumerable<Reserva>> GetReservasAsync(int? idUsuario = null, int? idParque = null, int? idTrilha = null, 
            DateOnly? dataInicio = null, DateOnly? dataFim = null, string? status = null);
        Task<Reserva?> GetReservaByIdAsync(int idReserva);
        Task<Reserva> CreateReservaAsync(Reserva reserva);
        Task<Reserva> UpdateReservaAsync(Reserva reserva);
        Task<bool> DeleteReservaAsync(int idReserva);
        Task<IEnumerable<Reserva>> GetReservasByUsuarioAsync(int idUsuario);
        Task<IEnumerable<Reserva>> GetReservasAtivasAsync();
        Task<int> GetNumeroVisitantesPorDataAsync(int idParque, DateOnly data, int? idTrilha = null);
        Task<bool> ExisteReservaAtivaAsync(int idUsuario, int idParque, DateOnly data);
        Task<IEnumerable<Reserva>> GetReservasPorCalendarioAsync(int idParque, DateOnly dataInicio, DateOnly dataFim);
        Task<bool> ReservaExisteAsync(int idReserva);
        Task<Reserva?> GetReservaComDetalhesAsync(int idReserva);
    }
}
