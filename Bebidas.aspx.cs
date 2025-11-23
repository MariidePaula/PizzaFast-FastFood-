using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FastPizza.Models;
using FastPizza.DataAccess;
using FastPizza.Utils;

namespace FastPizza
{
    // Página que exibe todas as bebidas disponíveis
    public partial class Bebidas : System.Web.UI.Page
    {
        // Carrega lista de bebidas disponíveis
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.MaintainScrollPositionOnPostBack = false;

                if (!IsPostBack)
                {
                    CarregarBebidas();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Bebidas: {ex.Message}");
                try
                {
                    if (!IsPostBack)
                    {
                        CarregarBebidas();
                    }
                }
                catch { }
            }
        }

        // Carrega bebidas disponíveis e exibe no Repeater
        private void CarregarBebidas()
        {
            try
            {
                var bebidas = BebidaDAO.ObterDisponiveis();

                if (bebidas != null && bebidas.Count > 0)
                {
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

                    // Exibe lista de bebidas
                    rptBebidas.DataSource = bebidas;
                    rptBebidas.DataBind();
                    rptBebidas.Visible = true;
                    lblSemBebidas.Visible = false;
                }
                else
                {
                    // Exibe mensagem se não houver bebidas
                    rptBebidas.Visible = false;
                    lblSemBebidas.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar bebidas: {ex.Message}");
                rptBebidas.Visible = false;
                lblSemBebidas.Text = "<i class='fas fa-exclamation-triangle'></i> Erro ao carregar as bebidas. Por favor, tente novamente mais tarde.";
                lblSemBebidas.CssClass = "alert alert-warning text-center";
                lblSemBebidas.Visible = true;
            }
        }

        // Evento do Repeater quando clica em "Adicionar ao Carrinho"
        protected void rptBebidas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AdicionarCarrinho")
            {
                // Extrai ID da bebida do argumento (formato "B_123" ou "123")
                string argumento = e.CommandArgument.ToString();
                int idBebida;

                if (argumento.StartsWith("B_"))
                {
                    // Formato novo: "B_123"
                    if (int.TryParse(argumento.Substring(2), out idBebida))
                    {
                        AdicionarAoCarrinho(idBebida);
                    }
                }
                else
                {
                    // Formato antigo: "123"
                    if (int.TryParse(argumento, out idBebida))
                    {
                        AdicionarAoCarrinho(idBebida);
                    }
                }
            }
        }

        // Adiciona bebida ao carrinho, verificando autenticação e bloqueio
        private void AdicionarAoCarrinho(int idBebida)
        {
            try
            {
                // Verifica autenticação
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

                CarregarBebidas();

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar bebida ao carrinho: {ex.Message}");
                CarregarBebidas();
                var sm3 = ScriptManager.GetCurrent(Page);
                if (sm3 != null)
                {
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "ErroAdicionar",
                        "setTimeout(function() { mostrarFeedback('Erro ao adicionar bebida ao carrinho. Tente novamente.', 'danger'); }, 100);", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ErroAdicionar",
                        "setTimeout(function() { mostrarFeedback('Erro ao adicionar bebida ao carrinho. Tente novamente.', 'danger'); }, 100);", true);
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

                    ClientScript.RegisterStartupScript(this.GetType(), "LoginSucesso",
                        "setTimeout(function() { " +
                        "var modal = bootstrap.Modal.getInstance(document.getElementById('modalLogin')); " +
                        "if(modal) modal.hide(); " +
                        "}, 100);", true);

                    ClientScript.RegisterStartupScript(this.GetType(), "AtualizarInterface",
                        "setTimeout(function() { window.location.reload(); }, 500);", true);
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

