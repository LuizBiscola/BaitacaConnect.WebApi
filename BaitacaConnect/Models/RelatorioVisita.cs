using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaitacaConnect.Models
{
    [Table("relatorio_visita")]
    public class RelatorioVisita
    {
        [Key]
        [Column("id_relatorio")]
        public int IdRelatorio { get; set; }

        [Required]
        [Column("id_reserva")]
        public int IdReserva { get; set; }

        [Range(1, 5)]
        [Column("avaliacao")]
        public int? Avaliacao { get; set; }

        [Column("comentarios", TypeName = "text")]
        public string? Comentarios { get; set; }

        [Column("problemas_encontrados", TypeName = "text")]
        public string? ProblemasEncontrados { get; set; }

        [Column("data_relatorio")]
        public DateTime DataRelatorio { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("IdReserva")]
        public virtual Reserva Reserva { get; set; } = null!;
    }
}