using System;

namespace FastPizza.Models
{
    // Modelo de dados que representa uma configuração do sistema
    // Usado para armazenar configurações gerais como URL do banner, etc.
    public class Configuracao
    {
        // Identificador único da configuração no banco de dados
        public int Id { get; set; }
        // Chave única que identifica a configuração (ex: "BannerImageUrl")
        public string Chave { get; set; }
        // Valor da configuração (armazenado como string)
        public string Valor { get; set; }
        // Data e hora da última atualização desta configuração
        public DateTime DataAtualizacao { get; set; }
    }
}

