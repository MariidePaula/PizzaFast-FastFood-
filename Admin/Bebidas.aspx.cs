using System;
using System.Globalization; // Usado para formatação de moeda
using System.Linq; // Usado para operações LINQ 
using System.Web.UI; // Classe base para controle de página
using FastPizza.DataAccess; // Camada de acesso a dados (DAO)
using FastPizza.Models; // Modelos de dados, como a classe Bebida
using FastPizza.Utils; // Classes utilitárias, como ImageMapper

namespace FastPizza.Admin
{
    // Página de gerenciamento de bebidas - CRUD completo
    // Herda de System.Web.UI.Page, sendo a classe Code-Behind da página Bebidas.aspx
    public partial class Bebidas : System.Web.UI.Page
    {
        // ---------------------------------------------------------------------
        // EVENTOS DE PÁGINA
        // ---------------------------------------------------------------------

        // Verifica autenticação e carrega lista de bebidas
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. **VERIFICAÇÃO DE AUTENTICAÇÃO:**
                // Verifica se a variável de sessão 'AdminLogado' existe e se é verdadeira.
                // Se o administrador não estiver logado, redireciona para a página de login.
                if (Session["AdminLogado"] == null || !(bool)Session["AdminLogado"])
                {
                    Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
                    return; // Interrompe o processamento da página
                }

                // 2. **ESTILIZAÇÃO DE BOTÃO (OPCIONAL):**
                // Adiciona um atributo para fins de estilização CSS (se o elemento btnNovo existir).
                if (btnNovo != null)
                {
                    btnNovo.Attributes.Add("data-admin-button", "true");
                }

                // 3. **CARREGAMENTO INICIAL DE DADOS:**
                // Carrega os dados da lista de bebidas apenas na primeira vez que a página é carregada (não em PostBacks).
                if (!IsPostBack)
                {
                    CarregarBebidas();
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro grave durante o Page_Load, registra o erro e redireciona para o login.
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Admin/Bebidas: {ex.Message}");
                Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
            }
        }

        // ---------------------------------------------------------------------
        // MÉTODOS DE CARREGAMENTO DE DADOS
        // ---------------------------------------------------------------------

        // Carrega todas as bebidas do banco de dados e resolve URLs de imagens para exibição correta no front-end.
        private void CarregarBebidas()
        {
            // Obtém todas as bebidas usando o Data Access Object (DAO)
            var bebidas = BebidaDAO.ObterTodos();

            // Garante que todas as bebidas tenham imagem resolvida corretamente
            if (bebidas != null)
            {
                foreach (var bebida in bebidas)
                {
                    string imagemUrl = bebida.ImagemUrl;

                    // Se a URL da imagem estiver vazia/nula no BD, tenta obter uma imagem padrão ou mapeada.
                    if (string.IsNullOrWhiteSpace(imagemUrl))
                    {
                        imagemUrl = ImageMapper.ObterImagemBebida(bebida);
                    }

                    // Sempre resolve a URL relativa para uma URL absoluta no contexto do servidor.
                    if (!string.IsNullOrWhiteSpace(imagemUrl))
                    {
                        // Se for URL relativa (começa com ~/), resolve para a URL absoluta da aplicação.
                        if (imagemUrl.StartsWith("~/"))
                        {
                            bebida.ImagemUrl = ResolveUrl(imagemUrl);
                        }
                        // Se não for URL absoluta (http/https) nem root-relative (/) e não for "~/".
                        else if (!imagemUrl.StartsWith("http://") && !imagemUrl.StartsWith("https://") && !imagemUrl.StartsWith("/"))
                        {
                            // Tenta resolver assumindo um caminho padrão relativo.
                            bebida.ImagemUrl = ResolveUrl("~/Images/Bebidas/" + imagemUrl);
                        }
                        // Se já for absoluta (http/https ou root-relative /), mantém como está (implicitamente).
                    }
                }
            }

            // Associa a lista de bebidas ao GridView (gridBebidas) e exibe os dados.
            gridBebidas.DataSource = bebidas;
            gridBebidas.DataBind();
        }

        // ---------------------------------------------------------------------
        // EVENTOS DE AÇÃO (CRIAÇÃO/EDIÇÃO/EXCLUSÃO)
        // ---------------------------------------------------------------------

        // Evento disparado ao clicar no botão "Novo" (btnNovo)
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("btnNovo_Click foi chamado!");

                // 1. **PREPARAÇÃO DO FORMULÁRIO:**
                LimparFormulario(); // Limpa todos os campos para um novo cadastro.
                lblModalTitulo.Text = "Nova Bebida"; // Define o título do modal.
                modalBebida.Style["display"] = "block"; // Tenta mostrar o modal via C# (pode ser inconsistente).

