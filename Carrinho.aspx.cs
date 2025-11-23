using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics.CodeAnalysis;
using FastPizza.Models;
using FastPizza.DataAccess;
using FastPizza.Business;
using FastPizza.Utils;

namespace FastPizza
{
    public partial class Carrinho : Page
    {
        // Carrega o carrinho quando a página é inicializada
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                CarregarCarrinho();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Carrinho: {ex.Message}");
                ExibirCarrinhoVazio();
            }
        }

        // Classe que representa um item no carrinho para exibição
        public class ItemCarrinho
        {
            public int ProdutoId { get; set; }
            public string Nome { get; set; }
            public string Tipo { get; set; }

            // Formata o nome completo do item conforme o tipo (ex: "Pizza Calabresa", "Coca-Cola")
            public string NomeCompleto
            {
                get
                {
                    if (Tipo == "Bebida")
                    {
                        return $"{Nome}";
                    }
                    else if (Tipo == "Pizza")
                    {
                        return $"pizza {Nome}";
                    }
                    else
                    {
                        return $"{Tipo.ToLower()} {Nome}";
                    }
                }
            }

            public decimal PrecoUnitario { get; set; }
            public int Quantidade { get; set; }

            // Calcula o subtotal (preço unitário x quantidade)
            public decimal Subtotal { get { return PrecoUnitario * Quantidade; } }
        }

        // Carrega os itens do carrinho da sessão e exibe na tela
        private void CarregarCarrinho()
        {
            // Verifica se existe carrinho na sessão
            if (Session["Carrinho"] == null)
            {
                ExibirCarrinhoVazio();
                return;
            }

            List<string> idsNoCarrinho;
            var carrinhoSession = Session["Carrinho"];

            // Migra formato antigo (List<int>) para novo formato (List<string> com prefixos)
            if (carrinhoSession is List<int>)
            {
                var carrinhoAntigo = (List<int>)carrinhoSession;
                idsNoCarrinho = carrinhoAntigo.Select(id => "P_" + id).ToList();
                Session["Carrinho"] = idsNoCarrinho;
            }
            else if (carrinhoSession is List<string>)
            {
                idsNoCarrinho = (List<string>)carrinhoSession;
            }
            else
            {
                ExibirCarrinhoVazio();
                return;
            }

            if (idsNoCarrinho.Count == 0)
            {
                ExibirCarrinhoVazio();
                return;
            }

            // Busca todos os produtos, bebidas e molhos do banco
            var todasPizzas = ProdutoDAO.ObterTodos();
            var todasBebidas = BebidaDAO.ObterTodos();
            var todosMolhos = MolhoDAO.ObterDisponiveis();

            // Agrupa itens iguais e conta quantidades
            var itensAgrupados = idsNoCarrinho
                .GroupBy(idComTipo => idComTipo)
                .Select(grupo =>
                {
                    string idComTipo = grupo.Key;
                    string tipo = "";
                    int idItem = 0;

                    // Identifica o tipo pelo prefixo (P_=Pizza, B_=Bebida, M_=Molho)
                    if (idComTipo.StartsWith("P_"))
                    {
                        tipo = "Pizza";
                        if (!int.TryParse(idComTipo.Substring(2), out idItem))
                        {
                            return null;
                        }
                    }
                    else if (idComTipo.StartsWith("B_"))
                    {
                        tipo = "Bebida";
                        if (!int.TryParse(idComTipo.Substring(2), out idItem))
                        {
                            return null;
                        }
                    }
                    else if (idComTipo.StartsWith("M_"))
                    {
                        tipo = "Molho";
                        if (!int.TryParse(idComTipo.Substring(2), out idItem))
                        {
                            return null;
                        }
                    }
                    else
                    {
                        // Formato antigo sem prefixo (assume Pizza)
                        tipo = "Pizza";
                        if (!int.TryParse(idComTipo, out idItem))
                        {
                            return null;
                        }
                    }

                    // Busca os dados do item conforme o tipo
                    if (tipo == "Bebida")
                    {
                        var bebida = todasBebidas.FirstOrDefault(b => b.Id == idItem);
                        if (bebida != null)
                        {
                            return new ItemCarrinho
                            {
                                ProdutoId = idItem,
                                Quantidade = grupo.Count(),
                                Nome = bebida.Nome,
                                Tipo = "Bebida",
                                PrecoUnitario = bebida.Preco
                            };
                        }
                    }
                    else if (tipo == "Pizza")
                    {
                        var produto = todasPizzas.FirstOrDefault(p => p.Id == idItem);
                        if (produto != null)
                        {
                            return new ItemCarrinho
                            {
                                ProdutoId = idItem,
                                Quantidade = grupo.Count(),
                                Nome = produto.Nome,
                                Tipo = "Pizza",
                                PrecoUnitario = produto.Preco
                            };
                        }
                    }
                    else if (tipo == "Molho")
                    {
                        var molho = todosMolhos.FirstOrDefault(m => m.Id == idItem);
                        if (molho != null)
                        {
                            return new ItemCarrinho
                            {
                                ProdutoId = idItem,
                                Quantidade = grupo.Count(),
                                Nome = molho.Nome,
                                Tipo = "Molho",
                                PrecoUnitario = molho.Preco
                            };
                        }
                    }

                    // Retorna item genérico se não encontrado
                    return new ItemCarrinho
                    {
                        ProdutoId = idItem,
                        Quantidade = grupo.Count(),
                        Nome = "Item Removido",
                        Tipo = "",
                        PrecoUnitario = 0
                    };
                })
                .Where(item => item != null && item.PrecoUnitario > 0)
                .ToList();

            // Exibe itens no grid e calcula total
            gridCarrinho.DataSource = itensAgrupados;
            gridCarrinho.DataBind();

            decimal totalGeral = itensAgrupados.Sum(x => x.Subtotal);
            lblTotal.Text = totalGeral.ToString("C");
        }

        // Exibe mensagem de carrinho vazio
        private void ExibirCarrinhoVazio()
        {
            lblTotal.Text = "R$ 0,00";
            gridCarrinho.DataSource = null;
            gridCarrinho.DataBind();
        }

        // Manipula botões de aumentar/diminuir quantidade no grid
        protected void gridCarrinho_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (Session["Carrinho"] == null)
                {
                    CarregarCarrinho();
                    return;
                }

                List<string> carrinho;
                var carrinhoSession = Session["Carrinho"];

                // Migra formato antigo se necessário
                if (carrinhoSession is List<int>)
                {
                    var carrinhoAntigo = (List<int>)carrinhoSession;
                    carrinho = carrinhoAntigo.Select(id => "P_" + id).ToList();
                    Session["Carrinho"] = carrinho;
                }
                else if (carrinhoSession is List<string>)
                {
                    carrinho = (List<string>)carrinhoSession;
                }
                else
                {
                    CarregarCarrinho();
                    return;
                }

                // Obtém o ID do item clicado
                int idItem;
                if (e.CommandArgument == null || !int.TryParse(e.CommandArgument.ToString(), out idItem))
                {
                    CarregarCarrinho();
                    return;
                }

                // Encontra o item no carrinho
                string itemParaModificar = carrinho.FirstOrDefault(item =>
                {
                    if (item.StartsWith("P_") || item.StartsWith("B_") || item.StartsWith("M_"))
                    {
                        int id = 0;
                        if (int.TryParse(item.Substring(2), out id))
                        {
                            return id == idItem;
                        }
                    }
                    else
                    {
                        // Formato antigo
                        int id = 0;
                        if (int.TryParse(item, out id))
                        {
                            return id == idItem;
                        }
                    }
                    return false;
                });

                if (itemParaModificar == null)
                {
                    CarregarCarrinho();
                    return;
                }

                // Aumenta ou diminui a quantidade
                if (e.CommandName == "Aumentar")
                {
                    carrinho.Add(itemParaModificar);
                }
                else if (e.CommandName == "Diminuir")
                {
                    carrinho.Remove(itemParaModificar);
                }

                // Salva carrinho atualizado e recarrega
                Session["Carrinho"] = carrinho;
                CarregarCarrinho();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no gridCarrinho_RowCommand: {ex.Message}");
                CarregarCarrinho();
            }
        }

        // Finaliza o pedido e salva no banco de dados
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "ASP.NET Web Forms event handler naming convention")]
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== btnFinalizar_Click INICIADO ===");
            System.Diagnostics.Debug.WriteLine($"Session ClienteId: {Session["ClienteId"]}");
            System.Diagnostics.Debug.WriteLine($"Session AdminLogado: {Session["AdminLogado"]}");
            System.Diagnostics.Debug.WriteLine($"Session Carrinho: {Session["Carrinho"]}");

            try
            {
                // Verifica se usuário está autenticado (cliente ou admin)
                bool isAdmin = Session["AdminLogado"] != null && (bool)Session["AdminLogado"];
                bool isCliente = Session["ClienteId"] != null;

                // Redireciona para login se não autenticado
                if (!isCliente && !isAdmin)
                {
                    Response.Redirect(ResolveUrl("~/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.Url.PathAndQuery)));
                    return;
                }

                // Verifica se cliente está bloqueado
                if (isCliente && !isAdmin)
                {
                    int clienteId = (int)Session["ClienteId"];
                    var cliente = ClienteDAO.ObterPorId(clienteId);
                    if (cliente != null && cliente.Bloqueado)
                    {
                        ClientScript.RegisterStartupScript(GetType(), "ClienteBloqueado",
                            "setTimeout(function() { mostrarFeedback('Você foi bloqueado pelo administrador e não pode fazer pedidos.', 'danger'); }, 100);", true);
                        return;
                    }
                }

                int clienteIdParaPedido;
                string nomeClienteParaPedido;

                // Se for admin sem registro de cliente, cria um
                if (isAdmin && !isCliente)
                {
                    string emailAdmin = Session["AdminUsuario"] != null ? Session["AdminUsuario"].ToString() : "admin@email.com";
                    var clienteAdmin = ClienteDAO.ObterPorEmail(emailAdmin);

                    if (clienteAdmin != null)
                    {
                        // Admin já tem registro de cliente
                        clienteIdParaPedido = clienteAdmin.Id;
                        nomeClienteParaPedido = clienteAdmin.Nome;
                    }
                    else
                    {
                        // Tenta buscar novamente
                        clienteAdmin = ClienteDAO.ObterPorEmail(emailAdmin);

                        if (clienteAdmin == null)
                        {
                            // Cria novo cliente para o admin
                            var novoClienteAdmin = new Cliente
                            {
                                Nome = Session["AdminUsuario"] != null ? Session["AdminUsuario"].ToString() : "Administrador",
                                Email = emailAdmin,
                                Telefone = "",
                                SenhaHash = PasswordHasher.HashPassword("admin123"),
                                DataCadastro = DateTime.Now,
                                Bloqueado = false
                            };

                            try
                            {
                                ClienteDAO.Adicionar(novoClienteAdmin);
                                clienteIdParaPedido = novoClienteAdmin.Id;
                                nomeClienteParaPedido = novoClienteAdmin.Nome;

                                Session["ClienteId"] = novoClienteAdmin.Id;
                                Session["ClienteNome"] = novoClienteAdmin.Nome;
                                Session["ClienteEmail"] = novoClienteAdmin.Email;
                            }
                            catch (Exception ex)
                            {
                                // Se falhar, tenta buscar novamente (possível concorrência)
                                System.Diagnostics.Debug.WriteLine($"Erro ao criar cliente para admin: {ex.Message}");
                                clienteAdmin = ClienteDAO.ObterPorEmail(emailAdmin);
                                if (clienteAdmin != null)
                                {
                                    clienteIdParaPedido = clienteAdmin.Id;
                                    nomeClienteParaPedido = clienteAdmin.Nome;
                                    Session["ClienteId"] = clienteAdmin.Id;
                                    Session["ClienteNome"] = clienteAdmin.Nome;
                                    Session["ClienteEmail"] = clienteAdmin.Email;
                                }
                                else
                                {
                                    // Usa primeiro cliente disponível como fallback
                                    var todosClientes = ClienteDAO.ObterTodos();
                                    if (todosClientes != null && todosClientes.Count > 0)
                                    {
                                        clienteIdParaPedido = todosClientes[0].Id;
                                        nomeClienteParaPedido = "Admin - " + (Session["AdminUsuario"] != null ? Session["AdminUsuario"].ToString() : "Administrador");
                                    }
                                    else
                                    {
                                        ClientScript.RegisterStartupScript(GetType(), "ErroCriarCliente",
                                            "setTimeout(function() { mostrarFeedback('Erro ao processar pedido. Por favor, tente novamente.', 'danger'); }, 100);", true);
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Cliente encontrado
                            clienteIdParaPedido = clienteAdmin.Id;
                            nomeClienteParaPedido = clienteAdmin.Nome;
                            Session["ClienteId"] = clienteAdmin.Id;
                            Session["ClienteNome"] = clienteAdmin.Nome;
                            Session["ClienteEmail"] = clienteAdmin.Email;
                        }
                    }
                }
                else
                {
                    // Usa dados do cliente logado
                    clienteIdParaPedido = (int)Session["ClienteId"];
                    nomeClienteParaPedido = Session["ClienteNome"] != null ? Session["ClienteNome"].ToString() : "Cliente";
                }

                // Processa itens do carrinho
                if (Session["Carrinho"] != null)
                {
                    // Migra formato antigo para novo se necessário
                    List<string> idsNoCarrinho;
                    var carrinhoSession = Session["Carrinho"];

                    if (carrinhoSession is List<int> carrinhoAntigo)
                    {
                        idsNoCarrinho = carrinhoAntigo.Select(id => "P_" + id).ToList();
                        Session["Carrinho"] = idsNoCarrinho;
                    }
                    else if (carrinhoSession is List<string> listaStrings)
                    {
                        idsNoCarrinho = listaStrings;
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(GetType(), "CarrinhoVazioInicial",
                            "setTimeout(function() { mostrarFeedback('Seu carrinho está vazio!', 'danger'); }, 100);", true);
                        return;
                    }

                    // Valida se carrinho tem itens
                    if (idsNoCarrinho.Count == 0)
                    {
                        ClientScript.RegisterStartupScript(GetType(), "CarrinhoVazioInicial",
                            "setTimeout(function() { mostrarFeedback('Seu carrinho está vazio!', 'danger'); }, 100);", true);
                        return;
                    }

                    // Cria objeto do pedido
                    var pedido = new Pedido
                    {
                        ClienteId = clienteIdParaPedido,
                        NomeCliente = nomeClienteParaPedido
                    };

                    // Busca dados de produtos, bebidas e molhos
                    var produtos = ProdutoDAO.ObterTodos();
                    var bebidas = BebidaDAO.ObterTodos();
                    var molhos = MolhoDAO.ObterDisponiveis();
                    decimal total = 0;

                    var itensTemporarios = new List<ItemPedido>();

                    // Agrupa itens por ID
                    var itensAgrupados = idsNoCarrinho
                        .GroupBy(idComTipo => idComTipo)
                        .Select(grupo =>
                        {
                            string idComTipo = grupo.Key;
                            string tipo = "";
                            int idItem = 0;

                            // Identifica tipo e extrai ID
                            if (idComTipo.StartsWith("P_"))
                            {
                                tipo = "Pizza";
                                if (!int.TryParse(idComTipo.Substring(2), out idItem))
                                {
                                    return null;
                                }
                            }
                            else if (idComTipo.StartsWith("B_"))
                            {
                                tipo = "Bebida";
                                if (!int.TryParse(idComTipo.Substring(2), out idItem))
                                {
                                    return null;
                                }
                            }
                            else if (idComTipo.StartsWith("M_"))
                            {
                                tipo = "Molho";
                                if (!int.TryParse(idComTipo.Substring(2), out idItem))
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                // Formato antigo
                                tipo = "Pizza";
                                if (!int.TryParse(idComTipo, out idItem))
                                {
                                    return null;
                                }
                            }

                            // Busca item no banco
                            var bebida = tipo == "Bebida" ? bebidas.FirstOrDefault(b => b.Id == idItem) : null;
                            var produto = tipo == "Pizza" ? produtos.FirstOrDefault(p => p.Id == idItem) : null;
                            var molho = tipo == "Molho" ? molhos.FirstOrDefault(m => m.Id == idItem) : null;

                            return new
                            {
                                Id = idItem,
                                Quantidade = grupo.Count(),
                                Produto = produto,
                                Bebida = bebida,
                                Molho = molho
                            };
                        })
                        .Where(x => x != null && (x.Produto != null || x.Bebida != null || x.Molho != null))
                        .ToList();

                    // Valida estoque e cria itens do pedido
                    foreach (var item in itensAgrupados)
                    {
                        if (item.Produto != null)
                        {
                            // Verifica estoque de pizza
                            if (item.Produto.Estoque < item.Quantidade)
                            {
                                string nomeProduto = item.Produto.Nome.Replace("'", "\\'");
                                ClientScript.RegisterStartupScript(GetType(), "EstoqueInsuficiente",
                                    "setTimeout(function() { mostrarFeedback('Produto " + nomeProduto + " não possui estoque suficiente. Disponível: " + item.Produto.Estoque + "', 'danger'); }, 100);", true);
                                return;
                            }

                            var itemPedido = new ItemPedido
                            {
                                ProdutoId = item.Produto.Id,
                                NomeProduto = item.Produto.Nome,
                                Quantidade = item.Quantidade,
                                PrecoUnitario = item.Produto.Preco
                            };
                            itensTemporarios.Add(itemPedido);
                            total += itemPedido.Subtotal;
                        }
                        else if (item.Bebida != null)
                        {
                            // Verifica estoque de bebida
                            if (item.Bebida.Estoque < item.Quantidade)
                            {
                                string nomeBebida = item.Bebida.Nome.Replace("'", "\\'");
                                ClientScript.RegisterStartupScript(GetType(), "EstoqueInsuficienteBebida",
                                    "setTimeout(function() { mostrarFeedback('Bebida " + nomeBebida + " não possui estoque suficiente. Disponível: " + item.Bebida.Estoque + "', 'danger'); }, 100);", true);
                                return;
                            }

                            var itemPedido = new ItemPedido
                            {
                                ProdutoId = item.Bebida.Id,
                                NomeProduto = item.Bebida.Nome,
                                Quantidade = item.Quantidade,
                                PrecoUnitario = item.Bebida.Preco
                            };
                            itensTemporarios.Add(itemPedido);
                            total += itemPedido.Subtotal;
                        }
                        else if (item.Molho != null)
                        {
                            // Verifica estoque de molho
                            if (item.Molho.Estoque < item.Quantidade)
                            {
                                string nomeMolho = item.Molho.Nome.Replace("'", "\\'");
                                ClientScript.RegisterStartupScript(GetType(), "EstoqueInsuficienteMolho",
                                    "setTimeout(function() { mostrarFeedback('Molho " + nomeMolho + " não possui estoque suficiente. Disponível: " + item.Molho.Estoque + "', 'danger'); }, 100);", true);
                                return;
                            }

                            var itemPedido = new ItemPedido
                            {
                                ProdutoId = item.Molho.Id,
                                NomeProduto = item.Molho.Nome,
                                Quantidade = item.Quantidade,
                                PrecoUnitario = item.Molho.Preco
                            };
                            itensTemporarios.Add(itemPedido);
                            total += itemPedido.Subtotal;
                        }
                    }

                    // Atualiza estoque após validação
                    foreach (var item in itensAgrupados)
                    {
                        if (item.Produto != null)
                        {
                            var produtoParaAtualizar = ProdutoDAO.ObterPorId(item.Produto.Id);
                            if (produtoParaAtualizar != null)
                            {
                                produtoParaAtualizar.Estoque -= item.Quantidade;
                                ProdutoDAO.Atualizar(produtoParaAtualizar);
                            }
                        }
                        else if (item.Bebida != null)
                        {
                            var bebidaParaAtualizar = BebidaDAO.ObterPorId(item.Bebida.Id);
                            if (bebidaParaAtualizar != null)
                            {
                                bebidaParaAtualizar.Estoque -= item.Quantidade;
                                BebidaDAO.Atualizar(bebidaParaAtualizar);
                            }
                        }
                        else if (item.Molho != null)
                        {
                            item.Molho.Estoque -= item.Quantidade;
                            MolhoDAO.Atualizar(item.Molho);
                        }
                    }

                    pedido.Total = total;

                    // Processa observações do pedido
                    try
                    {
                        if (txtObservacoes != null && !string.IsNullOrWhiteSpace(txtObservacoes.Text))
                        {
                            string observacoes = txtObservacoes.Text.Trim();

                            // Limita observações a 1000 caracteres
                            if (observacoes.Length > 1000)
                            {
                                observacoes = observacoes.Substring(0, 1000);
                            }
                            pedido.Observacoes = observacoes;
                        }
                        else
                        {
                            pedido.Observacoes = null;
                        }
                    }
                    catch (Exception exObs)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao processar observações: {exObs.Message}");
                        pedido.Observacoes = null;
                    }

                    // Salva pedido no banco de dados
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Tentando salvar pedido - ClienteId: {pedido.ClienteId}, Total: {pedido.Total}, Itens: {itensTemporarios.Count}");
                        PedidoBusiness.Adicionar(pedido, itensTemporarios);
                        System.Diagnostics.Debug.WriteLine("Pedido salvo com sucesso!");
                    }
                    catch (Exception exSalvar)
                    {
                        // Se falhar, reverte o estoque
                        System.Diagnostics.Debug.WriteLine($"=== ERRO ao salvar pedido ===");
                        System.Diagnostics.Debug.WriteLine($"Mensagem: {exSalvar.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {exSalvar.StackTrace}");
                        if (exSalvar.InnerException != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Inner exception: {exSalvar.InnerException.Message}");
                            System.Diagnostics.Debug.WriteLine($"Inner stack trace: {exSalvar.InnerException.StackTrace}");
                        }

                        // Reverte estoque de todos os itens
                        foreach (var item in itensAgrupados)
                        {
                            try
                            {
                                if (item.Produto != null)
                                {
                                    var produtoParaReverter = ProdutoDAO.ObterPorId(item.Produto.Id);
                                    if (produtoParaReverter != null)
                                    {
                                        produtoParaReverter.Estoque += item.Quantidade;
                                        ProdutoDAO.Atualizar(produtoParaReverter);
                                    }
                                }
                                else if (item.Bebida != null)
                                {
                                    var bebidaParaReverter = BebidaDAO.ObterPorId(item.Bebida.Id);
                                    if (bebidaParaReverter != null)
                                    {
                                        bebidaParaReverter.Estoque += item.Quantidade;
                                        BebidaDAO.Atualizar(bebidaParaReverter);
                                    }
                                }
                                else if (item.Molho != null)
                                {
                                    item.Molho.Estoque += item.Quantidade;
                                    MolhoDAO.Atualizar(item.Molho);
                                }
                            }
                            catch
                            {
                                // Ignora erros ao reverter
                            }
                        }

                        throw;
                    }

                    // Limpa carrinho após sucesso
                    Session["Carrinho"] = null;

                    System.Diagnostics.Debug.WriteLine("Pedido finalizado com sucesso! Redirecionando...");

                    // Redireciona para página de pedidos com mensagem de sucesso
                    Response.Redirect(ResolveUrl("~/MeusPedidos.aspx?pedidoSucesso=true"), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "CarrinhoVazio",
                        "setTimeout(function() { mostrarFeedback('Seu carrinho está vazio!', 'danger'); }, 100);", true);
                }
            }
            catch (Exception ex)
            {
                // Tratamento de erros com mensagens específicas
                System.Diagnostics.Debug.WriteLine($"=== ERRO ao finalizar pedido ===");
                System.Diagnostics.Debug.WriteLine($"Mensagem: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }

                string mensagemErro = "Erro ao finalizar pedido. Tente novamente.";
                string mensagemDetalhada = ex.Message;

                if (ex.InnerException != null)
                {
                    mensagemDetalhada += " | " + ex.InnerException.Message;
                }

                System.Diagnostics.Debug.WriteLine($"Mensagem detalhada do erro: {mensagemDetalhada}");

                // Identifica tipo de erro e personaliza mensagem
                if (mensagemDetalhada.Contains("Observacoes") || mensagemDetalhada.Contains("column") || mensagemDetalhada.Contains("Invalid column") || mensagemDetalhada.

                    Contains("does not exist"))
                {
                    mensagemErro = "Erro: O banco de dados precisa ser atualizado. A coluna 'Observacoes' não existe. Por favor, recrie o banco de dados ou execute uma migração.";
                }
                else if (mensagemDetalhada.Contains("ClienteId") || mensagemDetalhada.Contains("obrigatório") || mensagemDetalhada.Contains("required"))
                {
                    mensagemErro = "Erro: Dados do cliente inválidos. Por favor, faça login novamente.";
                }
                else if (mensagemDetalhada.Contains("estoque") || mensagemDetalhada.Contains("Estoque") || mensagemDetalhada.Contains("stock"))
                {
                    mensagemErro = ex.Message;
                }
                else if (mensagemDetalhada.Contains("Total") || mensagemDetalhada.Contains("total"))
                {
                    mensagemErro = "Erro: O total do pedido está inválido. Por favor, verifique os itens do carrinho.";
                }
                else if (mensagemDetalhada.Contains("item") || mensagemDetalhada.Contains("Item"))
                {
                    mensagemErro = "Erro: O pedido deve ter pelo menos um item. Seu carrinho pode estar vazio.";
                }
                else
                {
                    // Mensagem genérica com detalhes da exceção
                    mensagemErro = $"Erro: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        mensagemErro += $" ({ex.InnerException.Message})";
                    }
                }

                // Sanitiza mensagem para JavaScript (remove caracteres especiais)
                mensagemErro = mensagemErro.Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");

                // Registra erro no console do navegador e exibe mensagem ao usuário
                string scriptErro = $@"
                    console.error('Erro ao finalizar pedido:', {{
                        message: '{ex.Message.Replace("'", "\\'")}',
                        innerException: '{ex.InnerException?.Message?.Replace("'", "\\'") ?? ""}',
                        stackTrace: '{ex.StackTrace?.Replace("'", "\\'").Replace("\r", "").Replace("\n", " ") ?? ""}'
                    }});
                    setTimeout(function() {{ mostrarFeedback('{mensagemErro}', 'danger'); }}, 100);
                ";

                ClientScript.RegisterStartupScript(GetType(), "ErroPedido", scriptErro, true);
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("=== btnFinalizar_Click FINALIZADO ===");
            }
        }
    }
}