using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.Models;
using FastPizza.Business;

namespace FastPizza.DataAccess
{
    // Camada de acesso a dados para endereços
    // Delega todas as operações para a camada de negócio (EnderecoBusiness)
    public class EnderecoDAO
    {
        // Retorna todos os endereços cadastrados por um cliente
        public static List<Endereco> ObterPorCliente(int clienteId)
        {
            return EnderecoBusiness.ObterPorCliente(clienteId);
        }

        // Busca um endereço específico pelo seu ID
        public static Endereco ObterPorId(int id)
        {
            return EnderecoBusiness.ObterPorId(id);
        }

        // Busca o endereço padrão do cliente (usado automaticamente para entrega)
        // Retorna o endereço marcado com EnderecoPadrao = true
        public static Endereco ObterEnderecoPadrao(int clienteId)
        {
            var enderecos = ObterPorCliente(clienteId);
            return enderecos.FirstOrDefault(e => e.EnderecoPadrao);
        }

        // Adiciona um novo endereço ao banco de dados
        public static void Adicionar(Endereco endereco)
        {
            EnderecoBusiness.Adicionar(endereco);
        }

        // Atualiza informações de um endereço existente
        public static void Atualizar(Endereco endereco)
        {
            EnderecoBusiness.Atualizar(endereco);
        }

        // Remove um endereço do banco de dados
        public static void Remover(int id)
        {
            EnderecoBusiness.Remover(id);
        }

        // Define um endereço como padrão do cliente
        // Automaticamente remove o padrão anterior e define o novo
        public static void DefinirComoPadrao(int id, int clienteId)
        {
            EnderecoBusiness.DefinirComoPadrao(id, clienteId);
        }
    }
}

