using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Repositories.Interfaces;
using BaitacaConnect.Services.Interfaces;

namespace BaitacaConnect.Services
{
    public class RelatorioVisitaService : IRelatorioVisitaService
    {
        private readonly IRelatorioVisitaRepository _relatorioRepository;
        private readonly IReservaRepository _reservaRepository;

        public RelatorioVisitaService(
            IRelatorioVisitaRepository relatorioRepository,
            IReservaRepository reservaRepository)
        {
            _relatorioRepository = relatorioRepository;
            _reservaRepository = reservaRepository;
        }

        public async Task<IEnumerable<RelatorioVisitaResponseDto>> GetRelatoriosAsync(int? idReserva = null, 
            int? idUsuario = null, int? idParque = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var relatorios = await _relatorioRepository.GetRelatoriosAsync(idReserva, idUsuario, idParque, dataInicio, dataFim);
            return relatorios.Select(MapToRelatorioResponseDto);
        }

        public async Task<RelatorioVisitaResponseDto?> GetRelatorioByIdAsync(int idRelatorio)
        {
            var relatorio = await _relatorioRepository.GetRelatorioByIdAsync(idRelatorio);
            return relatorio != null ? MapToRelatorioResponseDto(relatorio) : null;
        }

        public async Task<RelatorioVisitaResponseDto?> GetRelatorioByReservaAsync(int idReserva)
        {
            var relatorio = await _relatorioRepository.GetRelatorioByReservaAsync(idReserva);
            return relatorio != null ? MapToRelatorioResponseDto(relatorio) : null;
        }

        public async Task<RelatorioVisitaResponseDto> CreateRelatorioAsync(CreateRelatorioVisitaDto createRelatorioDto)
        {
            // Validar se a reserva existe
            var reserva = await _reservaRepository.GetReservaByIdAsync(createRelatorioDto.IdReserva);
            if (reserva == null)
                throw new ArgumentException("Reserva não encontrada");

            // Validar se a reserva foi finalizada (teve check-out)
            if (!reserva.CheckOut.HasValue)
                throw new InvalidOperationException("Só é possível criar relatório após finalizar a visita (check-out)");

            // Verificar se já existe relatório para esta reserva
            var relatorioExistente = await _relatorioRepository.ExisteRelatorioParaReservaAsync(createRelatorioDto.IdReserva);
            if (relatorioExistente)
                throw new InvalidOperationException("Já existe um relatório para esta reserva");

            // Criar o relatório
            var relatorio = new RelatorioVisita
            {
                IdReserva = createRelatorioDto.IdReserva,
                Avaliacao = createRelatorioDto.Avaliacao,
                Comentarios = createRelatorioDto.Comentarios,
                ProblemasEncontrados = createRelatorioDto.ProblemasEncontrados,
                DataRelatorio = DateTime.Now
            };

            var relatorioCriado = await _relatorioRepository.CreateRelatorioAsync(relatorio);
            var relatorioCompleto = await _relatorioRepository.GetRelatorioByIdAsync(relatorioCriado.IdRelatorio);
            
            return MapToRelatorioResponseDto(relatorioCompleto!);
        }

        public async Task<RelatorioVisitaResponseDto> UpdateRelatorioAsync(int idRelatorio, UpdateRelatorioVisitaDto updateRelatorioDto)
        {
            var relatorio = await _relatorioRepository.GetRelatorioByIdAsync(idRelatorio);
            if (relatorio == null)
                throw new ArgumentException("Relatório não encontrado");

            // Verificar se o relatório foi criado nas últimas 24 horas (permite edição)
            var prazoEdicao = relatorio.DataRelatorio.AddHours(24);
            if (DateTime.Now > prazoEdicao)
                throw new InvalidOperationException("Relatório só pode ser editado nas primeiras 24 horas após criação");

            // Aplicar as alterações
            if (updateRelatorioDto.Avaliacao.HasValue)
                relatorio.Avaliacao = updateRelatorioDto.Avaliacao.Value;

            if (!string.IsNullOrEmpty(updateRelatorioDto.Comentarios))
                relatorio.Comentarios = updateRelatorioDto.Comentarios;

            if (!string.IsNullOrEmpty(updateRelatorioDto.ProblemasEncontrados))
                relatorio.ProblemasEncontrados = updateRelatorioDto.ProblemasEncontrados;

            var relatorioAtualizado = await _relatorioRepository.UpdateRelatorioAsync(relatorio);
            return MapToRelatorioResponseDto(relatorioAtualizado);
        }

        public async Task<bool> DeleteRelatorioAsync(int idRelatorio)
        {
            var relatorio = await _relatorioRepository.GetRelatorioByIdAsync(idRelatorio);
            if (relatorio == null)
                return false;

            // Verificar se o relatório foi criado nas últimas 24 horas (permite exclusão)
            var prazoEdicao = relatorio.DataRelatorio.AddHours(24);
            if (DateTime.Now > prazoEdicao)
                throw new InvalidOperationException("Relatório só pode ser excluído nas primeiras 24 horas após criação");

            return await _relatorioRepository.DeleteRelatorioAsync(idRelatorio);
        }

