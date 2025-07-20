using Microsoft.AspNetCore.Mvc;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;

namespace BaitacaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResumoDto>>> GetUsuarios([FromQuery] string? filtroNome, [FromQuery] string? filtroTipo, [FromQuery] bool? ativo)
        {
            var usuarios = await _usuarioService.GetUsuariosAsync(filtroNome, filtroTipo, ativo);
            return Ok(usuarios);
        }

        // GET: api/usuarios/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            return Ok(usuario);
        }

        // POST: api/usuarios/{id}/validar-senha
        [HttpPost("{id}/validar-senha")]
        public async Task<ActionResult<bool>> ValidarSenha(int id, [FromBody] ValidarSenhaDto validarSenhaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _usuarioService.UsuarioExisteAsync(id))
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            if (!await _usuarioService.UsuarioAtivoAsync(id))
            {
                return BadRequest(new { message = "Usuário inativo" });
            }

            var senhaValida = await _usuarioService.ValidarSenhaAsync(id, validarSenhaDto.Senha);

            return Ok(new { senhaValida });
        }

        // PUT: api/usuarios/{id}/alterar-senha
        [HttpPut("{id}/alterar-senha")]
        public async Task<IActionResult> AlterarSenha(int id, [FromBody] AlterarSenhaDto alterarSenhaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _usuarioService.UsuarioExisteAsync(id))
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            if (!await _usuarioService.UsuarioAtivoAsync(id))
            {
                return BadRequest(new { message = "Usuário inativo" });
            }

            var sucesso = await _usuarioService.AlterarSenhaAsync(id, alterarSenhaDto);

            if (!sucesso)
            {
                return BadRequest(new { message = "Senha atual incorreta" });
            }

            return Ok(new { message = "Senha alterada com sucesso" });
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UpdateUsuarioDto updateUsuarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _usuarioService.UsuarioExisteAsync(id))
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            try
            {
                var sucesso = await _usuarioService.AtualizarUsuarioAsync(id, updateUsuarioDto);

                if (!sucesso)
                {
                    return StatusCode(500, new { message = "Erro interno do servidor" });
                }

                return Ok(new { message = "Usuário atualizado com sucesso" });
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

        // POST: api/usuarios/{id}/ativar
        [HttpPost("{id}/ativar")]
        public async Task<IActionResult> AtivarUsuario(int id)
        {
            var sucesso = await _usuarioService.AtivarUsuarioAsync(id);

            if (!sucesso)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            return Ok(new { message = "Usuário ativado com sucesso" });
        }

        // POST: api/usuarios/{id}/desativar
        [HttpPost("{id}/desativar")]
        public async Task<IActionResult> DesativarUsuario(int id)
        {
            var sucesso = await _usuarioService.DesativarUsuarioAsync(id);

            if (!sucesso)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            return Ok(new { message = "Usuário desativado com sucesso" });
        }
    }
}
