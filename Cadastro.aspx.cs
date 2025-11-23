using System;
using FastPizza.Models;
using FastPizza.DataAccess;
using FastPizza.Utils;

namespace FastPizza
{
    public partial class Cadastro : System.Web.UI.Page
    {
        // Verifica se o usuário já está logado e redireciona se necessário
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Se já estiver logado, não precisa cadastrar novamente
                if (Session["ClienteId"] != null)
                {
                    Response.Redirect(ResolveUrl("~/Default.aspx"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Cadastro: {ex.Message}");
            }
        }

        // Processa o cadastro de novo cliente com validações
        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensagem.Visible = false;
                lblSucesso.Visible = false;

                // Validação do ASP.NET (RequiredFieldValidator)
                if (!Page.IsValid)
                {
                    lblMensagem.Text = "Por favor, preencha todos os campos obrigatórios!";
                    lblMensagem.Visible = true;
                    return;
                }

                // Obtém valores dos campos
                string nome = txtNome.Text.Trim();
                string email = txtEmail.Text.Trim();
                string telefone = txtTelefone.Text.Trim();
                string senha = txtSenha.Text.Trim();
                string confirmarSenha = txtConfirmarSenha.Text.Trim();

                // Validações de regra de negócio
                if (string.IsNullOrWhiteSpace(nome))
                {
                    lblMensagem.Text = "O nome não pode estar vazio!";
                    lblMensagem.Visible = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    lblMensagem.Text = "O email não pode estar vazio!";
                    lblMensagem.Visible = true;
                    return;
                }

                // Valida tamanho mínimo da senha
                if (senha.Length < 6)
                {
                    lblMensagem.Text = "A senha deve ter no mínimo 6 caracteres!";
                    lblMensagem.Visible = true;
                    return;
                }

                // Verifica se as senhas coincidem
                if (senha != confirmarSenha)
                {
                    lblMensagem.Text = "As senhas não coincidem!";
                    lblMensagem.Visible = true;
                    return;
                }

                // Verifica se o email já está cadastrado
                if (ClienteDAO.EmailExiste(email))
                {
                    // Verifica se o cliente está bloqueado para dar mensagem mais específica
                    var clienteExistente = ClienteDAO.ObterPorEmail(email);
                    if (clienteExistente != null && clienteExistente.Bloqueado)
                    {
                        lblMensagem.Text = "Esta conta foi bloqueada. Para mais informações, entre em contato com o suporte.";
                        lblMensagem.CssClass = "alert alert-danger";
                    }
                    else
                    {
                        lblMensagem.Text = "Este email já está cadastrado! Por favor, use outro email ou faça login se já possui uma conta.";
                    }
                    lblMensagem.Visible = true;
                    txtEmail.Focus();
                    return;
                }

                // Cria novo cliente com senha hasheada
                var cliente = new Cliente
                {
                    Nome = nome,
                    Email = email,
                    Telefone = telefone,
                    SenhaHash = PasswordHasher.HashPassword(senha),
                    DataCadastro = DateTime.Now,
                    Bloqueado = false
                };

            try
            {
                // Salva o cliente no banco de dados
                ClienteDAO.Adicionar(cliente);
            }
            catch (ArgumentException ex)
            {
                // Trata erros de validação (ex: email duplicado)
                if (ex.Message.Contains("Email") || ex.Message.Contains("email"))
                {
                    lblMensagem.Text = "Este email já está cadastrado! Por favor, use outro email ou faça login se já possui uma conta.";
                }
                else
                {
                    lblMensagem.Text = $"Erro de validação: {ex.Message}";
                }
                lblMensagem.Visible = true;
                txtEmail.Focus();
                return;
            }

            // Cria sessão do novo cliente e redireciona para completar perfil
            Session["ClienteId"] = cliente.Id;
            Session["ClienteNome"] = cliente.Nome;
            Session["ClienteEmail"] = cliente.Email;

            Response.Redirect(ResolveUrl("~/MeuPerfil.aspx?novo=true"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao cadastrar: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                if (ex.Message.Contains("Email") || ex.Message.Contains("email") || ex.Message.Contains("cadastrado"))
                {
                    lblMensagem.Text = "Este email já está cadastrado! Por favor, use outro email ou faça login se já possui uma conta.";
                }
                else
                {
                    lblMensagem.Text = "Erro ao realizar cadastro. Por favor, verifique os dados e tente novamente.";
                }
                lblMensagem.Visible = true;
                txtEmail.Focus();
            }
        }
    }
}

