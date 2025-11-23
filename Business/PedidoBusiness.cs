using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.DataAccess;
using FastPizza.Models;

namespace FastPizza.Business
{
    public class PedidoBusiness
    {
        // Retorna todos os pedidos ordenados por data (mais recentes primeiro)
        public static List<Pedido> ObterTodos()
        {
            using (var context = new FastPizzaDbContext())
            {
                var pedidos = context.Pedidos
                    .OrderByDescending(p => p.DataPedido)
                    .ToList();

                // Carrega os itens de cada pedido separadamente (lazy loading)
                foreach (var pedido in pedidos)
                {
                    pedido.Itens = context.PedidoItens
                        .Where(i => i.PedidoId == pedido.Id)
                        .ToList();
                }

                return pedidos;
            }
        }

        // Retorna todos os pedidos de um cliente específico
        public static List<Pedido> ObterPorCliente(int clienteId)
        {
            using (var context = new FastPizzaDbContext())
            {
                var pedidos = context.Pedidos
                    .Where(p => p.ClienteId == clienteId)
                    .OrderByDescending(p => p.DataPedido)
                    .ToList();

                // Carrega os itens de cada pedido
                foreach (var pedido in pedidos)
                {
                    pedido.Itens = context.PedidoItens
                        .Where(i => i.PedidoId == pedido.Id)
                        .ToList();
                }

                return pedidos;
            }
        }

        // Busca um pedido específico pelo ID e carrega seus itens
        public static Pedido ObterPorId(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                var pedido = context.Pedidos
                    .FirstOrDefault(p => p.Id == id);

                if (pedido != null)
                {
                    // Carrega os itens do pedido
                    pedido.Itens = context.PedidoItens
                        .Where(i => i.PedidoId == pedido.Id)
                        .ToList();
                }

                return pedido;
            }
        }

