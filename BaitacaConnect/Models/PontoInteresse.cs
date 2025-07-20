using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaitacaConnect.Models
{
    [Table("ponto_interesse")]
    public class PontoInteresse
    {
        [Key]
        [Column("id_parque", Order = 1)]
        public int IdParque { get; set; }

        [Key]
        [Column("id_trilha", Order = 2)]
        public int IdTrilha { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome_ponto_interesse")]
        public string NomePontoInteresse { get; set; } = string.Empty;

        [Column("descricao_ponto_interesse", TypeName = "text")]
        public string? DescricaoPontoInteresse { get; set; }

        [MaxLength(50)]
        [Column("tipo")]
        public string? Tipo { get; set; }

        [Column("coordenadas", TypeName = "point")]
        public string? Coordenadas { get; set; }

        [Column("ordem_na_trilha")]
        public int? OrdemNaTrilha { get; set; }

        // Navigation Properties
        [ForeignKey("IdParque")]
        public virtual Parque Parque { get; set; } = null!;

        [ForeignKey("IdTrilha")]
        public virtual Trilha Trilha { get; set; } = null!;
    }
}