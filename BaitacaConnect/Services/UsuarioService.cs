using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using BaitacaConnect.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BaitacaConnect.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<UsuarioResumoDto>> GetUsuariosAsync(string? filtroNome, string? filtroTipo, bool? ativo)
        {
            var usuarios = await _usuarioRepository.GetWithFiltersAsync(filtroNome, filtroTipo, ativo);

            return usuarios.Select(u => new UsuarioResumoDto
            {
                IdUsuario = u.IdUsuario,
                NomeUsuario = u.NomeUsuario,
                EmailUsuario = u.EmailUsuario,
                TipoUsuario = u.TipoUsuario,
                Ativo = u.Ativo,
                DataCriacao = u.DataCriacao
            }).ToList();
        }

        public async Task<UsuarioResponseDto?> GetUsuarioByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                return null;

            return new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                NomeUsuario = usuario.NomeUsuario,
                EmailUsuario = usuario.EmailUsuario,
                TelefoneUsuario = usuario.TelefoneUsuario,
                TipoUsuario = usuario.TipoUsuario,
                DataCriacao = usuario.DataCriacao,
                Ativo = usuario.Ativo,
                IdadeUsuario = usuario.IdadeUsuario,
                TotalReservas = usuario.Reservas.Count,
                ReservasAtivas = usuario.Reservas.Count(r => r.Status == "ativa")
            };
        }

        public async Task<UsuarioResponseDto?> GetUsuarioByEmailAsync(string email)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);

            if (usuario == null)
                return null;

            return new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                NomeUsuario = usuario.NomeUsuario,
                EmailUsuario = usuario.EmailUsuario,
                TelefoneUsuario = usuario.TelefoneUsuario,
                TipoUsuario = usuario.TipoUsuario,
                DataCriacao = usuario.DataCriacao,
                Ativo = usuario.Ativo,
                IdadeUsuario = usuario.IdadeUsuario,
                TotalReservas = usuario.Reservas.Count,
                ReservasAtivas = usuario.Reservas.Count(r => r.Status == "ativa")
            };
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _usuarioRepository.EmailExistsAsync(email);
        }

        public async Task<bool> ValidarSenhaAsync(int id, string senha)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null || !usuario.Ativo || string.IsNullOrEmpty(usuario.SenhaUsuario))
                return false;

            return VerificarSenha(senha, usuario.SenhaUsuario);
        }

        public async Task<bool> ValidarSenhaByEmailAsync(string email, string senha)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);

            if (usuario == null || !usuario.Ativo || string.IsNullOrEmpty(usuario.SenhaUsuario))
                return false;

            return VerificarSenha(senha, usuario.SenhaUsuario);
        }

        public async Task<UsuarioResponseDto> CriarUsuarioAsync(CreateUsuarioDto createUsuarioDto)
        {
            if (await _usuarioRepository.EmailExistsAsync(createUsuarioDto.EmailUsuario))
            {
                throw new InvalidOperationException("Email já está em uso");
            }

            var usuario = new Usuario
            {
                NomeUsuario = createUsuarioDto.NomeUsuario,
                EmailUsuario = createUsuarioDto.EmailUsuario,
                TelefoneUsuario = createUsuarioDto.TelefoneUsuario,
                TipoUsuario = createUsuarioDto.TipoUsuario,
                IdadeUsuario = createUsuarioDto.IdadeUsuario,
                SenhaUsuario = HashSenha(createUsuarioDto.SenhaUsuario),
                DataCriacao = DateTime.Now,
                Ativo = true
            };

            var usuarioCriado = await _usuarioRepository.CreateAsync(usuario);

            return new UsuarioResponseDto
            {
                IdUsuario = usuarioCriado.IdUsuario,
                NomeUsuario = usuarioCriado.NomeUsuario,
                EmailUsuario = usuarioCriado.EmailUsuario,
                TelefoneUsuario = usuarioCriado.TelefoneUsuario,
                TipoUsuario = usuarioCriado.TipoUsuario,
                DataCriacao = usuarioCriado.DataCriacao,
                Ativo = usuarioCriado.Ativo,
                IdadeUsuario = usuarioCriado.IdadeUsuario,
                TotalReservas = 0,
                ReservasAtivas = 0
            };
        }

        public async Task<bool> AtualizarUsuarioAsync(int id, UpdateUsuarioDto updateUsuarioDto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                return false;

            if (!string.IsNullOrEmpty(updateUsuarioDto.EmailUsuario) && 
                updateUsuarioDto.EmailUsuario != usuario.EmailUsuario)
            {
                if (await _usuarioRepository.EmailExistsAsync(updateUsuarioDto.EmailUsuario))
                {
                    throw new InvalidOperationException("Email já está em uso");
                }
            }

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrEmpty(updateUsuarioDto.NomeUsuario))
                usuario.NomeUsuario = updateUsuarioDto.NomeUsuario;
            
            if (!string.IsNullOrEmpty(updateUsuarioDto.EmailUsuario))
                usuario.EmailUsuario = updateUsuarioDto.EmailUsuario;
            
            if (!string.IsNullOrEmpty(updateUsuarioDto.TelefoneUsuario))
                usuario.TelefoneUsuario = updateUsuarioDto.TelefoneUsuario;
            
            if (!string.IsNullOrEmpty(updateUsuarioDto.TipoUsuario))
                usuario.TipoUsuario = updateUsuarioDto.TipoUsuario;
            
            if (updateUsuarioDto.IdadeUsuario.HasValue)
                usuario.IdadeUsuario = updateUsuarioDto.IdadeUsuario;
            
            if (updateUsuarioDto.Ativo.HasValue)
                usuario.Ativo = updateUsuarioDto.Ativo.Value;

            try
            {
                await _usuarioRepository.UpdateAsync(usuario);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AtualizarSenhaAsync(int id, string senhaAtual, string novaSenha)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null || string.IsNullOrEmpty(usuario.SenhaUsuario) || !VerificarSenha(senhaAtual, usuario.SenhaUsuario))
                return false;

            usuario.SenhaUsuario = HashSenha(novaSenha);

            try
            {
                await _usuarioRepository.UpdateAsync(usuario);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AlterarSenhaAsync(int id, AlterarSenhaDto alterarSenhaDto)
        {
            return await AtualizarSenhaAsync(id, alterarSenhaDto.SenhaAtual, alterarSenhaDto.NovaSenha);
        }

        public async Task<bool> AlterarSenhaByEmailAsync(AlterarSenhaComEmailDto alterarSenhaDto)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(alterarSenhaDto.EmailUsuario);

            if (usuario == null || string.IsNullOrEmpty(usuario.SenhaUsuario) || !VerificarSenha(alterarSenhaDto.SenhaAtual, usuario.SenhaUsuario))
                return false;

            usuario.SenhaUsuario = HashSenha(alterarSenhaDto.NovaSenha);

            try
            {
                await _usuarioRepository.UpdateAsync(usuario);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AtivarUsuarioAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                return false;

            usuario.Ativo = true;

            try
            {
                await _usuarioRepository.UpdateAsync(usuario);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DesativarUsuarioAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                return false;

            usuario.Ativo = false;

            try
            {
                await _usuarioRepository.UpdateAsync(usuario);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UsuarioExisteAsync(int id)
        {
            return await _usuarioRepository.ExistsAsync(id);
        }

        public async Task<bool> UsuarioAtivoAsync(int id)
        {
            return await _usuarioRepository.IsActiveAsync(id);
        }

        // Métodos auxiliares para criptografia
        private string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerificarSenha(string senha, string hashSenha)
        {
            var hashSenhaInput = HashSenha(senha);
            return hashSenhaInput == hashSenha;
        }
    }
}
