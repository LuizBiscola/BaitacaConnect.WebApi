using System.ComponentModel.DataAnnotations;

namespace BaitacaConnect.Models.DTOs
{
    public class CreateTrilhaDto
    {
        [Required]
        public int IdParque { get; set; }

        [Required]
        [MaxLength(100)]
        public string NomeTrilha { get; set; } = string.Empty;

        public string? DescricaoTrilha { get; set; }

        [MaxLength(20)]
        public string? DificuldadeTrilha { get; set; }

        [Range(0.1, 1000)]
        public decimal? DistanciaKm { get; set; }

        [Range(1, 1440)]
        public int? TempoEstimadoMinutos { get; set; }

        [Range(1, 500)]
        public int? CapacidadeMaxima { get; set; }

        public string? CoordenadasTrilha { get; set; } // JSON string
    }

    public class UpdateTrilhaDto
    {
        [MaxLength(100)]
        public string? NomeTrilha { get; set; }

        public string? DescricaoTrilha { get; set; }

        [MaxLength(20)]
        public string? DificuldadeTrilha { get; set; }

        [Range(0.1, 1000)]
        public decimal? DistanciaKm { get; set; }

        [Range(1, 1440)]
        public int? TempoEstimadoMinutos { get; set; }

        [Range(1, 500)]
        public int? CapacidadeMaxima { get; set; }

        public string? CoordenadasTrilha { get; set; }

        public bool? Ativo { get; set; }
    }

    public class TrilhaResponseDto
    {
        public int IdTrilha { get; set; }
        public int IdParque { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public string NomeTrilha { get; set; } = string.Empty;
        public string? DescricaoTrilha { get; set; }
        public string? DificuldadeTrilha { get; set; }
        public decimal? DistanciaKm { get; set; }
        public int? TempoEstimadoMinutos { get; set; }
        public int? CapacidadeMaxima { get; set; }
        public string? CoordenadasTrilha { get; set; }
        public bool Ativo { get; set; }
        public int TotalPontosInteresse { get; set; }
        public int TotalEspeciesFaunaFlora { get; set; }
        public int ReservasHoje { get; set; }
        public int OcupacaoAtual { get; set; }
    }

    public class TrilhaResumoDto
    {
        public int IdTrilha { get; set; }
        public string NomeTrilha { get; set; } = string.Empty;
        public string? DificuldadeTrilha { get; set; }
        public decimal? DistanciaKm { get; set; }
        public int? TempoEstimadoMinutos { get; set; }
        public int? CapacidadeMaxima { get; set; }
        public int OcupacaoAtual { get; set; }
        public bool Disponivel { get; set; }
        public bool Ativo { get; set; }
    }

    public class TrilhaComMapaDto
    {
        public int IdTrilha { get; set; }
        public string NomeTrilha { get; set; } = string.Empty;
        public string? DescricaoTrilha { get; set; }
        public string? DificuldadeTrilha { get; set; }
        public decimal? DistanciaKm { get; set; }
        public int? TempoEstimadoMinutos { get; set; }
        public string? CoordenadasTrilha { get; set; }
        public List<PontoInteresseResumoDto>? PontosInteresse { get; set; }
    }

    public class ValidarCapacidadeDto
    {
        [Required]
        [Range(1, 50)]
        public int NumeroVisitantes { get; set; } = 1;
    }

    public class EstatisticasTrilhaDto
    {
        public int IdTrilha { get; set; }
        public string NomeTrilha { get; set; } = string.Empty;
        public string NomeParque { get; set; } = string.Empty;
        public int TotalReservas { get; set; }
        public int ReservasAtivas { get; set; }
        public int ReservasHoje { get; set; }
        public int ReservasMesAtual { get; set; }
        public int TotalPontosInteresse { get; set; }
        public int TotalEspeciesFaunaFlora { get; set; }
        public int CapacidadeMaxima { get; set; }
        public decimal TaxaOcupacaoMedia { get; set; }
        public decimal? DistanciaKm { get; set; }
        public int? TempoEstimadoMinutos { get; set; }
        public string? DificuldadeTrilha { get; set; }
    }
}