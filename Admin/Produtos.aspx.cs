using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using FastPizza.DataAccess;
using FastPizza.Models;

namespace FastPizza.Admin
{
    // Página de gerenciamento de produtos (pizzas) - CRUD completo
    public partial class Produtos : System.Web.UI.Page
    {
        // Verifica autenticação e carrega lista de produtos
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Verifica se administrador está autenticado
                if (Session["AdminLogado"] == null || !(bool)Session["AdminLogado"])
                {
                    Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
                    return;
                }

                // Marca botão como botão de admin (para estilização)
                if (btnNovo != null)
                {
                    btnNovo.Attributes.Add("data-admin-button", "true");
                }

                if (!IsPostBack)
                {
                    CarregarProdutos();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Admin/Produtos: {ex.Message}");
                Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
            }
        }

        // Carrega todos os produtos e exibe no GridView
        private void CarregarProdutos()
        {
            gridProdutos.DataSource = ProdutoDAO.ObterTodos();
            gridProdutos.DataBind();
        }

        // Abre modal para cadastrar novo produto
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("btnNovo_Click foi chamado!");

                LimparFormulario();
                lblModalTitulo.Text = "Nova Pizza";
                chkEmDestaque.Checked = false;
                modalProduto.Style["display"] = "block";

                // Abre modal via JavaScript (compatibilidade com Bootstrap)
                string script = @"
                    setTimeout(function() {
                        var modal = document.getElementById('" + modalProduto.ClientID + @"');
                        if (modal) {
                            modal.style.display = 'block';
                            console.log('Modal aberto via JavaScript');
                        } else {
                            console.error('Modal não encontrado: " + modalProduto.ClientID + @"');
                        }
                    }, 100);";

