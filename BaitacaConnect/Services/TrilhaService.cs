using Microsoft.EntityFrameworkCore;
using BaitacaConnect.Data;
using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Services.Interfaces;

namespace BaitacaConnect.Services
{
    public class TrilhaService : ITrilhaService
    {
        private readonly BaitacaDbContext _context;

        public TrilhaService(BaitacaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrilhaResumoDto>> GetTrilhasAsync(string? filtroNome, string? filtroDificuldade, bool? ativo, int? idParque)
        {
            var query = _context.Trilhas
                .Include(t => t.Parque)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtroNome))
            {
                query = query.Where(t => t.NomeTrilha.Contains(filtroNome));
            }

            if (!string.IsNullOrEmpty(filtroDificuldade))
            {
                query = query.Where(t => t.DificuldadeTrilha == filtroDificuldade);
            }

            if (ativo.HasValue)
            {
                query = query.Where(t => t.Ativo == ativo.Value);
            }

            if (idParque.HasValue)
            {
                query = query.Where(t => t.IdParque == idParque.Value);
            }

            // Primeiro busca os dados básicos do banco
            var trilhasDb = await query
                .Select(t => new
                {
                    t.IdTrilha,
                    t.NomeTrilha,
                    t.DificuldadeTrilha,
                    t.DistanciaKm,
                    t.TempoEstimadoMinutos,
                    t.CapacidadeMaxima,
                    t.Ativo
                })
                .OrderBy(t => t.NomeTrilha)
                .ToListAsync();

            // Depois calcula a ocupação atual para cada trilha
            var trilhasDtos = new List<TrilhaResumoDto>();
            
            foreach (var trilha in trilhasDb)
            {
                trilhasDtos.Add(new TrilhaResumoDto
                {
                    IdTrilha = trilha.IdTrilha,
                    NomeTrilha = trilha.NomeTrilha,
                    DificuldadeTrilha = trilha.DificuldadeTrilha,
                    DistanciaKm = trilha.DistanciaKm,
                    TempoEstimadoMinutos = trilha.TempoEstimadoMinutos,
                    CapacidadeMaxima = trilha.CapacidadeMaxima,
                    Ativo = trilha.Ativo,
                    OcupacaoAtual = CalcularOcupacaoAtual(trilha.IdTrilha),
                    Disponivel = trilha.Ativo
                });
            }

            return trilhasDtos;
        }

