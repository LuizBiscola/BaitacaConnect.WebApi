using System.Text;
using Microsoft.EntityFrameworkCore;
using BaitacaConnect.Models;

namespace BaitacaConnect.Data
{
    public class BaitacaDbContext : DbContext
    {
        public BaitacaDbContext(DbContextOptions<BaitacaDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Parque> Parques { get; set; }
        public DbSet<Trilha> Trilhas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<PontoInteresse> PontosInteresse { get; set; }
        public DbSet<FaunaFlora> FaunaFlora { get; set; }
        public DbSet<RelatorioVisita> RelatoriosVisita { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== CONFIGURAÇÕES GERAIS ====================
            
            // Configurar schema padrão se necessário
            // modelBuilder.HasDefaultSchema("baitaca");

            // ==================== CONFIGURAÇÕES DE ENTIDADES ====================

            // Configuração da entidade Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");
                
                entity.HasKey(e => e.IdUsuario);
                
                entity.Property(e => e.IdUsuario)
                    .HasColumnName("id_usuario")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.NomeUsuario)
                    .HasColumnName("nome_usuario")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.EmailUsuario)
                    .HasColumnName("email_usuario")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.TelefoneUsuario)
                    .HasColumnName("telefone_usuario")
                    .HasMaxLength(20);

                entity.Property(e => e.TipoUsuario)
                    .HasColumnName("tipo_usuario")
                    .HasMaxLength(20)
                    .HasDefaultValue("visitante");

