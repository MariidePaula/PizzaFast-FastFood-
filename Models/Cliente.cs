using System;

namespace FastPizza.Models
{
    // Modelo de dados que representa um cliente do sistema
    public class Cliente
    {
        // Identificador único do cliente no banco de dados
        public int Id { get; set; }
        // Nome completo do cliente
        public string Nome { get; set; }
        // Email usado para login e comunicação
        public string Email { get; set; }
        // Telefone de contato do cliente
        public string Telefone { get; set; }
        // Endereço principal do cliente (usado para entrega)
        public string Endereco { get; set; }
        // Cidade onde o cliente reside
        public string Cidade { get; set; }
        // CEP do endereço do cliente
        public string CEP { get; set; }
        // Hash da senha do cliente (nunca armazena senha em texto plano)
        public string SenhaHash { get; set; }
        // Indica se o cliente está bloqueado e não pode fazer pedidos
        public bool Bloqueado { get; set; }
        // Data e hora em que o cliente foi cadastrado no sistema
        public DateTime DataCadastro { get; set; }

        // Construtor padrão - inicializa valores padrão para novo cliente
        public Cliente()
        {
            // Define data de cadastro como data/hora atual
            DataCadastro = DateTime.Now;
            // Novo cliente não está bloqueado por padrão
            Bloqueado = false;
        }
    }
}

