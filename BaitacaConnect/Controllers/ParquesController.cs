using Microsoft.AspNetCore.Mvc;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;

namespace BaitacaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParquesController : ControllerBase
    {
        private readonly IParqueService _parqueService;

        public ParquesController(IParqueService parqueService)
        {
            _parqueService = parqueService;
        }

        // GET: api/parques
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParqueResumoDto>>> GetParques([FromQuery] string? filtroNome, [FromQuery] bool? ativo)
        {
            var parques = await _parqueService.GetParquesAsync(filtroNome, ativo);
            return Ok(parques);
        }

        // GET: api/parques/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ParqueResponseDto>> GetParque(int id)
        {
            var parque = await _parqueService.GetParqueByIdAsync(id);

            if (parque == null)
            {
                return NotFound(new { message = "Parque não encontrado" });
            }

            return Ok(parque);
        }

        // GET: api/parques/com-trilhas
        [HttpGet("com-trilhas")]
        public async Task<ActionResult<IEnumerable<ParqueComTrilhasDto>>> GetParquesComTrilhas()
        {
            var parques = await _parqueService.GetParquesComTrilhasAsync();
            return Ok(parques);
        }

        // GET: api/parques/{id}/estatisticas
        [HttpGet("{id}/estatisticas")]
        public async Task<ActionResult<EstatisticasParqueDto>> GetEstatisticasParque(int id)
        {
            if (!await _parqueService.ParqueExisteAsync(id))
            {
                return NotFound(new { message = "Parque não encontrado" });
            }

            try
            {
                var estatisticas = await _parqueService.GetEstatisticasParqueAsync(id);
                return Ok(estatisticas);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/parques
        [HttpPost]
        public async Task<ActionResult<ParqueResponseDto>> CreateParque([FromBody] CreateParqueDto createParqueDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var resultado = await _parqueService.CriarParqueAsync(createParqueDto);
                return CreatedAtAction(nameof(GetParque), new { id = resultado.IdParque }, resultado);
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

        // PUT: api/parques/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParque(int id, [FromBody] UpdateParqueDto updateParqueDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _parqueService.ParqueExisteAsync(id))
            {
                return NotFound(new { message = "Parque não encontrado" });
            }

            try
            {
                var sucesso = await _parqueService.AtualizarParqueAsync(id, updateParqueDto);

                if (!sucesso)
                {
                    return StatusCode(500, new { message = "Erro interno do servidor" });
                }

                return Ok(new { message = "Parque atualizado com sucesso" });
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

        // POST: api/parques/{id}/ativar
        [HttpPost("{id}/ativar")]
        public async Task<IActionResult> AtivarParque(int id)
        {
            var sucesso = await _parqueService.AtivarParqueAsync(id);

            if (!sucesso)
            {
                return NotFound(new { message = "Parque não encontrado" });
            }

            return Ok(new { message = "Parque ativado com sucesso" });
        }

        // POST: api/parques/{id}/desativar
        [HttpPost("{id}/desativar")]
        public async Task<IActionResult> DesativarParque(int id)
        {
            var sucesso = await _parqueService.DesativarParqueAsync(id);

            if (!sucesso)
            {
                return NotFound(new { message = "Parque não encontrado" });
            }

            return Ok(new { message = "Parque desativado com sucesso" });
        }

        // GET: api/parques/{id}/capacidade
        [HttpGet("{id}/capacidade")]
        public async Task<ActionResult<int>> GetCapacidadeParque(int id)
        {
            if (!await _parqueService.ParqueExisteAsync(id))
            {
                return NotFound(new { message = "Parque não encontrado" });
            }

            var capacidade = await _parqueService.GetCapacidadeParqueAsync(id);
            return Ok(new { capacidade });
        }
    }
}
