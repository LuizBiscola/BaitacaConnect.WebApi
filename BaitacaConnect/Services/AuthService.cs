using Microsoft.EntityFrameworkCore;
using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BaitacaConnect.Services
{
    public class AuthService : IAuthService
    {
        private readonly BaitacaDbContext _context;

        public AuthService(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Reservas)
                .FirstOrDefaultAsync(u => u.EmailUsuario == loginDto.EmailUsuario);

            if (usuario == null || !usuario.Ativo)
            {
                return null;
            }

            if (!VerificarSenha(loginDto.SenhaUsuario, usuario.SenhaUsuario))
            {
                return null;
            }

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

        public async Task<UsuarioResponseDto> RegistroAsync(CreateUsuarioDto createUsuarioDto)
        {
            // Verificar se o email já existe
            if (await _context.Usuarios.AnyAsync(u => u.EmailUsuario == createUsuarioDto.EmailUsuario))
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

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

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
                TotalReservas = 0,
                ReservasAtivas = 0
            };
        }

        public async Task<bool> ValidarSenhaAsync(ValidarSenhaComEmailDto validarSenhaDto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.EmailUsuario == validarSenhaDto.EmailUsuario);

            if (usuario == null || !usuario.Ativo)
            {
                return false;
            }

            return VerificarSenha(validarSenhaDto.Senha, usuario.SenhaUsuario);
        }

        public async Task<bool> AlterarSenhaAsync(AlterarSenhaComEmailDto alterarSenhaDto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.EmailUsuario == alterarSenhaDto.EmailUsuario);

            if (usuario == null || !usuario.Ativo)
            {
                return false;
            }

            if (!VerificarSenha(alterarSenhaDto.SenhaAtual, usuario.SenhaUsuario))
            {
                return false;
            }

            usuario.SenhaUsuario = HashSenha(alterarSenhaDto.NovaSenha);
            
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerificarSenha(string senha, string? senhaHash)
        {
            if (string.IsNullOrEmpty(senhaHash))
                return false;

            var hashSenhaInput = HashSenha(senha);
            return hashSenhaInput == senhaHash;
        }
    }
}
