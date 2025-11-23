using System;

namespace FastPizza.Models
{
    // Modelo de dados que representa um molho adicional do cardápio
    // Permite adicionar molhos extras aos pedidos
    public class Molho
    {
        // Identificador único do molho no banco de dados
        public int Id { get; set; }
        // Nome do molho exibido no cardápio
        public string Nome { get; set; }
        // Descrição do molho (ingredientes, características)
        public string Descricao { get; set; }
        // Categoria do molho (ex: Picante, Doce, Tradicional)
        public string Categoria { get; set; }
        // Preço adicional do molho
        public decimal Preco { get; set; }
        // URL ou caminho da imagem do molho para exibição
        public string ImagemUrl { get; set; }
        // Indica se o molho está disponível para venda no momento
        public bool Disponivel { get; set; }
        // Quantidade em estoque do molho
        public int Estoque { get; set; }
    }
}

//não usado
