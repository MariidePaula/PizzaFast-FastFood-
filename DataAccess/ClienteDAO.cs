using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.Models;
using FastPizza.Business;

namespace FastPizza.DataAccess
{
    // Camada de acesso a dados para clientes
    // Delega todas as operações para a camada de negócio (ClienteBusiness)
    // Mantém separação de responsabilidades: DAO apenas expõe interface, Business implementa lógica
    public class ClienteDAO
    {
        // Retorna lista com todos os clientes cadastrados no sistema
        public static List<Cliente> ObterTodos()
        {
            return ClienteBusiness.ObterTodos();
        }

        // Busca um cliente específico pelo seu ID
        public static Cliente ObterPorId(int id)
        {
            return ClienteBusiness.ObterPorId(id);
        }

        // Busca um cliente pelo email (usado para verificar duplicidade e login)
        public static Cliente ObterPorEmail(string email)
        {
            return ClienteBusiness.ObterPorEmail(email);
        }

        // Autentica um cliente verificando email e senha
        // Retorna o cliente se credenciais estiverem corretas, null caso contrário
        public static Cliente Autenticar(string email, string senha)
        {
            return ClienteBusiness.Autenticar(email, senha);
        }

        // Verifica se já existe um cliente cadastrado com o email informado
        // Usado para validar cadastro e evitar emails duplicados
        public static bool EmailExiste(string email)
        {
            return ClienteBusiness.EmailExiste(email);
        }

        // Adiciona um novo cliente ao banco de dados
        public static void Adicionar(Cliente cliente)
        {
            ClienteBusiness.Adicionar(cliente);
        }

        // Atualiza informações de um cliente existente
        public static void Atualizar(Cliente cliente)
        {
            ClienteBusiness.Atualizar(cliente);
        }

        // Atualiza apenas a senha de um cliente específico
        // A senha é automaticamente hasheada antes de ser salva
        public static void AtualizarSenha(int id, string novaSenha)
        {
            ClienteBusiness.AtualizarSenha(id, novaSenha);
        }

        // Bloqueia ou desbloqueia um cliente
        // Clientes bloqueados não podem fazer pedidos
        public static void Bloquear(int id, bool bloquear)
        {
            ClienteBusiness.Bloquear(id, bloquear);
        }

        // Exclui permanentemente um cliente do banco de dados
        // CUIDADO: Esta operação é irreversível
        public static void Excluir(int id)
        {
            ClienteBusiness.Excluir(id);
        }
    }
}

