using System.ComponentModel.DataAnnotations;

namespace BaitacaConnect.Models.DTOs
{
    public class CreateReservaDto
    {
        [Required]
        public int IdParque { get; set; }

        public int? IdTrilha { get; set; }

        [Required]
        public DateOnly DataVisita { get; set; }

        public TimeSpan? HorarioEntrada { get; set; }

        [Range(1, 50)]
        public int NumeroVisitantes { get; set; } = 1;
    }

    public class UpdateReservaDto
    {
        public DateOnly? DataVisita { get; set; }
        
        public TimeSpan? HorarioEntrada { get; set; }
        
        [Range(1, 50)]
        public int? NumeroVisitantes { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }
    }

    public class ReservaResponseDto
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public int IdParque { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public int? IdTrilha { get; set; }
        public string? NomeTrilha { get; set; }
        public DateOnly DataVisita { get; set; }
        public TimeSpan? HorarioEntrada { get; set; }
        public int NumeroVisitantes { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class ReservaResumoDto
    {
        public int IdReserva { get; set; }
        public string NomeParque { get; set; } = string.Empty;
        public string? NomeTrilha { get; set; }
        public DateOnly DataVisita { get; set; }
        public string Status { get; set; } = string.Empty;
        public int NumeroVisitantes { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class CheckInDto
    {
        [Required]
        public int IdReserva { get; set; }
        
        public DateTime? HorarioCheckIn { get; set; } = DateTime.Now;
    }

    public class CheckOutDto
    {
        [Required]
        public int IdReserva { get; set; }
        
        public DateTime? HorarioCheckOut { get; set; } = DateTime.Now;
    }

    public class ReservaCalendarioDto
    {
        public DateOnly Data { get; set; }
        public int TotalReservas { get; set; }
        public int TotalVisitantes { get; set; }
        public List<ReservaResumoDto> Reservas { get; set; } = new();
    }

    public class ValidarReservaDto
    {
        [Required]
        public int IdParque { get; set; }
        
        public int? IdTrilha { get; set; }
        
        [Required]
        public DateOnly DataVisita { get; set; }
        
        [Range(1, 50)]
        public int NumeroVisitantes { get; set; } = 1;
    }

    public class ValidarReservaResponseDto
    {
        public bool Disponivel { get; set; }
        public string? MotivoIndisponibilidade { get; set; }
        public int VagasDisponiveis { get; set; }
        public int CapacidadeMaxima { get; set; }
        public List<string>? Alertas { get; set; }
    }
}