using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Repositories.Interfaces;
using BaitacaConnect.Services.Interfaces;

namespace BaitacaConnect.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IParqueRepository _parqueRepository;
        private readonly ITrilhaRepository _trilhaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ReservaService(
            IReservaRepository reservaRepository,
            IParqueRepository parqueRepository,
            ITrilhaRepository trilhaRepository,
            IUsuarioRepository usuarioRepository)
        {
            _reservaRepository = reservaRepository;
            _parqueRepository = parqueRepository;
            _trilhaRepository = trilhaRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<ReservaResponseDto>> GetReservasAsync(int? idUsuario = null, int? idParque = null,
            int? idTrilha = null, DateOnly? dataInicio = null, DateOnly? dataFim = null, string? status = null)
        {
            var reservas = await _reservaRepository.GetReservasAsync(idUsuario, idParque, idTrilha, dataInicio, dataFim, status);
            return reservas.Select(MapToReservaResponseDto);
        }

        public async Task<ReservaResponseDto?> GetReservaByIdAsync(int idReserva)
        {
            var reserva = await _reservaRepository.GetReservaByIdAsync(idReserva);
            return reserva != null ? MapToReservaResponseDto(reserva) : null;
        }

        public async Task<ReservaResponseDto> CreateReservaAsync(int idUsuario, CreateReservaDto createReservaDto)
        {
            // Validar se o usuário existe
            var usuario = await _usuarioRepository.GetUsuarioByIdAsync(idUsuario);
            if (usuario == null)
                throw new ArgumentException("Usuário não encontrado");

            // Validar se o parque existe
            var parque = await _parqueRepository.GetParqueByIdAsync(createReservaDto.IdParque);
            if (parque == null)
                throw new ArgumentException("Parque não encontrado");

            // Validar se a trilha existe (se informada)
            if (createReservaDto.IdTrilha.HasValue)
            {
                var trilha = await _trilhaRepository.GetTrilhaByIdAsync(createReservaDto.IdTrilha.Value);
                if (trilha == null)
                    throw new ArgumentException("Trilha não encontrada");
                
                if (trilha.IdParque != createReservaDto.IdParque)
                    throw new ArgumentException("A trilha não pertence ao parque informado");
            }

            // Validar disponibilidade
            var validacao = await ValidarReservaAsync(new ValidarReservaDto
            {
                IdParque = createReservaDto.IdParque,
                IdTrilha = createReservaDto.IdTrilha,
                DataVisita = createReservaDto.DataVisita,
                NumeroVisitantes = createReservaDto.NumeroVisitantes
            });

            if (!validacao.Disponivel)
                throw new InvalidOperationException($"Reserva não disponível: {validacao.MotivoIndisponibilidade}");

            // Verificar se o usuário já tem reserva ativa para o mesmo parque na mesma data
            var jaTemReserva = await _reservaRepository.ExisteReservaAtivaAsync(idUsuario, createReservaDto.IdParque, createReservaDto.DataVisita);
            if (jaTemReserva)
                throw new InvalidOperationException("Você já possui uma reserva ativa para este parque nesta data");

            // Criar a reserva
            var reserva = new Reserva
            {
                IdUsuario = idUsuario,
                IdParque = createReservaDto.IdParque,
                IdTrilha = createReservaDto.IdTrilha,
                DataVisita = createReservaDto.DataVisita,
                HorarioEntrada = createReservaDto.HorarioEntrada,
                NumeroVisitantes = createReservaDto.NumeroVisitantes,
                Status = "ativa",
                DataCriacao = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };

            var reservaCriada = await _reservaRepository.CreateReservaAsync(reserva);
            var reservaCompleta = await _reservaRepository.GetReservaByIdAsync(reservaCriada.IdReserva);
            
            return MapToReservaResponseDto(reservaCompleta!);
        }

        public async Task<ReservaResponseDto> UpdateReservaAsync(int idReserva, UpdateReservaDto updateReservaDto)
        {
            var reserva = await _reservaRepository.GetReservaByIdAsync(idReserva);
            if (reserva == null)
                throw new ArgumentException("Reserva não encontrada");

            if (reserva.Status == "cancelada")
                throw new InvalidOperationException("Não é possível alterar uma reserva cancelada");

            if (reserva.CheckIn.HasValue)
                throw new InvalidOperationException("Não é possível alterar uma reserva após o check-in");

            // Se está alterando a data ou número de visitantes, validar disponibilidade
            if (updateReservaDto.DataVisita.HasValue || updateReservaDto.NumeroVisitantes.HasValue)
            {
                var dataVisita = updateReservaDto.DataVisita ?? reserva.DataVisita;
                var numeroVisitantes = updateReservaDto.NumeroVisitantes ?? reserva.NumeroVisitantes;

                var validacao = await ValidarReservaAsync(new ValidarReservaDto
                {
                    IdParque = reserva.IdParque,
                    IdTrilha = reserva.IdTrilha,
                    DataVisita = dataVisita,
                    NumeroVisitantes = numeroVisitantes
                });

                if (!validacao.Disponivel)
                    throw new InvalidOperationException($"Alteração não disponível: {validacao.MotivoIndisponibilidade}");
            }

            // Aplicar as alterações
            if (updateReservaDto.DataVisita.HasValue)
                reserva.DataVisita = updateReservaDto.DataVisita.Value;

            if (updateReservaDto.HorarioEntrada.HasValue)
                reserva.HorarioEntrada = updateReservaDto.HorarioEntrada.Value;

            if (updateReservaDto.NumeroVisitantes.HasValue)
                reserva.NumeroVisitantes = updateReservaDto.NumeroVisitantes.Value;

            if (!string.IsNullOrEmpty(updateReservaDto.Status))
                reserva.Status = updateReservaDto.Status;

            var reservaAtualizada = await _reservaRepository.UpdateReservaAsync(reserva);
            return MapToReservaResponseDto(reservaAtualizada);
        }

        public async Task<bool> DeleteReservaAsync(int idReserva)
        {
            var reserva = await _reservaRepository.GetReservaByIdAsync(idReserva);
            if (reserva == null)
                return false;

            if (reserva.CheckIn.HasValue)
                throw new InvalidOperationException("Não é possível excluir uma reserva após o check-in");

            return await _reservaRepository.DeleteReservaAsync(idReserva);
        }

        public async Task<IEnumerable<ReservaResumoDto>> GetMinhasReservasAsync(int idUsuario)
        {
            var reservas = await _reservaRepository.GetReservasByUsuarioAsync(idUsuario);
            return reservas.Select(r => new ReservaResumoDto
            {
                IdReserva = r.IdReserva,
                NomeParque = r.Parque.NomeParque,
                NomeTrilha = r.Trilha?.NomeTrilha,
                DataVisita = r.DataVisita,
                Status = r.Status,
                NumeroVisitantes = r.NumeroVisitantes,
                DataCriacao = r.DataCriacao
            });
        }

        public async Task<ReservaResponseDto> CheckInAsync(int idReserva)
        {
            var reserva = await _reservaRepository.GetReservaByIdAsync(idReserva);
            if (reserva == null)
                throw new ArgumentException("Reserva não encontrada");

            if (reserva.Status != "ativa")
                throw new InvalidOperationException("Apenas reservas ativas podem fazer check-in");

            if (reserva.CheckIn.HasValue)
                throw new InvalidOperationException("Check-in já foi realizado");

            if (reserva.DataVisita != DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("Check-in só pode ser realizado na data da visita");

            reserva.CheckIn = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            
            var reservaAtualizada = await _reservaRepository.UpdateReservaAsync(reserva);
            return MapToReservaResponseDto(reservaAtualizada);
        }

        public async Task<ReservaResponseDto> CheckOutAsync(int idReserva)
        {
            var reserva = await _reservaRepository.GetReservaByIdAsync(idReserva);
            if (reserva == null)
                throw new ArgumentException("Reserva não encontrada");

            if (!reserva.CheckIn.HasValue)
                throw new InvalidOperationException("Check-in deve ser realizado antes do check-out");

            if (reserva.CheckOut.HasValue)
                throw new InvalidOperationException("Check-out já foi realizado");

            reserva.CheckOut = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            reserva.Status = "finalizada";
            
            var reservaAtualizada = await _reservaRepository.UpdateReservaAsync(reserva);
            return MapToReservaResponseDto(reservaAtualizada);
        }

        public async Task<ValidarReservaResponseDto> ValidarReservaAsync(ValidarReservaDto validarReservaDto)
        {
            var parque = await _parqueRepository.GetParqueByIdAsync(validarReservaDto.IdParque);
            if (parque == null)
            {
                return new ValidarReservaResponseDto
                {
                    Disponivel = false,
                    MotivoIndisponibilidade = "Parque não encontrado",
                    VagasDisponiveis = 0,
                    CapacidadeMaxima = 0
                };
            }

            if (!parque.Ativo)
            {
                return new ValidarReservaResponseDto
                {
                    Disponivel = false,
                    MotivoIndisponibilidade = "Parque está inativo",
                    VagasDisponiveis = 0,
                    CapacidadeMaxima = parque.CapacidadeMaxima ?? 0
                };
            }

            // Verificar se a data é no passado
            if (validarReservaDto.DataVisita < DateOnly.FromDateTime(DateTime.Today))
            {
                return new ValidarReservaResponseDto
                {
                    Disponivel = false,
                    MotivoIndisponibilidade = "Não é possível fazer reserva para datas passadas",
                    VagasDisponiveis = 0,
                    CapacidadeMaxima = parque.CapacidadeMaxima ?? 0
                };
            }

            int capacidadeMaxima = parque.CapacidadeMaxima ?? 0;
            string contexto = "parque";

            // Se há trilha específica, validar a capacidade da trilha
            if (validarReservaDto.IdTrilha.HasValue)
            {
                var trilha = await _trilhaRepository.GetTrilhaByIdAsync(validarReservaDto.IdTrilha.Value);
                if (trilha == null)
                {
                    return new ValidarReservaResponseDto
                    {
                        Disponivel = false,
                        MotivoIndisponibilidade = "Trilha não encontrada",
                        VagasDisponiveis = 0,
                        CapacidadeMaxima = 0
                    };
                }

                if (!trilha.Ativo)
                {
                    return new ValidarReservaResponseDto
                    {
                        Disponivel = false,
                        MotivoIndisponibilidade = "Trilha está inativa",
                        VagasDisponiveis = 0,
                        CapacidadeMaxima = trilha.CapacidadeMaxima ?? 0
                    };
                }

                capacidadeMaxima = trilha.CapacidadeMaxima ?? 0;
                contexto = "trilha";
            }

            // Calcular ocupação atual
            var visitantesJaReservados = await _reservaRepository.GetNumeroVisitantesPorDataAsync(
                validarReservaDto.IdParque, 
                validarReservaDto.DataVisita, 
                validarReservaDto.IdTrilha);

            var vagasDisponiveis = capacidadeMaxima - visitantesJaReservados;
            var disponivel = vagasDisponiveis >= validarReservaDto.NumeroVisitantes;

            var alertas = new List<string>();
            
            // Adicionar alertas se a ocupação estiver alta
            var percentualOcupacao = (double)(visitantesJaReservados + validarReservaDto.NumeroVisitantes) / capacidadeMaxima * 100;
            if (percentualOcupacao >= 80)
            {
                alertas.Add($"Ocupação alta: {percentualOcupacao:F1}% da capacidade do {contexto}");
            }

            return new ValidarReservaResponseDto
            {
                Disponivel = disponivel,
                MotivoIndisponibilidade = disponivel ? null : $"Vagas insuficientes no {contexto}. Disponível: {vagasDisponiveis}, Solicitado: {validarReservaDto.NumeroVisitantes}",
                VagasDisponiveis = vagasDisponiveis,
                CapacidadeMaxima = capacidadeMaxima,
                Alertas = alertas.Any() ? alertas : null
            };
        }

        public async Task<IEnumerable<ReservaCalendarioDto>> GetCalendarioReservasAsync(int idParque, DateOnly dataInicio, DateOnly dataFim)
        {
            var reservas = await _reservaRepository.GetReservasPorCalendarioAsync(idParque, dataInicio, dataFim);
            
            var reservasPorData = reservas
                .GroupBy(r => r.DataVisita)
                .ToDictionary(g => g.Key, g => g.ToList());

            var calendario = new List<ReservaCalendarioDto>();
            
            for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
            {
                var reservasDoDia = reservasPorData.GetValueOrDefault(data, new List<Reserva>());
                
                calendario.Add(new ReservaCalendarioDto
                {
                    Data = data,
                    TotalReservas = reservasDoDia.Count,
                    TotalVisitantes = reservasDoDia.Sum(r => r.NumeroVisitantes),
                    Reservas = reservasDoDia.Select(r => new ReservaResumoDto
                    {
                        IdReserva = r.IdReserva,
                        NomeParque = r.Parque.NomeParque,
                        NomeTrilha = r.Trilha?.NomeTrilha,
                        DataVisita = r.DataVisita,
                        Status = r.Status,
                        NumeroVisitantes = r.NumeroVisitantes,
                        DataCriacao = r.DataCriacao
                    }).ToList()
                });
            }

            return calendario;
        }

        public async Task<ReservaResponseDto> CancelarReservaAsync(int idReserva, int idUsuario)
        {
            var reserva = await _reservaRepository.GetReservaByIdAsync(idReserva);
            if (reserva == null)
                throw new ArgumentException("Reserva não encontrada");

            if (reserva.IdUsuario != idUsuario)
                throw new UnauthorizedAccessException("Você não tem permissão para cancelar esta reserva");

            if (reserva.Status == "cancelada")
                throw new InvalidOperationException("Reserva já está cancelada");

            if (reserva.CheckIn.HasValue)
                throw new InvalidOperationException("Não é possível cancelar uma reserva após o check-in");

            reserva.Status = "cancelada";
            
            var reservaAtualizada = await _reservaRepository.UpdateReservaAsync(reserva);
            return MapToReservaResponseDto(reservaAtualizada);
        }

        public async Task<bool> PodeRealizarReservaAsync(int idUsuario, int idParque, DateOnly dataVisita)
        {
            return !await _reservaRepository.ExisteReservaAtivaAsync(idUsuario, idParque, dataVisita);
        }

        private static ReservaResponseDto MapToReservaResponseDto(Reserva reserva)
        {
            return new ReservaResponseDto
            {
                IdReserva = reserva.IdReserva,
                IdUsuario = reserva.IdUsuario,
                NomeUsuario = reserva.Usuario?.NomeUsuario ?? string.Empty,
                IdParque = reserva.IdParque,
                NomeParque = reserva.Parque?.NomeParque ?? string.Empty,
                IdTrilha = reserva.IdTrilha,
                NomeTrilha = reserva.Trilha?.NomeTrilha,
                DataVisita = reserva.DataVisita,
                HorarioEntrada = reserva.HorarioEntrada,
                NumeroVisitantes = reserva.NumeroVisitantes,
                Status = reserva.Status,
                CheckIn = reserva.CheckIn,
                CheckOut = reserva.CheckOut,
                DataCriacao = reserva.DataCriacao
            };
        }
    }
}