        public async Task<TrilhaResponseDto?> GetTrilhaByIdAsync(int id)
        {
            var trilha = await _context.Trilhas
                .Include(t => t.Parque)
                .Include(t => t.Reservas)
                .Include(t => t.PontosInteresse)
                .FirstOrDefaultAsync(t => t.IdTrilha == id);

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
                TotalEspeciesFaunaFlora = 0, // Removido pois FaunaFlora não existe na trilha
                ReservasHoje = trilha.Reservas.Count(r => DateOnly.FromDateTime(r.DataVisita.ToDateTime(TimeOnly.MinValue)) == DateOnly.FromDateTime(agora)),
                OcupacaoAtual = CalcularOcupacaoAtual(trilha.IdTrilha)
            };
        }

        public async Task<TrilhaResponseDto> CriarTrilhaAsync(CreateTrilhaDto createTrilhaDto)
        {
            try
            {
                // Verificar se o parque existe
                if (!await _context.Parques.AnyAsync(p => p.IdParque == createTrilhaDto.IdParque))
                {
                    throw new InvalidOperationException("Parque não encontrado");
                }

                // Verificar se já existe trilha com o mesmo nome no parque
                if (await NomeTrilhaExisteNoParqueAsync(createTrilhaDto.NomeTrilha, createTrilhaDto.IdParque))
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

                _context.Trilhas.Add(trilha);
                await _context.SaveChangesAsync();

                // Buscar o parque para retornar o nome
                var parque = await _context.Parques.FindAsync(createTrilhaDto.IdParque);

                return new TrilhaResponseDto
                {
                    IdTrilha = trilha.IdTrilha,
                    IdParque = trilha.IdParque,
                    NomeParque = parque?.NomeParque ?? "",
                    NomeTrilha = trilha.NomeTrilha,
                    DescricaoTrilha = trilha.DescricaoTrilha,
                    DificuldadeTrilha = trilha.DificuldadeTrilha,
                    DistanciaKm = trilha.DistanciaKm,
                    TempoEstimadoMinutos = trilha.TempoEstimadoMinutos,
                    CapacidadeMaxima = trilha.CapacidadeMaxima,
                    CoordenadasTrilha = trilha.CoordenadasTrilha,
                    Ativo = trilha.Ativo,
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
            var trilha = await _context.Trilhas.FindAsync(id);

            if (trilha == null)
                return false;

            // Verificar se o novo nome já está em uso por outra trilha no mesmo parque
            if (!string.IsNullOrEmpty(updateTrilhaDto.NomeTrilha) && 
                updateTrilhaDto.NomeTrilha != trilha.NomeTrilha)
            {
                if (await NomeTrilhaExisteNoParqueAsync(updateTrilhaDto.NomeTrilha, trilha.IdParque))
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
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AtivarTrilhaAsync(int id)
        {
            var trilha = await _context.Trilhas.FindAsync(id);

            if (trilha == null)
                return false;

            trilha.Ativo = true;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DesativarTrilhaAsync(int id)
        {
            var trilha = await _context.Trilhas.FindAsync(id);

            if (trilha == null)
                return false;

            trilha.Ativo = false;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TrilhaExisteAsync(int id)
        {
            return await _context.Trilhas.AnyAsync(t => t.IdTrilha == id);
        }

        public async Task<bool> TrilhaAtivaAsync(int id)
        {
            var trilha = await _context.Trilhas.FindAsync(id);
            return trilha?.Ativo ?? false;
        }

        public async Task<bool> NomeTrilhaExisteNoParqueAsync(string nome, int idParque)
        {
            return await _context.Trilhas.AnyAsync(t => t.NomeTrilha == nome && t.IdParque == idParque);
        }

        public async Task<int> GetCapacidadeTrilhaAsync(int id)
        {
            var trilha = await _context.Trilhas.FindAsync(id);
            return trilha?.CapacidadeMaxima ?? 0;
        }

        public async Task<IEnumerable<TrilhaResumoDto>> GetTrilhasPorParqueAsync(int idParque)
        {
            // Primeiro busca os dados básicos do banco
            var trilhasDb = await _context.Trilhas
                .Where(t => t.IdParque == idParque)
                .Select(t => new
                {
                    t.IdTrilha,
                    t.NomeTrilha,
                    t.DificuldadeTrilha,
                    t.DistanciaKm,
                    t.TempoEstimadoMinutos,
                    t.CapacidadeMaxima,
                    t.Ativo
                })
                .OrderBy(t => t.NomeTrilha)
                .ToListAsync();

            // Depois calcula a ocupação atual para cada trilha
            var trilhasDtos = new List<TrilhaResumoDto>();
            
            foreach (var trilha in trilhasDb)
            {
                trilhasDtos.Add(new TrilhaResumoDto
                {
                    IdTrilha = trilha.IdTrilha,
                    NomeTrilha = trilha.NomeTrilha,
                    DificuldadeTrilha = trilha.DificuldadeTrilha,
                    DistanciaKm = trilha.DistanciaKm,
                    TempoEstimadoMinutos = trilha.TempoEstimadoMinutos,
                    CapacidadeMaxima = trilha.CapacidadeMaxima,
                    Ativo = trilha.Ativo,
                    OcupacaoAtual = CalcularOcupacaoAtual(trilha.IdTrilha),
                    Disponivel = trilha.Ativo
                });
            }

            return trilhasDtos;
        }

        public async Task<IEnumerable<TrilhaComMapaDto>> GetTrilhasComMapaAsync(int? idParque = null)
        {
            var query = _context.Trilhas
                .Include(t => t.PontosInteresse)
                .Where(t => t.Ativo);

            if (idParque.HasValue)
            {
                query = query.Where(t => t.IdParque == idParque.Value);
            }

            return await query
                .Select(t => new TrilhaComMapaDto
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
                })
                .ToListAsync();
        }

        public async Task<bool> ValidarCapacidadeTrilhaAsync(int id, int numeroVisitantes)
        {
            var capacidade = await GetCapacidadeTrilhaAsync(id);
            
            if (capacidade == 0)
                return true; // Sem limite de capacidade

            var ocupacaoAtual = CalcularOcupacaoAtual(id);
            return (ocupacaoAtual + numeroVisitantes) <= capacidade;
        }

        public async Task<EstatisticasTrilhaDto> GetEstatisticasTrilhaAsync(int id)
        {
            var trilha = await _context.Trilhas
                .Include(t => t.Parque)
                .Include(t => t.Reservas)
                .Include(t => t.PontosInteresse)
                .FirstOrDefaultAsync(t => t.IdTrilha == id);

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
                TotalEspeciesFaunaFlora = 0, // Removido pois FaunaFlora não existe na trilha
                CapacidadeMaxima = trilha.CapacidadeMaxima ?? 0,
                TaxaOcupacaoMedia = CalcularTaxaOcupacaoMedia(trilha.Reservas.ToList(), trilha.CapacidadeMaxima ?? 0),
                DistanciaKm = trilha.DistanciaKm,
                TempoEstimadoMinutos = trilha.TempoEstimadoMinutos,
                DificuldadeTrilha = trilha.DificuldadeTrilha
            };
        }

        public async Task<IEnumerable<TrilhaResumoDto>> GetTrilhasPorDificuldadeAsync(string dificuldade)
        {
            // Primeiro busca os dados básicos do banco
            var trilhasDb = await _context.Trilhas
                .Where(t => t.DificuldadeTrilha == dificuldade && t.Ativo)
                .Select(t => new
                {
                    t.IdTrilha,
                    t.NomeTrilha,
                    t.DificuldadeTrilha,
                    t.DistanciaKm,
                    t.TempoEstimadoMinutos,
                    t.CapacidadeMaxima,
                    t.Ativo
                })
                .OrderBy(t => t.NomeTrilha)
                .ToListAsync();

            // Depois calcula a ocupação atual para cada trilha
            var trilhasDtos = new List<TrilhaResumoDto>();
            
            foreach (var trilha in trilhasDb)
            {
                trilhasDtos.Add(new TrilhaResumoDto
                {
                    IdTrilha = trilha.IdTrilha,
                    NomeTrilha = trilha.NomeTrilha,
                    DificuldadeTrilha = trilha.DificuldadeTrilha,
                    DistanciaKm = trilha.DistanciaKm,
                    TempoEstimadoMinutos = trilha.TempoEstimadoMinutos,
                    CapacidadeMaxima = trilha.CapacidadeMaxima,
                    Ativo = trilha.Ativo,
                    OcupacaoAtual = CalcularOcupacaoAtual(trilha.IdTrilha),
                    Disponivel = true
                });
            }

            return trilhasDtos;
        }

        public async Task<IEnumerable<TrilhaResumoDto>> GetTrilhasDisponiveisAsync(DateOnly data, int numeroVisitantes)
        {
            var trilhas = await _context.Trilhas
                .Where(t => t.Ativo)
                .Include(t => t.Reservas)
                .ToListAsync();

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
        private int CalcularOcupacaoAtual(int idTrilha)
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            return _context.Reservas
                .Where(r => r.IdTrilha == idTrilha && r.DataVisita == hoje && r.Status == "ativa")
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
