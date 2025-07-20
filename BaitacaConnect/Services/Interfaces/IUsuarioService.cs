using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioResumoDto>> GetUsuariosAsync(string? filtroNome, string? filtroTipo, bool? ativo);
        Task<UsuarioResponseDto?> GetUsuarioByIdAsync(int id);
        Task<UsuarioResponseDto?> GetUsuarioByEmailAsync(string email);
        Task<bool> EmailExisteAsync(string email);
        Task<bool> ValidarSenhaAsync(int id, string senha);
        Task<bool> ValidarSenhaByEmailAsync(string email, string senha);
        Task<UsuarioResponseDto> CriarUsuarioAsync(CreateUsuarioDto createUsuarioDto);
        Task<bool> AtualizarUsuarioAsync(int id, UpdateUsuarioDto updateUsuarioDto);
        Task<bool> AlterarSenhaAsync(int id, AlterarSenhaDto alterarSenhaDto);
        Task<bool> AlterarSenhaByEmailAsync(AlterarSenhaComEmailDto alterarSenhaDto);
        Task<bool> AtivarUsuarioAsync(int id);
        Task<bool> DesativarUsuarioAsync(int id);
        Task<bool> UsuarioExisteAsync(int id);
        Task<bool> UsuarioAtivoAsync(int id);
    }
}
