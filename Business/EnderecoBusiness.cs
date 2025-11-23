using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.DataAccess;
using FastPizza.Models;

namespace FastPizza.Business
{
    // Camada de regras de negócio para operações com endereços de clientes
    public class EnderecoBusiness
    {
        // Retorna todos os endereços de um cliente, ordenados por padrão primeiro
        public static List<Endereco> ObterPorCliente(int clienteId)
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Enderecos
                    .Where(e => e.ClienteId == clienteId)
                    .OrderByDescending(e => e.EnderecoPadrao)
                    .ThenBy(e => e.DataCadastro)
                    .ToList();
            }
        }

        // Busca um endereço específico pelo ID
        public static Endereco ObterPorId(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Enderecos.Find(id);
            }
        }

        // Cadastra um novo endereço para um cliente com validações obrigatórias
        // Se for marcado como padrão, remove padrão dos outros endereços
        // Atualiza cidade do cliente se for primeiro endereço ou padrão
        public static void Adicionar(Endereco endereco)
        {
            if (endereco == null)
                throw new ArgumentNullException(nameof(endereco));

            // Validações de campos obrigatórios
            if (string.IsNullOrWhiteSpace(endereco.Rua))
                throw new ArgumentException("Rua é obrigatória");

            if (string.IsNullOrWhiteSpace(endereco.Numero))
                throw new ArgumentException("Número é obrigatório");

            if (string.IsNullOrWhiteSpace(endereco.Bairro))
                throw new ArgumentException("Bairro é obrigatório");

            if (string.IsNullOrWhiteSpace(endereco.Cidade))
                throw new ArgumentException("Cidade é obrigatória");

            if (string.IsNullOrWhiteSpace(endereco.CEP))
                throw new ArgumentException("CEP é obrigatório");

            using (var context = new FastPizzaDbContext())
            {
                // Verifica se é o primeiro endereço do cliente
                bool ehPrimeiroEndereco = !context.Enderecos.Any(e => e.ClienteId == endereco.ClienteId);

                // Se este endereço for padrão, remove padrão dos outros
                if (endereco.EnderecoPadrao)
                {
                    var outrosEnderecos = context.Enderecos
                        .Where(e => e.ClienteId == endereco.ClienteId && e.EnderecoPadrao);
                    foreach (var outro in outrosEnderecos)
                    {
                        outro.EnderecoPadrao = false;
                    }
                }

                endereco.DataCadastro = DateTime.Now;
                context.Enderecos.Add(endereco);
                context.SaveChanges();

                // Sincroniza cidade do cliente com endereço padrão ou primeiro endereço
                var cliente = context.Clientes.Find(endereco.ClienteId);
                if (cliente != null)
                {
                    if (endereco.EnderecoPadrao || ehPrimeiroEndereco)
                    {
                        cliente.Cidade = endereco.Cidade;
                        cliente.Endereco = endereco.Rua;
                        cliente.CEP = endereco.CEP;
                        context.SaveChanges();
                    }
                }
            }
        }

        // Atualiza dados de um endereço existente
        // Se for marcado como padrão, remove padrão dos outros endereços do mesmo cliente
        public static void Atualizar(Endereco endereco)
        {
            if (endereco == null)
                throw new ArgumentNullException(nameof(endereco));

            // Validações de campos obrigatórios
            if (string.IsNullOrWhiteSpace(endereco.Rua))
                throw new ArgumentException("Rua é obrigatória");

            if (string.IsNullOrWhiteSpace(endereco.Numero))
                throw new ArgumentException("Número é obrigatório");

            if (string.IsNullOrWhiteSpace(endereco.Bairro))
                throw new ArgumentException("Bairro é obrigatório");

            if (string.IsNullOrWhiteSpace(endereco.Cidade))
                throw new ArgumentException("Cidade é obrigatória");

            if (string.IsNullOrWhiteSpace(endereco.CEP))
                throw new ArgumentException("CEP é obrigatório");

            using (var context = new FastPizzaDbContext())
            {
                var enderecoExistente = context.Enderecos.Find(endereco.Id);
                if (enderecoExistente == null)
                    throw new ArgumentException("Endereço não encontrado");

                // Se este endereço está sendo marcado como padrão, remove padrão dos outros
                if (endereco.EnderecoPadrao && !enderecoExistente.EnderecoPadrao)
                {
                    var outrosEnderecos = context.Enderecos
                        .Where(e => e.ClienteId == endereco.ClienteId && e.Id != endereco.Id && e.EnderecoPadrao);
                    foreach (var outro in outrosEnderecos)
                    {
                        outro.EnderecoPadrao = false;
                    }
                }

                // Atualiza todos os campos do endereço
                enderecoExistente.Rua = endereco.Rua;
                enderecoExistente.Numero = endereco.Numero;
                enderecoExistente.Complemento = endereco.Complemento;
                enderecoExistente.Bairro = endereco.Bairro;
                enderecoExistente.Cidade = endereco.Cidade;
                enderecoExistente.CEP = endereco.CEP;
                enderecoExistente.Referencia = endereco.Referencia;
                enderecoExistente.EnderecoPadrao = endereco.EnderecoPadrao;

                context.SaveChanges();

                // Sincroniza cidade do cliente se este endereço for padrão
                if (endereco.EnderecoPadrao)
                {
                    var cliente = context.Clientes.Find(endereco.ClienteId);
                    if (cliente != null)
                    {
                        cliente.Cidade = endereco.Cidade;
                        cliente.Endereco = endereco.Rua;
                        cliente.CEP = endereco.CEP;
                        context.SaveChanges();
                    }
                }
            }
        }

        // Remove um endereço do sistema
        public static void Remover(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                var endereco = context.Enderecos.Find(id);
                if (endereco != null)
                {
                    context.Enderecos.Remove(endereco);
                    context.SaveChanges();
                }
            }
        }

        // Define um endereço como padrão do cliente e remove padrão dos outros
        // Atualiza cidade do cliente com dados do endereço padrão
        public static void DefinirComoPadrao(int enderecoId, int clienteId)
        {
            using (var context = new FastPizzaDbContext())
            {
                // Remove padrão de todos os endereços do cliente
                var todosEnderecos = context.Enderecos
                    .Where(e => e.ClienteId == clienteId);
                foreach (var endereco in todosEnderecos)
                {
                    endereco.EnderecoPadrao = false;
                }

                // Define o endereço selecionado como padrão
                var enderecoPadrao = context.Enderecos.Find(enderecoId);
                if (enderecoPadrao != null && enderecoPadrao.ClienteId == clienteId)
                {
                    enderecoPadrao.EnderecoPadrao = true;

                    // Sincroniza dados do cliente com o endereço padrão
                    var cliente = context.Clientes.Find(clienteId);
                    if (cliente != null)
                    {
                        cliente.Cidade = enderecoPadrao.Cidade;
                        cliente.Endereco = enderecoPadrao.Rua;
                        cliente.CEP = enderecoPadrao.CEP;
                    }
                }

                context.SaveChanges();
            }
        }
    }
}

