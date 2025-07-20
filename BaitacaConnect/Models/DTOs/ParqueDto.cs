using System.ComponentModel.DataAnnotations;

namespace BaitacaConnect.Models.DTOs
{
    public class CreateParqueDto
    {
        [Required]
        [MaxLength(100)]
        public string NomeParque { get; set; } = string.Empty;

        public string? DescricaoParque { get; set; }

        public string? EnderecoParque { get; set; }

        [Range(1, 10000)]
        public int? CapacidadeMaxima { get; set; }

        public string? HorarioFuncionamento { get; set; } // JSON string

        public string? CoordenadasParque { get; set; } // Point format: "(lat,lng)"
    }

    public class UpdateParqueDto
    {
        [MaxLength(100)]
        public string? NomeParque { get; set; }

        public string? DescricaoParque { get; set; }

        public string? EnderecoParque { get; set; }

        [Range(1, 10000)]
        public int? CapacidadeMaxima { get; set; }

        public string? HorarioFuncionamento { get; set; }

        public string? CoordenadasParque { get; set; }

        public bool? Ativo { get; set; }
    }

    public class ParqueResponseDto
    {
        public int IdParque { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public string? DescricaoParque { get; set; }
        public string? EnderecoParque { get; set; }
        public int? CapacidadeMaxima { get; set; }
        public string? HorarioFuncionamento { get; set; }
        public string? CoordenadasParque { get; set; }
        public bool Ativo { get; set; }
        public int TotalTrilhas { get; set; }
        public int TotalReservasHoje { get; set; }
        public int OcupacaoAtual { get; set; }
    }

    public class ParqueResumoDto
    {
        public int IdParque { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public string? DescricaoParque { get; set; }
        public int? CapacidadeMaxima { get; set; }
        public bool Ativo { get; set; }
        public int TotalTrilhas { get; set; }
        public int OcupacaoAtual { get; set; }
    }

    public class DisponibilidadeParqueDto
    {
        public int IdParque { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public DateOnly Data { get; set; }
        public int CapacidadeMaxima { get; set; }
        public int ReservasConfirmadas { get; set; }
        public int VagasDisponiveis { get; set; }
        public bool Disponivel { get; set; }
        public List<DisponibilidadeTrilhaDto>? TrilhasDisponibilidade { get; set; }
    }

    public class DisponibilidadeTrilhaDto
    {
        public int IdTrilha { get; set; }
        public string NomeTrilha { get; set; } = string.Empty;
        public int CapacidadeMaxima { get; set; }
        public int ReservasConfirmadas { get; set; }
        public int VagasDisponiveis { get; set; }
        public bool Disponivel { get; set; }
    }
}