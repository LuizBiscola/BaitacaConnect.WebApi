using BaitacaConnect.Models;
using BaitacaConnect.Models.DTOs;
using BaitacaConnect.Repositories.Interfaces;
using BaitacaConnect.Services.Interfaces;

namespace BaitacaConnect.Services
{
    public class FaunaFloraService : IFaunaFloraService
    {
        private readonly IFaunaFloraRepository _faunaFloraRepository;
        private readonly ITrilhaRepository _trilhaRepository;

        public FaunaFloraService(IFaunaFloraRepository faunaFloraRepository, ITrilhaRepository trilhaRepository)
        {
            _faunaFloraRepository = faunaFloraRepository;
            _trilhaRepository = trilhaRepository;
        }

        public async Task<IEnumerable<FaunaFloraResponseDto>> GetAllAsync()
        {
            var especies = await _faunaFloraRepository.GetAllAsync();
            return especies.Select(MapToResponseDto);
        }

        public async Task<FaunaFloraResponseDto?> GetByIdAsync(int id)
        {
            var especie = await _faunaFloraRepository.GetByIdAsync(id);
            return especie != null ? MapToResponseDto(especie) : null;
        }

        public async Task<IEnumerable<FaunaFloraResponseDto>> GetWithFiltersAsync(string? nome = null, string? tipo = null,
            string? categoria = null, int? idTrilha = null)
        {
            var especies = await _faunaFloraRepository.GetWithFiltersAsync(nome, tipo, categoria, idTrilha);
            return especies.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<FaunaFloraResponseDto>> GetByTipoAsync(string tipo)
        {
            // Validar tipo
            if (tipo != "fauna" && tipo != "flora")
                throw new ArgumentException("Tipo deve ser 'fauna' ou 'flora'");

            var especies = await _faunaFloraRepository.GetByTipoAsync(tipo);
            return especies.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<FaunaFloraResponseDto>> GetByCategoriaAsync(string categoria)
        {
            var especies = await _faunaFloraRepository.GetByCategoriaAsync(categoria);
            return especies.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<FaunaFloraResponseDto>> GetByTrilhaAsync(int idTrilha)
        {
            // Validar se a trilha existe
            var trilhaExiste = await _trilhaRepository.ExistsAsync(idTrilha);
            if (!trilhaExiste)
                throw new ArgumentException("Trilha não encontrada");

            var especies = await _faunaFloraRepository.GetByTrilhaAsync(idTrilha);
            return especies.Select(MapToResponseDto);
        }

        public async Task<FaunaFloraResponseDto> CreateAsync(CreateFaunaFloraDto createDto)
        {
            // Validar se já existe
            var jaExiste = await _faunaFloraRepository.NomeExistsAsync(createDto.NomePopular, createDto.NomeCientifico);
            if (jaExiste)
                throw new InvalidOperationException("Já existe uma espécie com este nome");

            // Validar tipo
            if (createDto.Tipo != "fauna" && createDto.Tipo != "flora")
                throw new ArgumentException("Tipo deve ser 'fauna' ou 'flora'");

            // Validar trilhas (se informadas)
            if (createDto.TrilhasOndeEncontra != null && createDto.TrilhasOndeEncontra.Any())
            {
                foreach (var idTrilha in createDto.TrilhasOndeEncontra)
                {
                    var trilhaExiste = await _trilhaRepository.ExistsAsync(idTrilha);
                    if (!trilhaExiste)
                        throw new ArgumentException($"Trilha com ID {idTrilha} não encontrada");
                }
            }

            var especie = new FaunaFlora
            {
                NomeCientifico = createDto.NomeCientifico,
                NomePopular = createDto.NomePopular,
                Tipo = createDto.Tipo,
                Categoria = createDto.Categoria,
                Descricao = createDto.Descricao,
                Caracteristicas = createDto.Caracteristicas,
                Imagens = createDto.Imagens,
                TrilhasOndeEncontra = createDto.TrilhasOndeEncontra
            };

            var especieCriada = await _faunaFloraRepository.CreateAsync(especie);
            return MapToResponseDto(especieCriada);
        }

        public async Task<FaunaFloraResponseDto> UpdateAsync(int id, UpdateFaunaFloraDto updateDto)
        {
            var especie = await _faunaFloraRepository.GetByIdAsync(id);
            if (especie == null)
                throw new ArgumentException("Espécie não encontrada");

            // Validar tipo se alterado
            if (!string.IsNullOrEmpty(updateDto.Tipo) && updateDto.Tipo != "fauna" && updateDto.Tipo != "flora")
                throw new ArgumentException("Tipo deve ser 'fauna' ou 'flora'");

            // Validar trilhas se alteradas
            if (updateDto.TrilhasOndeEncontra != null && updateDto.TrilhasOndeEncontra.Any())
            {
                foreach (var idTrilha in updateDto.TrilhasOndeEncontra)
                {
                    var trilhaExiste = await _trilhaRepository.ExistsAsync(idTrilha);
                    if (!trilhaExiste)
                        throw new ArgumentException($"Trilha com ID {idTrilha} não encontrada");
                }
            }

            // Aplicar alterações
            if (!string.IsNullOrEmpty(updateDto.NomeCientifico))
                especie.NomeCientifico = updateDto.NomeCientifico;

            if (!string.IsNullOrEmpty(updateDto.NomePopular))
                especie.NomePopular = updateDto.NomePopular;

            if (!string.IsNullOrEmpty(updateDto.Tipo))
                especie.Tipo = updateDto.Tipo;

            if (!string.IsNullOrEmpty(updateDto.Categoria))
                especie.Categoria = updateDto.Categoria;

            if (!string.IsNullOrEmpty(updateDto.Descricao))
                especie.Descricao = updateDto.Descricao;

            if (!string.IsNullOrEmpty(updateDto.Caracteristicas))
                especie.Caracteristicas = updateDto.Caracteristicas;

            if (!string.IsNullOrEmpty(updateDto.Imagens))
                especie.Imagens = updateDto.Imagens;

            if (updateDto.TrilhasOndeEncontra != null)
                especie.TrilhasOndeEncontra = updateDto.TrilhasOndeEncontra;

            var especieAtualizada = await _faunaFloraRepository.UpdateAsync(especie);
            return MapToResponseDto(especieAtualizada);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existe = await _faunaFloraRepository.ExistsAsync(id);
            if (!existe)
                return false;

            return await _faunaFloraRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<string>> GetCategoriasAsync()
        {
            return await _faunaFloraRepository.GetCategoriasAsync();
        }

        public async Task<EstatisticasFaunaFloraDto> GetEstatisticasAsync()
        {
            var estatisticasPorTipo = await _faunaFloraRepository.GetEstatisticasPorTipoAsync();
            var categorias = await _faunaFloraRepository.GetCategoriasAsync();

            return new EstatisticasFaunaFloraDto
            {
                TotalFauna = estatisticasPorTipo.GetValueOrDefault("fauna", 0),
                TotalFlora = estatisticasPorTipo.GetValueOrDefault("flora", 0),
                TotalEspecies = estatisticasPorTipo.Values.Sum(),
                TotalCategorias = categorias.Count(),
                DistribuicaoPorTipo = estatisticasPorTipo,
                Categorias = categorias.ToList()
            };
        }

        public async Task<IEnumerable<FaunaFloraResponseDto>> SearchAsync(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                throw new ArgumentException("Termo de busca não pode estar vazio");

            var especies = await _faunaFloraRepository.SearchAsync(termo);
            return especies.Select(MapToResponseDto);
        }

        public async Task<GuiaEspeciesDto> GetGuiaEspeciesAsync(int idTrilha)
        {
            // Validar se a trilha existe
            var trilha = await _trilhaRepository.GetByIdAsync(idTrilha);
            if (trilha == null)
                throw new ArgumentException("Trilha não encontrada");

            var especies = await _faunaFloraRepository.GetByTrilhaAsync(idTrilha);
            var especiesDto = especies.Select(MapToResponseDto).ToList();

            return new GuiaEspeciesDto
            {
                IdTrilha = idTrilha,
                NomeTrilha = trilha.NomeTrilha,
                TotalEspecies = especiesDto.Count,
                TotalFauna = especiesDto.Count(e => e.Tipo == "fauna"),
                TotalFlora = especiesDto.Count(e => e.Tipo == "flora"),
                Especies = especiesDto
            };
        }

        private static FaunaFloraResponseDto MapToResponseDto(FaunaFlora faunaFlora)
        {
            return new FaunaFloraResponseDto
            {
                IdFaunaFlora = faunaFlora.IdFaunaFlora,
                NomeCientifico = faunaFlora.NomeCientifico,
                NomePopular = faunaFlora.NomePopular,
                Tipo = faunaFlora.Tipo,
                Categoria = faunaFlora.Categoria,
                Descricao = faunaFlora.Descricao,
                Caracteristicas = faunaFlora.Caracteristicas,
                Imagens = faunaFlora.Imagens,
                TrilhasOndeEncontra = faunaFlora.TrilhasOndeEncontra ?? new List<int>()
            };
        }
    }
}
