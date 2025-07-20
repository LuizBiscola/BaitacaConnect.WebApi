using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;
using BaitacaConnect.Repositories.Interfaces;

namespace BaitacaConnect.Services
{
    public class TrilhaService : ITrilhaService
    {
        private readonly ITrilhaRepository _trilhaRepository;
        private readonly IParqueRepository _parqueRepository;

        public TrilhaService(ITrilhaRepository trilhaRepository, IParqueRepository parqueRepository)
        {
            _trilhaRepository = trilhaRepository;
            _parqueRepository = parqueRepository;
        }

        public async Task<IEnumerable<TrilhaResumoDto>> GetTrilhasAsync(string? filtroNome, string? filtroDificuldade, bool? ativo, int? idParque)
        {
            var trilhas = await _trilhaRepository.GetWithFiltersAsync(filtroNome, filtroDificuldade, ativo, idParque);

            var trilhasDto = new List<TrilhaResumoDto>();
            foreach (var t in trilhas)
            {
                trilhasDto.Add(new TrilhaResumoDto
                {
                    IdTrilha = t.IdTrilha,
                    NomeTrilha = t.NomeTrilha,
                    DificuldadeTrilha = t.DificuldadeTrilha,
                    DistanciaKm = t.DistanciaKm,
                    TempoEstimadoMinutos = t.TempoEstimadoMinutos,
                    CapacidadeMaxima = t.CapacidadeMaxima,
                    Ativo = t.Ativo,
                    OcupacaoAtual = await CalcularOcupacaoAtual(t.IdTrilha),
                    Disponivel = t.Ativo
                });
            }
            
            return trilhasDto;
        }

        public async Task<TrilhaResponseDto?> GetTrilhaByIdAsync(int id)
        {
            var trilha = await _trilhaRepository.GetByIdAsync(id);

            if (trilha == null)
                return null;

            var agora = DateTime.Now.Date;

            return new TrilhaResponseDto
            {
                IdTrilha = trilha.IdTrilha,
                IdParque = trilha.IdParque,
                NomeParque = trilha.Parque.NomeParque,
                NomeTrilha = trilha.NomeTrilha,
                DescricaoTrilha = trilha.DescricaoTrilha,
                DificuldadeTrilha = trilha.DificuldadeTrilha,
                DistanciaKm = trilha.DistanciaKm,
                TempoEstimadoMinutos = trilha.TempoEstimadoMinutos,
                CapacidadeMaxima = trilha.CapacidadeMaxima,
                CoordenadasTrilha = trilha.CoordenadasTrilha,
                Ativo = trilha.Ativo,
                TotalPontosInteresse = trilha.PontosInteresse.Count,
                TotalEspeciesFaunaFlora = 0,
                ReservasHoje = trilha.Reservas.Count(r => DateOnly.FromDateTime(r.DataVisita.ToDateTime(TimeOnly.MinValue)) == DateOnly.FromDateTime(agora)),
                OcupacaoAtual = await CalcularOcupacaoAtual(trilha.IdTrilha)
            };
        }

        public async Task<TrilhaResponseDto> CriarTrilhaAsync(CreateTrilhaDto createTrilhaDto)
        {
            try
            {
                if (!await _parqueRepository.ExistsAsync(createTrilhaDto.IdParque))
                {
                    throw new InvalidOperationException("Parque não encontrado");
                }

                if (await _trilhaRepository.NomeExistsInParqueAsync(createTrilhaDto.NomeTrilha, createTrilhaDto.IdParque))
                {
                    throw new InvalidOperationException("Já existe uma trilha com este nome no parque");
                }

                var trilha = new Trilha
                {
                    IdParque = createTrilhaDto.IdParque,
                    NomeTrilha = createTrilhaDto.NomeTrilha,
                    DescricaoTrilha = createTrilhaDto.DescricaoTrilha,
                    DificuldadeTrilha = createTrilhaDto.DificuldadeTrilha,
                    DistanciaKm = createTrilhaDto.DistanciaKm,
                    TempoEstimadoMinutos = createTrilhaDto.TempoEstimadoMinutos,
                    CapacidadeMaxima = createTrilhaDto.CapacidadeMaxima,
                    CoordenadasTrilha = ValidarEFormatarCoordenadas(createTrilhaDto.CoordenadasTrilha),
                    Ativo = true
                };

                var trilhaCriada = await _trilhaRepository.CreateAsync(trilha);
                var parque = await _parqueRepository.GetByIdAsync(createTrilhaDto.IdParque);

                return new TrilhaResponseDto
                {
                    IdTrilha = trilhaCriada.IdTrilha,
                    IdParque = trilhaCriada.IdParque,
                    NomeParque = parque?.NomeParque ?? "",
                    NomeTrilha = trilhaCriada.NomeTrilha,
                    DescricaoTrilha = trilhaCriada.DescricaoTrilha,
                    DificuldadeTrilha = trilhaCriada.DificuldadeTrilha,
                    DistanciaKm = trilhaCriada.DistanciaKm,
                    TempoEstimadoMinutos = trilhaCriada.TempoEstimadoMinutos,
                    CapacidadeMaxima = trilhaCriada.CapacidadeMaxima,
                    CoordenadasTrilha = trilhaCriada.CoordenadasTrilha,
                    Ativo = trilhaCriada.Ativo,
                    TotalPontosInteresse = 0,
                    TotalEspeciesFaunaFlora = 0,
                    ReservasHoje = 0,
                    OcupacaoAtual = 0
                };
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao criar trilha: {ex.Message}", ex);
            }
        }

