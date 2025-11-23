using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FastPizza.Models;
using FastPizza.DataAccess;
using FastPizza.Admin;
using FastPizza.Utils;

namespace FastPizza
{
    public partial class _Default : System.Web.UI.Page
    {
        public string BannerImageUrl { get; set; }
        // Carrega a página inicial com produtos em destaque e configurações
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.MaintainScrollPositionOnPostBack = false;

                // Obtém URL do banner configurado pelo administrador ou usa padrão da pasta Banners
                string bannerUrl = Configuracoes.ObterBannerUrl();
                // Se for uma URL relativa (começa com ~/), resolve para URL absoluta
                if (!string.IsNullOrEmpty(bannerUrl) && bannerUrl.StartsWith("~/"))
                {
                    BannerImageUrl = ResolveUrl(bannerUrl);
                }
                else
                {
                    BannerImageUrl = bannerUrl;
                }

                if (!IsPostBack)
                {
                    CarregarCardapio();

                    // Exibe mensagem de logout se houver
                    if (Session["MensagemLogout"] != null)
                    {
                        string mensagemLogout = Session["MensagemLogout"].ToString();
                        Session["MensagemLogout"] = null;

                        ClientScript.RegisterStartupScript(this.GetType(), "MensagemLogout",
                            $"setTimeout(function() {{ mostrarFeedback('{mensagemLogout.Replace("'", "\\'")}', 'danger'); }}, 300);", true);
                    }

                    // Limpa parâmetro de login da URL após sucesso
                    bool loginOk = Request.QueryString["login"] == "ok";

                    if (loginOk)
                    {
                        Session["ModalLoginAberto"] = null;
                        // Remove parâmetro da URL sem recarregar página
                        ClientScript.RegisterStartupScript(this.GetType(), "LimparURL",
                            "if(window.history && window.history.replaceState) { " +
                            "window.history.replaceState({}, document.title, window.location.pathname); " +
                            "}", true);
                    }

                    else if (Session["ClienteId"] != null)
                    {
                        Session["ModalLoginAberto"] = null;
                    }

                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load: {ex.Message}");

                try
                {
                    // Obtém URL do banner configurado pelo administrador ou usa padrão da pasta Banners
                    string bannerUrl = Configuracoes.ObterBannerUrl();
                    // Se for uma URL relativa (começa com ~/), resolve para URL absoluta
                    if (!string.IsNullOrEmpty(bannerUrl) && bannerUrl.StartsWith("~/"))
                    {
                        BannerImageUrl = ResolveUrl(bannerUrl);
                    }
                    else
                    {
                        BannerImageUrl = bannerUrl;
                    }

                    if (!IsPostBack)
                    {
                        CarregarCardapio();
                    }
                }
                catch
                {
                    // Em caso de erro, tenta usar banner padrão da pasta Banners
                    try
                    {
                        BannerImageUrl = ResolveUrl("~/Images/Banners/hero-pizzeria.jpg");
                    }
                    catch
                    {
                        BannerImageUrl = null;
                    }
                }

                ClientScript.RegisterStartupScript(this.GetType(), "ErroCarregamento",
                    "console.error('Erro ao carregar a página: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        // Carrega produtos para exibir na página inicial (8 cards)
        private void CarregarCardapio()
        {
            try
            {
                // Obtém todos os produtos disponíveis
                var produtos = ProdutoDAO.ObterDisponiveis();

                if (produtos != null && produtos.Count > 0)
                {
                    // Preenche as imagens automaticamente se estiverem vazias
                    foreach (var produto in produtos)
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

                    // Limita a 8 produtos para a página inicial
                    var produtosLimitados = produtos.Take(8).ToList();

                    // Exibe produtos no Repeater
                    rptPizzas.DataSource = produtosLimitados;
                    rptPizzas.DataBind();
                    rptPizzas.Visible = true;
                    lblSemProdutos.Visible = false;
                }
                else
                {
                    // Exibe mensagem se não houver produtos
                    rptPizzas.Visible = false;
                    lblSemProdutos.Visible = true;
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Erro ao carregar cardápio: {ex.Message}");

                rptPizzas.Visible = false;
                lblSemProdutos.Text = "<i class='fas fa-exclamation-triangle'></i> Erro ao carregar o cardápio. Por favor, tente novamente mais tarde.";
                lblSemProdutos.CssClass = "alert alert-warning text-center";
                lblSemProdutos.Visible = true;
            }
        }

        // Evento do Repeater quando clica em "Adicionar ao Carrinho"
        protected void rptPizzas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AdicionarCarrinho")
            {
                // Extrai ID do produto do argumento (formato "P_123" ou "123")
                string argumento = e.CommandArgument.ToString();
                int idPizza;

                if (argumento.StartsWith("P_"))
                {
                    // Formato novo: "P_123"
                    if (int.TryParse(argumento.Substring(2), out idPizza))
                    {
                        AdicionarAoCarrinho(idPizza);
                    }
                }
                else
                {
                    // Formato antigo: "123"
                    if (int.TryParse(argumento, out idPizza))
                    {
                        AdicionarAoCarrinho(idPizza);
                    }
                }
            }
        }

        // Adiciona produto ao carrinho, verificando autenticação e bloqueio
        private void AdicionarAoCarrinho(int idProduto)
        {
            try
            {
                // Verifica se usuário está autenticado (cliente ou admin)
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
                        "setTimeout(function() { mostrarFeedback('Pizza adicionada ao carrinho com sucesso!', 'success'); }, 50);", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "PizzaAdicionada",
                        "setTimeout(function() { mostrarFeedback('Pizza adicionada ao carrinho com sucesso!', 'success'); }, 50);", true);
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar ao carrinho: {ex.Message}");

                CarregarCardapio();

                var sm3 = ScriptManager.GetCurrent(Page);
                if (sm3 != null)
                {
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "ErroAdicionar",
                        "setTimeout(function() { mostrarFeedback('Erro ao adicionar pizza ao carrinho. Tente novamente.', 'danger'); }, 100);", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ErroAdicionar",
                        "setTimeout(function() { mostrarFeedback('Erro ao adicionar pizza ao carrinho. Tente novamente.', 'danger'); }, 100);", true);
                }
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

                    txtEmailModal.Text = "";
                    txtSenhaModal.Text = "";
                    lblMensagemModal.Visible = false;

                    var master = Master as SiteMaster;
                    if (master != null)
                    {
                        master.AtualizarMenuUsuario();
                        master.AtualizarCarrinho();
                    }

                    ClientScript.RegisterStartupScript(this.GetType(), "LoginSucesso",
                        "setTimeout(function() { " +
                        "var modalElement = document.getElementById('modalLogin'); " +
                        "if(modalElement) { " +
                        "var modal = bootstrap.Modal.getInstance(modalElement); " +
                        "if(modal) modal.hide(); " +
                        "} " +
                        "window.usuarioLogado = true; " +
                        "mostrarFeedback('Login realizado com sucesso!', 'success'); " +
                        "}, 300);", true);
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
                lblMensagemModal.Text = "Erro ao realizar login. Tente novamente.";
                lblMensagemModal.Visible = true;
            }
        }
    }
}
