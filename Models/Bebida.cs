using System;

namespace FastPizza.Models
{
    // Modelo de dados que representa uma bebida do cardápio
    public class Bebida
    {
        // Identificador único da bebida no banco de dados
        public int Id { get; set; }
        // Nome da bebida exibido no cardápio
        public string Nome { get; set; }
        // Descrição da bebida (sabor, tipo, etc.)
        public string Descricao { get; set; }
        // Categoria da bebida (ex: Refrigerante, Suco, Chá, Água)
        public string Categoria { get; set; }
        // Preço de venda da bebida
        public decimal Preco { get; set; }
        // URL ou caminho da imagem da bebida para exibição
        public string ImagemUrl { get; set; }
        // Indica se a bebida está disponível para venda no momento
        public bool Disponivel { get; set; }
        // Quantidade em estoque da bebida
        public int Estoque { get; set; }
    }
}

