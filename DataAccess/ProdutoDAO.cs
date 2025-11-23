using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.Models;
using FastPizza.Business;

namespace FastPizza.DataAccess
{
    // Camada de acesso a dados para produtos (pizzas)
    // Delega todas as operações para a camada de negócio (ProdutoBusiness)
    public class ProdutoDAO
    {
        // Retorna lista com todos os produtos cadastrados (disponíveis e indisponíveis)
        public static List<Produto> ObterTodos()
        {
            return ProdutoBusiness.ObterTodos();
        }

        // Retorna apenas produtos disponíveis para venda (Disponivel = true)
        // Usado para exibir no cardápio público
        public static List<Produto> ObterDisponiveis()
        {
            return ProdutoBusiness.ObterDisponiveis();
        }

        // Retorna produtos marcados como em destaque
        // Usado para exibir na página inicial
        public static List<Produto> ObterEmDestaque()
        {
            return ProdutoBusiness.ObterEmDestaque();
        }

        // Retorna produtos filtrados por categoria (ex: Clássica, Premium, Vegetariana)
        public static List<Produto> ObterPorCategoria(string categoria)
        {
            return ProdutoBusiness.ObterPorCategoria(categoria);
        }

        // Busca um produto específico pelo seu ID
        public static Produto ObterPorId(int id)
        {
            return ProdutoBusiness.ObterPorId(id);
        }

        // Adiciona um novo produto ao banco de dados
        public static void Adicionar(Produto produto)
        {
            ProdutoBusiness.Adicionar(produto);
        }

        // Atualiza informações de um produto existente
        public static void Atualizar(Produto produto)
        {
            ProdutoBusiness.Atualizar(produto);
        }

        // Exclui permanentemente um produto do banco de dados
        public static void Excluir(int id)
        {
            ProdutoBusiness.Excluir(id);
        }
    }
}

