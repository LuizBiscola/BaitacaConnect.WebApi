using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Repositories.Interfaces;
using BaitacaConnect.Services.Interfaces;

namespace BaitacaConnect.Services
{
    public class PontoInteresseService : IPontoInteresseService
    {
        private readonly IPontoInteresseRepository _pontoInteresseRepository;
        private readonly ITrilhaRepository _trilhaRepository;
        private readonly IParqueRepository _parqueRepository;

        public PontoInteresseService(
            IPontoInteresseRepository pontoInteresseRepository,
            ITrilhaRepository trilhaRepository,
            IParqueRepository parqueRepository)
        {
            _pontoInteresseRepository = pontoInteresseRepository;
            _trilhaRepository = trilhaRepository;
            _parqueRepository = parqueRepository;
        }

        public async Task<IEnumerable<PontoInteresseResponseDto>> GetAllAsync()
        {
            var pontos = await _pontoInteresseRepository.GetAllAsync();
            return pontos.Select(MapToResponseDto);
        }

        public async Task<PontoInteresseResponseDto?> GetByIdAsync(int idParque, int idTrilha, string nomePonto)
        {
            var ponto = await _pontoInteresseRepository.GetByIdAsync(idParque, idTrilha, nomePonto);
            return ponto != null ? MapToResponseDto(ponto) : null;
        }

