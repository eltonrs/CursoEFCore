using AppEFCore.Domain;
using AppEFCore.Extensions;
using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AppEFCore
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Forma 2 de aplicar as migrações no banco de dados: usando Migrate(). Não recomendada para produção, porém está sendo usada no Facelift.
            using var db = new Data.CursoEFCoreContext();

            //var migracoesPendentes = Task.Run(async () => await db.Database.GetPendingMigrationsAsync()).Result;
            var migracoesPendentes = db.Database.GetPendingMigrations();

            if (migracoesPendentes.Any())
            {
                // Em Produção: pode ser informado que há migrações pendentes e impedir que a aplicação subo. Impedindo que hajam erros graves.
                
                db.Database.Migrate(); // Executa somente as migrations que não foram executadas.
            }

            //Inserir();
            //InserirUmRegistroDeDiferentesEntidades();
            //InserirVariasRegistrosDeUmaEntidade();
            //InserirPedidoItem();
            //InserirMuitosDadosComBogus();

            //ConsultaForma1();
            //ConsultaForma2();
            //ConsultaComAsNoTracking();
            //ConsultaComOrdenacao();
            //ConsultarClientesPorIdComLINQExpression(3);

            //ConsultasComPropriedadesNavegacaoCarregamentoAdiantado();
            //ConsultasComPropriedadesNavegacaoCarregamentoExplicito();
            //ConsultasComPropriedadesNavegacaoCarregamentoLento();
            //ConsultaSelectMany();
            //ConsultarPaginavel();

            //IncluirInstanciasNaoRastreadas();
            //AtualizarSimples();

            //RemoverUmRegistroDeUmaInstancia();
        }

        #region Inserções

        private static void InserirMuitosDadosComBogus()
        {
            var faker = new Faker<Cliente>("pt_BR")
                .RuleFor(c => c.Nome, f => f.Name.FullName().Truncar(100))
                .RuleFor(c => c.Telefone, f => f.Phone.PhoneNumber().ReplaceMany(new string[] { "(", ")", "-", " ", "+" }, "").Truncar(15))
                .RuleFor(c => c.CEP, f => f.Address.ZipCode("########"))
                .RuleFor(c => c.Cidade, f => f.Address.City().Truncar(100))
                .RuleFor(c => c.Estado, f => f.Address.StateAbbr().Truncar(2));

            var clientesFakes = faker.Generate(30);

            using var db = new Data.CursoEFCoreContext();

            db.Clientes.AddRange(clientesFakes);
            db.SaveChanges();
        }

        private static void IncluirInstanciasNaoRastreadas()
        {
            using var db = new Data.CursoEFCoreContext();

            var cliente = new Cliente
            {
                Nome = "Josefina da Silva",
                Telefone = "187545754",
                CEP = "18741852",
                Estado = "PR",
                Cidade = "Londrina"
            };

            db.Attach(cliente); // o State fica como Added

            if (db.Entry(cliente).State == EntityState.Unchanged)
                return;

            db.SaveChanges();
        }

        private static void Inserir()
        {
            var produto = new Produto()
            {
                Descricao = "Produto 1",
                CodigoBarras = "54368446146134537137697",
                Valor = 32.9m
            };

            using var db = new Data.CursoEFCoreContext();
            // Opção 1: básica
            db.Produtos.Add(produto);

            // Opção 2: utilizando método genérico
            db.Set<Produto>().Add(produto);

            // Opção 3: forçando o rastreamento de uma determinada entidade
            db.Entry(produto).State = EntityState.Added; // indico explicitamente que a entidade está em estado de adição.

            // Opção 4: própria instância do contexto
            db.Add(produto);

            /* Das 4 formas acima, as mais indicadas são a 1 e 2.
             * As operações acima foram feitas apenas em memória, ainda não persistidas em memória.
             */

            var registros = db.SaveChanges(); // tudo está sendo rastreado e que contenha modificações.
            System.Console.WriteLine($"Registros: {registros}");
            // Saída do console:
            // Registros: 1

            /* Apesar dos 4 métodos acima estarem realizando o procedimento de adição do produto, que aparentemente está sendo feitas 4 vezes.
             * O EF adiciou apenas 1 registro.
             * Pq?
             * O EF faz o rastreamento da instância da objeto do tipo da entidade, como "produto" não teve alteração
             * depois de sua inicialização (e entre os 4 procedimentos de inserção), ao realizar o salvamento das modificações,
             * o EF detecta apenas uma modificação.
             */
        }

        private static void InserirUmRegistroDeDiferentesEntidades()
        {
            var produto = new Produto
            {
                Descricao = "Produto Xis",
                CodigoBarras = "329487230483276409",
                Valor = 13.21m,
                TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda
            };

            var cliente = new Cliente {
                Nome = "Lívia Souza",
                Telefone = "187974967",
                CEP = "190" +
                "26680",
                Cidade = "Pres. Prudente",
                Estado = "SP",
                Email = "liviags.com"
            };

            using var db = new Data.CursoEFCoreContext();

            db.AddRange(cliente, produto); // disponível a partir do EF Core (Framework não tinha)
            db.SaveChanges();
        }

        private static void InserirPedidoItem()
        {
            using var dbContext = new Data.CursoEFCoreContext();

            var cliente = dbContext.Clientes.FirstOrDefault();
            var produto = dbContext.Produtos.FirstOrDefault();
            var produto2 = dbContext.Produtos.FirstOrDefault(c => c.Id != produto.Id);

            if (cliente == null || produto == null)
                return;

            var quantidade = 3;

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEM = DateTime.Now,
                Observacao = "Pedido teste.",
                TipoFrete = ValueObjects.TipoFrete.SemFrete,
                Status = true,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Quantidade = quantidade,
                        Valor = quantidade * produto.Valor,
                        ValorDesconto = 0
                    }
                }
            };

            dbContext.Pedidos.Add(pedido);

            dbContext.SaveChanges();
        }

        private static void InserirVariasRegistrosDeUmaEntidade()
        {
            var produtos = new Produto[] {
                new Produto
                {
                    Descricao = "Em passa 01",
                    CodigoBarras = "329487230483276409",
                    Valor = 13.21m,
                    TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda
                },
                new Produto
                {
                    Descricao = "Em passa 02",
                    CodigoBarras = "498057320948673432",
                    Valor = 49.18m,
                    TipoProduto = ValueObjects.TipoProduto.Embalagem
                }
            };

            using var db = new Data.CursoEFCoreContext();

            db.Produtos.AddRange(produtos);
            
            db.SaveChanges();
        }

        #endregion

        #region Consultas

        // Forma 1: LINQ Query Sintax (mais próximo do SQL)
        private static void ConsultaForma1()
        {
            Console.WriteLine("LINQ Query Sintax");
            Console.WriteLine();

            using var db = new Data.CursoEFCoreContext();

            // para fazer projetar mais de um campo no SELECT, tem duas formas:

            // usando objeto anonimo
            // Usando o ToList() executa imediatamente o comando na base de dados e materializa os dados.
            //var resultadoConsulta = (from c in db.Clientes where c.Id > 0 select new { Blabla = c.Nome, xxxxx = c.Id }).ToList();

            // usando tupla
            var resultadoConsulta = from c in db.Clientes 
                                    where c.Id > 0 
                                    select (new Tuple<int, string>(c.Id, c.Nome));

            foreach (var c in resultadoConsulta)
            {
                // usando objeto anonimo
                //System.Console.WriteLine($"Cliente {c.xxxxx}: {c.Blabla}");

                // Usando tuples
                Console.WriteLine($"Cliente {c.Item1}: {c.Item2}");
            }

            Console.WriteLine();
        }

        // Forma 2: Extensions methods do LINQ (fluent way)
        private static void ConsultaForma2()
        {
            Console.WriteLine("Extensions methods do LINQ (fluent way)");
            Console.WriteLine();

            using var db = new Data.CursoEFCoreContext();

            // Com objeto anonimo (para retornar campos personalizados no SELECT), senão retornaria o próprio objeto da instância de Cliente
            var resultadoConsulta = db.Clientes
                .Where(c => c.Id > 0)
                .Select(c => new { Codigo = c.Id, NomeCliente = c.Nome })
                .ToList();

            foreach(var c in resultadoConsulta)
            {
                Console.WriteLine($"Cliente {c.Codigo}: {c.NomeCliente}");
            }

            Console.WriteLine();
        }

        private static void ConsultaComAsNoTracking()
        {
            Console.WriteLine("Consulta com entidade em rastreamento.");
            Console.WriteLine();

            using var db = new Data.CursoEFCoreContext();
            
            var resultadoConsulta = db.Clientes
                .AsNoTracking() // qdo materializar o objeto em memória, não rastrear. Forçando o "Find" por exemplo, a sempre buscar na memória.
                .Where(c => c.Id > 0)
                .Select(c => new { Codigo = c.Id, NomeCliente = c.Nome })
                .ToList();

            foreach (var c in resultadoConsulta)
            {
                var idParaConsulta = 4;

                Console.WriteLine();
                Console.WriteLine($"Procurando cliente {idParaConsulta}");

                // O "Find" primeiramente busca nos objetos/entidades em memória, se não encontrar, consulta no banco.
                //var cliente = db.Clientes.Find(c.Codigo); // nessa caso, não vai precisar fazer consulta no banco de dados pq estou procurando pelo mesmo código que já foi consultado.
                //var cliente = db.Clientes.Find(idParaConsulta);

                var cliente = db.Clientes.FirstOrDefault(c => c.Id == idParaConsulta); // APENAS o Find tentar consultar em memório, os demais métodos não.

                if (cliente == null)
                    Console.WriteLine("Cliente não localizado.");
                else
                    Console.WriteLine($"Cliente {c.Codigo}: {c.NomeCliente} encontrado.");
            }

            Console.WriteLine();
        }

        private static void ConsultaComOrdenacao()
        {
            Console.WriteLine("Consulta com entidade em rastreamento.");
            Console.WriteLine();

            using var db = new Data.CursoEFCoreContext();

            var resultadoConsulta = db.Clientes
                .AsNoTracking() // qdo materializar o objeto em memória, não rastrear. Forçando o "Find" por exemplo, a sempre buscar na memória.
                .Where(c => c.Id > 0)
                .OrderBy(c => c.Cidade)
                    .ThenBy(c => c.Nome)
                .Select(c => new { Codigo = c.Id, NomeCliente = c.Nome })
                .ToList();

            foreach (var c in resultadoConsulta)
            {
                var idParaConsulta = 4;

                Console.WriteLine();
                Console.WriteLine($"Procurando cliente {idParaConsulta}");

                // O "Find" primeiramente busca nos objetos/entidades em memória, se não encontrar, consulta no banco.
                //var cliente = db.Clientes.Find(c.Codigo); // nessa caso, não vai precisar fazer consulta no banco de dados pq estou procurando pelo mesmo código que já foi consultado.
                //var cliente = db.Clientes.Find(idParaConsulta);

                var cliente = db.Clientes.FirstOrDefault(c => c.Id == idParaConsulta); // APENAS o Find tentar consultar em memório, os demais métodos não.

                if (cliente == null)
                    Console.WriteLine("Cliente não localizado.");
                else
                    Console.WriteLine($"Cliente {c.Codigo}: {c.NomeCliente} encontrado.");
            }

            Console.WriteLine();
        }

        #endregion

        #region Consultas (avançadas)

        private static void ConsultasComPropriedadesNavegacaoCarregamentoAdiantado()
        {
            // Significa que as entidades que são propriedade de navegação, são carregadas numa única consulta;

            Console.WriteLine("Carregamento Adiantado.");
            Console.WriteLine();

            using var db = new Data.CursoEFCoreContext();

            var pedido = db.Pedidos
                .Include(p => p.Itens) // carregamento adiantado (usando lambda expressions, ou...)
                //.Include("Itens") // ... através da nomenclatura da propriedade de navegação.
                    .ThenInclude(pi => pi.Produto) // carrega (adiantado) o Produto qdo carregar o PedidoItem (é um Include condicional)
                .FirstOrDefault();
            
            Console.WriteLine($"Quantidade de pedidos: {pedido.Itens.Count()}"); // Sem o carregamento adiantado, "Itens" estaria nulo.
            foreach(var pedidoItem in pedido.Itens)
            {
                Console.WriteLine($"Id do produto: {pedidoItem.Produto.Descricao}"); // Sem o carregamento adiantado, "Produto" estaria nulo.
            }
            
            Console.WriteLine();
        }

        private static void ConsultasComPropriedadesNavegacaoCarregamentoExplicito()
        {
            // Significa que são carregados pelo usuário em um momento posterior;
            // Parece com Lazy Loading, mas de forma explícita;

            Console.WriteLine("Carregamento Explícito.");
            Console.WriteLine();

            using var db = new Data.CursoEFCoreContext();

            var pedidoItem = db.PedidoItens.FirstOrDefault();

            db.Entry(pedidoItem)
                .Reference(pi => pi.Pedido) // Inicialmente não foi carregado...
                .Load(); // ... carregamento explícito

            Console.WriteLine(pedidoItem.Pedido.Observacao);
        }

        private static void ConsultasComPropriedadesNavegacaoCarregamentoLento()
        {
            // Significa que são carregados sobre demanda;
            // Lazy Loading;

            // Link: https://www.tektutorialshub.com/entity-framework-core/explicit-loading-in-entity-framework-core/#:~:text=Explicit%20Loading%20in%20EF%20Core,of%20the%20related%20entity%27s%20DbContext.
        }

        private static void ConsultaSelectMany()
        {
            Console.WriteLine("SelectMany.");
            Console.WriteLine();

            using var db = new Data.CursoEFCoreContext();

            var pedidoItens = db.Pedidos
                .Where(p => p.Id > 0)
                .SelectMany(p => p.Itens)
                .ToList();
            
            foreach (var pedidoItem in pedidoItens)
            {
                //Console.WriteLine($"Pedido: {pedidoItem.Pedido.Id} - Item: {pedidoItem.Id}"); // com "Pedido.Id" é feito uma consulta novamente ao banco.
                Console.WriteLine($"Pedido: {pedidoItem.PedidoId} - Item: {pedidoItem.Id}");
            }

            Console.WriteLine();
        }

        private static void ConsultarClientesPorIdComLINQExpression(int? clienteId)
        {
            using var db = new Data.CursoEFCoreContext();

            Expression<Func<Cliente, bool>> expressao = c => c.Id == clienteId;

            Func<Cliente, bool> funcao = expressao.Compile();

            var cliente = db.Clientes.Where(funcao).FirstOrDefault();

            if (cliente == null)
                return;

            Console.WriteLine($"Cliente localizado: {cliente.Nome}");
        }

        private static void ConsultarPaginavel()
        {
            using var db = new Data.CursoEFCoreContext();

            int paginaInicial = 0;
            int tamanhoPagina = 20;

            var clientes = db.Clientes
                .OrderBy(c => c.Id)
                .Take(tamanhoPagina)
                .Skip(paginaInicial)
                .ToList();

            Console.WriteLine($"Paginação {paginaInicial}/{tamanhoPagina}");
            Console.WriteLine();

            //while (!clientes.ENuloOuVazio())
            while (clientes.Any())
            {
                foreach (var cliente in clientes)
                {
                    Console.WriteLine($"Cliente: {cliente.Nome}");
                    Console.WriteLine();
                }

                paginaInicial += 4;

                clientes = db.Clientes
                    .OrderBy(c => c.Id)
                    .Take(tamanhoPagina)
                    .Skip(paginaInicial)
                    .ToList();
            }
        }

        private static void ConsultarComLINQQuerySintax()
        {
            using var db = new Data.CursoEFCoreContext();

            var clientes = from c in db.Clientes
                           select c.Nome;


        }

        #endregion

        #region Atualizações

        private static void AtualizarSimples()
        {
            using var db = new Data.CursoEFCoreContext();

            var cliente = db.Clientes.Where(c => c.Nome.Contains("Elton")).FirstOrDefault();

            if (cliente == null)
                return;

            
            /* Modo otimizado (instrução UPDATE somente com o campo alterado). O EF faz uma pré checagem no valor do campo para detectar que realmente ficou diferente, se sim, marca a instância como modificada. Se não, nem realizada o UPDATE no banco.
             */
            cliente.Nome = "Elton Souza IV";
            //db.Entry(cliente).State = EntityState.Modified; // se o valor não mudar, mas eu ainda quero realizar o UPDATE.

            //db.Clientes.Update(cliente); // se fizer dessa forma, o EF irá sobrescrever todas as informações. Gerando o comando UPDATE com todos os campos.

            db.SaveChanges();
        }

        #endregion

        #region Remoções

        private static void RemoverUmRegistroDeUmaInstancia()
        {
            using var db = new Data.CursoEFCoreContext();

            var clienteRemover = db.Clientes.Find(4);

            if (clienteRemover == null)
                return;

            // Existem 3 formas de excluir

            db.Clientes.Remove(clienteRemover); // mais legível
            //db.Remove(clienteRemover);
            //db.Entry(clienteRemover).State = EntityState.Deleted;

            db.SaveChanges();
        }

        #endregion
    }
}
