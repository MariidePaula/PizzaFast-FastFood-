using System;
using FastPizza.DataAccess;

namespace FastPizza
{
    public partial class Login : System.Web.UI.Page
    {
        // Verifica se o usuário já está logado e redireciona se necessário
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Se já estiver logado, redireciona para a página inicial
                if (Session["ClienteId"] != null)
                {
                    Response.Redirect(ResolveUrl("~/Default.aspx"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Login: {ex.Message}");
            }
        }

        // Processa o login do cliente verificando credenciais e bloqueio
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string senha = txtSenha.Text.Trim();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                {
                    lblMensagem.Text = "Por favor, preencha todos os campos!";
                    lblMensagem.Visible = true;
                    return;
                }

                // Verifica bloqueio antes de autenticar (segurança)
                var clientePorEmail = ClienteDAO.ObterPorEmail(email);
                if (clientePorEmail != null && clientePorEmail.Bloqueado)
                {
                    lblMensagem.Text = "Sua conta foi bloqueada. Para mais informações, entre em contato com o suporte.";
                    lblMensagem.CssClass = "alert alert-danger";
                    lblMensagem.Visible = true;
                    return;
                }

                // Autentica com email e senha
                var cliente = ClienteDAO.Autenticar(email, senha);

                if (cliente != null)
                {
                    // Cria sessão do cliente logado
                    Session["ClienteId"] = cliente.Id;
                    Session["ClienteNome"] = cliente.Nome;
                    Session["ClienteEmail"] = cliente.Email;

                    // Redireciona para URL de retorno ou página inicial
                    string returnUrl = Request.QueryString["ReturnUrl"];
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        Response.Redirect(returnUrl);
                    }
                    else
                    {
                        Response.Redirect(ResolveUrl("~/Default.aspx"));
                    }
                }
                else
                {
                    lblMensagem.Text = "Email ou senha inválidos!";
                    lblMensagem.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no btnLogin_Click: {ex.Message}");
                lblMensagem.Text = "Erro ao realizar login. Tente novamente.";
                lblMensagem.Visible = true;
            }
        }

        // Redireciona para a página de login do administrador
        protected void btnAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no btnAdmin_Click: {ex.Message}");
            }
        }
    }
}

