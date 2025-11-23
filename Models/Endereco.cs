using System;

namespace FastPizza.Models
{
    // Modelo de dados que representa um endereço de entrega de um cliente
    // Permite que clientes cadastrem múltiplos endereços e escolham um como padrão
    public class Endereco
    {
        // Identificador único do endereço no banco de dados
        public int Id { get; set; }
        // ID do cliente proprietário deste endereço
        public int ClienteId { get; set; }
        // Nome da rua ou avenida
        public string Rua { get; set; }
        // Número do endereço
        public string Numero { get; set; }
        // Complemento do endereço (apto, bloco, etc.)
        public string Complemento { get; set; }
        // Bairro onde está localizado o endereço
        public string Bairro { get; set; }
        // Cidade do endereço
        public string Cidade { get; set; }
        // CEP do endereço
        public string CEP { get; set; }
        // Ponto de referência para facilitar a localização
        public string Referencia { get; set; }
        // Indica se este é o endereço padrão do cliente (usado automaticamente)
        public bool EnderecoPadrao { get; set; }
        // Data e hora em que o endereço foi cadastrado
        public DateTime DataCadastro { get; set; }

        // Construtor padrão - inicializa valores padrão para novo endereço
        public Endereco()
        {
            // Define data de cadastro como data/hora atual
            DataCadastro = DateTime.Now;
            // Novo endereço não é padrão por padrão
            EnderecoPadrao = false;
        }

        // Propriedade calculada que retorna o endereço formatado completo
        public string EnderecoCompleto
        {
            get
            {
                // Monta endereço básico: Rua, Número
                string endereco = $"{Rua}, {Numero}";
                // Adiciona complemento se existir
                if (!string.IsNullOrEmpty(Complemento))
                    endereco += $" - {Complemento}";
                // Adiciona bairro, cidade e CEP
                endereco += $" - {Bairro}, {Cidade} - {CEP}";
                return endereco;
            }
        }
    }
}

