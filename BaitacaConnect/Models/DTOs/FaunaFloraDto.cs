using System.ComponentModel.DataAnnotations;

namespace BaitacaConnect.Models.DTOs
{
    public class CreateFaunaFloraDto
    {
        [MaxLength(150)]
        public string? NomeCientifico { get; set; }

        [Required]
        [MaxLength(100)]
        public string NomePopular { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Tipo { get; set; } = string.Empty; // fauna, flora

        [MaxLength(50)]
        public string? Categoria { get; set; }

        public string? Descricao { get; set; }

        public string? Caracteristicas { get; set; } // JSON string

        public string? Imagens { get; set; } // JSON string

        public List<int>? TrilhasOndeEncontra { get; set; }
    }

    public class UpdateFaunaFloraDto
    {
        [MaxLength(150)]
        public string? NomeCientifico { get; set; }

        [MaxLength(100)]
        public string? NomePopular { get; set; }

        [MaxLength(20)]
        public string? Tipo { get; set; }

        [MaxLength(50)]
        public string? Categoria { get; set; }

        public string? Descricao { get; set; }

        public string? Caracteristicas { get; set; }

        public string? Imagens { get; set; }

        public List<int>? TrilhasOndeEncontra { get; set; }
    }

    public class FaunaFloraResponseDto
    {
        public int IdFaunaFlora { get; set; }
        public string? NomeCientifico { get; set; }
        public string NomePopular { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string? Descricao { get; set; }
        public string? Caracteristicas { get; set; }
        public string? Imagens { get; set; }
        public List<int> TrilhasOndeEncontra { get; set; } = new();
    }

    public class FaunaFloraResumoDto
    {
        public int IdFaunaFlora { get; set; }
        public string? NomeCientifico { get; set; }
        public string NomePopular { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string? ImagemPrincipal { get; set; }
    }

    public class FaunaFloraTrilhaDto
    {
        public int IdFaunaFlora { get; set; }
        public string NomePopular { get; set; } = string.Empty;
        public string? NomeCientifico { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string? ImagemPrincipal { get; set; }
        public string? DescricaoResumida { get; set; }
    }

    public class BuscarFaunaFloraDto
    {
        public string? Termo { get; set; }
        public string? Tipo { get; set; }
        public string? Categoria { get; set; }
        public int? IdTrilha { get; set; }
        public int? IdParque { get; set; }
    }

    public class EstatisticasFaunaFloraDto
    {
        public int TotalFauna { get; set; }
        public int TotalFlora { get; set; }
        public int TotalEspecies { get; set; }
        public int TotalCategorias { get; set; }
        public Dictionary<string, int> DistribuicaoPorTipo { get; set; } = new();
        public List<string> Categorias { get; set; } = new();
    }

    public class GuiaEspeciesDto
    {
        public int IdTrilha { get; set; }
        public string NomeTrilha { get; set; } = string.Empty;
        public int TotalEspecies { get; set; }
        public int TotalFauna { get; set; }
        public int TotalFlora { get; set; }
        public List<FaunaFloraResponseDto> Especies { get; set; } = new();
    }
}