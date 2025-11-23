using System;

namespace FastPizza.Models
{
    // Modelo de dados que representa um produto (pizza) do cardápio
    public class Produto
    {
        // Identificador único do produto no banco de dados
        public int Id { get; set; }
        // Nome da pizza exibido no cardápio
        public string Nome { get; set; }
        // Descrição detalhada dos ingredientes e características da pizza
        public string Descricao { get; set; }
        // Categoria da pizza (ex: Clássica, Tradicional, Premium, Vegetariana)
        public string Categoria { get; set; }
        // Preço de venda do produto
        public decimal Preco { get; set; }
        // URL ou caminho da imagem do produto para exibição
        public string ImagemUrl { get; set; }
        // Indica se o produto está disponível para venda no momento
        public bool Disponivel { get; set; }
        // Quantidade em estoque do produto
        public int Estoque { get; set; }
        // Indica se o produto deve aparecer em destaque na página inicial
        public bool EmDestaque { get; set; }
    }
}