                ClientScript.RegisterStartupScript(this.GetType(), "AbrirModalProduto", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em btnNovo_Click: {ex.Message}");
                string erroScript = "alert('Erro ao abrir modal: " + ex.Message.Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ") + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Erro", erroScript, true);
            }
        }

        // Processa ações do GridView (Editar, Excluir)
        protected void gridProdutos_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "Editar")
                {
                    EditarProduto(id);
                }
                else if (e.CommandName == "Excluir")
                {
                    System.Diagnostics.Debug.WriteLine($"Excluindo pizza com ID: {id}");

                    // Remove produto do banco de dados
                    ProdutoDAO.Excluir(id);
                    CarregarProdutos();

                    ClientScript.RegisterStartupScript(this.GetType(), "Sucesso",
                        "setTimeout(function() { mostrarFeedback('Pizza excluída com sucesso!', 'success'); }, 100);", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em gridProdutos_RowCommand: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                string mensagemErro = ex.Message.Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ");
                ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                    "setTimeout(function() { mostrarFeedback('Erro ao excluir pizza: " + mensagemErro + "', 'danger'); }, 100);", true);
            }
        }

        // Carrega dados do produto no formulário e abre modal de edição
        private void EditarProduto(int id)
        {
            var produto = ProdutoDAO.ObterPorId(id);
            if (produto != null)
            {
                hdnProdutoId.Value = produto.Id.ToString();
                txtNome.Text = produto.Nome;
                txtDescricao.Text = produto.Descricao;
                if (ddlCategoria.Items.FindByValue(produto.Categoria) != null)
                {
                    ddlCategoria.SelectedValue = produto.Categoria;
                }
                txtPreco.Text = produto.Preco.ToString("F2", CultureInfo.GetCultureInfo("pt-BR"));
                txtImagemUrl.Text = produto.ImagemUrl;
                txtEstoque.Text = produto.Estoque.ToString();
                chkDisponivel.Checked = produto.Disponivel;
                chkEmDestaque.Checked = produto.EmDestaque;

                lblModalTitulo.Text = "Editar Pizza";
                modalProduto.Style["display"] = "block";

                ClientScript.RegisterStartupScript(this.GetType(), "AbrirModal",
                    "setTimeout(function() { " +
                    "var modal = document.getElementById('" + modalProduto.ClientID + "'); " +
                    "if (modal) modal.style.display = 'block'; " +
                    "}, 100);", true);
            }
        }

        // Salva ou atualiza produto no banco de dados
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validações de campos obrigatórios
                if (string.IsNullOrWhiteSpace(txtNome.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                        "mostrarFeedback('Nome é obrigatório', 'danger');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlCategoria.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                        "mostrarFeedback('Categoria é obrigatória', 'danger');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPreco.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                        "mostrarFeedback('Preço é obrigatório', 'danger');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEstoque.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                        "mostrarFeedback('Estoque é obrigatório', 'danger');", true);
                    return;
                }

                var produto = new Produto();

                // Se tiver ID, está editando; senão, está criando novo
                if (!string.IsNullOrEmpty(hdnProdutoId.Value))
                {
                    produto.Id = Convert.ToInt32(hdnProdutoId.Value);
                }

                // Preenche dados do produto
                produto.Nome = txtNome.Text.Trim();
                produto.Descricao = txtDescricao.Text.Trim();
                produto.Categoria = ddlCategoria.SelectedValue;
                produto.Preco = ConverterPreco(txtPreco.Text);
                produto.ImagemUrl = txtImagemUrl.Text.Trim();
                produto.Estoque = Convert.ToInt32(txtEstoque.Text);
                produto.Disponivel = chkDisponivel.Checked;
                produto.EmDestaque = chkEmDestaque.Checked;

                // Salva ou atualiza no banco
                if (produto.Id > 0)
                {
                    ProdutoDAO.Atualizar(produto);
                    ClientScript.RegisterStartupScript(this.GetType(), "Sucesso",
                        "mostrarFeedback('Pizza atualizada com sucesso!', 'success');", true);
                }
                else
                {
                    ProdutoDAO.Adicionar(produto);
                    ClientScript.RegisterStartupScript(this.GetType(), "Sucesso",
                        "mostrarFeedback('Pizza cadastrada com sucesso!', 'success');", true);
                }

                modalProduto.Style["display"] = "none";
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar pizza: {ex.Message}");
                string mensagemErro = ex.Message.Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ");
                ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                    "setTimeout(function() { mostrarFeedback('Erro ao salvar pizza: " + mensagemErro + "', 'danger'); }, 100);", true);
            }
        }

        // Fecha modal sem salvar
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            modalProduto.Style["display"] = "none";
        }

        // Limpa campos do formulário para novo cadastro
        private void LimparFormulario()
        {
            hdnProdutoId.Value = "";
            txtNome.Text = "";
            txtDescricao.Text = "";
            ddlCategoria.SelectedIndex = 0;
            txtPreco.Text = "";
            txtImagemUrl.Text = "";
            txtEstoque.Text = "0";
            chkDisponivel.Checked = true;
            chkEmDestaque.Checked = false;
        }

        // Evento do GridView quando uma linha é renderizada (não utilizado)
        protected void gridProdutos_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {

        }

        // Converte string de preço para decimal (suporta formatos brasileiro e americano)
        private decimal ConverterPreco(string valorTexto)
        {
            if (string.IsNullOrWhiteSpace(valorTexto))
                return 0;

            valorTexto = valorTexto.Trim();

            // Remove símbolos de moeda e espaços
            valorTexto = valorTexto.Replace(" ", "").Replace("R$", "").Replace("$", "");

            // Se contém vírgula, assume formato brasileiro (1.234,56)
            if (valorTexto.Contains(","))
            {
                // Remove pontos de milhar e converte vírgula para ponto decimal
                valorTexto = valorTexto.Replace(".", "").Replace(",", ".");
            }

            // Tenta converter para decimal
            decimal resultado;
            if (decimal.TryParse(valorTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out resultado))
            {
                return resultado;
            }

            throw new FormatException($"Valor de preço inválido: {valorTexto}");
        }
    }
}

