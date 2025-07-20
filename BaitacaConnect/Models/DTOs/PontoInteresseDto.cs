using System.ComponentModel.DataAnnotations;

namespace BaitacaConnect.Models.DTOs
{
    public class CreatePontoInteresseDto
    {
        [Required]
        public int IdParque { get; set; }

        [Required]
        public int IdTrilha { get; set; }

        [Required]
        [MaxLength(100)]
        public string NomePontoInteresse { get; set; } = string.Empty;

        public string? DescricaoPontoInteresse { get; set; }

        [MaxLength(50)]
        public string? Tipo { get; set; }

        public string? Coordenadas { get; set; } // Point format: "(lat,lng)"

        [Range(1, 999)]
        public int? OrdemNaTrilha { get; set; }
    }

    public class UpdatePontoInteresseDto
    {
        [MaxLength(100)]
        public string? NomePontoInteresse { get; set; }

        public string? DescricaoPontoInteresse { get; set; }

        [MaxLength(50)]
        public string? Tipo { get; set; }

        public string? Coordenadas { get; set; }

        [Range(1, 999)]
        public int? OrdemNaTrilha { get; set; }
    }

    public class PontoInteresseResponseDto
    {
        public int IdParque { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public int IdTrilha { get; set; }
        public string NomeTrilha { get; set; } = string.Empty;
        public string NomePontoInteresse { get; set; } = string.Empty;
        public string? DescricaoPontoInteresse { get; set; }
        public string? Tipo { get; set; }
        public string? Coordenadas { get; set; }
        public int? OrdemNaTrilha { get; set; }
    }

    public class PontoInteresseResumoDto
    {
        public int IdParque { get; set; }
        public int IdTrilha { get; set; }
        public string NomePontoInteresse { get; set; } = string.Empty;
        public string? Tipo { get; set; }
        public string? Coordenadas { get; set; }
        public int? OrdemNaTrilha { get; set; }
    }

    public class PontoInteresseMapaDto
    {
        public string NomePontoInteresse { get; set; } = string.Empty;
        public string? Tipo { get; set; }
        public string? Coordenadas { get; set; }
        public int? OrdemNaTrilha { get; set; }
        public string? DescricaoResumida { get; set; }
    }
}