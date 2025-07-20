using BaitacaConnect.Models.DTOs;

namespace BaitacaConnect.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UsuarioResponseDto?> LoginAsync(LoginDto loginDto);
        Task<UsuarioResponseDto> RegistroAsync(CreateUsuarioDto createUsuarioDto);
        Task<bool> ValidarSenhaAsync(ValidarSenhaComEmailDto validarSenhaDto);
        Task<bool> AlterarSenhaAsync(AlterarSenhaComEmailDto alterarSenhaDto);
        string HashSenha(string senha);
        bool VerificarSenha(string senha, string? senhaHash);
    }
}
