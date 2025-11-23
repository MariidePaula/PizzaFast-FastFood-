using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.Models;
using FastPizza.Business;

namespace FastPizza.DataAccess
{
    // Camada de acesso a dados para bebidas
    // Delega todas as operações para a camada de negócio (BebidaBusiness)
    public class BebidaDAO
    {
        // Retorna lista com todas as bebidas cadastradas (disponíveis e indisponíveis)
        public static List<Bebida> ObterTodos()
        {
            return BebidaBusiness.ObterTodos();
        }

        // Retorna apenas bebidas disponíveis para venda (Disponivel = true)
        // Usado para exibir no cardápio público
        public static List<Bebida> ObterDisponiveis()
        {
            return BebidaBusiness.ObterDisponiveis();
        }

        // Retorna bebidas filtradas por categoria (ex: Refrigerante, Suco, Chá, Água)
        public static List<Bebida> ObterPorCategoria(string categoria)
        {
            return BebidaBusiness.ObterPorCategoria(categoria);
        }

        // Busca uma bebida específica pelo seu ID
        public static Bebida ObterPorId(int id)
        {
            return BebidaBusiness.ObterPorId(id);
        }

        // Adiciona uma nova bebida ao banco de dados
        public static void Adicionar(Bebida bebida)
        {
            BebidaBusiness.Adicionar(bebida);
        }

        // Atualiza informações de uma bebida existente
        public static void Atualizar(Bebida bebida)
        {
            BebidaBusiness.Atualizar(bebida);
        }

        // Exclui permanentemente uma bebida do banco de dados
        public static void Excluir(int id)
        {
            BebidaBusiness.Excluir(id);
        }
    }
}