        public async Task<IEnumerable<PontoInteresseResponseDto>> GetByTrilhaAsync(int idTrilha)
        {
            // Validar se a trilha existe
            var trilhaExiste = await _trilhaRepository.ExistsAsync(idTrilha);
            if (!trilhaExiste)
                throw new ArgumentException("Trilha não encontrada");

            var pontos = await _pontoInteresseRepository.GetByTrilhaAsync(idTrilha);
            return pontos.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<PontoInteresseResponseDto>> GetByParqueAsync(int idParque)
        {
            // Validar se o parque existe
            var parqueExiste = await _parqueRepository.ExistsAsync(idParque);
            if (!parqueExiste)
                throw new ArgumentException("Parque não encontrado");

            var pontos = await _pontoInteresseRepository.GetByParqueAsync(idParque);
            return pontos.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<PontoInteresseResponseDto>> GetWithFiltersAsync(int? idParque = null, int? idTrilha = null,
            string? tipo = null, string? nome = null)
        {
            var pontos = await _pontoInteresseRepository.GetWithFiltersAsync(idParque, idTrilha, tipo, nome);
            return pontos.Select(MapToResponseDto);
        }

        public async Task<PontoInteresseResponseDto> CreateAsync(CreatePontoInteresseDto createDto)
        {
            // Validar se o parque existe
            var parqueExiste = await _parqueRepository.ExistsAsync(createDto.IdParque);
            if (!parqueExiste)
                throw new ArgumentException("Parque não encontrado");

            // Validar se a trilha existe
            var trilhaExiste = await _trilhaRepository.ExistsAsync(createDto.IdTrilha);
            if (!trilhaExiste)
                throw new ArgumentException("Trilha não encontrada");

            // Verificar se já existe um ponto com o mesmo nome na mesma trilha
            var jaExiste = await _pontoInteresseRepository.ExistsAsync(createDto.IdParque, createDto.IdTrilha, createDto.NomePontoInteresse);
            if (jaExiste)
                throw new InvalidOperationException("Já existe um ponto de interesse com este nome nesta trilha");

            var ponto = new PontoInteresse
            {
                IdParque = createDto.IdParque,
                IdTrilha = createDto.IdTrilha,
                NomePontoInteresse = createDto.NomePontoInteresse,
                DescricaoPontoInteresse = createDto.DescricaoPontoInteresse,
                Tipo = createDto.Tipo,
                Coordenadas = createDto.Coordenadas,
                OrdemNaTrilha = createDto.OrdemNaTrilha
            };

            var pontoCriado = await _pontoInteresseRepository.CreateAsync(ponto);
            var pontoCompleto = await _pontoInteresseRepository.GetByIdAsync(pontoCriado.IdParque, pontoCriado.IdTrilha, pontoCriado.NomePontoInteresse);
            return MapToResponseDto(pontoCompleto!);
        }

        public async Task<PontoInteresseResponseDto> UpdateAsync(int idParque, int idTrilha, string nomePonto, UpdatePontoInteresseDto updateDto)
        {
            var ponto = await _pontoInteresseRepository.GetByIdAsync(idParque, idTrilha, nomePonto);
            if (ponto == null)
                throw new ArgumentException("Ponto de interesse não encontrado");

            // Aplicar alterações
            if (!string.IsNullOrEmpty(updateDto.DescricaoPontoInteresse))
                ponto.DescricaoPontoInteresse = updateDto.DescricaoPontoInteresse;

            if (!string.IsNullOrEmpty(updateDto.Tipo))
                ponto.Tipo = updateDto.Tipo;

            if (!string.IsNullOrEmpty(updateDto.Coordenadas))
                ponto.Coordenadas = updateDto.Coordenadas;

            if (updateDto.OrdemNaTrilha.HasValue)
                ponto.OrdemNaTrilha = updateDto.OrdemNaTrilha.Value;

            var pontoAtualizado = await _pontoInteresseRepository.UpdateAsync(ponto);
            return MapToResponseDto(pontoAtualizado);
        }

        public async Task<bool> DeleteAsync(int idParque, int idTrilha, string nomePonto)
        {
            var existe = await _pontoInteresseRepository.ExistsAsync(idParque, idTrilha, nomePonto);
            if (!existe)
                return false;

            return await _pontoInteresseRepository.DeleteAsync(idParque, idTrilha, nomePonto);
        }

        public async Task<MapaTrilhaDto> GetMapaTrilhaAsync(int idTrilha)
        {
            // Validar se a trilha existe
            var trilha = await _trilhaRepository.GetByIdAsync(idTrilha);
            if (trilha == null)
                throw new ArgumentException("Trilha não encontrada");

            var pontos = await _pontoInteresseRepository.GetMapaPontosAsync(idTrilha);
            var pontosDto = pontos.Select(MapToPontoMapaDto).ToList();

            return new MapaTrilhaDto
            {
                IdTrilha = idTrilha,
                NomeTrilha = trilha.NomeTrilha,
                CoordenadasTrilha = trilha.CoordenadasTrilha,
                TotalPontos = pontosDto.Count,
                Pontos = pontosDto
            };
        }

        public async Task<IEnumerable<string>> GetTiposAsync()
        {
            return await _pontoInteresseRepository.GetTiposAsync();
        }

        public async Task<EstatisticasPontosDto> GetEstatisticasAsync()
        {
            var estatisticasPorTipo = await _pontoInteresseRepository.GetEstatisticasPorTipoAsync();
            var tipos = await _pontoInteresseRepository.GetTiposAsync();

            return new EstatisticasPontosDto
            {
                TotalPontos = estatisticasPorTipo.Values.Sum(),
                TotalTipos = tipos.Count(),
                DistribuicaoPorTipo = estatisticasPorTipo,
                Tipos = tipos.ToList()
            };
        }

        public async Task<IEnumerable<PontoInteresseResponseDto>> GetPontosProximosAsync(string coordenadas, double raioKm)
        {
            if (string.IsNullOrEmpty(coordenadas))
                throw new ArgumentException("Coordenadas não podem estar vazias");

            if (raioKm <= 0)
                throw new ArgumentException("Raio deve ser maior que zero");

            var pontos = await _pontoInteresseRepository.GetPontosProximosAsync(coordenadas, raioKm);
            return pontos.Select(MapToResponseDto);
        }

        public async Task<bool> ReordenarPontosAsync(int idTrilha, List<ReordenarPontoDto> novaOrdem)
        {
            // Validar se a trilha existe
            var trilhaExiste = await _trilhaRepository.ExistsAsync(idTrilha);
            if (!trilhaExiste)
                throw new ArgumentException("Trilha não encontrada");

            try
            {
                foreach (var item in novaOrdem)
                {
                    var ponto = await _pontoInteresseRepository.GetByIdAsync(item.IdParque, idTrilha, item.NomePonto);
                    if (ponto != null)
                    {
                        ponto.OrdemNaTrilha = item.NovaOrdem;
                        await _pontoInteresseRepository.UpdateAsync(ponto);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static PontoInteresseResponseDto MapToResponseDto(PontoInteresse ponto)
        {
            return new PontoInteresseResponseDto
            {
                IdParque = ponto.IdParque,
                IdTrilha = ponto.IdTrilha,
                NomeParque = ponto.Parque?.NomeParque ?? string.Empty,
                NomeTrilha = ponto.Trilha?.NomeTrilha ?? string.Empty,
                NomePontoInteresse = ponto.NomePontoInteresse,
                DescricaoPontoInteresse = ponto.DescricaoPontoInteresse,
                Tipo = ponto.Tipo,
                Coordenadas = ponto.Coordenadas,
                OrdemNaTrilha = ponto.OrdemNaTrilha ?? 0
            };
        }

        private static PontoMapaDto MapToPontoMapaDto(PontoInteresse ponto)
        {
            return new PontoMapaDto
            {
                Nome = ponto.NomePontoInteresse,
                Tipo = ponto.Tipo ?? "generico",
                Coordenadas = ponto.Coordenadas ?? string.Empty,
                Descricao = ponto.DescricaoPontoInteresse,
                Ordem = ponto.OrdemNaTrilha ?? 0
            };
        }
    }
}