        public async Task<IEnumerable<RelatorioVisitaResumoDto>> GetMeusRelatoriosAsync(int idUsuario)
        {
            var relatorios = await _relatorioRepository.GetRelatoriosByUsuarioAsync(idUsuario);
            return relatorios.Select(r => new RelatorioVisitaResumoDto
            {
                IdRelatorio = r.IdRelatorio,
                IdReserva = r.IdReserva,
                NomeParque = r.Reserva.Parque.NomeParque,
                NomeTrilha = r.Reserva.Trilha?.NomeTrilha,
                DataVisita = r.Reserva.DataVisita,
                Avaliacao = r.Avaliacao,
                DataRelatorio = r.DataRelatorio,
                TemProblemas = !string.IsNullOrEmpty(r.ProblemasEncontrados)
            });
        }

        public async Task<IEnumerable<RelatorioVisitaResponseDto>> GetRelatoriosByParqueAsync(int idParque)
        {
            var relatorios = await _relatorioRepository.GetRelatoriosByParqueAsync(idParque);
            return relatorios.Select(MapToRelatorioResponseDto);
        }

        public async Task<EstatisticasRelatorioDto> GetEstatisticasParqueAsync(int idParque)
        {
            var relatorios = await _relatorioRepository.GetRelatoriosByParqueAsync(idParque);
            var avaliacaoMedia = await _relatorioRepository.GetAvaliacaoMediaParqueAsync(idParque);
            var estatisticasAvaliacoes = await _relatorioRepository.GetEstatisticasAvaliacoesAsync(idParque);

            return new EstatisticasRelatorioDto
            {
                TotalRelatorios = relatorios.Count(),
                AvaliacaoMedia = Math.Round(avaliacaoMedia, 2),
                RelatoriosComProblemas = relatorios.Count(r => !string.IsNullOrEmpty(r.ProblemasEncontrados)),
                DistribuicaoAvaliacoes = estatisticasAvaliacoes,
                UltimoRelatorio = relatorios.OrderByDescending(r => r.DataRelatorio).FirstOrDefault()?.DataRelatorio
            };
        }

        public async Task<IEnumerable<RelatorioVisitaResponseDto>> GetRelatoriosComProblemasAsync()
        {
            var relatorios = await _relatorioRepository.GetRelatoriosComProblemasAsync();
            return relatorios.Select(MapToRelatorioResponseDto);
        }

        public async Task<EstatisticasGeralDto> GetEstatisticasGeraisAsync()
        {
            var totalRelatorios = await _relatorioRepository.GetTotalRelatoriosAsync();
            var relatoriosComProblemas = await _relatorioRepository.GetRelatoriosComProblemasAsync();
            var estatisticasAvaliacoes = await _relatorioRepository.GetEstatisticasAvaliacoesAsync();

            return new EstatisticasGeralDto
            {
                TotalRelatorios = totalRelatorios,
                RelatoriosComProblemas = relatoriosComProblemas.Count(),
                PercentualProblemas = totalRelatorios > 0 ? Math.Round((double)relatoriosComProblemas.Count() / totalRelatorios * 100, 2) : 0,
                DistribuicaoAvaliacoes = estatisticasAvaliacoes,
                AvaliacaoMediaGeral = estatisticasAvaliacoes.Any() ? 
                    Math.Round(estatisticasAvaliacoes.Sum(kv => kv.Key * kv.Value) / (double)estatisticasAvaliacoes.Sum(kv => kv.Value), 2) : 0
            };
        }

        public async Task<bool> PodeCriarRelatorioAsync(int idReserva)
        {
            // Verificar se a reserva existe
            var reserva = await _reservaRepository.GetReservaByIdAsync(idReserva);
            if (reserva == null)
                return false;

            // Verificar se teve check-out
            if (!reserva.CheckOut.HasValue)
                return false;

            // Verificar se já existe relatório
            var relatorioExistente = await _relatorioRepository.ExisteRelatorioParaReservaAsync(idReserva);
            return !relatorioExistente;
        }

        private static RelatorioVisitaResponseDto MapToRelatorioResponseDto(RelatorioVisita relatorio)
        {
            return new RelatorioVisitaResponseDto
            {
                IdRelatorio = relatorio.IdRelatorio,
                IdReserva = relatorio.IdReserva,
                IdUsuario = relatorio.Reserva.IdUsuario,
                NomeUsuario = relatorio.Reserva.Usuario.NomeUsuario,
                IdParque = relatorio.Reserva.IdParque,
                NomeParque = relatorio.Reserva.Parque.NomeParque,
                IdTrilha = relatorio.Reserva.IdTrilha,
                NomeTrilha = relatorio.Reserva.Trilha?.NomeTrilha,
                DataVisita = relatorio.Reserva.DataVisita,
                Avaliacao = relatorio.Avaliacao,
                Comentarios = relatorio.Comentarios,
                ProblemasEncontrados = relatorio.ProblemasEncontrados,
                DataRelatorio = relatorio.DataRelatorio,
                PodeEditar = DateTime.Now <= relatorio.DataRelatorio.AddHours(24)
            };
        }
    }
}
