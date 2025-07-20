using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using BaitacaConnect.Repositories.Interfaces;

namespace BaitacaConnect.Services
{
    public class ParqueService : IParqueService
    {
        private readonly IParqueRepository _parqueRepository;

        public ParqueService(IParqueRepository parqueRepository)
        {
            _parqueRepository = parqueRepository;
        }

        public async Task<IEnumerable<ParqueResumoDto>> GetParquesAsync(string? filtroNome, bool? ativo)
        {
            var parques = await _parqueRepository.GetWithFiltersAsync(filtroNome, ativo);

            return parques.Select(p => new ParqueResumoDto
            {
                IdParque = p.IdParque,
                NomeParque = p.NomeParque,
                EnderecoParque = p.EnderecoParque,
                CapacidadeMaxima = p.CapacidadeMaxima,
                Ativo = p.Ativo,
                TotalTrilhas = p.Trilhas.Count,
                TotalReservas = p.Reservas.Count
            }).ToList();
        }

        public async Task<ParqueResponseDto?> GetParqueByIdAsync(int id)
        {
            var parque = await _parqueRepository.GetByIdWithDetailsAsync(id);

            if (parque == null)
                return null;

            return new ParqueResponseDto
            {
                IdParque = parque.IdParque,
                NomeParque = parque.NomeParque,
                DescricaoParque = parque.DescricaoParque,
                EnderecoParque = parque.EnderecoParque,
                CapacidadeMaxima = parque.CapacidadeMaxima,
                HorarioFuncionamento = parque.HorarioFuncionamento,
                CoordenadasParque = parque.CoordenadasParque,
                Ativo = parque.Ativo,
                TotalTrilhas = parque.Trilhas.Count,
                TrilhasAtivas = parque.Trilhas.Count(t => t.Ativo),
                TotalReservas = parque.Reservas.Count,
                ReservasAtivas = parque.Reservas.Count(r => r.Status == "ativa"),
                TotalPontosInteresse = parque.PontosInteresse.Count
            };
        }

        public async Task<ParqueResponseDto> CriarParqueAsync(CreateParqueDto createParqueDto)
        {
            try
            {
                if (await _parqueRepository.NomeExistsAsync(createParqueDto.NomeParque))
                {
                    throw new InvalidOperationException("Já existe um parque com este nome");
                }

                var parque = new Parque
                {
                    NomeParque = createParqueDto.NomeParque,
                    DescricaoParque = createParqueDto.DescricaoParque,
                    EnderecoParque = createParqueDto.EnderecoParque,
                    CapacidadeMaxima = createParqueDto.CapacidadeMaxima,
                    HorarioFuncionamento = createParqueDto.HorarioFuncionamento,
                    CoordenadasParque = ValidarEFormatarCoordenadas(createParqueDto.CoordenadasParque),
                    Ativo = true
                };

                var parqueCriado = await _parqueRepository.CreateAsync(parque);

                return new ParqueResponseDto
                {
                    IdParque = parqueCriado.IdParque,
                    NomeParque = parqueCriado.NomeParque,
                    DescricaoParque = parqueCriado.DescricaoParque,
                    EnderecoParque = parqueCriado.EnderecoParque,
                    CapacidadeMaxima = parqueCriado.CapacidadeMaxima,
                    HorarioFuncionamento = parqueCriado.HorarioFuncionamento,
                    CoordenadasParque = parqueCriado.CoordenadasParque,
                    Ativo = parqueCriado.Ativo,
                    TotalTrilhas = 0,
                    TrilhasAtivas = 0,
                    TotalReservas = 0,
                    ReservasAtivas = 0,
                    TotalPontosInteresse = 0
                };
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao criar parque: {ex.Message}", ex);
            }
        }

        public async Task<bool> AtualizarParqueAsync(int id, UpdateParqueDto updateParqueDto)
        {
            var parque = await _parqueRepository.GetByIdAsync(id);

            if (parque == null)
                return false;

            if (!string.IsNullOrEmpty(updateParqueDto.NomeParque) && 
                updateParqueDto.NomeParque != parque.NomeParque)
            {
                if (await _parqueRepository.NomeExistsAsync(updateParqueDto.NomeParque))
                {
                    throw new InvalidOperationException("Já existe um parque com este nome");
                }
            }

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrEmpty(updateParqueDto.NomeParque))
                parque.NomeParque = updateParqueDto.NomeParque;
            
            if (updateParqueDto.DescricaoParque != null)
                parque.DescricaoParque = updateParqueDto.DescricaoParque;
            
            if (updateParqueDto.EnderecoParque != null)
                parque.EnderecoParque = updateParqueDto.EnderecoParque;
            
            if (updateParqueDto.CapacidadeMaxima.HasValue)
                parque.CapacidadeMaxima = updateParqueDto.CapacidadeMaxima;
            
            if (updateParqueDto.HorarioFuncionamento != null)
                parque.HorarioFuncionamento = updateParqueDto.HorarioFuncionamento;
            
            if (updateParqueDto.CoordenadasParque != null)
                parque.CoordenadasParque = updateParqueDto.CoordenadasParque;
            
            if (updateParqueDto.Ativo.HasValue)
                parque.Ativo = updateParqueDto.Ativo.Value;

