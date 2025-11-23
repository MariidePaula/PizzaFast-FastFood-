using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FastPizza.DataAccess;
using FastPizza.Models;
using FastPizza.Utils;

namespace FastPizza
{
    // Página de cardápio com pizzas e bebidas disponíveis
    public partial class Cardapio : System.Web.UI.Page
    {
        // Carrega produtos baseado na categoria informada na URL
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = false;

            if (!IsPostBack)
            {
                // Verifica categoria na URL para decidir o que carregar
                string categoriaUrl = Request.QueryString["categoria"];

                if (categoriaUrl == "Bebidas")
                {
                    CarregarBebidasNoGrid();
                }
                else
                {
                    CarregarPizzas(categoriaUrl);
                }
            }
        }

        // Carrega pizzas filtradas por categoria ou todas disponíveis
        private void CarregarPizzas(string categoria)
        {
            List<Produto> listaPizzas;

            // Se categoria vazia ou "Todas", busca todas as pizzas disponíveis
            if (string.IsNullOrEmpty(categoria) || categoria == "Todas")
            {
                listaPizzas = ProdutoDAO.ObterDisponiveis();
            }
            else
            {
                // Filtra por categoria específica
                listaPizzas = ProdutoDAO.ObterPorCategoria(categoria);
            }

            // Preenche as imagens automaticamente se estiverem vazias
            foreach (var produto in listaPizzas)
            {
                if (string.IsNullOrWhiteSpace(produto.ImagemUrl))
                {
                    string imagemUrl = ImageMapper.ObterImagemProduto(produto);
                    // Resolve a URL relativa para uma URL absoluta
                    if (!string.IsNullOrWhiteSpace(imagemUrl))
                    {
                        produto.ImagemUrl = ResolveUrl(imagemUrl);
                    }
                }
            }

            // Exibe mensagem se não houver produtos
            if (listaPizzas.Count == 0)
            {
                lblMensagem.Text = "Nenhuma pizza encontrada nesta categoria.";
                lblMensagem.Visible = true;
                lblMensagem.CssClass = "alert alert-info";
            }
            else
            {
                lblMensagem.Visible = false;
            }

            rptCardapio.DataSource = listaPizzas;
            rptCardapio.DataBind();
        }

        // Carrega todas as bebidas disponíveis
        private void CarregarBebidasNoGrid()
        {
            var bebidas = BebidaDAO.ObterDisponiveis();

            // Garante que todas as bebidas tenham imagem resolvida corretamente
            foreach (var bebida in bebidas)
            {
                string imagemUrl = bebida.ImagemUrl;
                
                // Se não tiver imagem ou estiver vazia, obtém do ImageMapper
                if (string.IsNullOrWhiteSpace(imagemUrl))
                {
                    imagemUrl = ImageMapper.ObterImagemBebida(bebida);
                }
                
                // Sempre resolve a URL relativa para uma URL absoluta
                // Isso garante que funcione em qualquer máquina
                if (!string.IsNullOrWhiteSpace(imagemUrl))
                {
                    // Se for URL relativa (começa com ~/), resolve para URL absoluta
                    if (imagemUrl.StartsWith("~/"))
                    {
                        bebida.ImagemUrl = ResolveUrl(imagemUrl);
                    }
                    // Se já for absoluta, mantém como está
                    else if (!imagemUrl.StartsWith("http://") && !imagemUrl.StartsWith("https://") && !imagemUrl.StartsWith("/"))
                    {
                        // Se não começar com /, pode ser relativa, tenta resolver
                        bebida.ImagemUrl = ResolveUrl("~/Images/Bebidas/" + imagemUrl);
                    }
                }
            }

            // Exibe mensagem se não houver bebidas
            if (bebidas.Count == 0)
            {
                lblMensagem.Text = "Nenhuma bebida disponível no momento.";
                lblMensagem.Visible = true;
                lblMensagem.CssClass = "alert alert-info";
            }
            else
            {
                lblMensagem.Visible = false;
            }

            rptCardapio.DataSource = bebidas;
            rptCardapio.DataBind();
        }