        // Cria um novo pedido com seus itens no banco de dados
        // Estratégia: salva o pedido primeiro para obter o ID, depois salva os itens
        // Isso evita problemas de relacionamento no Entity Framework
        public static void Adicionar(Pedido pedido, List<ItemPedido> itens)
        {
            if (pedido == null)
                throw new ArgumentNullException(nameof(pedido));

            // Validações de regra de negócio
            if (pedido.ClienteId <= 0)
                throw new ArgumentException("ClienteId é obrigatório");

            if (string.IsNullOrWhiteSpace(pedido.NomeCliente))
                throw new ArgumentException("Nome do cliente é obrigatório");

            if (itens == null || itens.Count == 0)
                throw new ArgumentException("Pedido deve ter pelo menos um item");

            if (pedido.Total <= 0)
                throw new ArgumentException("Total do pedido deve ser maior que zero");

            using (var context = new FastPizzaDbContext())
            {
                // Verifica e cria coluna Observacoes se não existir (migração automática)
                // Necessário para compatibilidade com bancos de dados antigos
                try
                {
                    var connection = context.Database.Connection;
                    var wasOpen = connection.State == System.Data.ConnectionState.Open;
                    if (!wasOpen)
                    {
                        connection.Open();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Pedidos]') AND name = 'Observacoes')
                            BEGIN
                                ALTER TABLE [dbo].[Pedidos] ADD [Observacoes] NVARCHAR(1000) NULL
                            END";
                        command.ExecuteNonQuery();
                    }

                    if (!wasOpen)
                    {
                        connection.Close();
                    }

                    System.Diagnostics.Debug.WriteLine("Coluna Observacoes verificada/criada com sucesso");
                }
                catch (Exception exColuna)
                {
                    System.Diagnostics.Debug.WriteLine($"Aviso ao verificar coluna Observacoes: {exColuna.Message}");
                }

                try
                {
                    // Define data e status inicial do pedido
                    pedido.DataPedido = DateTime.Now;
                    pedido.Status = StatusPedido.Pendente;

                    // Normaliza observações vazias para null
                    if (string.IsNullOrWhiteSpace(pedido.Observacoes))
                    {
                        pedido.Observacoes = null;
                    }

                    // Salva o pedido primeiro para obter o ID gerado
                    context.Pedidos.Add(pedido);
                    context.SaveChanges();

                    System.Diagnostics.Debug.WriteLine($"Pedido criado com ID: {pedido.Id}");

                    // Agora adiciona os itens do pedido com o PedidoId correto
                    foreach (var item in itens)
                    {
                        var pedidoItem = new PedidoItem
                        {
                            PedidoId = pedido.Id,
                            ProdutoId = item.ProdutoId,
                            NomeProduto = item.NomeProduto ?? "Produto",
                            Quantidade = item.Quantidade,
                            PrecoUnitario = item.PrecoUnitario
                        };
                        context.PedidoItens.Add(pedidoItem);
                    }

                    // Salva os itens do pedido
                    context.SaveChanges();
                    System.Diagnostics.Debug.WriteLine($"Pedido salvo com sucesso! ID: {pedido.Id}, Itens: {itens.Count}");
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    // Tratamento específico para erros de atualização no banco
                    // Tenta criar coluna faltante se o erro for relacionado a schema
                    string mensagemErro = dbEx.Message;
                    System.Diagnostics.Debug.WriteLine($"=== DbUpdateException ===");
                    System.Diagnostics.Debug.WriteLine($"Mensagem: {dbEx.Message}");

                    if (dbEx.InnerException != null)
                    {
                        mensagemErro = dbEx.InnerException.Message;
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                        System.Diagnostics.Debug.WriteLine($"Inner Exception Type: {dbEx.InnerException.GetType().Name}");

                        // Se o erro for sobre coluna faltante, tenta criar automaticamente
                        if (dbEx.InnerException.Message.Contains("Observacoes") ||
                            dbEx.InnerException.Message.Contains("column") ||
                            dbEx.InnerException.Message.Contains("Invalid column") ||
                            dbEx.InnerException.Message.Contains("does not exist"))
                        {
                            try
                            {
                                System.Diagnostics.Debug.WriteLine("Tentando criar coluna Observacoes...");
                                context.Database.ExecuteSqlCommand(@"
                                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Pedidos]') AND name = 'Observacoes')
                                    BEGIN
                                        ALTER TABLE [dbo].[Pedidos] ADD [Observacoes] NVARCHAR(1000) NULL
                                    END");

                                System.Diagnostics.Debug.WriteLine("Coluna Observacoes criada. Tentando salvar novamente...");

                                context.SaveChanges();
                                System.Diagnostics.Debug.WriteLine("Pedido salvo com sucesso após criar coluna!");
                                return;
                            }
                            catch (Exception exColuna)
                            {
                                System.Diagnostics.Debug.WriteLine($"Erro ao criar coluna: {exColuna.Message}");
                            }
                        }
                    }

                    var mensagemCompleta = $"Erro ao salvar pedido no banco de dados. {mensagemErro}";
                    if (dbEx.InnerException != null)
                    {
                        mensagemCompleta += $" Detalhes: {dbEx.InnerException.Message}";
                    }
                    throw new Exception(mensagemCompleta, dbEx);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"=== Exception genérica ===");
                    System.Diagnostics.Debug.WriteLine($"Tipo: {ex.GetType().Name}");
                    System.Diagnostics.Debug.WriteLine($"Mensagem: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
        }

        public static void AdicionarComItens(Pedido pedido)
        {
            if (pedido == null)
                throw new ArgumentNullException(nameof(pedido));

            if (pedido.ClienteId <= 0)
                throw new ArgumentException("ClienteId é obrigatório");

            if (string.IsNullOrWhiteSpace(pedido.NomeCliente))
                throw new ArgumentException("Nome do cliente é obrigatório");

            if (pedido.Total <= 0)
                throw new ArgumentException("Total do pedido deve ser maior que zero");

            using (var context = new FastPizzaDbContext())
            {
                pedido.DataPedido = DateTime.Now;
                pedido.Status = StatusPedido.Pendente;

                context.Pedidos.Add(pedido);
                context.SaveChanges();
            }
        }

        // Atualiza o status de um pedido (Pendente, Em Preparo, Entregue, etc.)
        // Se cancelado, registra motivo e data de cancelamento
        public static void AtualizarStatus(int id, StatusPedido novoStatus, string motivoCancelamento = null)
        {
            using (var context = new FastPizzaDbContext())
            {
                var pedido = context.Pedidos.Find(id);
                if (pedido != null)
                {
                    pedido.Status = novoStatus;

                    // Se cancelado, registra informações do cancelamento
                    if (novoStatus == StatusPedido.Cancelado)
                    {
                        pedido.MotivoCancelamento = motivoCancelamento;
                        pedido.DataCancelamento = DateTime.Now;
                    }

                    context.SaveChanges();
                }
            }
        }

        // Atualiza os itens de um pedido existente
        // Não permite editar pedidos já cancelados ou entregues
        public static void AtualizarPedido(int pedidoId, List<ItemPedido> novosItens)
        {
            if (novosItens == null || novosItens.Count == 0)
                throw new ArgumentException("Pedido deve ter pelo menos um item");

            using (var context = new FastPizzaDbContext())
            {
                var pedido = context.Pedidos.Find(pedidoId);
                if (pedido == null)
                    throw new ArgumentException("Pedido não encontrado");

                // Regra de negócio: não permite editar pedidos finalizados
                if (pedido.Status == StatusPedido.Cancelado || pedido.Status == StatusPedido.Entregue)
                    throw new InvalidOperationException("Não é possível editar pedidos cancelados ou entregues");

                // Remove todos os itens antigos
                var itensExistentes = context.PedidoItens.Where(i => i.PedidoId == pedidoId).ToList();
                foreach (var item in itensExistentes)
                {
                    context.PedidoItens.Remove(item);
                }

                // Adiciona os novos itens e recalcula o total
                decimal novoTotal = 0;
                foreach (var item in novosItens)
                {
                    var pedidoItem = new PedidoItem
                    {
                        PedidoId = pedidoId,
                        ProdutoId = item.ProdutoId,
                        NomeProduto = item.NomeProduto,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = item.PrecoUnitario
                    };
                    context.PedidoItens.Add(pedidoItem);
                    novoTotal += item.Subtotal;
                }

                pedido.Total = novoTotal;

                context.SaveChanges();
            }
        }

        // Exclui permanentemente um pedido e todos seus itens
        public static void ExcluirPermanentemente(int id)
        {
            using (var context = new FastPizzaDbContext())
            {
                var pedido = context.Pedidos.Find(id);
                if (pedido == null)
                    throw new ArgumentException("Pedido não encontrado");

                // Remove itens antes de remover o pedido
                var itens = context.PedidoItens.Where(i => i.PedidoId == id).ToList();
                foreach (var item in itens)
                {
                    context.PedidoItens.Remove(item);
                }

                context.Pedidos.Remove(pedido);

                context.SaveChanges();
            }
        }
    }
}

