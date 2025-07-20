using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaitacaConnect.Models
{
    [Table("trilha")]
    public class Trilha
    {
        [Key]
        [Column("id_trilha")]
        public int IdTrilha { get; set; }

        [Required]
        [Column("id_parque")]
        public int IdParque { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome_trilha")]
        public string NomeTrilha { get; set; } = string.Empty;

        [Column("descricao_trilha", TypeName = "text")]
        public string? DescricaoTrilha { get; set; }

        [MaxLength(20)]
        [Column("dificuldade_trilha")]
        public string? DificuldadeTrilha { get; set; }

        [Column("distancia_km", TypeName = "decimal(5,2)")]
        public decimal? DistanciaKm { get; set; }

        [Column("tempo_estimado_minutos")]
        public int? TempoEstimadoMinutos { get; set; }

        [Column("capacidade_maxima")]
        public int? CapacidadeMaxima { get; set; }

        [Column("coordenadas_trilha", TypeName = "jsonb")]
        public string? CoordenadasTrilha { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        // Navigation Properties
        [ForeignKey("IdParque")]
        public virtual Parque Parque { get; set; } = null!;
        
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public virtual ICollection<PontoInteresse> PontosInteresse { get; set; } = new List<PontoInteresse>();
    }
}