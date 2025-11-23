using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.DataAccess;
using FastPizza.Models;
using FastPizza.Utils;

namespace FastPizza.Business
{
    // Camada de regras de negócio para operações com clientes
    public class ClienteBusiness
    {
        // Retorna todos os clientes cadastrados no sistema
        public static List<Cliente> ObterTodos()
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Clientes.ToList();
            }
        }

        // Busca um cliente específico pelo ID
        public static Cliente ObterPorId(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                return context.Clientes.Find(id);
            }
        }

        // Busca um cliente pelo email (usado para verificar duplicidade e login)
        public static Cliente ObterPorEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            using (var context = new FastPizzaDbContext())
            {
                return context.Clientes
                    .FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
        }

        // Autentica cliente verificando email, senha e se está bloqueado
        // Retorna null se credenciais inválidas ou cliente bloqueado
        public static Cliente Autenticar(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                return null;

            try
            {
                var cliente = ObterPorEmail(email);
                // Verifica se cliente existe, não está bloqueado e senha está correta
                if (cliente != null && !cliente.Bloqueado &&
                    PasswordHasher.VerifyPassword(senha, cliente.SenhaHash))
                {
                    return cliente;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao autenticar cliente: {ex.Message}");
            }

            return null;
        }

        // Verifica se já existe um cliente cadastrado com o email informado
        public static bool EmailExiste(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            using (var context = new FastPizzaDbContext())
            {
                return context.Clientes
                    .Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
        }

        // Cadastra um novo cliente no sistema com validações de regra de negócio
        public static void Adicionar(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            // Validações obrigatórias antes de cadastrar
            if (string.IsNullOrWhiteSpace(cliente.Nome))
                throw new ArgumentException("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(cliente.Email))
                throw new ArgumentException("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(cliente.SenhaHash))
                throw new ArgumentException("Senha é obrigatória");

            // Impede cadastro de email duplicado
            if (EmailExiste(cliente.Email))
                throw new ArgumentException("Email já cadastrado");

            // Se a senha não estiver hasheada (menos de 64 caracteres), faz o hash
            if (!string.IsNullOrEmpty(cliente.SenhaHash) && cliente.SenhaHash.Length < 64)
            {
                cliente.SenhaHash = PasswordHasher.HashPassword(cliente.SenhaHash);
            }

            using (var context = new FastPizzaDbContext())
            {
                // Cria novo registro com data de cadastro atual
                var novoCliente = new Cliente
                {
                    Nome = cliente.Nome,
                    Email = cliente.Email,
                    Telefone = cliente.Telefone,
                    Endereco = cliente.Endereco,
                    Cidade = cliente.Cidade,
                    CEP = cliente.CEP,
                    SenhaHash = cliente.SenhaHash,
                    Bloqueado = cliente.Bloqueado,
                    DataCadastro = DateTime.Now
                };

                context.Clientes.Add(novoCliente);
                context.SaveChanges();

                // Retorna o ID gerado para o objeto original
                cliente.Id = novoCliente.Id;
            }
        }

        // Atualiza dados de um cliente existente, validando email único
        public static void Atualizar(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            if (string.IsNullOrWhiteSpace(cliente.Nome))
                throw new ArgumentException("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(cliente.Email))
                throw new ArgumentException("Email é obrigatório");

            using (var context = new FastPizzaDbContext())
            {
                var clienteExistente = context.Clientes.Find(cliente.Id);
                if (clienteExistente == null)
                    throw new ArgumentException("Cliente não encontrado");

                // Verifica se o novo email já está em uso por outro cliente
                if (clienteExistente.Email != cliente.Email && EmailExiste(cliente.Email))
                    throw new ArgumentException("Email já cadastrado");

                // Atualiza apenas os campos permitidos
                clienteExistente.Nome = cliente.Nome;
                clienteExistente.Email = cliente.Email;
                clienteExistente.Telefone = cliente.Telefone;
                clienteExistente.Endereco = cliente.Endereco;
                clienteExistente.Cidade = cliente.Cidade;
                clienteExistente.CEP = cliente.CEP;

                // Atualiza senha apenas se foi informada e já estiver hasheada
                if (!string.IsNullOrEmpty(cliente.SenhaHash) && cliente.SenhaHash.Length >= 64)
                {
                    clienteExistente.SenhaHash = cliente.SenhaHash;
                }

                context.SaveChanges();
            }
        }

        // Altera a senha do cliente, fazendo hash antes de salvar
        public static void AtualizarSenha(int id, string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(novaSenha))
                throw new ArgumentException("Senha é obrigatória");

            // Valida tamanho mínimo da senha
            if (novaSenha.Length < 6)
                throw new ArgumentException("Senha deve ter no mínimo 6 caracteres");

            using (var context = new FastPizzaDbContext())
            {
                var cliente = context.Clientes.Find(id);
                if (cliente != null)
                {
                    // Aplica hash na nova senha antes de salvar
                    cliente.SenhaHash = PasswordHasher.HashPassword(novaSenha);
                    context.SaveChanges();
                }
            }
        }

        // Bloqueia ou desbloqueia um cliente (usado pelo administrador)
        public static void Bloquear(int id, bool bloquear)
        {
            using (var context = new FastPizzaDbContext())
            {
                var cliente = context.Clientes.Find(id);
                if (cliente != null)
                {
                    cliente.Bloqueado = bloquear;
                    context.SaveChanges();
                }
            }
        }

        // Exclui permanentemente um cliente e todos seus dados relacionados
        // Remove endereços, pedidos e itens de pedidos em cascata
        public static void Excluir(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                var cliente = context.Clientes.Find(id);
                if (cliente == null)
                    throw new ArgumentException("Cliente não encontrado");

                // Remove todos os endereços do cliente
                var enderecos = context.Enderecos.Where(e => e.ClienteId == id).ToList();
                foreach (var endereco in enderecos)
                {
                    context.Enderecos.Remove(endereco);
                }

                // Remove todos os pedidos e seus itens
                var pedidos = context.Pedidos.Where(p => p.ClienteId == id).ToList();
                foreach (var pedido in pedidos)
                {
                    // Remove itens do pedido antes de remover o pedido
                    var itens = context.PedidoItens.Where(i => i.PedidoId == pedido.Id).ToList();
                    foreach (var item in itens)
                    {
                        context.PedidoItens.Remove(item);
                    }

                    context.Pedidos.Remove(pedido);
                }

                // Remove o cliente por último
                context.Clientes.Remove(cliente);

                context.SaveChanges();
            }
        }
    }
}