        // Evento do Repeater quando clica em "Adicionar ao Carrinho"
        protected void rptCardapio_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AdicionarCarrinho")
            {
                // Extrai ID do item do argumento (formato "P_123", "B_123" ou "123")
                string argumento = e.CommandArgument.ToString();
                int idItem;

                if (argumento.StartsWith("P_"))
                {
                    // Produto (pizza): remove prefixo e adiciona
                    if (int.TryParse(argumento.Substring(2), out idItem))
                    {
                        AdicionarAoCarrinho(idItem);
                    }
                }
                else if (argumento.StartsWith("B_"))
                {
                    // Bebida: remove prefixo e adiciona
                    if (int.TryParse(argumento.Substring(2), out idItem))
                    {
                        AdicionarBebidaAoCarrinho(idItem);
                    }
                }
                else
                {
                    // Formato antigo: decide pelo parâmetro de categoria na URL
                    if (int.TryParse(argumento, out idItem))
                    {
                        string categoriaUrl = Request.QueryString["categoria"];
                        if (categoriaUrl == "Bebidas")
                        {
                            AdicionarBebidaAoCarrinho(idItem);
                        }
                        else
                        {
                            AdicionarAoCarrinho(idItem);
                        }
                    }
                }
            }
        }

        // Adiciona produto (pizza) ao carrinho, verificando autenticação e bloqueio
        private void AdicionarAoCarrinho(int idProduto)
        {
            // Verifica autenticação
            bool isAdmin = Session["AdminLogado"] != null && (bool)Session["AdminLogado"];
            bool isCliente = Session["ClienteId"] != null;

            // Se não estiver logado, abre modal de login
            if (!isCliente && !isAdmin)
            {
                var sm = ScriptManager.GetCurrent(Page);
                if (sm != null)
                {
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "PrecisaLogin",
                        "setTimeout(function() { var modalElement = document.getElementById('modalLogin'); if(modalElement) { var modal = new bootstrap.Modal(modalElement); modal.show(); } }, 100);", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "PrecisaLogin",
                        "setTimeout(function() { var modalElement = document.getElementById('modalLogin'); if(modalElement) { var modal = new bootstrap.Modal(modalElement); modal.show(); } }, 100);", true);
                }
                return;
            }

            // Verifica se cliente está bloqueado (não pode adicionar ao carrinho)
            if (isCliente && !isAdmin)
            {
                int clienteId = (int)Session["ClienteId"];
                var cliente = ClienteDAO.ObterPorId(clienteId);
                if (cliente != null && cliente.Bloqueado)
                {
                    var sm = ScriptManager.GetCurrent(Page);
                    if (sm != null)
                    {
                        ScriptManager.RegisterStartupScript(Page, this.GetType(), "ClienteBloqueado",
                            "setTimeout(function() { mostrarFeedback('Você foi bloqueado pelo administrador e não pode fazer pedidos.', 'danger'); }, 100);", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ClienteBloqueado",
                            "setTimeout(function() { mostrarFeedback('Você foi bloqueado pelo administrador e não pode fazer pedidos.', 'danger'); }, 100);", true);
                    }
                    return;
                }
            }

            List<string> carrinho;
            var carrinhoSession = Session["Carrinho"];

            if (carrinhoSession == null)
            {
                carrinho = new List<string>();
            }
            else if (carrinhoSession is List<string>)
            {
                carrinho = (List<string>)carrinhoSession;
            }
            else if (carrinhoSession is List<int>)
            {

                var listaAntiga = (List<int>)carrinhoSession;
                carrinho = listaAntiga.Select(id => "P_" + id).ToList();
            }
            else
            {
                carrinho = new List<string>();
            }

            carrinho.Add("P_" + idProduto);
            Session["Carrinho"] = carrinho;

            var master = Master as SiteMaster;
            if (master != null)
            {
                master.AtualizarCarrinho();
            }

            var sm2 = ScriptManager.GetCurrent(Page);
            if (sm2 != null)
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "PizzaAdicionada",
                    "setTimeout(function() { mostrarFeedback('Pizza adicionada ao carrinho com sucesso! 🍕', 'success'); }, 50);", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "PizzaAdicionada",
                    "setTimeout(function() { mostrarFeedback('Pizza adicionada ao carrinho com sucesso! 🍕', 'success'); }, 50);", true);
            }
        }

        private void AdicionarBebidaAoCarrinho(int idBebida)
        {
            bool isAdmin = Session["AdminLogado"] != null && (bool)Session["AdminLogado"];
            bool isCliente = Session["ClienteId"] != null;

            if (!isCliente && !isAdmin)
            {
                var sm = ScriptManager.GetCurrent(Page);
                if (sm != null)
                {
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "PrecisaLogin",
                        "setTimeout(function() { var modalElement = document.getElementById('modalLogin'); if(modalElement) { var modal = new bootstrap.Modal(modalElement); modal.show(); } }, 100);", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "PrecisaLogin",
                        "setTimeout(function() { var modalElement = document.getElementById('modalLogin'); if(modalElement) { var modal = new bootstrap.Modal(modalElement); modal.show(); } }, 100);", true);
                }
                return;
            }

            // Verifica se o cliente está bloqueado
            if (isCliente && !isAdmin)
            {
                int clienteId = (int)Session["ClienteId"];
                var cliente = ClienteDAO.ObterPorId(clienteId);
                if (cliente != null && cliente.Bloqueado)
                {
                    var sm = ScriptManager.GetCurrent(Page);
                    if (sm != null)
                    {
                        ScriptManager.RegisterStartupScript(Page, this.GetType(), "ClienteBloqueado",
                            "setTimeout(function() { mostrarFeedback('Você foi bloqueado pelo administrador e não pode fazer pedidos.', 'danger'); }, 100);", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ClienteBloqueado",
                            "setTimeout(function() { mostrarFeedback('Você foi bloqueado pelo administrador e não pode fazer pedidos.', 'danger'); }, 100);", true);
                    }
                    return;
                }
            }

            List<string> carrinho;
            var carrinhoSession = Session["Carrinho"];

            if (carrinhoSession == null)
            {
                carrinho = new List<string>();
            }
            else if (carrinhoSession is List<string>)
            {
                carrinho = (List<string>)carrinhoSession;
            }
            else if (carrinhoSession is List<int>)
            {

                var listaAntiga = (List<int>)carrinhoSession;
                carrinho = listaAntiga.Select(id => "P_" + id).ToList();
            }
            else
            {
                carrinho = new List<string>();
            }

            carrinho.Add("B_" + idBebida);
            Session["Carrinho"] = carrinho;

            var master = Master as SiteMaster;
            if (master != null)
            {
                master.AtualizarCarrinho();
            }

            var sm2 = ScriptManager.GetCurrent(Page);
            if (sm2 != null)
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "BebidaAdicionada",
                    "setTimeout(function() { mostrarFeedback('Bebida adicionada ao carrinho com sucesso! 🥤', 'success'); }, 50);", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "BebidaAdicionada",
                    "setTimeout(function() { mostrarFeedback('Bebida adicionada ao carrinho com sucesso! 🥤', 'success'); }, 50);", true);
            }
        }

        protected void btnLoginModal_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmailModal.Text.Trim();
                string senha = txtSenhaModal.Text.Trim();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                {
                    lblMensagemModal.Text = "Por favor, preencha todos os campos!";
                    lblMensagemModal.Visible = true;
                    return;
                }

                // Verifica se o cliente existe e está bloqueado antes de autenticar
                var clientePorEmail = ClienteDAO.ObterPorEmail(email);
                if (clientePorEmail != null && clientePorEmail.Bloqueado)
                {
                    lblMensagemModal.Text = "Sua conta foi bloqueada. Para mais informações, entre em contato com o suporte.";
                    lblMensagemModal.Visible = true;
                    lblMensagemModal.CssClass = "alert alert-danger";
                    return;
                }

                var cliente = ClienteDAO.Autenticar(email, senha);

                if (cliente != null)
                {
                    Session["ClienteId"] = cliente.Id;
                    Session["ClienteNome"] = cliente.Nome;
                    Session["ClienteEmail"] = cliente.Email;
                    Session["ModalLoginAberto"] = null;

                    ClientScript.RegisterStartupScript(this.GetType(), "LoginSucesso",
                        "setTimeout(function() { " +
                        "var modal = bootstrap.Modal.getInstance(document.getElementById('modalLogin')); " +
                        "if(modal) modal.hide(); " +
                        "}, 100);", true);

                    ClientScript.RegisterStartupScript(this.GetType(), "AtualizarInterface",
                        "setTimeout(function() { " +
                        "window.location.reload(); " +
                        "}, 500);", true);
                }
                else
                {
                    lblMensagemModal.Text = "Email ou senha inválidos!";
                    lblMensagemModal.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no btnLoginModal_Click: {ex.Message}");
                lblMensagemModal.Text = "Erro ao fazer login. Tente novamente.";
                lblMensagemModal.Visible = true;
            }
        }
    }
}
