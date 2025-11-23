using System;

namespace FastPizza.Models
{
    // Modelo de dados que representa um item individual dentro de um pedido
    // Armazena informações sobre cada produto (pizza ou bebida) adicionado ao pedido
    public class PedidoItem
    {
        // Identificador único do item no banco de dados
        public int Id { get; set; }
        // ID do pedido ao qual este item pertence
        public int PedidoId { get; set; }
        // ID do produto (pizza ou bebida) adicionado
        public int ProdutoId { get; set; }
        // Nome do produto no momento da compra (para histórico)
        public string NomeProduto { get; set; }
        // Quantidade deste item no pedido
        public int Quantidade { get; set; }
        // Preço unitário do produto no momento da compra
        public decimal PrecoUnitario { get; set; }
        // Calcula o subtotal do item multiplicando quantidade pelo preço unitário
        public decimal Subtotal { get { return Quantidade * PrecoUnitario; } }

        // Navegação para o pedido ao qual este item pertence (Entity Framework)
        public virtual Pedido Pedido { get; set; }
    }
}

