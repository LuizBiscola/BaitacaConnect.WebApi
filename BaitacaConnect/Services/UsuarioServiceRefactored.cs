using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using BaitacaConnect.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BaitacaConnect.Services
{
    public class UsuarioServiceRefactored : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioServiceRefactored(IUsuarioRepository usuarioRepository)
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
            });
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

        public async Task<bool> ValidarSenhaAsync(int id, string senha)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            
            if (usuario == null || !usuario.Ativo)
                return false;

            return VerificarSenha(senha, usuario.SenhaUsuario);
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _usuarioRepository.EmailExistsAsync(email);
        }

        public async Task<bool> UsuarioExisteAsync(int id)
        {
            return await _usuarioRepository.ExistsAsync(id);
        }

        public async Task<bool> UsuarioAtivoAsync(int id)
        {
            return await _usuarioRepository.IsActiveAsync(id);
        }

        // Métodos auxiliares permanecem no service (lógica de negócio)
        private string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerificarSenha(string senha, string? senhaHash)
        {
            if (string.IsNullOrEmpty(senhaHash))
                return false;

            var hashSenhaInput = HashSenha(senha);
            return hashSenhaInput == senhaHash;
        }

        // ... outros métodos da interface seguem o mesmo padrão
        public Task<UsuarioResponseDto?> GetUsuarioByEmailAsync(string email) => throw new NotImplementedException();
        public Task<bool> ValidarSenhaByEmailAsync(string email, string senha) => throw new NotImplementedException();
        public Task<bool> AtualizarUsuarioAsync(int id, UpdateUsuarioDto updateUsuarioDto) => throw new NotImplementedException();
        public Task<bool> AlterarSenhaAsync(int id, AlterarSenhaDto alterarSenhaDto) => throw new NotImplementedException();
        public Task<bool> AlterarSenhaByEmailAsync(AlterarSenhaComEmailDto alterarSenhaDto) => throw new NotImplementedException();
        public Task<bool> AtivarUsuarioAsync(int id) => throw new NotImplementedException();
        public Task<bool> DesativarUsuarioAsync(int id) => throw new NotImplementedException();
    }
}
