using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.DataAccess;
using FastPizza.Models;

namespace FastPizza.Business
{
    // Camada de regras de negócio para operações com produtos (pizzas)
    // Implementa validações e lógica de negócio antes de acessar o banco de dados
    public class ProdutoBusiness
    {
        // Retorna todos os produtos cadastrados (disponíveis e indisponíveis)
        public static List<Produto> ObterTodos()
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Produtos.ToList();
            }
        }

        // Retorna apenas produtos disponíveis para venda
        // Filtra por Disponivel = true e Estoque > 0
        public static List<Produto> ObterDisponiveis()
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Produtos
                    .Where(p => p.Disponivel && p.Estoque > 0)
                    .ToList();
            }
        }

        // Retorna produtos marcados como em destaque e disponíveis
        // Usado para exibir na página inicial
        public static List<Produto> ObterEmDestaque()
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Produtos
                    .Where(p => p.Disponivel && p.Estoque > 0 && p.EmDestaque)
                    .ToList();
            }
        }

        // Retorna produtos filtrados por categoria
        // Normaliza nomes de categorias (ex: "Clássicas" -> "Clássica")
        public static List<Produto> ObterPorCategoria(string categoria)
        {
            using (var context = new FastPizzaDbContext())
            {
                // Se categoria for vazia ou "Todas", retorna todos disponíveis
                if (string.IsNullOrEmpty(categoria) || categoria == "Todas")
                {
                    return ObterDisponiveis();
                }

                // Normaliza nome da categoria para corresponder ao banco de dados
                string categoriaNormalizada = categoria;
                if (categoria == "Clássicas")
                    categoriaNormalizada = "Clássica";
                else if (categoria == "Tradicionais")
                    categoriaNormalizada = "Tradicional";
                else if (categoria == "Vegetarianas")
                    categoriaNormalizada = "Vegetariana";

                return context.Produtos
                    .Where(p => p.Disponivel && p.Estoque > 0 && p.Categoria == categoriaNormalizada)
                    .ToList();
            }
        }

        // Busca um produto específico pelo ID
        public static Produto ObterPorId(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Produtos.Find(id);
            }
        }

        // Adiciona um novo produto ao banco de dados
        // Valida dados obrigatórios e regras de negócio antes de salvar
        public static void Adicionar(Produto produto)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            // Validações de regra de negócio
            if (string.IsNullOrWhiteSpace(produto.Nome))
                throw new ArgumentException("Nome do produto é obrigatório");

            if (produto.Preco <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (produto.Estoque < 0)
                throw new ArgumentException("Estoque não pode ser negativo");

            using (var context = new FastPizzaDbContext())
            {
                // Cria novo produto com dados validados
                var novoProduto = new Produto
                {
                    Nome = produto.Nome,
                    Descricao = produto.Descricao,
                    Categoria = produto.Categoria,
                    Preco = produto.Preco,
                    ImagemUrl = produto.ImagemUrl,
                    Estoque = produto.Estoque,
                    Disponivel = produto.Disponivel,
                    EmDestaque = produto.EmDestaque
                };

                context.Produtos.Add(novoProduto);
                context.SaveChanges();

                // Retorna o ID gerado para o objeto original
                produto.Id = novoProduto.Id;
            }
        }

        // Atualiza informações de um produto existente
        // Valida dados antes de atualizar
        public static void Atualizar(Produto produto)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            // Validações de regra de negócio
            if (string.IsNullOrWhiteSpace(produto.Nome))
                throw new ArgumentException("Nome do produto é obrigatório");

            if (produto.Preco <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (produto.Estoque < 0)
                throw new ArgumentException("Estoque não pode ser negativo");

            using (var context = new FastPizzaDbContext())
            {
                var produtoExistente = context.Produtos.Find(produto.Id);
                if (produtoExistente == null)
                    throw new ArgumentException("Produto não encontrado");

                // Atualiza todos os campos do produto
                produtoExistente.Nome = produto.Nome;
                produtoExistente.Descricao = produto.Descricao;
                produtoExistente.Categoria = produto.Categoria;
                produtoExistente.Preco = produto.Preco;
                produtoExistente.ImagemUrl = produto.ImagemUrl;
                produtoExistente.Estoque = produto.Estoque;
                produtoExistente.Disponivel = produto.Disponivel;
                produtoExistente.EmDestaque = produto.EmDestaque;

                context.SaveChanges();
            }
        }

        // Exclui permanentemente um produto do banco de dados
        public static void Excluir(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                var produto = context.Produtos.Find(id);
                if (produto != null)
                {
                    context.Produtos.Remove(produto);
                    context.SaveChanges();
                }
            }
        }
    }
}

