using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.DataAccess;
using FastPizza.Models;

namespace FastPizza.Business
{
    // Camada de regras de negócio para operações com bebidas
    // Implementa validações e lógica de negócio antes de acessar o banco de dados
    public class BebidaBusiness
    {
        // Retorna todas as bebidas cadastradas (disponíveis e indisponíveis)
        public static List<Bebida> ObterTodos()
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Bebidas.ToList();
            }
        }

        // Retorna apenas bebidas disponíveis para venda
        // Filtra por Disponivel = true e Estoque > 0
        public static List<Bebida> ObterDisponiveis()
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Bebidas
                    .Where(b => b.Disponivel && b.Estoque > 0)
                    .ToList();
            }
        }

        // Retorna bebidas filtradas por categoria
        // Se categoria for vazia ou "Todas", retorna todas disponíveis
        public static List<Bebida> ObterPorCategoria(string categoria)
        {
            using (var context = new FastPizzaDbContext())
            {
                if (string.IsNullOrEmpty(categoria) || categoria == "Todas")
                {
                    return ObterDisponiveis();
                }

                return context.Bebidas
                    .Where(b => b.Disponivel && b.Estoque > 0 && b.Categoria == categoria)
                    .ToList();
            }
        }

        // Busca uma bebida específica pelo ID
        public static Bebida ObterPorId(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Bebidas.Find(id);
            }
        }

        // Adiciona uma nova bebida ao banco de dados
        // Valida dados obrigatórios e regras de negócio antes de salvar
        public static void Adicionar(Bebida bebida)
        {
            if (bebida == null)
                throw new ArgumentNullException(nameof(bebida));

            // Validações de regra de negócio
            if (string.IsNullOrWhiteSpace(bebida.Nome))
                throw new ArgumentException("Nome da bebida é obrigatório");

            if (bebida.Preco <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (bebida.Estoque < 0)
                throw new ArgumentException("Estoque não pode ser negativo");

            using (var context = new FastPizzaDbContext())
            {
                // Cria nova bebida com dados validados
                var novaBebida = new Bebida
                {
                    Nome = bebida.Nome,
                    Descricao = bebida.Descricao,
                    Categoria = bebida.Categoria,
                    Preco = bebida.Preco,
                    ImagemUrl = bebida.ImagemUrl,
                    Estoque = bebida.Estoque,
                    Disponivel = bebida.Disponivel
                };

                context.Bebidas.Add(novaBebida);
                context.SaveChanges();

                // Retorna o ID gerado para o objeto original
                bebida.Id = novaBebida.Id;
            }
        }

        // Atualiza informações de uma bebida existente
        // Valida dados antes de atualizar
        public static void Atualizar(Bebida bebida)
        {
            if (bebida == null)
                throw new ArgumentNullException(nameof(bebida));

            // Validações de regra de negócio
            if (string.IsNullOrWhiteSpace(bebida.Nome))
                throw new ArgumentException("Nome da bebida é obrigatório");

            if (bebida.Preco <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (bebida.Estoque < 0)
                throw new ArgumentException("Estoque não pode ser negativo");

            using (var context = new FastPizzaDbContext())
            {
                var bebidaExistente = context.Bebidas.Find(bebida.Id);
                if (bebidaExistente == null)
                    throw new ArgumentException("Bebida não encontrada");

                // Atualiza todos os campos da bebida
                bebidaExistente.Nome = bebida.Nome;
                bebidaExistente.Descricao = bebida.Descricao;
                bebidaExistente.Categoria = bebida.Categoria;
                bebidaExistente.Preco = bebida.Preco;
                bebidaExistente.ImagemUrl = bebida.ImagemUrl;
                bebidaExistente.Estoque = bebida.Estoque;
                bebidaExistente.Disponivel = bebida.Disponivel;

                context.SaveChanges();
            }
        }

        // Exclui permanentemente uma bebida do banco de dados
        public static void Excluir(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                var bebida = context.Bebidas.Find(id);
                if (bebida != null)
                {
                    context.Bebidas.Remove(bebida);
                    context.SaveChanges();
                }
            }
        }
    }
}

