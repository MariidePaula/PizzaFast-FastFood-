using System;
using System.Collections.Generic;
using System.Web.UI;

namespace FastPizza
{
    // Master page que controla layout e autenticação global
    public partial class SiteMaster : MasterPage
    {
        // Inicializa página e atualiza menu de usuário
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = false;

            // Remove overlay de postback após carregamento
            if (!IsPostBack)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RemoveOverlay",
                    "setTimeout(function() { var o = document.getElementById('postbackOverlay'); if(o) o.classList.remove('active'); }, 100);", true);
            }

            // Verifica se cliente logado foi bloqueado durante a sessão
            if (Session["ClienteId"] != null && Session["AdminLogado"] == null)
            {
                try
                {
                    int clienteId = (int)Session["ClienteId"];
                    var cliente = DataAccess.ClienteDAO.ObterPorId(clienteId);
                    if (cliente != null && cliente.Bloqueado)
                    {
                        // Cliente foi bloqueado: limpa sessão e redireciona
                        Session["ClienteId"] = null;
                        Session["ClienteNome"] = null;
                        Session["ClienteEmail"] = null;
                        Session["Carrinho"] = null;
                        
                        // Redireciona para página inicial com mensagem
                        Session["MensagemLogout"] = "Sua conta foi bloqueada. Entre em contato com o suporte.";
                        Response.Redirect(ResolveUrl("~/Default.aspx"));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao verificar bloqueio no Site.Master: {ex.Message}");
                }
            }

            // Atualiza visibilidade de menus baseado em autenticação
            AtualizarMenuUsuario();
        }

        // Atualiza contador do carrinho antes de renderizar página
        protected void Page_PreRender(object sender, EventArgs e)
        {
            AtualizarContadorCarrinho();
        }

        // Atualiza contador e painel do carrinho (usado após adicionar itens)
        public void AtualizarCarrinho()
        {
            AtualizarContadorCarrinho();
            var upCarrinho = FindControl("upCarrinho") as System.Web.UI.UpdatePanel;
            if (upCarrinho != null)
            {
                upCarrinho.Update();
            }
        }

        // Processa login do modal global (disponível em todas as páginas)
        protected void btnLoginModal_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmailModal.Text.Trim();
                string senha = txtSenhaModal.Text.Trim();

                // Validação de campos obrigatórios
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                {
                    lblMensagemModal.Text = "Por favor, preencha todos os campos!";
                    lblMensagemModal.Visible = true;
                    return;
                }

                // Verifica bloqueio antes de autenticar (segurança)
                var clientePorEmail = DataAccess.ClienteDAO.ObterPorEmail(email);
                if (clientePorEmail != null && clientePorEmail.Bloqueado)
                {
                    lblMensagemModal.Text = "Sua conta foi bloqueada. Para mais informações, entre em contato com o suporte.";
                    lblMensagemModal.Visible = true;
                    lblMensagemModal.CssClass = "alert alert-danger";
                    return;
                }

                // Autentica cliente
                var cliente = DataAccess.ClienteDAO.Autenticar(email, senha);

                if (cliente != null)
                {
                    // Cria sessão do cliente logado
                    Session["ClienteId"] = cliente.Id;
                    Session["ClienteNome"] = cliente.Nome;
                    Session["ClienteEmail"] = cliente.Email;
                    
                    // Limpa campos do modal
                    txtEmailModal.Text = "";
                    txtSenhaModal.Text = "";
                    lblMensagemModal.Visible = false;
                    
                    // Atualiza interface (menu e carrinho)
                    AtualizarMenuUsuario();
                    AtualizarCarrinho();
                    
                    // Fecha modal e exibe feedback de sucesso
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginSucesso", 
                        "setTimeout(function() { " +
                        "var modal = bootstrap.Modal.getInstance(document.getElementById('modalLogin')); " +
                        "if (modal) modal.hide(); " +
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

        // Atualiza visibilidade de menus baseado no tipo de usuário logado
        // Controla exibição de links e painéis de admin/cliente/visitante
        public void AtualizarMenuUsuario()
        {
            try
            {
                bool isAdmin = Session["AdminLogado"] != null && (bool)Session["AdminLogado"];

                // Controla links de navegação (Meus Pedidos vs Dashboard Admin)
                if (pnlLinkMeusPedidos != null && pnlLinkDashboardAdmin != null)
                {
                    if (isAdmin)
                    {
                        pnlLinkMeusPedidos.Visible = false;
                        pnlLinkDashboardAdmin.Visible = true;
                    }
                    else
                    {
                        pnlLinkMeusPedidos.Visible = true;
                        pnlLinkDashboardAdmin.Visible = false;
                    }
                }

                // Exibe painel correto baseado no tipo de usuário
                if (isAdmin)
                {
                    // Admin logado: mostra menu de admin
                    if (pnlAdminLogado != null)
                    {
                        pnlAdminLogado.Visible = true;
                    }
                    if (pnlUsuarioLogado != null)
                    {
                        pnlUsuarioLogado.Visible = false;
                    }
                    if (pnlUsuarioDeslogado != null)
                    {
                        pnlUsuarioDeslogado.Visible = false;
                    }
                }
                else if (Session["ClienteId"] != null)
                {
                    // Cliente logado: mostra menu de cliente
                    if (pnlUsuarioLogado != null)
                    {
                        pnlUsuarioLogado.Visible = true;
                    }
                    if (pnlAdminLogado != null)
                    {
                        pnlAdminLogado.Visible = false;
                    }
                    if (pnlUsuarioDeslogado != null)
                    {
                        pnlUsuarioDeslogado.Visible = false;
                    }
                }
                else
                {
                    // Visitante: mostra opções de login/cadastro
                    if (pnlUsuarioLogado != null)
                    {
                        pnlUsuarioLogado.Visible = false;
                    }
                    if (pnlAdminLogado != null)
                    {
                        pnlAdminLogado.Visible = false;
                    }
                    if (pnlUsuarioDeslogado != null)
                    {
                        pnlUsuarioDeslogado.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em AtualizarMenuUsuario: {ex.Message}");
            }
        }

        // Faz logout do administrador e limpa sessão
        protected void BtnLogoutAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                // Define mensagem de logout para exibir na página inicial
                Session["MensagemLogout"] = "Você saiu e foi deslogado com sucesso!";

                // Limpa sessão do admin
                Session["AdminLogado"] = null;
                Session["AdminUsuario"] = null;
                Response.Redirect(ResolveUrl("~/Default.aspx"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em BtnLogoutAdmin_Click: {ex.Message}");
                Response.Redirect(ResolveUrl("~/Default.aspx"));
            }
        }

        // Faz logout do cliente e limpa sessão (incluindo carrinho)
        protected void BtnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                // Define mensagem de logout para exibir na página inicial
                Session["MensagemLogout"] = "Você saiu e foi deslogado com sucesso!";

                // Limpa toda a sessão do cliente (incluindo carrinho)
                Session["ClienteId"] = null;
                Session["ClienteNome"] = null;
                Session["ClienteEmail"] = null;
                Session["Carrinho"] = null;

                Response.Redirect(ResolveUrl("~/Default.aspx"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em BtnLogout_Click: {ex.Message}");
                Response.Redirect(ResolveUrl("~/Default.aspx"));
            }
        }

        // Atualiza contador de itens no carrinho (badge no menu)
        // Suporta formato antigo (List<int>) e novo (List<string>)
        private void AtualizarContadorCarrinho()
        {
            try
            {
                if (lblQtdCarrinho == null)
                {
                    return;
                }

                if (Session["Carrinho"] != null)
                {
                    int quantidadeItens = 0;
                    var carrinhoSession = Session["Carrinho"];

                    // Suporta ambos os formatos de carrinho (compatibilidade)
                    if (carrinhoSession is List<int> listaInt)
                    {
                        quantidadeItens = listaInt.Count;
                    }
                    else if (carrinhoSession is List<string> listaString)
                    {
                        quantidadeItens = listaString.Count;
                    }

                    // Exibe contador apenas se houver itens
                    if (quantidadeItens > 0)
                    {
                        lblQtdCarrinho.Text = quantidadeItens.ToString();
                        lblQtdCarrinho.Visible = true;
                    }
                    else
                    {
                        lblQtdCarrinho.Visible = false;
                    }
                }
                else
                {
                    // Carrinho vazio: oculta contador
                    lblQtdCarrinho.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em AtualizarContadorCarrinho: {ex.Message}");
            }
        }
    }
}
