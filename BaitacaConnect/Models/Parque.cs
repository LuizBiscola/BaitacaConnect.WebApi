using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaitacaConnect.Models
{
    [Table("parque")]
    public class Parque
    {
        [Key]
        [Column("id_parque")]
        public int IdParque { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome_parque")]
        public string NomeParque { get; set; } = string.Empty;

        [Column("descricao_parque", TypeName = "text")]
        public string? DescricaoParque { get; set; }

        [Column("endereco_parque", TypeName = "text")]
        public string? EnderecoParque { get; set; }

        [Column("capacidade_maxima")]
        public int? CapacidadeMaxima { get; set; }

        [Column("horario_funcionamento", TypeName = "jsonb")]
        public string? HorarioFuncionamento { get; set; }

        [Column("coordenadas_parque", TypeName = "point")]
        public string? CoordenadasParque { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<Trilha> Trilhas { get; set; } = new List<Trilha>();
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public virtual ICollection<PontoInteresse> PontosInteresse { get; set; } = new List<PontoInteresse>();
    }
}