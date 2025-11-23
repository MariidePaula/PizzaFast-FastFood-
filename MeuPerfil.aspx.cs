using System;
using System.Linq;
using FastPizza.Models;
using FastPizza.DataAccess;
using FastPizza.Utils;

namespace FastPizza
{
    // Página de gerenciamento de perfil e endereços do cliente
    public partial class MeuPerfil : System.Web.UI.Page
    {
        // Verifica autenticação e carrega dados do cliente
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Redireciona para login se não estiver autenticado
                if (Session["ClienteId"] == null)
                {
                    Response.Redirect(ResolveUrl("~/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.Url.PathAndQuery)));
                    return;
                }

                if (!IsPostBack)
                {
                    CarregarDadosCliente();
                    CarregarEnderecos();

                    // Mensagem de boas-vindas para novos clientes
                    if (Request.QueryString["novo"] == "true")
                    {
                        lblMensagem.Text = "Bem-vindo! Complete seu cadastro adicionando um endereço.";
                        lblMensagem.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load MeuPerfil: {ex.Message}");

                if (Session["ClienteId"] == null)
                {
                    Response.Redirect(ResolveUrl("~/Login.aspx"));
                }
            }
        }

        // Carrega dados pessoais do cliente nos campos do formulário
        private void CarregarDadosCliente()
        {
            int clienteId = (int)Session["ClienteId"];
            var cliente = ClienteDAO.ObterPorId(clienteId);

            if (cliente != null)
            {
                txtNome.Text = cliente.Nome;
                txtEmail.Text = cliente.Email;
                txtTelefone.Text = cliente.Telefone;
            }
        }

        // Carrega lista de endereços do cliente
        private void CarregarEnderecos()
        {
            int clienteId = (int)Session["ClienteId"];
            var enderecos = EnderecoDAO.ObterPorCliente(clienteId);

            if (enderecos.Count > 0)
            {
                // Exibe lista de endereços
                rptEnderecos.DataSource = enderecos;
                rptEnderecos.DataBind();
                rptEnderecos.Visible = true;
                pnlSemEnderecos.Visible = false;
            }
            else
            {
                // Exibe mensagem se não houver endereços
                rptEnderecos.Visible = false;
                pnlSemEnderecos.Visible = true;
            }
        }

        // Atualiza dados pessoais do cliente (nome, email, telefone)
        protected void BtnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validação do ASP.NET
                if (!Page.IsValid)
                {
                    lblErro.Text = "Por favor, preencha todos os campos obrigatórios!";
                    lblErro.Visible = true;
                    return;
                }

                int clienteId = (int)Session["ClienteId"];
                var cliente = ClienteDAO.ObterPorId(clienteId);

                if (cliente != null)
                {
                    string nome = txtNome.Text.Trim();
                    string email = txtEmail.Text.Trim();
                    string telefone = txtTelefone.Text.Trim();

                    // Validações de regra de negócio
                    if (string.IsNullOrWhiteSpace(nome))
                    {
                        lblErro.Text = "O nome não pode estar vazio!";
                        lblErro.Visible = true;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(email))
                    {
                        lblErro.Text = "O email não pode estar vazio!";
                        lblErro.Visible = true;
                        return;
                    }

                    // Atualiza dados do cliente
                    cliente.Nome = nome;
                    cliente.Email = email;
                    cliente.Telefone = telefone;

                    ClienteDAO.Atualizar(cliente);

                    // Atualiza sessão com novos dados
                    Session["ClienteNome"] = cliente.Nome;
                    Session["ClienteEmail"] = cliente.Email;

                    lblMensagem.Text = "Dados atualizados com sucesso!";
                    lblMensagem.Visible = true;
                    lblErro.Visible = false;

                    ClientScript.RegisterStartupScript(typeof(MeuPerfil), "Sucesso",
                        "mostrarFeedback('Dados atualizados com sucesso!', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar perfil: {ex.Message}");
                lblErro.Text = "Erro ao salvar os dados. Tente novamente.";
                lblErro.Visible = true;
                lblMensagem.Visible = false;

                ClientScript.RegisterStartupScript(typeof(MeuPerfil), "Erro",
                    "mostrarFeedback('Erro ao salvar os dados. Tente novamente.', 'danger');", true);
            }
        }

        protected void BtnCancelar_Click(object sender, EventArgs e)
        {
            CarregarDadosCliente();
        }

        // Altera senha do cliente com validações de segurança
        protected void BtnAlterarSenha_Click(object sender, EventArgs e)
        {
            try
            {
                int clienteId = (int)Session["ClienteId"];
                var cliente = ClienteDAO.ObterPorId(clienteId);

                if (cliente == null)
                {
                    lblErro.Text = "Cliente não encontrado!";
                    lblErro.Visible = true;
                    return;
                }

                // Obtém valores dos campos de senha
                string senhaAtual = txtSenhaAtual != null && txtSenhaAtual.Text != null ? txtSenhaAtual.Text.Trim() : "";
                string novaSenha = txtNovaSenha != null && txtNovaSenha.Text != null ? txtNovaSenha.Text.Trim() : "";
                string confirmarSenha = txtConfirmarNovaSenha != null && txtConfirmarNovaSenha.Text != null ? txtConfirmarNovaSenha.Text.Trim() : "";

                // Validação de campos obrigatórios
                if (string.IsNullOrEmpty(senhaAtual) || string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(confirmarSenha))
                {
                    lblErro.Text = "Por favor, preencha todos os campos!";
                    lblErro.Visible = true;
                    return;
                }

                // Verifica se senha atual está correta
                if (cliente.SenhaHash == null || !PasswordHasher.VerifyPassword(senhaAtual, cliente.SenhaHash))
                {
                    lblErro.Text = "Senha atual incorreta!";
                    lblErro.Visible = true;
                    return;
                }

                // Valida tamanho mínimo da nova senha
                if (novaSenha.Length < 6)
                {
                    lblErro.Text = "A nova senha deve ter no mínimo 6 caracteres!";
                    lblErro.Visible = true;
                    return;
                }

                // Verifica se as senhas coincidem
                if (novaSenha != confirmarSenha)
                {
                    lblErro.Text = "As senhas não coincidem!";
                    lblErro.Visible = true;
                    return;
                }

                // Atualiza senha no banco (faz hash automaticamente)
                ClienteDAO.AtualizarSenha(clienteId, novaSenha);

                lblMensagem.Text = "Senha alterada com sucesso!";
                lblMensagem.Visible = true;
                lblErro.Visible = false;

                // Limpa campos após sucesso
                txtSenhaAtual.Text = "";
                txtNovaSenha.Text = "";
                txtConfirmarNovaSenha.Text = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao alterar senha: {ex.Message}");
                lblErro.Text = "Erro ao alterar senha. Tente novamente.";
                lblErro.Visible = true;
                lblMensagem.Visible = false;
            }
        }

        // Salva ou atualiza endereço do cliente
        protected void BtnSalvarEndereco_Click(object sender, EventArgs e)
        {
            try
            {
                int clienteId = (int)Session["ClienteId"];
                int enderecoId = 0;

                // Obtém ID do endereço se estiver editando
                int parsedId;
                if (hdnEnderecoId != null && !string.IsNullOrEmpty(hdnEnderecoId.Value) && int.TryParse(hdnEnderecoId.Value, out parsedId))
                {
                    enderecoId = parsedId;
                }

                // Obtém valores dos campos
                string rua = txtRua != null && txtRua.Text != null ? txtRua.Text.Trim() : "";
                string numero = txtNumero != null && txtNumero.Text != null ? txtNumero.Text.Trim() : "";
                string bairro = txtBairro != null && txtBairro.Text != null ? txtBairro.Text.Trim() : "";
                string cidade = txtCidade != null && txtCidade.Text != null ? txtCidade.Text.Trim() : "";
                string cep = txtCEP != null && txtCEP.Text != null ? txtCEP.Text.Trim() : "";

                // Validação de campos obrigatórios
                if (string.IsNullOrWhiteSpace(rua) || string.IsNullOrWhiteSpace(numero) ||
                    string.IsNullOrWhiteSpace(bairro) || string.IsNullOrWhiteSpace(cidade) ||
                    string.IsNullOrWhiteSpace(cep))
                {
                    lblErro.Text = "Por favor, preencha todos os campos obrigatórios!";
                    lblErro.Visible = true;
                    return;
                }

                Endereco endereco;

                if (enderecoId > 0)
                {
                    // Modo edição: busca endereço existente e valida propriedade
                    endereco = EnderecoDAO.ObterPorId(enderecoId);
                    if (endereco == null || endereco.ClienteId != clienteId)
                    {
                        lblErro.Text = "Endereço não encontrado!";
                        lblErro.Visible = true;
                        return;
                    }
                }
                else
                {
                    // Modo criação: cria novo endereço
                    endereco = new Endereco
                    {
                        ClienteId = clienteId
                    };
                }

                endereco.Rua = rua;
                endereco.Numero = numero;
                endereco.Complemento = txtComplemento != null && txtComplemento.Text != null ? txtComplemento.Text.Trim() : "";
                endereco.Bairro = bairro;
                endereco.Cidade = cidade;
                endereco.CEP = cep;
                endereco.Referencia = txtReferencia != null && txtReferencia.Text != null ? txtReferencia.Text.Trim() : "";
                endereco.EnderecoPadrao = chkEnderecoPadrao != null ? chkEnderecoPadrao.Checked : false;

                if (enderecoId > 0)
                {
                    EnderecoDAO.Atualizar(endereco);
                }
                else
                {
                    EnderecoDAO.Adicionar(endereco);
                }

                LimparFormularioEndereco();

                CarregarEnderecos();

                string mensagemSucesso = enderecoId > 0 ? "Endereço atualizado com sucesso!" : "Endereço adicionado com sucesso!";
                lblMensagem.Text = mensagemSucesso;
                lblMensagem.Visible = true;
                lblErro.Visible = false;

                ClientScript.RegisterStartupScript(typeof(MeuPerfil), "FecharModal",
                    "var modal = bootstrap.Modal.getInstance(document.getElementById('modalEndereco')); if(modal) modal.hide();", true);

                ClientScript.RegisterStartupScript(typeof(MeuPerfil), "SucessoEndereco",
                    $"mostrarFeedback('{mensagemSucesso}', 'success');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar endereço: {ex.Message}");
                lblErro.Text = "Erro ao salvar endereço. Tente novamente.";
                lblErro.Visible = true;
                lblMensagem.Visible = false;

                ClientScript.RegisterStartupScript(typeof(MeuPerfil), "ErroEndereco",
                    "mostrarFeedback('Erro ao salvar endereço. Tente novamente.', 'danger');", true);
            }
        }

        // Processa ações do Repeater de endereços (Editar, Excluir, DefinirPadrao)
        protected void RptEnderecos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            try
            {
                int clienteId = (int)Session["ClienteId"];

                // Valida ID do endereço
                int enderecoId;
                if (e.CommandArgument == null || !int.TryParse(e.CommandArgument.ToString(), out enderecoId))
                {
                    lblErro.Text = "Endereço inválido!";
                    lblErro.Visible = true;
                    return;
                }

                if (e.CommandName == "Editar")
                {
                    // Carrega dados do endereço no formulário e abre modal
                    var endereco = EnderecoDAO.ObterPorId(enderecoId);
                    if (endereco != null && endereco.ClienteId == clienteId)
                    {
                        hdnEnderecoId.Value = endereco.Id.ToString();
                        txtRua.Text = endereco.Rua ?? "";
                        txtNumero.Text = endereco.Numero ?? "";
                        txtComplemento.Text = endereco.Complemento ?? "";
                        txtBairro.Text = endereco.Bairro ?? "";
                        txtCidade.Text = endereco.Cidade ?? "";
                        txtCEP.Text = endereco.CEP ?? "";
                        txtReferencia.Text = endereco.Referencia ?? "";
                        chkEnderecoPadrao.Checked = endereco.EnderecoPadrao;
                        lblTituloModal.Text = "Editar Endereço";

                        ClientScript.RegisterStartupScript(typeof(MeuPerfil), "AbrirModal", "var modal = new bootstrap.Modal(document.getElementById('modalEndereco')); modal.show();", true);
                    }
                }
                else if (e.CommandName == "Excluir")
                {
                    // Remove endereço após validar propriedade
                    var endereco = EnderecoDAO.ObterPorId(enderecoId);
                    if (endereco != null && endereco.ClienteId == clienteId)
                    {
                        EnderecoDAO.Remover(enderecoId);
                        CarregarEnderecos();
                        lblMensagem.Text = "Endereço excluído com sucesso!";
                        lblMensagem.Visible = true;
                    }
                }
                else if (e.CommandName == "DefinirPadrao")
                {
                    // Define endereço como padrão (remove padrão dos outros)
                    EnderecoDAO.DefinirComoPadrao(enderecoId, clienteId);
                    CarregarEnderecos();
                    lblMensagem.Text = "Endereço definido como padrão!";
                    lblMensagem.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no rptEnderecos_ItemCommand: {ex.Message}");
                lblErro.Text = "Erro ao processar ação. Tente novamente.";
                lblErro.Visible = true;
            }
        }

        private void LimparFormularioEndereco()
        {
            hdnEnderecoId.Value = "0";
            txtRua.Text = "";
            txtNumero.Text = "";
            txtComplemento.Text = "";
            txtBairro.Text = "";
            txtCidade.Text = "";
            txtCEP.Text = "";
            txtReferencia.Text = "";
            chkEnderecoPadrao.Checked = false;
            lblTituloModal.Text = "Adicionar Endereço";
        }
    }
}

