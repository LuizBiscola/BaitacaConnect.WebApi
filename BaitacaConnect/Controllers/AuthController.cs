using Microsoft.AspNetCore.Mvc;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;

namespace BaitacaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _authService.LoginAsync(loginDto);

            if (resultado == null)
            {
                return BadRequest(new { message = "Email ou senha inválidos" });
            }

            return Ok(resultado);
        }

        // POST: api/auth/registro
        [HttpPost("registro")]
        public async Task<ActionResult<UsuarioResponseDto>> Registro([FromBody] CreateUsuarioDto createUsuarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var resultado = await _authService.RegistroAsync(createUsuarioDto);
                return CreatedAtAction("GetUsuario", "Usuarios", new { id = resultado.IdUsuario }, resultado);
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

        // POST: api/auth/validar-senha
        [HttpPost("validar-senha")]
        public async Task<ActionResult<bool>> ValidarSenha([FromBody] ValidarSenhaComEmailDto validarSenhaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var senhaValida = await _authService.ValidarSenhaAsync(validarSenhaDto);

            if (!senhaValida)
            {
                return BadRequest(new { message = "Usuário não encontrado ou inativo" });
            }

            return Ok(new { senhaValida });
        }

        // POST: api/auth/alterar-senha
        [HttpPost("alterar-senha")]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaComEmailDto alterarSenhaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sucesso = await _authService.AlterarSenhaAsync(alterarSenhaDto);

            if (!sucesso)
            {
                return BadRequest(new { message = "Usuário não encontrado, inativo ou senha atual incorreta" });
            }

            return Ok(new { message = "Senha alterada com sucesso" });
        }
    }
}
