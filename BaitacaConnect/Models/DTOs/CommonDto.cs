namespace BaitacaConnect.Models.DTOs
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; }
        public bool OrderDescending { get; set; } = false;
    }

    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class ApiResponseDto<T>
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class ApiResponseDto : ApiResponseDto<object>
    {
    }

    public class ErrorDto
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Field { get; set; }
        public object? Details { get; set; }
    }

    public class BuscarDto
    {
        public string? Termo { get; set; }
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string? OrderBy { get; set; }
        public bool? OrderDescending { get; set; } = false;
    }

    public class FiltroDataDto
    {
        public DateOnly? DataInicio { get; set; }
        public DateOnly? DataFim { get; set; }
    }

    public class EstatisticasGeraisDto
    {
        public int TotalParques { get; set; }
        public int ParquesAtivos { get; set; }
        public int TotalTrilhas { get; set; }
        public int TrilhasAtivas { get; set; }
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public int TotalReservas { get; set; }
        public int ReservasAtivas { get; set; }
        public int TotalEspeciesFaunaFlora { get; set; }
        public int TotalPontosInteresse { get; set; }
        public int TotalRelatoriosVisita { get; set; }
        public decimal AvaliacaoMediaGeral { get; set; }
    }

    public class HealthCheckDto
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Version { get; set; } = string.Empty;
        public Dictionary<string, object> Services { get; set; } = new();
    }
}