using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaitacaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Relatórios de Visita")]
    public class RelatoriosController : ControllerBase
    {
        private readonly IRelatorioVisitaService _relatorioService;

        public RelatoriosController(IRelatorioVisitaService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        /// <summary>
        /// Obter todos os relatórios com filtros opcionais
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RelatorioVisitaResponseDto>>> GetRelatorios(
            [FromQuery] int? idReserva = null,
            [FromQuery] int? idUsuario = null,
            [FromQuery] int? idParque = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            try
            {
                var relatorios = await _relatorioService.GetRelatoriosAsync(idReserva, idUsuario, idParque, dataInicio, dataFim);
                return Ok(relatorios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar relatórios: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter relatório por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RelatorioVisitaResponseDto>> GetRelatorio(int id)
        {
            try
            {
                var relatorio = await _relatorioService.GetRelatorioByIdAsync(id);
                if (relatorio == null)
                    return NotFound(new { message = "Relatório não encontrado" });

                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar relatório: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter relatório por ID da reserva
        /// </summary>
        [HttpGet("reserva/{idReserva}")]
        public async Task<ActionResult<RelatorioVisitaResponseDto>> GetRelatorioPorReserva(int idReserva)
        {
            try
            {
                var relatorio = await _relatorioService.GetRelatorioByReservaAsync(idReserva);
                if (relatorio == null)
                    return NotFound(new { message = "Relatório não encontrado para esta reserva" });

                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar relatório: {ex.Message}" });
            }
        }

        /// <summary>
        /// Criar novo relatório de visita
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RelatorioVisitaResponseDto>> CreateRelatorio(CreateRelatorioVisitaDto createRelatorioDto)
        {
            try
            {
                var relatorio = await _relatorioService.CreateRelatorioAsync(createRelatorioDto);
                return CreatedAtAction(nameof(GetRelatorio), new { id = relatorio.IdRelatorio }, relatorio);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao criar relatório: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atualizar relatório existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<RelatorioVisitaResponseDto>> UpdateRelatorio(int id, UpdateRelatorioVisitaDto updateRelatorioDto)
        {
            try
            {
                var relatorio = await _relatorioService.UpdateRelatorioAsync(id, updateRelatorioDto);
                return Ok(relatorio);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao atualizar relatório: {ex.Message}" });
            }
        }

        /// <summary>
        /// Excluir relatório
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRelatorio(int id)
        {
            try
            {
                var sucesso = await _relatorioService.DeleteRelatorioAsync(id);
                if (!sucesso)
                    return NotFound(new { message = "Relatório não encontrado" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao excluir relatório: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter relatórios de um usuário específico
        /// </summary>
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<RelatorioVisitaResumoDto>>> GetMeusRelatorios(int idUsuario)
        {
            try
            {
                var relatorios = await _relatorioService.GetMeusRelatoriosAsync(idUsuario);
                return Ok(relatorios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar relatórios: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter relatórios de um parque específico
        /// </summary>
        [HttpGet("parque/{idParque}")]
        public async Task<ActionResult<IEnumerable<RelatorioVisitaResponseDto>>> GetRelatoriosPorParque(int idParque)
        {
            try
            {
                var relatorios = await _relatorioService.GetRelatoriosByParqueAsync(idParque);
                return Ok(relatorios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar relatórios: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter estatísticas de relatórios de um parque
        /// </summary>
        [HttpGet("parque/{idParque}/estatisticas")]
        public async Task<ActionResult<EstatisticasRelatorioDto>> GetEstatisticasParque(int idParque)
        {
            try
            {
                var estatisticas = await _relatorioService.GetEstatisticasParqueAsync(idParque);
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar estatísticas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter relatórios que reportaram problemas
        /// </summary>
        [HttpGet("problemas")]
        public async Task<ActionResult<IEnumerable<RelatorioVisitaResponseDto>>> GetRelatoriosComProblemas()
        {
            try
            {
                var relatorios = await _relatorioService.GetRelatoriosComProblemasAsync();
                return Ok(relatorios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar relatórios com problemas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter estatísticas gerais do sistema
        /// </summary>
        [HttpGet("estatisticas")]
        public async Task<ActionResult<EstatisticasGeralDto>> GetEstatisticasGerais()
        {
            try
            {
                var estatisticas = await _relatorioService.GetEstatisticasGeraisAsync();
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar estatísticas gerais: {ex.Message}" });
            }
        }

        /// <summary>
        /// Verificar se é possível criar relatório para uma reserva
        /// </summary>
        [HttpGet("pode-criar/{idReserva}")]
        public async Task<ActionResult<bool>> PodeCriarRelatorio(int idReserva)
        {
            try
            {
                var podeCriar = await _relatorioService.PodeCriarRelatorioAsync(idReserva);
                return Ok(new { podeCriar = podeCriar });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao verificar possibilidade: {ex.Message}" });
            }
        }
    }
}
