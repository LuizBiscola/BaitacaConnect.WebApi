using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Repositories.Interfaces
{
    public interface IRelatorioVisitaRepository
    {
        Task<IEnumerable<RelatorioVisita>> GetRelatoriosAsync(int? idReserva = null, int? idUsuario = null, 
            int? idParque = null, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<RelatorioVisita?> GetRelatorioByIdAsync(int idRelatorio);
        Task<RelatorioVisita?> GetRelatorioByReservaAsync(int idReserva);
        Task<RelatorioVisita> CreateRelatorioAsync(RelatorioVisita relatorio);
        Task<RelatorioVisita> UpdateRelatorioAsync(RelatorioVisita relatorio);
        Task<bool> DeleteRelatorioAsync(int idRelatorio);
        Task<bool> ExisteRelatorioParaReservaAsync(int idReserva);
        Task<IEnumerable<RelatorioVisita>> GetRelatoriosByUsuarioAsync(int idUsuario);
        Task<IEnumerable<RelatorioVisita>> GetRelatoriosByParqueAsync(int idParque);
        Task<double> GetAvaliacaoMediaParqueAsync(int idParque);
        Task<IEnumerable<RelatorioVisita>> GetRelatoriosComProblemasAsync();
        Task<int> GetTotalRelatoriosAsync();
        Task<Dictionary<int, int>> GetEstatisticasAvaliacoesAsync(int? idParque = null);
    }
}
