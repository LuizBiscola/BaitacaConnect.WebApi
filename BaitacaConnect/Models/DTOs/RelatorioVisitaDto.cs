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
        public int IdParque { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public int? IdTrilha { get; set; }
        public string? NomeTrilha { get; set; }
        public DateOnly DataVisita { get; set; }
        public int? Avaliacao { get; set; }
        public string? Comentarios { get; set; }
        public string? ProblemasEncontrados { get; set; }
        public DateTime DataRelatorio { get; set; }
        public bool PodeEditar { get; set; }
    }

    public class RelatorioVisitaResumoDto
    {
        public int IdRelatorio { get; set; }
        public int IdReserva { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public string? NomeTrilha { get; set; }
        public DateOnly DataVisita { get; set; }
        public int? Avaliacao { get; set; }
        public DateTime DataRelatorio { get; set; }
        public bool TemProblemas { get; set; }
    }

    public class EstatisticasRelatorioDto
    {
        public int TotalRelatorios { get; set; }
        public double AvaliacaoMedia { get; set; }
        public int RelatoriosComProblemas { get; set; }
        public Dictionary<int, int> DistribuicaoAvaliacoes { get; set; } = new();
        public DateTime? UltimoRelatorio { get; set; }
    }

    public class EstatisticasGeralDto
    {
        public int TotalRelatorios { get; set; }
        public int RelatoriosComProblemas { get; set; }
        public double PercentualProblemas { get; set; }
        public Dictionary<int, int> DistribuicaoAvaliacoes { get; set; } = new();
        public double AvaliacaoMediaGeral { get; set; }
    }
}