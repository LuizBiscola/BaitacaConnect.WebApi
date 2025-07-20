using System.ComponentModel.DataAnnotations;

namespace BaitacaConnect.Models.DTOs
{
    public class CreateUsuarioDto
    {
        [Required]
        [MaxLength(100)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string EmailUsuario { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public string? TelefoneUsuario { get; set; }

        [MaxLength(20)]
        public string TipoUsuario { get; set; } = "visitante";

        [Range(1, 120)]
        public int? IdadeUsuario { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string SenhaUsuario { get; set; } = string.Empty;
    }

    public class UpdateUsuarioDto
    {
        [MaxLength(100)]
        public string? NomeUsuario { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? EmailUsuario { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? TelefoneUsuario { get; set; }

        [MaxLength(20)]
        public string? TipoUsuario { get; set; }

        [Range(1, 120)]
        public int? IdadeUsuario { get; set; }

        public bool? Ativo { get; set; }
    }

    public class UsuarioResponseDto
    {
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string EmailUsuario { get; set; } = string.Empty;
        public string? TelefoneUsuario { get; set; }
        public string TipoUsuario { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }
        public int? IdadeUsuario { get; set; }
        public int TotalReservas { get; set; }
        public int ReservasAtivas { get; set; }
    }

    public class UsuarioResumoDto
    {
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string EmailUsuario { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string EmailUsuario { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string SenhaUsuario { get; set; } = string.Empty;
    }

    public class AlterarSenhaDto
    {
        [Required]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string NovaSenha { get; set; } = string.Empty;

        [Required]
        [Compare("NovaSenha")]
        public string ConfirmarNovaSenha { get; set; } = string.Empty;
    }

    public class ValidarSenhaDto
    {
        [Required]
        [MinLength(8)]
        public string Senha { get; set; } = string.Empty;
    }

    public class ValidarSenhaComEmailDto
    {
        [Required]
        [EmailAddress]
        public string EmailUsuario { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Senha { get; set; } = string.Empty;
    }

    public class AlterarSenhaComEmailDto
    {
        [Required]
        [EmailAddress]
        public string EmailUsuario { get; set; } = string.Empty;

        [Required]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string NovaSenha { get; set; } = string.Empty;

        [Required]
        [Compare("NovaSenha")]
        public string ConfirmarNovaSenha { get; set; } = string.Empty;
    }

    public class UsuarioPerfilDto
    {
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string EmailUsuario { get; set; } = string.Empty;
        public string? TelefoneUsuario { get; set; }
        public int? IdadeUsuario { get; set; }
        public DateTime DataCriacao { get; set; }
        public int TotalReservas { get; set; }
        public int VisitasRealizadas { get; set; }
        public decimal? AvaliacaoMedia { get; set; }
        public List<ReservaResumoDto> ReservasRecentes { get; set; } = new();
    }
}
