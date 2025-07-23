using Microsoft.EntityFrameworkCore;
using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Repositories.Interfaces;

namespace BaitacaConnect.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly BaitacaDbContext _context;

        public UsuarioRepository(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.Reservas)
                .FirstOrDefaultAsync(u => u.EmailUsuario == email);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios
                .OrderBy(u => u.NomeUsuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetWithFiltersAsync(string? filtroNome, string? filtroTipo, bool? ativo)
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
                .OrderBy(u => u.NomeUsuario)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(u => u.IdUsuario == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.EmailUsuario == email);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsActiveAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            return usuario?.Ativo ?? false;
        }

        public async Task<int> GetReservasCountAsync(int id)
        {
            return await _context.Reservas
                .CountAsync(r => r.IdUsuario == id);
        }

        public async Task<int> GetReservasAtivasCountAsync(int id)
        {
            return await _context.Reservas
                .CountAsync(r => r.IdUsuario == id && r.Status == "ativa");
        }
    }
}
