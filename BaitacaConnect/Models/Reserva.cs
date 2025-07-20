using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaitacaConnect.Models
{
    [Table("reserva")]
    public class Reserva
    {
        [Key]
        [Column("id_reserva")]
        public int IdReserva { get; set; }

        [Required]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("id_parque")]
        public int IdParque { get; set; }

        [Column("id_trilha")]
        public int? IdTrilha { get; set; }

        [Required]
        [Column("data_visita")]
        public DateOnly DataVisita { get; set; }

        [Column("horario_entrada")]
        public TimeSpan? HorarioEntrada { get; set; }

        [Column("numero_visitantes")]
        public int NumeroVisitantes { get; set; } = 1;

        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "ativa";

        [Column("check_in")]
        public DateTime? CheckIn { get; set; }

        [Column("check_out")]
        public DateTime? CheckOut { get; set; }

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; } = null!;

        [ForeignKey("IdParque")]
        public virtual Parque Parque { get; set; } = null!;

        [ForeignKey("IdTrilha")]
        public virtual Trilha? Trilha { get; set; }

        public virtual ICollection<RelatorioVisita> RelatoriosVisita { get; set; } = new List<RelatorioVisita>();
    }
}