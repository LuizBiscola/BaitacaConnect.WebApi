using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaitacaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Fauna e Flora")]
    public class FaunaFloraController : ControllerBase
    {
        private readonly IFaunaFloraService _faunaFloraService;

        public FaunaFloraController(IFaunaFloraService faunaFloraService)
        {
            _faunaFloraService = faunaFloraService;
        }

        /// <summary>
        /// Obter todas as espécies de fauna e flora
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FaunaFloraResponseDto>>> GetEspecies(
            [FromQuery] string? nome = null,
            [FromQuery] string? tipo = null,
            [FromQuery] string? categoria = null,
            [FromQuery] int? idTrilha = null)
        {
            try
            {
                var especies = await _faunaFloraService.GetWithFiltersAsync(nome, tipo, categoria, idTrilha);
                return Ok(especies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar espécies: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter espécie por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FaunaFloraResponseDto>> GetEspecie(int id)
        {
            try
            {
                var especie = await _faunaFloraService.GetByIdAsync(id);
                if (especie == null)
                    return NotFound(new { message = "Espécie não encontrada" });

                return Ok(especie);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar espécie: {ex.Message}" });
            }
        }

        /// <summary>
        /// Criar nova espécie
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<FaunaFloraResponseDto>> CreateEspecie(CreateFaunaFloraDto createDto)
        {
            try
            {
                var especie = await _faunaFloraService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetEspecie), new { id = especie.IdFaunaFlora }, especie);
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
                return BadRequest(new { message = $"Erro ao criar espécie: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atualizar espécie existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<FaunaFloraResponseDto>> UpdateEspecie(int id, UpdateFaunaFloraDto updateDto)
        {
            try
            {
                var especie = await _faunaFloraService.UpdateAsync(id, updateDto);
                return Ok(especie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao atualizar espécie: {ex.Message}" });
            }
        }

        /// <summary>
        /// Excluir espécie
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEspecie(int id)
        {
            try
            {
                var sucesso = await _faunaFloraService.DeleteAsync(id);
                if (!sucesso)
                    return NotFound(new { message = "Espécie não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao excluir espécie: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter espécies por tipo (fauna ou flora)
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<FaunaFloraResponseDto>>> GetEspeciesPorTipo(string tipo)
        {
            try
            {
                var especies = await _faunaFloraService.GetByTipoAsync(tipo);
                return Ok(especies);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar espécies por tipo: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter espécies por categoria
        /// </summary>
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<FaunaFloraResponseDto>>> GetEspeciesPorCategoria(string categoria)
        {
            try
            {
                var especies = await _faunaFloraService.GetByCategoriaAsync(categoria);
                return Ok(especies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar espécies por categoria: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter espécies encontradas em uma trilha específica
        /// </summary>
        [HttpGet("trilha/{idTrilha}")]
        public async Task<ActionResult<IEnumerable<FaunaFloraResponseDto>>> GetEspeciesPorTrilha(int idTrilha)
        {
            try
            {
                var especies = await _faunaFloraService.GetByTrilhaAsync(idTrilha);
                return Ok(especies);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar espécies por trilha: {ex.Message}" });
            }
        }

        /// <summary>
        /// Buscar espécies por termo
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<FaunaFloraResponseDto>>> BuscarEspecies([FromQuery] string termo)
        {
            try
            {
                var especies = await _faunaFloraService.SearchAsync(termo);
                return Ok(especies);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar espécies: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter todas as categorias disponíveis
        /// </summary>
        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategorias()
        {
            try
            {
                var categorias = await _faunaFloraService.GetCategoriasAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar categorias: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter estatísticas gerais de fauna e flora
        /// </summary>
        [HttpGet("estatisticas")]
        public async Task<ActionResult<EstatisticasFaunaFloraDto>> GetEstatisticas()
        {
            try
            {
                var estatisticas = await _faunaFloraService.GetEstatisticasAsync();
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar estatísticas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter guia de espécies para uma trilha específica
        /// </summary>
        [HttpGet("guia-trilha/{idTrilha}")]
        public async Task<ActionResult<GuiaEspeciesDto>> GetGuiaEspecies(int idTrilha)
        {
            try
            {
                var guia = await _faunaFloraService.GetGuiaEspeciesAsync(idTrilha);
                return Ok(guia);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao gerar guia de espécies: {ex.Message}" });
            }
        }
    }
}
