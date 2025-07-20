using System.ComponentModel.DataAnnotations;

namespace BaitacaConnect.Models.DTOs
{
    public class CreateRelatorioVisitaDto
    {
        [Required]
        public int IdReserva { get; set; }

        [Range(1, 5)]
        public int? Avaliacao { get; set; }

        public string? Comentarios { get; set; }

        public string? ProblemasEncontrados { get; set; }
    }

    public class UpdateRelatorioVisitaDto
    {
        [Range(1, 5)]
        public int? Avaliacao { get; set; }

        public string? Comentarios { get; set; }

        public string? ProblemasEncontrados { get; set; }
    }

    public class RelatorioVisitaResponseDto
    {
        public int IdRelatorio { get; set; }
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string NomeParque { get; set; } = string.Empty;
        public string? NomeTrilha { get; set; }
        public DateOnly DataVisita { get; set; }
        public int? Avaliacao { get; set; }
        public string? Comentarios { get; set; }
        public string? ProblemasEncontrados { get; set; }
        public DateTime DataRelatorio { get; set; }
    }

    public class RelatorioVisitaResumoDto
    {
        public int IdRelatorio { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public string? NomeTrilha { get; set; }
        public DateOnly DataVisita { get; set; }
        public int? Avaliacao { get; set; }
        public DateTime DataRelatorio { get; set; }
    }
}