        public async Task<bool> AtualizarTrilhaAsync(int id, UpdateTrilhaDto updateTrilhaDto)
        {
            var trilha = await _trilhaRepository.GetByIdAsync(id);

            if (trilha == null)
                return false;

            if (!string.IsNullOrEmpty(updateTrilhaDto.NomeTrilha) && 
                updateTrilhaDto.NomeTrilha != trilha.NomeTrilha)
            {
                if (await _trilhaRepository.NomeExistsInParqueAsync(updateTrilhaDto.NomeTrilha, trilha.IdParque))
                {
                    throw new InvalidOperationException("Já existe uma trilha com este nome no parque");
                }
            }

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrEmpty(updateTrilhaDto.NomeTrilha))
                trilha.NomeTrilha = updateTrilhaDto.NomeTrilha;
            
            if (updateTrilhaDto.DescricaoTrilha != null)
                trilha.DescricaoTrilha = updateTrilhaDto.DescricaoTrilha;
            
            if (!string.IsNullOrEmpty(updateTrilhaDto.DificuldadeTrilha))
                trilha.DificuldadeTrilha = updateTrilhaDto.DificuldadeTrilha;
            
            if (updateTrilhaDto.DistanciaKm.HasValue)
                trilha.DistanciaKm = updateTrilhaDto.DistanciaKm;
            
            if (updateTrilhaDto.TempoEstimadoMinutos.HasValue)
                trilha.TempoEstimadoMinutos = updateTrilhaDto.TempoEstimadoMinutos;
            
            if (updateTrilhaDto.CapacidadeMaxima.HasValue)
                trilha.CapacidadeMaxima = updateTrilhaDto.CapacidadeMaxima;
            
            if (updateTrilhaDto.CoordenadasTrilha != null)
                trilha.CoordenadasTrilha = ValidarEFormatarCoordenadas(updateTrilhaDto.CoordenadasTrilha);
            
            if (updateTrilhaDto.Ativo.HasValue)
                trilha.Ativo = updateTrilhaDto.Ativo.Value;

