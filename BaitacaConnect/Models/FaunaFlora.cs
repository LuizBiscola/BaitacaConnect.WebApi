using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaitacaConnect.Models
{
    [Table("fauna_flora")]
    public class FaunaFlora
    {
        [Key]
        [Column("id_fauna_flora")]
        public int IdFaunaFlora { get; set; }

        [MaxLength(150)]
        [Column("nome_cientifico")]
        public string? NomeCientifico { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome_popular")]
        public string NomePopular { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty; // fauna, flora

        [MaxLength(50)]
        [Column("categoria")]
        public string? Categoria { get; set; }

        [Column("descricao", TypeName = "text")]
        public string? Descricao { get; set; }

        [Column("caracteristicas", TypeName = "jsonb")]
        public string? Caracteristicas { get; set; }

        [Column("imagens", TypeName = "jsonb")]
        public string? Imagens { get; set; }

        [Column("trilhas_onde_encontra")]
        public List<int>? TrilhasOndeEncontra { get; set; }
    }
}