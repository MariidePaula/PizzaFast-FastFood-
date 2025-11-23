using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.Models;
using FastPizza.Business;

namespace FastPizza.DataAccess
{
    // Camada de acesso a dados para pedidos
    // Delega todas as operações para a camada de negócio (PedidoBusiness)
    public class PedidoDAO
    {
        // Retorna lista com todos os pedidos do sistema
        public static List<Pedido> ObterTodos()
        {
            return PedidoBusiness.ObterTodos();
        }

        // Busca um pedido específico pelo seu ID
        public static Pedido ObterPorId(int id)
        {
            return PedidoBusiness.ObterPorId(id);
        }

        // Adiciona um novo pedido ao banco de dados
        // Converte itens do modelo PedidoItem para ItemPedido antes de salvar
        public static void Adicionar(Pedido pedido)
        {
            // Converte itens do pedido para formato ItemPedido (modelo auxiliar)
            var itens = pedido.Itens != null ?
                pedido.Itens.Select(i => new ItemPedido
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.NomeProduto,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList() :
                new List<ItemPedido>();

            PedidoBusiness.Adicionar(pedido, itens);
        }

        // Atualiza o status de um pedido (ex: Pendente -> EmPreparo -> Entregue)
        // Se cancelado, pode incluir motivo do cancelamento
        public static void AtualizarStatus(int id, StatusPedido novoStatus, string motivoCancelamento = null)
        {
            PedidoBusiness.AtualizarStatus(id, novoStatus, motivoCancelamento);
        }

        // Método reservado para atualizações futuras de pedidos
        public static void Atualizar(Pedido pedido)
        {
            // Método reservado para atualizações futuras
        }

        // Atualiza os itens de um pedido existente
        // Permite adicionar, remover ou modificar itens após criação do pedido
        public static void AtualizarItens(int pedidoId, List<ItemPedido> novosItens)
        {
            PedidoBusiness.AtualizarPedido(pedidoId, novosItens);
        }

        // Cancela um pedido ao invés de excluir permanentemente (soft delete)
        // Mantém histórico do pedido no banco, apenas marca como cancelado
        public static Pedido Excluir(int id)
        {
            var pedido = ObterPorId(id);
            if (pedido != null)
            {
                // Marca como cancelado ao invés de excluir permanentemente
                AtualizarStatus(id, StatusPedido.Cancelado, "Pedido excluído pelo administrador");
                return pedido;
            }
            return null;
        }

        // Exclui pedido permanentemente do banco de dados (hard delete)
        // CUIDADO: Esta operação é irreversível e remove todos os dados do pedido
        public static void ExcluirPermanentemente(int id)
        {
            PedidoBusiness.ExcluirPermanentemente(id);
        }

        // Retorna todos os pedidos de um cliente específico
        // Usado para exibir histórico de pedidos do cliente
        public static List<Pedido> ObterPorCliente(int clienteId)
        {
            return PedidoBusiness.ObterPorCliente(clienteId);
        }
    }
}