                entity.Property(e => e.DataCriacao)
                    .HasColumnName("data_criacao")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Ativo)
                    .HasColumnName("ativo")
                    .HasDefaultValue(true);

                entity.Property(e => e.IdadeUsuario)
                    .HasColumnName("idade_usuario");

                entity.Property(e => e.SenhaUsuario)
                    .HasColumnName("senha_usuario")
                    .HasMaxLength(200);

                // Ignorar propriedade SenhaTemporaria que não deve ir para o banco
                entity.Ignore(e => e.SenhaTemporaria);

                // Índices
                entity.HasIndex(e => e.EmailUsuario)
                    .IsUnique()
                    .HasDatabaseName("IX_usuario_email_usuario");

                entity.HasIndex(e => e.TipoUsuario)
                    .HasDatabaseName("IX_usuario_tipo_usuario");
            });

            // Configuração da entidade Parque
            modelBuilder.Entity<Parque>(entity =>
            {
                entity.ToTable("parque");
                
                entity.HasKey(e => e.IdParque);
                
                entity.Property(e => e.IdParque)
                    .HasColumnName("id_parque")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.NomeParque)
                    .HasColumnName("nome_parque")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.DescricaoParque)
                    .HasColumnName("descricao_parque")
                    .HasColumnType("text");

                entity.Property(e => e.EnderecoParque)
                    .HasColumnName("endereco_parque")
                    .HasColumnType("text");

                entity.Property(e => e.CapacidadeMaxima)
                    .HasColumnName("capacidade_maxima");

                entity.Property(e => e.HorarioFuncionamento)
                    .HasColumnName("horario_funcionamento")
                    .HasColumnType("jsonb");

                entity.Property(e => e.CoordenadasParque)
                    .HasColumnName("coordenadas_parque")
                    .HasConversion(
                        v => v, // string para string (PostgreSQL aceita string para POINT)
                        v => v) // string para string
                    .HasColumnType("text"); // Mudar para text temporariamente

                entity.Property(e => e.Ativo)
                    .HasColumnName("ativo")
                    .HasDefaultValue(true);

                // Índices
                entity.HasIndex(e => e.NomeParque)
                    .HasDatabaseName("IX_parque_nome_parque");

                entity.HasIndex(e => e.Ativo)
                    .HasDatabaseName("IX_parque_ativo");
            });

            // Configuração da entidade Trilha
            modelBuilder.Entity<Trilha>(entity =>
            {
                entity.ToTable("trilha");
                
                entity.HasKey(e => e.IdTrilha);
                
                entity.Property(e => e.IdTrilha)
                    .HasColumnName("id_trilha")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdParque)
                    .HasColumnName("id_parque")
                    .IsRequired();

                entity.Property(e => e.NomeTrilha)
                    .HasColumnName("nome_trilha")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.DescricaoTrilha)
                    .HasColumnName("descricao_trilha")
                    .HasColumnType("text");

                entity.Property(e => e.DificuldadeTrilha)
                    .HasColumnName("dificuldade_trilha")
                    .HasMaxLength(20);

                entity.Property(e => e.DistanciaKm)
                    .HasColumnName("distancia_km")
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.TempoEstimadoMinutos)
                    .HasColumnName("tempo_estimado_minutos");

                entity.Property(e => e.CapacidadeMaxima)
                    .HasColumnName("capacidade_maxima");

                entity.Property(e => e.CoordenadasTrilha)
                    .HasColumnName("coordenadas_trilha")
                    .HasColumnType("jsonb");

                entity.Property(e => e.Ativo)
                    .HasColumnName("ativo")
                    .HasDefaultValue(true);

                // Relacionamento com Parque
                entity.HasOne(e => e.Parque)
                    .WithMany(p => p.Trilhas)
                    .HasForeignKey(e => e.IdParque)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_trilha_parque");

                // Índices
                entity.HasIndex(e => e.IdParque)
                    .HasDatabaseName("IX_trilha_id_parque");

                entity.HasIndex(e => e.DificuldadeTrilha)
                    .HasDatabaseName("IX_trilha_dificuldade_trilha");

                entity.HasIndex(e => e.Ativo)
                    .HasDatabaseName("IX_trilha_ativo");
            });

            // Configuração da entidade Reserva
            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.ToTable("reserva");
                
                entity.HasKey(e => e.IdReserva);
                
                entity.Property(e => e.IdReserva)
                    .HasColumnName("id_reserva")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("id_usuario")
                    .IsRequired();

                entity.Property(e => e.IdParque)
                    .HasColumnName("id_parque")
                    .IsRequired();

                entity.Property(e => e.IdTrilha)
                    .HasColumnName("id_trilha");

                entity.Property(e => e.DataVisita)
                    .HasColumnName("data_visita")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.HorarioEntrada)
                    .HasColumnName("horario_entrada")
                    .HasColumnType("time");

                entity.Property(e => e.NumeroVisitantes)
                    .HasColumnName("numero_visitantes")
                    .HasDefaultValue(1);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(20)
                    .HasDefaultValue("ativa");

                entity.Property(e => e.CheckIn)
                    .HasColumnName("check_in")
                    .HasColumnType("timestamp");

                entity.Property(e => e.CheckOut)
                    .HasColumnName("check_out")
                    .HasColumnType("timestamp");

                entity.Property(e => e.DataCriacao)
                    .HasColumnName("data_criacao")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relacionamentos
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Reservas)
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_reserva_usuario");

                entity.HasOne(e => e.Parque)
                    .WithMany(p => p.Reservas)
                    .HasForeignKey(e => e.IdParque)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_reserva_parque");

                entity.HasOne(e => e.Trilha)
                    .WithMany(t => t.Reservas)
                    .HasForeignKey(e => e.IdTrilha)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_reserva_trilha");

                // Índices
                entity.HasIndex(e => e.IdUsuario)
                    .HasDatabaseName("IX_reserva_id_usuario");

                entity.HasIndex(e => e.IdParque)
                    .HasDatabaseName("IX_reserva_id_parque");

                entity.HasIndex(e => e.IdTrilha)
                    .HasDatabaseName("IX_reserva_id_trilha");

                entity.HasIndex(e => e.DataVisita)
                    .HasDatabaseName("IX_reserva_data_visita");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_reserva_status");

                entity.HasIndex(e => new { e.DataVisita, e.Status })
                    .HasDatabaseName("IX_reserva_data_visita_status");
            });

            // Configuração da entidade PontoInteresse (chave composta)
            modelBuilder.Entity<PontoInteresse>(entity =>
            {
                entity.ToTable("ponto_interesse");
                
                // Chave composta
                entity.HasKey(e => new { e.IdParque, e.IdTrilha })
                    .HasName("PK_ponto_interesse");

                entity.Property(e => e.IdParque)
                    .HasColumnName("id_parque");

                entity.Property(e => e.IdTrilha)
                    .HasColumnName("id_trilha");

                entity.Property(e => e.NomePontoInteresse)
                    .HasColumnName("nome_ponto_interesse")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.DescricaoPontoInteresse)
                    .HasColumnName("descricao_ponto_interesse")
                    .HasColumnType("text");

                entity.Property(e => e.Tipo)
                    .HasColumnName("tipo")
                    .HasMaxLength(50);

                entity.Property(e => e.Coordenadas)
                    .HasColumnName("coordenadas")
                    .HasColumnType("text"); // Mudar para text temporariamente

                entity.Property(e => e.OrdemNaTrilha)
                    .HasColumnName("ordem_na_trilha");

                // Relacionamentos
                entity.HasOne(e => e.Parque)
                    .WithMany(p => p.PontosInteresse)
                    .HasForeignKey(e => e.IdParque)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ponto_interesse_parque");

                entity.HasOne(e => e.Trilha)
                    .WithMany(t => t.PontosInteresse)
                    .HasForeignKey(e => e.IdTrilha)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ponto_interesse_trilha");

                // Índices
                entity.HasIndex(e => e.Tipo)
                    .HasDatabaseName("IX_ponto_interesse_tipo");

                entity.HasIndex(e => e.OrdemNaTrilha)
                    .HasDatabaseName("IX_ponto_interesse_ordem_na_trilha");
            });

            // Configuração da entidade FaunaFlora
            modelBuilder.Entity<FaunaFlora>(entity =>
            {
                entity.ToTable("fauna_flora");
                
                entity.HasKey(e => e.IdFaunaFlora);
                
                entity.Property(e => e.IdFaunaFlora)
                    .HasColumnName("id_fauna_flora")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.NomeCientifico)
                    .HasColumnName("nome_cientifico")
                    .HasMaxLength(150);

                entity.Property(e => e.NomePopular)
                    .HasColumnName("nome_popular")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Tipo)
                    .HasColumnName("tipo")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Categoria)
                    .HasColumnName("categoria")
                    .HasMaxLength(50);

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao")
                    .HasColumnType("text");

                entity.Property(e => e.Caracteristicas)
                    .HasColumnName("caracteristicas")
                    .HasColumnType("jsonb");

                entity.Property(e => e.Imagens)
                    .HasColumnName("imagens")
                    .HasColumnType("jsonb");

                entity.Property(e => e.TrilhasOndeEncontra)
                    .HasColumnName("trilhas_onde_encontra")
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<List<int>>(v, (System.Text.Json.JsonSerializerOptions?)null))
                    .HasColumnType("jsonb")
                    .Metadata.SetValueComparer(new ListIntValueComparer());

                // Índices
                entity.HasIndex(e => e.Tipo)
                    .HasDatabaseName("IX_fauna_flora_tipo");

                entity.HasIndex(e => e.Categoria)
                    .HasDatabaseName("IX_fauna_flora_categoria");

                entity.HasIndex(e => e.NomePopular)
                    .HasDatabaseName("IX_fauna_flora_nome_popular");

                entity.HasIndex(e => e.NomeCientifico)
                    .HasDatabaseName("IX_fauna_flora_nome_cientifico");
            });

            // Configuração da entidade RelatorioVisita
            modelBuilder.Entity<RelatorioVisita>(entity =>
            {
                entity.ToTable("relatorio_visita");
                
                entity.HasKey(e => e.IdRelatorio);
                
                entity.Property(e => e.IdRelatorio)
                    .HasColumnName("id_relatorio")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdReserva)
                    .HasColumnName("id_reserva")
                    .IsRequired();

                entity.Property(e => e.Avaliacao)
                    .HasColumnName("avaliacao");

                entity.Property(e => e.Comentarios)
                    .HasColumnName("comentarios")
                    .HasColumnType("text");

                entity.Property(e => e.ProblemasEncontrados)
                    .HasColumnName("problemas_encontrados")
                    .HasColumnType("text");

                entity.Property(e => e.DataRelatorio)
                    .HasColumnName("data_relatorio")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relacionamento
                entity.HasOne(e => e.Reserva)
                    .WithMany(r => r.RelatoriosVisita)
                    .HasForeignKey(e => e.IdReserva)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_relatorio_visita_reserva");

                // Índices
                entity.HasIndex(e => e.IdReserva)
                    .HasDatabaseName("IX_relatorio_visita_id_reserva");

                entity.HasIndex(e => e.Avaliacao)
                    .HasDatabaseName("IX_relatorio_visita_avaliacao");

                entity.HasIndex(e => e.DataRelatorio)
                    .HasDatabaseName("IX_relatorio_visita_data_relatorio");
            });

            // ==================== CONFIGURAÇÕES ADICIONAIS ====================

            // Configurar conversões para DateOnly (se necessário)
            if (Database.IsNpgsql())
            {
                modelBuilder.Entity<Reserva>()
                    .Property(e => e.DataVisita)
                    .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            }

            // Configurações específicas do PostgreSQL
            ConfigurePostgreSqlSpecifics(modelBuilder);

            // Configurar dados iniciais (seed data)
            SeedData(modelBuilder);
        }

        private void ConfigurePostgreSqlSpecifics(ModelBuilder modelBuilder)
        {
            // Configurar naming convention para PostgreSQL (snake_case)
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Converter nomes de tabelas para snake_case se necessário
                if (entity.GetTableName() != null)
                {
                    entity.SetTableName(entity.GetTableName()!.ToSnakeCase());
                }

                // Converter nomes de colunas para snake_case se necessário
                foreach (var property in entity.GetProperties())
                {
                    if (property.GetColumnName() != null)
                    {
                        property.SetColumnName(property.GetColumnName()!.ToSnakeCase());
                    }
                }
            }
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Dados iniciais para tipos de usuário, dificuldades, etc.
            // Isso pode ser configurado aqui ou através de migrations
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configuração de fallback se necessário
                // optionsBuilder.UseNpgsql("DefaultConnection");
            }

            // Configurações adicionais para PostgreSQL
            optionsBuilder.UseNpgsql(options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });

            // Configurar logging em desenvolvimento
            #if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
            #endif
        }
    }

    // Extensão para converter nomes para snake_case
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]) && i > 0 && !char.IsUpper(input[i - 1]))
                {
                    result.Append('_');
                }
                result.Append(char.ToLower(input[i]));
            }
            return result.ToString();
        }
    }

    // Conversor para DateOnly (se necessário)
    public class DateOnlyConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(
            dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime))
        {
        }
    }

    public class DateOnlyComparer : Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<DateOnly>
    {
        public DateOnlyComparer() : base(
            (d1, d2) => d1 == d2,
            d => d.GetHashCode())
        {
        }
    }

    public class ListIntValueComparer : Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<int>>
    {
        public ListIntValueComparer() : base(
            (l1, l2) => l1.SequenceEqual(l2),
            l => l.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())))
        {
        }
    }
}
