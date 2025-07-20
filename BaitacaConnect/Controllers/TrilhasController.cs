using Microsoft.AspNetCore.Mvc;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;

namespace BaitacaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrilhasController : ControllerBase
    {
        private readonly ITrilhaService _trilhaService;

        public TrilhasController(ITrilhaService trilhaService)
        {
            _trilhaService = trilhaService;
        }

        // GET: api/trilhas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrilhaResumoDto>>> GetTrilhas(
            [FromQuery] string? filtroNome, 
            [FromQuery] string? filtroDificuldade, 
            [FromQuery] bool? ativo, 
            [FromQuery] int? idParque)
        {
            var trilhas = await _trilhaService.GetTrilhasAsync(filtroNome, filtroDificuldade, ativo, idParque);
            return Ok(trilhas);
        }

        // GET: api/trilhas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TrilhaResponseDto>> GetTrilha(int id)
        {
            var trilha = await _trilhaService.GetTrilhaByIdAsync(id);

            if (trilha == null)
            {
                return NotFound(new { message = "Trilha não encontrada" });
            }

            return Ok(trilha);
        }

        // GET: api/trilhas/parque/{idParque}
        [HttpGet("parque/{idParque}")]
        public async Task<ActionResult<IEnumerable<TrilhaResumoDto>>> GetTrilhasPorParque(int idParque)
        {
            var trilhas = await _trilhaService.GetTrilhasPorParqueAsync(idParque);
            return Ok(trilhas);
        }

        // GET: api/trilhas/dificuldade/{dificuldade}
        [HttpGet("dificuldade/{dificuldade}")]
        public async Task<ActionResult<IEnumerable<TrilhaResumoDto>>> GetTrilhasPorDificuldade(string dificuldade)
        {
            var trilhas = await _trilhaService.GetTrilhasPorDificuldadeAsync(dificuldade);
            return Ok(trilhas);
        }

        // GET: api/trilhas/disponiveis
        [HttpGet("disponiveis")]
        public async Task<ActionResult<IEnumerable<TrilhaResumoDto>>> GetTrilhasDisponiveis(
            [FromQuery] DateOnly data, 
            [FromQuery] int numeroVisitantes = 1)
        {
            var trilhas = await _trilhaService.GetTrilhasDisponiveisAsync(data, numeroVisitantes);
            return Ok(trilhas);
        }

        // GET: api/trilhas/com-mapa
        [HttpGet("com-mapa")]
        public async Task<ActionResult<IEnumerable<TrilhaComMapaDto>>> GetTrilhasComMapa([FromQuery] int? idParque)
        {
            var trilhas = await _trilhaService.GetTrilhasComMapaAsync(idParque);
            return Ok(trilhas);
        }

        // GET: api/trilhas/{id}/estatisticas
        [HttpGet("{id}/estatisticas")]
        public async Task<ActionResult<EstatisticasTrilhaDto>> GetEstatisticasTrilha(int id)
        {
            if (!await _trilhaService.TrilhaExisteAsync(id))
            {
                return NotFound(new { message = "Trilha não encontrada" });
            }

            try
            {
                var estatisticas = await _trilhaService.GetEstatisticasTrilhaAsync(id);
                return Ok(estatisticas);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/trilhas/{id}/capacidade
        [HttpGet("{id}/capacidade")]
        public async Task<ActionResult<int>> GetCapacidadeTrilha(int id)
        {
            if (!await _trilhaService.TrilhaExisteAsync(id))
            {
                return NotFound(new { message = "Trilha não encontrada" });
            }

            var capacidade = await _trilhaService.GetCapacidadeTrilhaAsync(id);
            return Ok(new { capacidade });
        }

        // POST: api/trilhas/{id}/validar-capacidade
        [HttpPost("{id}/validar-capacidade")]
        public async Task<ActionResult<bool>> ValidarCapacidadeTrilha(int id, [FromBody] ValidarCapacidadeDto validarCapacidadeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _trilhaService.TrilhaExisteAsync(id))
            {
                return NotFound(new { message = "Trilha não encontrada" });
            }

            if (!await _trilhaService.TrilhaAtivaAsync(id))
            {
                return BadRequest(new { message = "Trilha inativa" });
            }

            var capacidadeDisponivel = await _trilhaService.ValidarCapacidadeTrilhaAsync(id, validarCapacidadeDto.NumeroVisitantes);
            return Ok(new { capacidadeDisponivel });
        }

        // POST: api/trilhas
        [HttpPost]
        public async Task<ActionResult<TrilhaResponseDto>> CreateTrilha([FromBody] CreateTrilhaDto createTrilhaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var resultado = await _trilhaService.CriarTrilhaAsync(createTrilhaDto);
                return CreatedAtAction(nameof(GetTrilha), new { id = resultado.IdTrilha }, resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        // PUT: api/trilhas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrilha(int id, [FromBody] UpdateTrilhaDto updateTrilhaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _trilhaService.TrilhaExisteAsync(id))
            {
                return NotFound(new { message = "Trilha não encontrada" });
            }

            try
            {
                var sucesso = await _trilhaService.AtualizarTrilhaAsync(id, updateTrilhaDto);

                if (!sucesso)
                {
                    return StatusCode(500, new { message = "Erro interno do servidor" });
                }

                return Ok(new { message = "Trilha atualizada com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        // POST: api/trilhas/{id}/ativar
        [HttpPost("{id}/ativar")]
        public async Task<IActionResult> AtivarTrilha(int id)
        {
            var sucesso = await _trilhaService.AtivarTrilhaAsync(id);

            if (!sucesso)
            {
                return NotFound(new { message = "Trilha não encontrada" });
            }

            return Ok(new { message = "Trilha ativada com sucesso" });
        }

        // POST: api/trilhas/{id}/desativar
        [HttpPost("{id}/desativar")]
        public async Task<IActionResult> DesativarTrilha(int id)
        {
            var sucesso = await _trilhaService.DesativarTrilhaAsync(id);

            if (!sucesso)
            {
                return NotFound(new { message = "Trilha não encontrada" });
            }

            return Ok(new { message = "Trilha desativada com sucesso" });
        }
    }
}
