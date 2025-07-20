using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaitacaConnect.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome_usuario")]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        [Column("email_usuario")]
        public string EmailUsuario { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        [Column("telefone_usuario")]
        public string? TelefoneUsuario { get; set; }

        [MaxLength(20)]
        [Column("tipo_usuario")]
        public string TipoUsuario { get; set; } = "visitante";

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        [Column("idade_usuario")]
        public int? IdadeUsuario { get; set; }

        [MaxLength(200)]
        [Column("senha_usuario")]
        public string? SenhaUsuario { get; set; } // TODO: Implementar hash da senha antes de salvar

        // Propriedade para verificação de senha (não mapeada para o banco)
        [NotMapped]
        public string? SenhaTemporaria { get; set; }

        // Navigation Properties
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}