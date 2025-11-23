using System;
using System.Data.Entity;
using FastPizza.Models;
using FastPizza.Utils;

namespace FastPizza.DataAccess
{
    // Inicializador do banco de dados - cria tabelas e popula dados iniciais
    public class DatabaseInitializer : CreateDatabaseIfNotExists<FastPizzaDbContext>
    {
        // Popula banco com dados iniciais (clientes, produtos, bebidas)
        protected override void Seed(FastPizzaDbContext context)
        {
            // Cria coluna Observacoes se não existir (migração automática)
            try
            {
                context.Database.ExecuteSqlCommand(@"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Pedidos]') AND name = 'Observacoes')
                    BEGIN
                        ALTER TABLE [dbo].[Pedidos] ADD [Observacoes] NVARCHAR(1000) NULL
                    END");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar/adicionar coluna Observacoes: {ex.Message}");
            }

            try
            {

                var cliente1 = new Cliente
                {
                    Nome = "João Silva",
                    Email = "joao@email.com",
                    Telefone = "(11) 99999-9999",
                    Endereco = "Rua Exemplo, 123",
                    Cidade = "São Paulo",
                    CEP = "01234-567",
                    SenhaHash = PasswordHasher.HashPassword("123456"),
                    Bloqueado = false,
                    DataCadastro = DateTime.Now.AddDays(-30)
                };

                var cliente2 = new Cliente
                {
                    Nome = "Maria Santos",
                    Email = "maria@email.com",
                    Telefone = "(11) 88888-8888",
                    Endereco = "Av. Teste, 456",
                    Cidade = "São Paulo",
                    CEP = "02345-678",
                    SenhaHash = PasswordHasher.HashPassword("123456"),
                    Bloqueado = false,
                    DataCadastro = DateTime.Now.AddDays(-15)
                };

                context.Clientes.Add(cliente1);
                context.Clientes.Add(cliente2);

                var produtos = new[]
                {
                    new Produto { Nome="Mussarela Tradicional", Descricao="Bastante queijo mussarela, rodelas de tomate fresco e orégano.", Categoria="Clássica", Preco=39.90m, ImagemUrl="", Disponivel=true, Estoque=10, EmDestaque=true },
                    new Produto { Nome="Marguerita Especial", Descricao="Molho de tomate artesanal, mussarela, manjericão fresco e azeite.", Categoria="Clássica", Preco=42.50m, ImagemUrl="", Disponivel=true, Estoque=12, EmDestaque=true },
                    new Produto { Nome="Napolitana", Descricao="Mussarela, tomate em cubos, parmesão ralado e alho frito crocante.", Categoria="Clássica", Preco=44.00m, ImagemUrl="", Disponivel=true, Estoque=8, EmDestaque=false },
                    new Produto { Nome="Calabresa Acebolada", Descricao="Generosas fatias de calabresa, cebola roxa e azeitonas pretas.", Categoria="Tradicional", Preco=48.90m, ImagemUrl="", Disponivel=true, Estoque=15, EmDestaque=true },
                    new Produto { Nome="Pizza Pepperoni", Descricao="Molho de tomate, mussarela e muitas fatias de pepperoni.", Categoria="Tradicional", Preco=52.90m, ImagemUrl="", Disponivel=true, Estoque=18, EmDestaque=true },
                    new Produto { Nome="Portuguesa Completa", Descricao="Presunto, ovos, cebola, ervilha, palmito e cobertura de mussarela.", Categoria="Tradicional", Preco=54.90m, ImagemUrl="", Disponivel=true, Estoque=14, EmDestaque=false },
                    new Produto { Nome="Quattro Formaggi (Quatro Queijos)", Descricao="Blend especial de Mussarela, Gorgonzola, Parmesão e Provolone.", Categoria="Premium", Preco=58.90m, ImagemUrl="", Disponivel=true, Estoque=8, EmDestaque=true },
                    new Produto { Nome="Parma com Rúcula", Descricao="Mussarela de búfala, presunto de parma fininho e rúcula fresca.", Categoria="Premium", Preco=65.00m, ImagemUrl="", Disponivel=true, Estoque=6, EmDestaque=false },
                    new Produto { Nome="Camarão com Catupiry", Descricao="Camarões refogados no alho e óleo coberto com Catupiry original.", Categoria="Premium", Preco=72.00m, ImagemUrl="", Disponivel=true, Estoque=5, EmDestaque=false },
                    new Produto { Nome="Vegetariana Especial", Descricao="Pimentões, cogumelos, cebola, azeitonas, milho e tomates frescos.", Categoria="Vegetariana", Preco=48.90m, ImagemUrl="", Disponivel=true, Estoque=12, EmDestaque=false },
                    new Produto { Nome="Tomate Seco com Rúcula", Descricao="Base de mussarela, tomate seco artesanal e folhas de rúcula.", Categoria="Vegetariana", Preco=51.00m, ImagemUrl="", Disponivel=true, Estoque=10, EmDestaque=false },
                    new Produto { Nome="Brócolis com Alho", Descricao="Brócolis ninja cozido no vapor, alho frito dourado e cobertura de catupiry.", Categoria="Vegetariana", Preco=46.00m, ImagemUrl="", Disponivel=true, Estoque=9, EmDestaque=false }
                };

                // Define as imagens usando o ImageMapper
                foreach (var produto in produtos)
                {
                    produto.ImagemUrl = ImageMapper.ObterImagemProduto(produto);
                    context.Produtos.Add(produto);
                }

                // 11 bebidas correspondentes às 11 imagens disponíveis
                var bebidas = new[]
                {
                    new Bebida { Nome="Coca-Cola Lata", Descricao="Refrigerante gelado e refrescante em lata.", Categoria="Refrigerante", Preco=5.50m, ImagemUrl="", Disponivel=true, Estoque=50 },
                    new Bebida { Nome="Guaraná Antarctica Lata", Descricao="Refrigerante gelado e refrescante em lata.", Categoria="Refrigerante", Preco=5.00m, ImagemUrl="", Disponivel=true, Estoque=40 },
                    new Bebida { Nome="Fanta Lata", Descricao="Refrigerante gelado e refrescante em lata.", Categoria="Refrigerante", Preco=5.00m, ImagemUrl="", Disponivel=true, Estoque=35 },
                    new Bebida { Nome="Sprite Lata", Descricao="Refrigerante gelado e refrescante em lata.", Categoria="Refrigerante", Preco=5.00m, ImagemUrl="", Disponivel=true, Estoque=30 },
                    new Bebida { Nome="Suco de Laranja", Descricao="Suco natural de laranja.", Categoria="Suco", Preco=8.00m, ImagemUrl="", Disponivel=true, Estoque=30 },
                    new Bebida { Nome="Suco de Uva", Descricao="Suco natural de uva.", Categoria="Suco", Preco=8.50m, ImagemUrl="", Disponivel=true, Estoque=25 },
                    new Bebida { Nome="Chá de Limão", Descricao="Chá gelado sabor limão.", Categoria="Chá", Preco=7.50m, ImagemUrl="", Disponivel=true, Estoque=28 },
                    new Bebida { Nome="Chá de Pêssego", Descricao="Chá gelado sabor pêssego.", Categoria="Chá", Preco=7.50m, ImagemUrl="", Disponivel=true, Estoque=25 },
                    new Bebida { Nome="H2OH Limão", Descricao="Água saborizada com limão.", Categoria="Água", Preco=4.50m, ImagemUrl="", Disponivel=true, Estoque=40 },
                    new Bebida { Nome="Água Mineral sem Gás", Descricao="Água mineral natural sem gás.", Categoria="Água", Preco=3.00m, ImagemUrl="", Disponivel=true, Estoque=60 },
                    new Bebida { Nome="Água com Gás", Descricao="Água mineral com gás.", Categoria="Água", Preco=3.50m, ImagemUrl="", Disponivel=true, Estoque=40 }
                };

                // Define as imagens usando o ImageMapper
                foreach (var bebida in bebidas)
                {
                    bebida.ImagemUrl = ImageMapper.ObterImagemBebida(bebida);
                    context.Bebidas.Add(bebida);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar banco de dados: {ex.Message}");
                throw;
            }
        }
    }
}