            try
            {
                await _parqueRepository.UpdateAsync(parque);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AtivarParqueAsync(int id)
        {
            var parque = await _parqueRepository.GetByIdAsync(id);

            if (parque == null)
                return false;

            parque.Ativo = true;

            try
            {
                await _parqueRepository.UpdateAsync(parque);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DesativarParqueAsync(int id)
        {
            var parque = await _parqueRepository.GetByIdAsync(id);

            if (parque == null)
                return false;

            parque.Ativo = false;

            try
            {
                await _parqueRepository.UpdateAsync(parque);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ParqueExisteAsync(int id)
        {
            return await _parqueRepository.ExistsAsync(id);
        }

        public async Task<bool> ParqueAtivoAsync(int id)
        {
            return await _parqueRepository.IsActiveAsync(id);
        }

        public async Task<bool> NomeParqueExisteAsync(string nome)
        {
            return await _parqueRepository.NomeExistsAsync(nome);
        }

        public async Task<int> GetCapacidadeParqueAsync(int id)
        {
            return await _parqueRepository.GetCapacidadeAsync(id);
        }

        public async Task<IEnumerable<ParqueComTrilhasDto>> GetParquesComTrilhasAsync()
        {
            var parques = await _parqueRepository.GetAtivasWithTrilhasAsync();

            return parques.Select(p => new ParqueComTrilhasDto
            {
                IdParque = p.IdParque,
                NomeParque = p.NomeParque,
                DescricaoParque = p.DescricaoParque,
                EnderecoParque = p.EnderecoParque,
                CapacidadeMaxima = p.CapacidadeMaxima,
                HorarioFuncionamento = p.HorarioFuncionamento,
                Trilhas = p.Trilhas.Where(t => t.Ativo).Select(t => new TrilhaResumoDto
                {
                    IdTrilha = t.IdTrilha,
                    NomeTrilha = t.NomeTrilha,
                    DificuldadeTrilha = t.DificuldadeTrilha,
                    DistanciaKm = t.DistanciaKm,
                    TempoEstimadoMinutos = t.TempoEstimadoMinutos,
                    CapacidadeMaxima = t.CapacidadeMaxima,
                    Ativo = t.Ativo,
                    OcupacaoAtual = 0,
                    Disponivel = true
                }).ToList()
            }).ToList();
        }

        public async Task<EstatisticasParqueDto> GetEstatisticasParqueAsync(int id)
        {
            var parque = await _parqueRepository.GetByIdWithDetailsAsync(id);

            if (parque == null)
                throw new ArgumentException("Parque não encontrado");

            var agora = DateTime.Now.Date;
            var inicioMes = new DateTime(agora.Year, agora.Month, 1);

            return new EstatisticasParqueDto
            {
                IdParque = parque.IdParque,
                NomeParque = parque.NomeParque,
                TotalTrilhas = parque.Trilhas.Count,
                TrilhasAtivas = parque.Trilhas.Count(t => t.Ativo),
                TotalReservas = parque.Reservas.Count,
                ReservasAtivas = parque.Reservas.Count(r => r.Status == "ativa"),
                ReservasHoje = parque.Reservas.Count(r => DateOnly.FromDateTime(r.DataVisita.ToDateTime(TimeOnly.MinValue)) == DateOnly.FromDateTime(agora)),
                ReservasMesAtual = parque.Reservas.Count(r => r.DataVisita.ToDateTime(TimeOnly.MinValue) >= inicioMes),
                TotalPontosInteresse = parque.PontosInteresse.Count,
                CapacidadeMaxima = parque.CapacidadeMaxima ?? 0,
                TaxaOcupacaoMedia = CalcularTaxaOcupacaoMedia(parque.Reservas.ToList(), parque.CapacidadeMaxima ?? 0)
            };
        }

        private decimal CalcularTaxaOcupacaoMedia(IList<Reserva> reservas, int capacidadeMaxima)
        {
            if (capacidadeMaxima == 0 || !reservas.Any())
                return 0;

            var totalVisitantes = reservas.Where(r => r.Status == "ativa").Sum(r => r.NumeroVisitantes);
            var totalDias = reservas.Select(r => r.DataVisita).Distinct().Count();

            if (totalDias == 0)
                return 0;

            var mediaVisitantesPorDia = (decimal)totalVisitantes / totalDias;
            return Math.Round((mediaVisitantesPorDia / capacidadeMaxima) * 100, 2);
        }

        private string? ValidarEFormatarCoordenadas(string? coordenadas)
        {
            if (string.IsNullOrEmpty(coordenadas))
                return null;

            // Se já está no formato correto do PostgreSQL, retorna como está
            if (coordenadas.StartsWith("(") && coordenadas.EndsWith(")"))
                return coordenadas;

            // Tenta converter diferentes formatos para o formato do PostgreSQL
            try
            {
                // Remove espaços e caracteres especiais
                var coords = coordenadas.Replace(" ", "").Replace("(", "").Replace(")", "");
                
                // Se tem vírgula, assume formato "lat,lng"
                if (coords.Contains(","))
                {
                    var partes = coords.Split(',');
                    if (partes.Length == 2)
                    {
                        if (decimal.TryParse(partes[0], out var lat) && decimal.TryParse(partes[1], out var lng))
                        {
                            return $"({lat},{lng})";
                        }
                    }
                }
                
                // Se não conseguir converter, retorna null para não dar erro no banco
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
