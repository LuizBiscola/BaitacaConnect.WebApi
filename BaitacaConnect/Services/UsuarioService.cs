using Microsoft.EntityFrameworkCore;
using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BaitacaConnect.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly BaitacaDbContext _context;

        public UsuarioService(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioResumoDto>> GetUsuariosAsync(string? filtroNome, string? filtroTipo, bool? ativo)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(filtroNome))
            {
                query = query.Where(u => u.NomeUsuario.Contains(filtroNome));
            }

            if (!string.IsNullOrEmpty(filtroTipo))
            {
                query = query.Where(u => u.TipoUsuario == filtroTipo);
            }

            if (ativo.HasValue)
            {
                query = query.Where(u => u.Ativo == ativo.Value);
            }

            return await query
                .Select(u => new UsuarioResumoDto
                {
                    IdUsuario = u.IdUsuario,
                    NomeUsuario = u.NomeUsuario,
                    EmailUsuario = u.EmailUsuario,
                    TipoUsuario = u.TipoUsuario,
                    Ativo = u.Ativo,
                    DataCriacao = u.DataCriacao
                })
                .OrderBy(u => u.NomeUsuario)
                .ToListAsync();
        }

        public async Task<UsuarioResponseDto?> GetUsuarioByIdAsync(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Reservas)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

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
            var usuario = await _context.Usuarios
                .Include(u => u.Reservas)
                .FirstOrDefaultAsync(u => u.EmailUsuario == email);

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
            return await _context.Usuarios.AnyAsync(u => u.EmailUsuario == email);
        }

        public async Task<bool> ValidarSenhaAsync(int id, string senha)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null || !usuario.Ativo)
                return false;

            return VerificarSenha(senha, usuario.SenhaUsuario);
        }

        public async Task<bool> ValidarSenhaByEmailAsync(string email, string senha)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.EmailUsuario == email);

            if (usuario == null || !usuario.Ativo)
                return false;

            return VerificarSenha(senha, usuario.SenhaUsuario);
        }

        public async Task<UsuarioResponseDto> CriarUsuarioAsync(CreateUsuarioDto createUsuarioDto)
        {
            if (await EmailExisteAsync(createUsuarioDto.EmailUsuario))
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

        public async Task<bool> AtualizarUsuarioAsync(int id, UpdateUsuarioDto updateUsuarioDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return false;

            // Verificar se o novo email já está em uso por outro usuário
            if (!string.IsNullOrEmpty(updateUsuarioDto.EmailUsuario) && 
                updateUsuarioDto.EmailUsuario != usuario.EmailUsuario)
            {
                if (await EmailExisteAsync(updateUsuarioDto.EmailUsuario))
                {
                    throw new InvalidOperationException("Email já está em uso");
                }
            }

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrEmpty(updateUsuarioDto.NomeUsuario))
                usuario.NomeUsuario = updateUsuarioDto.NomeUsuario;
            
            if (!string.IsNullOrEmpty(updateUsuarioDto.EmailUsuario))
                usuario.EmailUsuario = updateUsuarioDto.EmailUsuario;
            
            if (updateUsuarioDto.TelefoneUsuario != null)
                usuario.TelefoneUsuario = updateUsuarioDto.TelefoneUsuario;
            
            if (!string.IsNullOrEmpty(updateUsuarioDto.TipoUsuario))
                usuario.TipoUsuario = updateUsuarioDto.TipoUsuario;
            
            if (updateUsuarioDto.IdadeUsuario.HasValue)
                usuario.IdadeUsuario = updateUsuarioDto.IdadeUsuario;
            
            if (updateUsuarioDto.Ativo.HasValue)
                usuario.Ativo = updateUsuarioDto.Ativo.Value;

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

        public async Task<bool> AlterarSenhaAsync(int id, AlterarSenhaDto alterarSenhaDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null || !usuario.Ativo)
                return false;

            if (!VerificarSenha(alterarSenhaDto.SenhaAtual, usuario.SenhaUsuario))
                return false;

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

        public async Task<bool> AlterarSenhaByEmailAsync(AlterarSenhaComEmailDto alterarSenhaDto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.EmailUsuario == alterarSenhaDto.EmailUsuario);

            if (usuario == null || !usuario.Ativo)
                return false;

            if (!VerificarSenha(alterarSenhaDto.SenhaAtual, usuario.SenhaUsuario))
                return false;

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

        public async Task<bool> AtivarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return false;

            usuario.Ativo = true;

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

        public async Task<bool> DesativarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return false;

            usuario.Ativo = false;

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

        public async Task<bool> UsuarioExisteAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(u => u.IdUsuario == id);
        }

        public async Task<bool> UsuarioAtivoAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            return usuario?.Ativo ?? false;
        }

        // Métodos auxiliares para hash de senha
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
    }
}