            try
            {
                await _trilhaRepository.UpdateAsync(trilha);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AtivarTrilhaAsync(int id)
        {
            var trilha = await _trilhaRepository.GetByIdAsync(id);

            if (trilha == null)
                return false;

            trilha.Ativo = true;

            try
            {
                await _trilhaRepository.UpdateAsync(trilha);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DesativarTrilhaAsync(int id)
        {
            var trilha = await _trilhaRepository.GetByIdAsync(id);

            if (trilha == null)
                return false;

            trilha.Ativo = false;

            try
            {
                await _trilhaRepository.UpdateAsync(trilha);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TrilhaExisteAsync(int id)
        {
            return await _trilhaRepository.ExistsAsync(id);
        }

        public async Task<bool> TrilhaAtivaAsync(int id)
        {
            return await _trilhaRepository.IsActiveAsync(id);
        }

        public async Task<bool> NomeTrilhaExisteNoParqueAsync(string nome, int idParque)
        {
            return await _trilhaRepository.NomeExistsInParqueAsync(nome, idParque);
        }

        public async Task<int> GetCapacidadeTrilhaAsync(int id)
        {
            return await _trilhaRepository.GetCapacidadeAsync(id);
        }

        public async Task<IEnumerable<TrilhaResumoDto>> GetTrilhasPorParqueAsync(int idParque)
        {
            var trilhas = await _trilhaRepository.GetByParqueAsync(idParque);

            var trilhasDto = new List<TrilhaResumoDto>();
            foreach (var t in trilhas)
            {
                trilhasDto.Add(new TrilhaResumoDto
                {
                    IdTrilha = t.IdTrilha,
                    NomeTrilha = t.NomeTrilha,
                    DificuldadeTrilha = t.DificuldadeTrilha,
                    DistanciaKm = t.DistanciaKm,
                    TempoEstimadoMinutos = t.TempoEstimadoMinutos,
                    CapacidadeMaxima = t.CapacidadeMaxima,
                    Ativo = t.Ativo,
                    OcupacaoAtual = await CalcularOcupacaoAtual(t.IdTrilha),
                    Disponivel = t.Ativo
                });
            }
            
            return trilhasDto;
        }

        public async Task<IEnumerable<TrilhaComMapaDto>> GetTrilhasComMapaAsync(int? idParque = null)
        {
            IEnumerable<Trilha> trilhas;
            
            if (idParque.HasValue)
            {
                trilhas = await _trilhaRepository.GetByParqueAsync(idParque.Value);
                trilhas = trilhas.Where(t => t.Ativo);
            }
            else
            {
                trilhas = await _trilhaRepository.GetAtivasAsync();
            }

            return trilhas.Select(t => new TrilhaComMapaDto
            {
                IdTrilha = t.IdTrilha,
                NomeTrilha = t.NomeTrilha,
                DescricaoTrilha = t.DescricaoTrilha,
                DificuldadeTrilha = t.DificuldadeTrilha,
                DistanciaKm = t.DistanciaKm,
                TempoEstimadoMinutos = t.TempoEstimadoMinutos,
                CoordenadasTrilha = t.CoordenadasTrilha,
                PontosInteresse = t.PontosInteresse.Select(pi => new PontoInteresseResumoDto
                {
                    IdParque = pi.IdParque,
                    IdTrilha = pi.IdTrilha,
                    NomePontoInteresse = pi.NomePontoInteresse,
                    DescricaoPontoInteresse = pi.DescricaoPontoInteresse,
                    Tipo = pi.Tipo,
                    Coordenadas = pi.Coordenadas
                }).ToList()
            }).ToList();
        }

        public async Task<bool> ValidarCapacidadeTrilhaAsync(int id, int numeroVisitantes)
        {
            var capacidade = await _trilhaRepository.GetCapacidadeAsync(id);
            
            if (capacidade == 0)
                return true;

            var ocupacaoAtual = await CalcularOcupacaoAtual(id);
            return (ocupacaoAtual + numeroVisitantes) <= capacidade;
        }

        public async Task<EstatisticasTrilhaDto> GetEstatisticasTrilhaAsync(int id)
        {
            var trilha = await _trilhaRepository.GetByIdAsync(id);

            if (trilha == null)
                throw new ArgumentException("Trilha não encontrada");

            var agora = DateTime.Now.Date;
            var inicioMes = new DateTime(agora.Year, agora.Month, 1);

            return new EstatisticasTrilhaDto
            {
                IdTrilha = trilha.IdTrilha,
                NomeTrilha = trilha.NomeTrilha,
                NomeParque = trilha.Parque.NomeParque,
                TotalReservas = trilha.Reservas.Count,
                ReservasAtivas = trilha.Reservas.Count(r => r.Status == "ativa"),
                ReservasHoje = trilha.Reservas.Count(r => DateOnly.FromDateTime(r.DataVisita.ToDateTime(TimeOnly.MinValue)) == DateOnly.FromDateTime(agora)),
                ReservasMesAtual = trilha.Reservas.Count(r => r.DataVisita.ToDateTime(TimeOnly.MinValue) >= inicioMes),
                TotalPontosInteresse = trilha.PontosInteresse.Count,
                TotalEspeciesFaunaFlora = 0,
                CapacidadeMaxima = trilha.CapacidadeMaxima ?? 0,
                TaxaOcupacaoMedia = CalcularTaxaOcupacaoMedia(trilha.Reservas.ToList(), trilha.CapacidadeMaxima ?? 0),
                DistanciaKm = trilha.DistanciaKm,
                TempoEstimadoMinutos = trilha.TempoEstimadoMinutos,
                DificuldadeTrilha = trilha.DificuldadeTrilha
            };
        }

        public async Task<IEnumerable<TrilhaResumoDto>> GetTrilhasPorDificuldadeAsync(string dificuldade)
        {
            var trilhas = await _trilhaRepository.GetByDificuldadeAsync(dificuldade);

            var trilhasDto = new List<TrilhaResumoDto>();
            foreach (var t in trilhas)
            {
                trilhasDto.Add(new TrilhaResumoDto
                {
                    IdTrilha = t.IdTrilha,
                    NomeTrilha = t.NomeTrilha,
                    DificuldadeTrilha = t.DificuldadeTrilha,
                    DistanciaKm = t.DistanciaKm,
                    TempoEstimadoMinutos = t.TempoEstimadoMinutos,
                    CapacidadeMaxima = t.CapacidadeMaxima,
                    Ativo = t.Ativo,
                    OcupacaoAtual = await CalcularOcupacaoAtual(t.IdTrilha),
                    Disponivel = true
                });
            }
            
            return trilhasDto;
        }

        public async Task<IEnumerable<TrilhaResumoDto>> GetTrilhasDisponiveisAsync(DateOnly data, int numeroVisitantes)
        {
            var trilhas = await _trilhaRepository.GetWithReservasByDateAsync(data);

            var trilhasDisponiveis = new List<TrilhaResumoDto>();

            foreach (var trilha in trilhas)
            {
                var reservasNaData = trilha.Reservas
                    .Where(r => r.DataVisita == data && r.Status == "ativa")
                    .Sum(r => r.NumeroVisitantes);

                var capacidadeDisponivel = (trilha.CapacidadeMaxima ?? int.MaxValue) - reservasNaData;

                if (capacidadeDisponivel >= numeroVisitantes)
                {
                    trilhasDisponiveis.Add(new TrilhaResumoDto
                    {
                        IdTrilha = trilha.IdTrilha,
                        NomeTrilha = trilha.NomeTrilha,
                        DificuldadeTrilha = trilha.DificuldadeTrilha,
                        DistanciaKm = trilha.DistanciaKm,
                        TempoEstimadoMinutos = trilha.TempoEstimadoMinutos,
                        CapacidadeMaxima = trilha.CapacidadeMaxima,
                        Ativo = trilha.Ativo,
                        OcupacaoAtual = reservasNaData,
                        Disponivel = true
                    });
                }
            }

            return trilhasDisponiveis.OrderBy(t => t.NomeTrilha);
        }

        // Métodos auxiliares
        private async Task<int> CalcularOcupacaoAtual(int idTrilha)
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            var trilhas = await _trilhaRepository.GetWithReservasByDateAsync(hoje);
            var trilha = trilhas.FirstOrDefault(t => t.IdTrilha == idTrilha);
            
            if (trilha == null) return 0;
            
            return trilha.Reservas
                .Where(r => r.DataVisita == hoje && r.Status == "ativa")
                .Sum(r => r.NumeroVisitantes);
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

            try
            {
                // Se for um JSON válido, retorna como está
                if (coordenadas.StartsWith("{") && coordenadas.EndsWith("}"))
                {
                    // Tenta validar se é um JSON válido
                    System.Text.Json.JsonDocument.Parse(coordenadas);
                    return coordenadas;
                }

                // Se for coordenadas no formato lat,lng, converte para JSON
                var coords = coordenadas.Replace(" ", "").Replace("(", "").Replace(")", "");
                
                if (coords.Contains(","))
                {
                    var partes = coords.Split(',');
                    if (partes.Length == 2)
                    {
                        if (decimal.TryParse(partes[0], out var lat) && decimal.TryParse(partes[1], out var lng))
                        {
                            return $"{{\"latitude\": {lat}, \"longitude\": {lng}}}";
                        }
                    }
                }
                
                // Se for uma string simples, cria um JSON básico
                return $"{{\"descricao\": \"{coordenadas}\"}}";
            }
            catch
            {
                // Se não conseguir processar, retorna um JSON com a string original
                return $"{{\"dados\": \"{coordenadas}\"}}";
            }
        }
    }
}