                // 2. **ABERTURA DO MODAL VIA JAVASCRIPT:**
                // Registra um script para garantir que o modal seja exibido no lado do cliente.
                // Um pequeno 'setTimeout' é usado para garantir a execução após a renderização do DOM.
                string script = @"
                    setTimeout(function() {
                        var modal = document.getElementById('" + modalBebida.ClientID + @"');
                        if (modal) {
                            modal.style.display = 'block';
                            console.log('Modal aberto via JavaScript');
                        } else {
                            console.error('Modal não encontrado: " + modalBebida.ClientID + @"');
                        }
                    }, 100);";

                ClientScript.RegisterStartupScript(this.GetType(), "AbrirModalBebida", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em btnNovo_Click: {ex.Message}");
                string erroScript = "alert('Erro ao abrir modal: " + ex.Message.Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ") + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Erro", erroScript, true);
            }
        }

        // Evento disparado ao clicar nos botões 'Editar' ou 'Excluir' dentro do GridView.
        protected void gridBebidas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            // O CommandArgument contém o ID da bebida (definido no ASPX).
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Editar")
            {
                // Chama o método para carregar os dados no formulário de edição.
                EditarBebida(id);
            }
            else if (e.CommandName == "Excluir")
            {
                try
                {
                    // Tenta excluir a bebida do banco de dados.
                    BebidaDAO.Excluir(id);
                    // Recarrega o GridView para exibir a lista atualizada.
                    CarregarBebidas();
                    // Exibe uma mensagem de sucesso no front-end.
                    ClientScript.RegisterStartupScript(this.GetType(), "Sucesso",
                        "mostrarFeedback('Bebida excluída com sucesso!', 'success');", true);
                }
                catch (Exception ex)
                {
                    // Em caso de falha na exclusão (ex: chave estrangeira), exibe um erro.
                    System.Diagnostics.Debug.WriteLine($"Erro ao excluir bebida: {ex.Message}");
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                        "mostrarFeedback('Erro ao excluir bebida. Tente novamente.', 'danger');", true);
                }
            }
        }

        // Lógica para carregar os dados de uma bebida existente para edição.
        private void EditarBebida(int id)
        {
            // Obtém a bebida do banco de dados pelo ID.
            var bebida = BebidaDAO.ObterPorId(id);
            if (bebida != null)
            {
                // 1. **POPULA FORMULÁRIO:**
                // Armazena o ID em um campo oculto (HiddenField) para saber se é uma atualização.
                hdnBebidaId.Value = bebida.Id.ToString();
                // Popula os campos do formulário com os dados da bebida.
                txtNome.Text = bebida.Nome;
                txtDescricao.Text = bebida.Descricao;
                // Seleciona o item correto no DropDownList de Categoria.
                if (ddlCategoria.Items.FindByValue(bebida.Categoria) != null)
                {
                    ddlCategoria.SelectedValue = bebida.Categoria;
                }
                // Formata o preço para o padrão brasileiro (pt-BR) para exibição.
                txtPreco.Text = bebida.Preco.ToString("F2", CultureInfo.GetCultureInfo("pt-BR"));
                txtImagemUrl.Text = bebida.ImagemUrl;
                txtEstoque.Text = bebida.Estoque.ToString();
                chkDisponivel.Checked = bebida.Disponivel;

                // 2. **ABERTURA DO MODAL:**
                lblModalTitulo.Text = "Editar Bebida";
                modalBebida.Style["display"] = "block"; // Tenta mostrar via C#.

                // Script para garantir a abertura do modal no cliente.
                ClientScript.RegisterStartupScript(this.GetType(), "AbrirModal",
                    "setTimeout(function() { " +
                    "var modal = document.getElementById('" + modalBebida.ClientID + "'); " +
                    "if (modal) modal.style.display = 'block'; " +
                    "}, 100);", true);
            }
        }

        // Evento disparado ao clicar no botão "Salvar" (btnSalvar) dentro do modal.
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. **VALIDAÇÕES DO FORMULÁRIO:**
                // Verifica se os campos obrigatórios foram preenchidos.
                if (string.IsNullOrWhiteSpace(txtNome.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro", "mostrarFeedback('Nome é obrigatório', 'danger');", true);
                    return;
                }
                // [Outras validações omitidas por brevidade, mas o código as inclui]
                if (string.IsNullOrWhiteSpace(ddlCategoria.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro", "mostrarFeedback('Categoria é obrigatória', 'danger');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtPreco.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro", "mostrarFeedback('Preço é obrigatório', 'danger');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtEstoque.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro", "mostrarFeedback('Estoque é obrigatório', 'danger');", true);
                    return;
                }

                var bebida = new Bebida();

                // 2. **IDENTIFICAÇÃO (NOVO ou EDIÇÃO):**
                // Se o HiddenField (hdnBebidaId) tiver valor, é uma edição.
                if (!string.IsNullOrEmpty(hdnBebidaId.Value))
                {
                    bebida.Id = Convert.ToInt32(hdnBebidaId.Value);
                }

                // 3. **MAPEAMENTO DE DADOS:**
                bebida.Nome = txtNome.Text.Trim();
                bebida.Descricao = txtDescricao.Text.Trim();
                bebida.Categoria = ddlCategoria.SelectedValue;
                // Converte o texto do preço, que pode estar no formato brasileiro, para decimal.
                bebida.Preco = ConverterPreco(txtPreco.Text);
                bebida.ImagemUrl = txtImagemUrl.Text.Trim();
                bebida.Estoque = Convert.ToInt32(txtEstoque.Text);
                bebida.Disponivel = chkDisponivel.Checked;

                // 4. **OPERAÇÃO DAO:**
                if (bebida.Id > 0)
                {
                    // Atualiza a bebida existente.
                    BebidaDAO.Atualizar(bebida);
                    ClientScript.RegisterStartupScript(this.GetType(), "Sucesso", "mostrarFeedback('Bebida atualizada com sucesso!', 'success');", true);
                }
                else
                {
                    // Adiciona uma nova bebida.
                    BebidaDAO.Adicionar(bebida);
                    ClientScript.RegisterStartupScript(this.GetType(), "Sucesso", "mostrarFeedback('Bebida cadastrada com sucesso!', 'success');", true);
                }

                // 5. **FINALIZAÇÃO:**
                modalBebida.Style["display"] = "none"; // Fecha o modal.
                CarregarBebidas(); // Recarrega o GridView.
            }
            catch (Exception ex)
            {
                // Em caso de erro ao salvar (ex: falha de conversão, erro de banco), exibe um erro.
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar bebida: {ex.Message}");
                string mensagemErro = ex.Message.Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ");
                ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                    "setTimeout(function() { mostrarFeedback('Erro ao salvar bebida: " + mensagemErro + "', 'danger'); }, 100);", true);
            }
        }

        // Evento disparado ao clicar no botão "Cancelar" (btnCancelar) dentro do modal.
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Apenas esconde o modal.
            modalBebida.Style["display"] = "none";
        }

        // ---------------------------------------------------------------------
        // MÉTODOS AUXILIARES
        // ---------------------------------------------------------------------

        // Limpa todos os campos do formulário no modal, preparando-o para um novo cadastro.
        private void LimparFormulario()
        {
            hdnBebidaId.Value = ""; // Limpa o ID (indica novo registro)
            txtNome.Text = "";
            txtDescricao.Text = "";
            ddlCategoria.SelectedIndex = 0; // Seleciona o primeiro item
            txtPreco.Text = "";
            txtImagemUrl.Text = "";
            txtEstoque.Text = "0";
            chkDisponivel.Checked = true;
        }

        // Evento do GridView (RowDataBound) - Vazio, mas disponível para customização de linhas.
        protected void gridBebidas_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            // Este evento pode ser usado para customizar a aparência de cada linha do GridView
            // com base nos dados. Atualmente, está sem implementação.
        }

        // Converte o texto do preço (que pode estar no formato brasileiro) para o tipo decimal.
        private decimal ConverterPreco(string valorTexto)
        {
            if (string.IsNullOrWhiteSpace(valorTexto))
                return 0;

            valorTexto = valorTexto.Trim();

            // Remove espaços, "R$" e "$" do texto.
            valorTexto = valorTexto.Replace(" ", "").Replace("R$", "").Replace("$", "");

            // Se o texto contém vírgula (formato brasileiro '1.000,50'),
            // remove o ponto (separador de milhares) e troca a vírgula por ponto (separador decimal)
            // para se adequar ao formato culture-invariant (padrão americano).
            if (valorTexto.Contains(","))
            {
                valorTexto = valorTexto.Replace(".", "").Replace(",", ".");
            }

            decimal resultado;
            // Tenta fazer o parse do valor usando a cultura InvariantCulture, que espera o ponto como separador decimal.
            if (decimal.TryParse(valorTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out resultado))
            {
                return resultado;
            }

            // Se a conversão falhar, lança uma exceção.
            throw new FormatException($"Valor de preço inválido: {valorTexto}");
        }
    }
}