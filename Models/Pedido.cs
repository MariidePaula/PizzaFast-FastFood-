using System;
using System.Collections.Generic;

namespace FastPizza.Models
{
    // Enumeração que define os possíveis status de um pedido
    public enum StatusPedido
    {
        Pendente = 1,          // Pedido recebido, aguardando preparo
        EmPreparo = 2,          // Pedido sendo preparado na cozinha
        SaiuParaEntrega = 3,    // Pedido saiu para entrega
        Entregue = 4,           // Pedido foi entregue ao cliente
        Cancelado = 5           // Pedido foi cancelado
    }

    // Classe auxiliar que representa um item individual dentro de um pedido
    public class ItemPedido
    {
        // ID do produto ou bebida adicionado ao pedido
        public int ProdutoId { get; set; }
        // Nome do produto para exibição
        public string NomeProduto { get; set; }
        // Quantidade do item no pedido
        public int Quantidade { get; set; }
        // Preço unitário do item no momento da compra
        public decimal PrecoUnitario { get; set; }
        // Calcula o subtotal multiplicando quantidade pelo preço unitário
        public decimal Subtotal { get { return Quantidade * PrecoUnitario; } }
    }

    // Modelo de dados que representa um pedido completo do sistema
    public class Pedido
    {
        // Identificador único do pedido no banco de dados
        public int Id { get; set; }
        // ID do cliente que fez o pedido
        public int ClienteId { get; set; }
        // Nome do cliente (armazenado para histórico, mesmo se cliente for excluído)
        public string NomeCliente { get; set; }
        // Data e hora em que o pedido foi realizado
        public DateTime DataPedido { get; set; }
        // Status atual do pedido no fluxo de entrega
        public StatusPedido Status { get; set; }
        // Valor total do pedido (soma de todos os itens)
        public decimal Total { get; set; }
        // Motivo do cancelamento, se o pedido foi cancelado
        public string MotivoCancelamento { get; set; }
        // Data e hora em que o pedido foi cancelado (null se não foi cancelado)
        public DateTime? DataCancelamento { get; set; }
        // Observações adicionais do cliente sobre o pedido
        public string Observacoes { get; set; }

        // Lista de itens (pizzas e bebidas) que compõem o pedido
        public virtual ICollection<PedidoItem> Itens { get; set; }

        // Construtor padrão - inicializa valores padrão para novo pedido
        public Pedido()
        {
            // Inicializa lista vazia de itens
            Itens = new System.Collections.Generic.List<PedidoItem>();
            // Define data do pedido como data/hora atual
            DataPedido = DateTime.Now;
            // Novo pedido sempre começa como Pendente
            Status = StatusPedido.Pendente;
        }
    }
}

