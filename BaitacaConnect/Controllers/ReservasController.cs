using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaitacaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Reservas")]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _reservaService;

        public ReservasController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        /// <summary>
        /// Obter todas as reservas com filtros opcionais (Admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ReservaResponseDto>>> GetReservas(
            [FromQuery] int? idUsuario = null,
            [FromQuery] int? idParque = null,
            [FromQuery] int? idTrilha = null,
            [FromQuery] DateOnly? dataInicio = null,
            [FromQuery] DateOnly? dataFim = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var reservas = await _reservaService.GetReservasAsync(idUsuario, idParque, idTrilha, dataInicio, dataFim, status);
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar reservas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter reserva por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ReservaResponseDto>> GetReserva(int id)
        {
            try
            {
                var reserva = await _reservaService.GetReservaByIdAsync(id);
                if (reserva == null)
                    return NotFound(new { message = "Reserva não encontrada" });

                // Verificar se o usuário pode acessar esta reserva
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (userRole != "Admin" && reserva.IdUsuario.ToString() != userIdClaim)
                    return Forbid();

                return Ok(reserva);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar reserva: {ex.Message}" });
            }
        }

        /// <summary>
        /// Criar nova reserva
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReservaResponseDto>> CreateReserva(CreateReservaDto createReservaDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int idUsuario))
                    return Unauthorized(new { message = "Usuário não identificado" });

                var reserva = await _reservaService.CreateReservaAsync(idUsuario, createReservaDto);
                return CreatedAtAction(nameof(GetReserva), new { id = reserva.IdReserva }, reserva);
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
                return BadRequest(new { message = $"Erro ao criar reserva: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atualizar reserva existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReservaResponseDto>> UpdateReserva(int id, UpdateReservaDto updateReservaDto)
        {
            try
            {
                var reservaExistente = await _reservaService.GetReservaByIdAsync(id);
                if (reservaExistente == null)
                    return NotFound(new { message = "Reserva não encontrada" });

                // Verificar se o usuário pode alterar esta reserva
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (userRole != "Admin" && reservaExistente.IdUsuario.ToString() != userIdClaim)
                    return Forbid();

                var reserva = await _reservaService.UpdateReservaAsync(id, updateReservaDto);
                return Ok(reserva);
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
                return BadRequest(new { message = $"Erro ao atualizar reserva: {ex.Message}" });
            }
        }

        /// <summary>
        /// Excluir reserva
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteReserva(int id)
        {
            try
            {
                var reservaExistente = await _reservaService.GetReservaByIdAsync(id);
                if (reservaExistente == null)
                    return NotFound(new { message = "Reserva não encontrada" });

                // Verificar se o usuário pode excluir esta reserva
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (userRole != "Admin" && reservaExistente.IdUsuario.ToString() != userIdClaim)
                    return Forbid();

                var sucesso = await _reservaService.DeleteReservaAsync(id);
                if (!sucesso)
                    return NotFound(new { message = "Reserva não encontrada" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao excluir reserva: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter minhas reservas (usuário logado)
        /// </summary>
        [HttpGet("minhas")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReservaResumoDto>>> GetMinhasReservas()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int idUsuario))
                    return Unauthorized(new { message = "Usuário não identificado" });

                var reservas = await _reservaService.GetMinhasReservasAsync(idUsuario);
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar reservas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Realizar check-in na reserva
        /// </summary>
        [HttpPost("{id}/checkin")]
        [Authorize]
        public async Task<ActionResult<ReservaResponseDto>> CheckIn(int id)
        {
            try
            {
                var reservaExistente = await _reservaService.GetReservaByIdAsync(id);
                if (reservaExistente == null)
                    return NotFound(new { message = "Reserva não encontrada" });

                // Verificar se o usuário pode fazer check-in nesta reserva
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (userRole != "Admin" && reservaExistente.IdUsuario.ToString() != userIdClaim)
                    return Forbid();

                var reserva = await _reservaService.CheckInAsync(id);
                return Ok(reserva);
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
                return BadRequest(new { message = $"Erro ao realizar check-in: {ex.Message}" });
            }
        }

        /// <summary>
        /// Realizar check-out na reserva
        /// </summary>
        [HttpPost("{id}/checkout")]
        [Authorize]
        public async Task<ActionResult<ReservaResponseDto>> CheckOut(int id)
        {
            try
            {
                var reservaExistente = await _reservaService.GetReservaByIdAsync(id);
                if (reservaExistente == null)
                    return NotFound(new { message = "Reserva não encontrada" });

                // Verificar se o usuário pode fazer check-out nesta reserva
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (userRole != "Admin" && reservaExistente.IdUsuario.ToString() != userIdClaim)
                    return Forbid();

                var reserva = await _reservaService.CheckOutAsync(id);
                return Ok(reserva);
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
                return BadRequest(new { message = $"Erro ao realizar check-out: {ex.Message}" });
            }
        }

        /// <summary>
        /// Cancelar reserva
        /// </summary>
        [HttpPost("{id}/cancelar")]
        [Authorize]
        public async Task<ActionResult<ReservaResponseDto>> CancelarReserva(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int idUsuario))
                    return Unauthorized(new { message = "Usuário não identificado" });

                var reserva = await _reservaService.CancelarReservaAsync(id, idUsuario);
                return Ok(reserva);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao cancelar reserva: {ex.Message}" });
            }
        }

        /// <summary>
        /// Validar disponibilidade para reserva
        /// </summary>
        [HttpPost("validar")]
        [Authorize]
        public async Task<ActionResult<ValidarReservaResponseDto>> ValidarReserva(ValidarReservaDto validarReservaDto)
        {
            try
            {
                var resultado = await _reservaService.ValidarReservaAsync(validarReservaDto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao validar reserva: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obter calendário de reservas por parque (Admin)
        /// </summary>
        [HttpGet("calendario/{idParque}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ReservaCalendarioDto>>> GetCalendarioReservas(
            int idParque,
            [FromQuery] DateOnly dataInicio,
            [FromQuery] DateOnly dataFim)
        {
            try
            {
                if (dataFim < dataInicio)
                    return BadRequest(new { message = "Data fim deve ser maior ou igual à data início" });

                var calendario = await _reservaService.GetCalendarioReservasAsync(idParque, dataInicio, dataFim);
                return Ok(calendario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar calendário: {ex.Message}" });
            }
        }
    }
}
