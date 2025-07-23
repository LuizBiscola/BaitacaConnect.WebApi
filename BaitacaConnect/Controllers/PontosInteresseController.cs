using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaitacaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Pontos de Interesse")]
    public class PontosInteresseController : ControllerBase
    {
        private readonly IPontoInteresseService _pontoInteresseService;

        public PontosInteresseController(IPontoInteresseService pontoInteresseService)
        {
            _pontoInteresseService = pontoInteresseService;
        }

        /// <summary>
        /// Obter todos os pontos de interesse com filtros opcionais
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PontoInteresseResponseDto>>> GetPontosInteresse(
            [FromQuery] int? idParque = null,
            [FromQuery] int? idTrilha = null,
            [FromQuery] string? tipo = null,
            [FromQuery] string? nome = null)
        {
            try
            {
                var pontos = await _pontoInteresseService.GetWithFiltersAsync(idParque, idTrilha, tipo, nome);
                return Ok(pontos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar pontos de interesse: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter ponto de interesse específico
        /// </summary>
        [HttpGet("{idParque}/{idTrilha}/{nomePonto}")]
        public async Task<ActionResult<PontoInteresseResponseDto>> GetPontoInteresse(int idParque, int idTrilha, string nomePonto)
        {
            try
            {
                var ponto = await _pontoInteresseService.GetByIdAsync(idParque, idTrilha, nomePonto);
                if (ponto == null)
                    return NotFound(new { message = "Ponto de interesse não encontrado" });

                return Ok(ponto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar ponto de interesse: {ex.Message}" });
            }
        }

        /// <summary>
        /// Criar novo ponto de interesse
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PontoInteresseResponseDto>> CreatePontoInteresse(CreatePontoInteresseDto createDto)
        {
            try
            {
                var ponto = await _pontoInteresseService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetPontoInteresse), 
                    new { idParque = ponto.IdParque, idTrilha = ponto.IdTrilha, nomePonto = ponto.NomePontoInteresse }, 
                    ponto);
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
                return BadRequest(new { message = $"Erro ao criar ponto de interesse: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atualizar ponto de interesse existente
        /// </summary>
        [HttpPut("{idParque}/{idTrilha}/{nomePonto}")]
        public async Task<ActionResult<PontoInteresseResponseDto>> UpdatePontoInteresse(
            int idParque, int idTrilha, string nomePonto, UpdatePontoInteresseDto updateDto)
        {
            try
            {
                var ponto = await _pontoInteresseService.UpdateAsync(idParque, idTrilha, nomePonto, updateDto);
                return Ok(ponto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao atualizar ponto de interesse: {ex.Message}" });
            }
        }

        /// <summary>
        /// Excluir ponto de interesse
        /// </summary>
        [HttpDelete("{idParque}/{idTrilha}/{nomePonto}")]
        public async Task<ActionResult> DeletePontoInteresse(int idParque, int idTrilha, string nomePonto)
        {
            try
            {
                var sucesso = await _pontoInteresseService.DeleteAsync(idParque, idTrilha, nomePonto);
                if (!sucesso)
                    return NotFound(new { message = "Ponto de interesse não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao excluir ponto de interesse: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter pontos de interesse de uma trilha específica
        /// </summary>
        [HttpGet("trilha/{idTrilha}")]
        public async Task<ActionResult<IEnumerable<PontoInteresseResponseDto>>> GetPontosPorTrilha(int idTrilha)
        {
            try
            {
                var pontos = await _pontoInteresseService.GetByTrilhaAsync(idTrilha);
                return Ok(pontos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar pontos da trilha: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter pontos de interesse de um parque específico
        /// </summary>
        [HttpGet("parque/{idParque}")]
        public async Task<ActionResult<IEnumerable<PontoInteresseResponseDto>>> GetPontosPorParque(int idParque)
        {
            try
            {
                var pontos = await _pontoInteresseService.GetByParqueAsync(idParque);
                return Ok(pontos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar pontos do parque: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter mapa da trilha com todos os pontos de interesse
        /// </summary>
        [HttpGet("mapa-trilha/{idTrilha}")]
        public async Task<ActionResult<MapaTrilhaDto>> GetMapaTrilha(int idTrilha)
        {
            try
            {
                var mapa = await _pontoInteresseService.GetMapaTrilhaAsync(idTrilha);
                return Ok(mapa);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao gerar mapa da trilha: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter todos os tipos de pontos de interesse disponíveis
        /// </summary>
        [HttpGet("tipos")]
        public async Task<ActionResult<IEnumerable<string>>> GetTipos()
        {
            try
            {
                var tipos = await _pontoInteresseService.GetTiposAsync();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar tipos: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter estatísticas gerais dos pontos de interesse
        /// </summary>
        [HttpGet("estatisticas")]
        public async Task<ActionResult<EstatisticasPontosDto>> GetEstatisticas()
        {
            try
            {
                var estatisticas = await _pontoInteresseService.GetEstatisticasAsync();
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar estatísticas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Buscar pontos de interesse próximos a uma coordenada
        /// </summary>
        [HttpGet("proximos")]
        public async Task<ActionResult<IEnumerable<PontoInteresseResponseDto>>> GetPontosProximos(
            [FromQuery] string coordenadas, [FromQuery] double raioKm = 1.0)
        {
            try
            {
                var pontos = await _pontoInteresseService.GetPontosProximosAsync(coordenadas, raioKm);
                return Ok(pontos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar pontos próximos: {ex.Message}" });
            }
        }

        /// <summary>
        /// Reordenar pontos de interesse em uma trilha
        /// </summary>
        [HttpPut("trilha/{idTrilha}/reordenar")]
        public async Task<ActionResult> ReordenarPontos(int idTrilha, List<ReordenarPontoDto> novaOrdem)
        {
            try
            {
                var sucesso = await _pontoInteresseService.ReordenarPontosAsync(idTrilha, novaOrdem);
                if (!sucesso)
                    return BadRequest(new { message = "Erro ao reordenar pontos" });

                return Ok(new { message = "Pontos reordenados com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao reordenar pontos: {ex.Message}" });
            }
        }
    }
}
