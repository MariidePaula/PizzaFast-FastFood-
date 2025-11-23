using System;
using System.Web;
using FastPizza.Utils;

namespace FastPizza.Admin
{
    // Página de login do administrador
    public partial class Login : System.Web.UI.Page
    {
        // Credenciais fixas do administrador (hardcoded para segurança)
        private const string ADMIN_USERNAME = "admin";
        private const string ADMIN_EMAIL = "admin@email.com";
        // Hash SHA256 da senha "admin123"
        private const string ADMIN_PASSWORD_HASH = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9";

        // Verifica se admin já está logado e redireciona se necessário
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Session["AdminLogado"] != null && (bool)Session["AdminLogado"])
                {
                    Response.Redirect(ResolveUrl("~/Admin/Dashboard.aspx"), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (!IsPostBack)
                {
                    if (Session["MensagemLogout"] != null)
                    {
                        string mensagemLogout = Session["MensagemLogout"].ToString();
                        Session["MensagemLogout"] = null;

                        ClientScript.RegisterStartupScript(this.GetType(), "MensagemLogout",
                            $"setTimeout(function() {{ mostrarFeedback('{mensagemLogout.Replace("'", "\\'")}', 'danger'); }}, 300);", true);
                    }

                    txtUsuario.Attributes.Add("type", "text");
                    txtUsuario.Attributes.Add("autocomplete", "off");
                    txtUsuario.Attributes.Remove("inputmode");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Admin/Login: {ex.Message}");
            }
        }

        // Processa login do administrador verificando credenciais
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = txtUsuario.Text.Trim();
                string senha = txtSenha.Text.Trim();

                // Validação de campos obrigatórios
                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
                {
                    lblMensagem.Text = "Por favor, preencha todos os campos!";
                    lblMensagem.Visible = true;
                    return;
                }

                // Verifica se usuário corresponde ao admin (aceita username ou email)
                bool usuarioValido = usuario.Equals(ADMIN_USERNAME, StringComparison.OrdinalIgnoreCase) ||
                                     usuario.Equals(ADMIN_EMAIL, StringComparison.OrdinalIgnoreCase);

                // Verifica senha usando hash (nunca compara senha em texto plano)
                if (usuarioValido && PasswordHasher.VerifyPassword(senha, ADMIN_PASSWORD_HASH))
                {
                    // Cria sessão do administrador logado
                    Session["AdminLogado"] = true;
                    Session["AdminUsuario"] = usuario;
                    Response.Redirect(ResolveUrl("~/Admin/Dashboard.aspx"), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    lblMensagem.Text = "Usuário ou senha inválidos!";
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
    }
}

