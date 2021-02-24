using AppEFCore.Data.Configurations;
using AppEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace AppEFCore.Data
{
    /* Há duas formas de mapear o modelo de dados (entidades x tabelas):
     * 
     * 1 - Expondo explicitamente a entidade através de uma propriedade genérica DbSet
     * 2 - Através do método OnModelCreating
     * 3 - Através de arquivos de configurações (desacoplamento de código)
     * 
     * Obs.: a forma com se configura, usando os métodos de extensão, é chamado de Fluent API
     */

    public class CursoEFCoreContext : DbContext
    {
        private static readonly ILoggerFactory _logger = LoggerFactory.Create(lb => lb.AddConsole());

        public DbSet<Pedido> Pedidos { get; set; } // Se eu adicionar apenas essa tabela, o EF ao executar um migration, irá identificar as demais tabelas devido a propriedade de navegação.
        
        public DbSet<Cliente> Clientes { get; set; }
        
        public DbSet<Produto> Produtos { get; set; }

        public DbSet<PedidoItem> PedidoItens { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder">É através desse parâmetro que é informado qual o Provider que será utilizado, dentre outras coisas.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /* Aqui na string de conecão está especificado o "Initial Catalog".
             * Isso define o nome do banco de dados, no qual será criada na primeira migration.
             */
            optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Initial Catalog=CursoEFCore;Integrated Security=true;",
                    opt =>
                    {
                        // Resiliência da conexão: habilita a opção do EF Core de reconectar em caso de falhas
                        opt.EnableRetryOnFailure(maxRetryCount: 2, // qtde de tentativas
                            maxRetryDelay: TimeSpan.FromSeconds(5), // tempo (em segundos) entre as tentativas
                            errorNumbersToAdd: null); // ?

                        //opt.MigrationsHistoryTable("EFMigrationsHistoryCurso"); // É possível alterar o nome da tabela onde são armazenados registros de execução do migrations (a tabela padrão do EF para histórico).
                        // IMPORTANTE: Isso deve ser feito antes do primeiro deploy, se não, a tabela anterior (nome padrão) continua.
                    })
                .UseLoggerFactory(_logger)
                .EnableSensitiveDataLogging() // Por padrão, o EF não exibe os valores dos parâmetros nos logs. Essa opção habilita a visualização dos parâmetros. CUIDADO!!!
                .UseLazyLoadingProxies(); // Exemplo de uso em Program.cs, método ConsultasComPropriedadesNavegacaoCarregamentoLento
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // As entidades Cliente e Produto estão sendo configuradas através de um arquivo de configuração a parte (geralmente é a melhor opção)

            // Forma onde tenho que chamar a configuração para cada entidade
            modelBuilder.ApplyConfiguration(new ClienteConfiguration());
            modelBuilder.ApplyConfiguration(new ProdutoConfiguration());

            // Forma onde configuro de forma automática todas as configurações implementadas (que herdam de IEntityTypeConfiguration)
            /* Lendo:
             * Procure todas as classes concretas que estão implementando a interface IEntityTypeConfiguration
             * e execute o método Configure(...);
             * 
             * O Assembly pode ser de um projeto a parte que só faz a modelagem dos dados.
             */
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CursoEFCoreContext).Assembly);

            // Outra forma de configuração (interna, no OnModeCreating - alto acoplamento de código)

            modelBuilder
                .Entity<Pedido>(p =>
                {
                    p.ToTable("Pedidos");

                    p.HasKey(p => p.Id);

                    p.Property(p => p.IniciadoEm)
                        .HasDefaultValueSql("GETDATE()")
                            .ValueGeneratedOnAdd();

                    p.Property(p => p.FinalizadoEM);

                    p.Property(p => p.TipoFrete)
                        .HasConversion<int>()
                        .IsRequired();

                    p.Property(p => p.Observacao)
                        .HasMaxLength(500);

                    // 1 (Pedido) : N (PedidoItem)
                    p.HasMany(p => p.Itens)
                        .WithOne(p => p.Pedido)
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }

        public override int SaveChanges()
        {
            
            
            return base.SaveChanges();
        }

        private void MapearPropriedadesEsquecidas(ModelBuilder modelBuilder)
        {

        }
    }
}