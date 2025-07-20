namespace BaitacaConnect.Models.DTOs
{
    public class RelatorioVisitacaoDto
    {
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public int TotalVisitantes { get; set; }
        public int TotalReservas { get; set; }
        public int ReservasCanceladas { get; set; }
        public int ReservasConcluidas { get; set; }
        public decimal TaxaOcupacao { get; set; }
        public decimal AvaliacaoMedia { get; set; }
        public List<VisitacaoPorDiaDto> VisitacaoPorDia { get; set; } = new();
        public List<ParqueMaisVisitadoDto> ParquesMaisVisitados { get; set; } = new();
        public List<TrilhaMaisVisitadaDto> TrilhasMaisVisitadas { get; set; } = new();
    }

    public class VisitacaoPorDiaDto
    {
        public DateOnly Data { get; set; }
        public int TotalVisitantes { get; set; }
        public int TotalReservas { get; set; }
        public int ReservasCanceladas { get; set; }
        public int ReservasConcluidas { get; set; }
    }

    public class ParqueMaisVisitadoDto
    {
        public int IdParque { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public int TotalVisitantes { get; set; }
        public int TotalReservas { get; set; }
        public decimal AvaliacaoMedia { get; set; }
    }

    public class TrilhaMaisVisitadaDto
    {
        public int IdTrilha { get; set; }
        public string NomeTrilha { get; set; } = string.Empty;
        public string NomeParque { get; set; } = string.Empty;
        public int TotalVisitantes { get; set; }
        public int TotalReservas { get; set; }
        public decimal AvaliacaoMedia { get; set; }
    }

    public class DashboardResumoDto
    {
        public int TotalParques { get; set; }
        public int TotalTrilhas { get; set; }
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public int ReservasHoje { get; set; }
        public int VisitantesHoje { get; set; }
        public int OcupacaoAtual { get; set; }
        public decimal TaxaOcupacaoMedia { get; set; }
        public decimal AvaliacaoMediaGeral { get; set; }
        public List<AlertaDto> Alertas { get; set; } = new();
        public List<MetricaRecenteDto> MetricasRecentes { get; set; } = new();
        public List<TendenciaDto> Tendencias { get; set; } = new();
    }

    public class AlertaDto
    {
        public string Tipo { get; set; } = string.Empty; // lotacao, manutencao, clima, sistema
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Severidade { get; set; } = string.Empty; // baixa, media, alta, critica
        public DateTime DataAlerta { get; set; }
        public int? IdParque { get; set; }
        public int? IdTrilha { get; set; }
    }

    public class MetricaRecenteDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string Unidade { get; set; } = string.Empty;
        public string Tendencia { get; set; } = string.Empty; // subindo, descendo, estavel
        public string Periodo { get; set; } = string.Empty;
        public decimal? VariacaoPercentual { get; set; }
    }

    public class TendenciaDto
    {
        public string Categoria { get; set; } = string.Empty;
        public List<PontoTendenciaDto> Pontos { get; set; } = new();
    }

    public class PontoTendenciaDto
    {
        public DateOnly Data { get; set; }
        public decimal Valor { get; set; }
    }

    public class FiltroRelatorioDto
    {
        public DateOnly? DataInicio { get; set; }
        public DateOnly? DataFim { get; set; }
        public int? IdParque { get; set; }
        public int? IdTrilha { get; set; }
        public string? Status { get; set; }
        public int? IdUsuario { get; set; }
        public string? TipoUsuario { get; set; }
        public int? AvaliacaoMinima { get; set; }
        public bool? ApenasComProblemas { get; set; }
    }

    public class RelatorioOcupacaoDto
    {
        public DateOnly Data { get; set; }
        public List<OcupacaoParqueDto> OcupacaoPorParque { get; set; } = new();
        public int TotalCapacidade { get; set; }
        public int TotalOcupado { get; set; }
        public decimal PercentualOcupacao { get; set; }
    }

    public class OcupacaoParqueDto
    {
        public int IdParque { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public int Capacidade { get; set; }
        public int Ocupado { get; set; }
        public decimal PercentualOcupacao { get; set; }
        public List<OcupacaoTrilhaDto> OcupacaoTrilhas { get; set; } = new();
    }

    public class OcupacaoTrilhaDto
    {
        public int IdTrilha { get; set; }
        public string NomeTrilha { get; set; } = string.Empty;
        public int Capacidade { get; set; }
        public int Ocupado { get; set; }
        public decimal PercentualOcupacao { get; set; }
    }
}