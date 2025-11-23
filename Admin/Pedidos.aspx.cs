using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.DataAccess;
using FastPizza.Models;

namespace FastPizza.Admin
{
    // Página administrativa de gerenciamento de pedidos - permite visualizar, alterar status, editar, cancelar e excluir pedidos
    public partial class Pedidos : System.Web.UI.Page
    {
        // Carrega a página e verifica autenticação do administrador
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

                if (!IsPostBack)
                {
                    CarregarPedidos();

                    // Verifica se há parâmetro de pedido na URL para abrir detalhes automaticamente
                    string pedidoIdParam = Request.QueryString["pedidoId"];
                    if (!string.IsNullOrEmpty(pedidoIdParam))
                    {
                        int pedidoId;
                        if (int.TryParse(pedidoIdParam, out pedidoId))
                        {
                            MostrarDetalhes(pedidoId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Admin/Pedidos: {ex.Message}");
                Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
            }
        }

        // Carrega todos os pedidos do banco e exibe no GridView
        private void CarregarPedidos()
        {
            gridPedidos.DataSource = PedidoDAO.ObterTodos();
            gridPedidos.DataBind();
        }

        // Processa comandos dos botões do GridView (Detalhes, AlterarStatus, Cancelar, Excluir, Editar)
        protected void gridPedidos_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Detalhes")
            {
                MostrarDetalhes(id);
            }
            else if (e.CommandName == "AlterarStatus")
            {
                MostrarAlterarStatus(id);
            }
            else if (e.CommandName == "Cancelar")
            {
                MostrarCancelar(id);
            }
            else if (e.CommandName == "Excluir")
            {
                ExcluirPedido(id);
            }
            else if (e.CommandName == "Editar")
            {
                MostrarEditar(id);
            }
        }

        // Exibe modal com detalhes completos do pedido (cliente, data, status, itens, observações)
        private void MostrarDetalhes(int id)
        {
            var pedido = PedidoDAO.ObterPorId(id);
            if (pedido != null)
            {
                hdnPedidoId.Value = pedido.Id.ToString();
                lblCliente.Text = pedido.NomeCliente;
                lblDataPedido.Text = pedido.DataPedido.ToString("dd/MM/yyyy HH:mm");
                lblStatusAtual.Text = GetStatusTexto(pedido.Status);
                lblTotal.Text = "R$ " + pedido.Total.ToString("F2");

                // Exibe observações se existirem
                if (!string.IsNullOrWhiteSpace(pedido.Observacoes))
                {
                    lblObservacoes.Text = pedido.Observacoes;
                    pnlObservacoes.Visible = true;
                }
                else
                {
                    pnlObservacoes.Visible = false;
                }

                // Carrega itens do pedido no repeater
                if (pedido.Itens != null && pedido.Itens.Count > 0)
                {
                    var itensList = pedido.Itens.Select(i => new
                    {
                        NomeProduto = i.NomeProduto,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario,
                        Subtotal = i.Subtotal
                    }).ToList();

                    rptItens.DataSource = itensList;
                    rptItens.DataBind();
                    phItensVazios.Visible = false;
                }
                else
                {
                    // Exibe mensagem se não há itens
                    rptItens.DataSource = null;
                    rptItens.DataBind();
                    phItensVazios.Visible = true;
                }

                // Configura visibilidade dos painéis do modal
                pnlDetalhes.Visible = true;
                pnlAlterarStatus.Visible = false;
                pnlCancelar.Visible = false;
                btnSalvarStatus.Visible = false;
                btnCancelarPedido.Visible = false;

                lblModalTitulo.Text = "Detalhes do Pedido";
                modalPedido.Style["display"] = "block";
            }
        }

        // Exibe modal para alterar status do pedido (não permite se já cancelado)
        private void MostrarAlterarStatus(int id)
        {
            var pedido = PedidoDAO.ObterPorId(id);
            if (pedido != null && pedido.Status != StatusPedido.Cancelado)
            {
                hdnPedidoId.Value = pedido.Id.ToString();
                ddlStatus.SelectedValue = ((int)pedido.Status).ToString();

                pnlDetalhes.Visible = false;
                pnlAlterarStatus.Visible = true;
                pnlCancelar.Visible = false;
                btnSalvarStatus.Visible = true;
                btnCancelarPedido.Visible = false;

                lblModalTitulo.Text = "Alterar Status do Pedido";
                modalPedido.Style["display"] = "block";
            }
        }

        // Exibe modal para cancelar pedido (não permite se já cancelado ou entregue)
        private void MostrarCancelar(int id)
        {
            var pedido = PedidoDAO.ObterPorId(id);
            if (pedido != null && pedido.Status != StatusPedido.Cancelado && pedido.Status != StatusPedido.Entregue)
            {
                hdnPedidoId.Value = pedido.Id.ToString();

                pnlDetalhes.Visible = false;
                pnlAlterarStatus.Visible = false;
                pnlCancelar.Visible = true;
                btnSalvarStatus.Visible = false;
                btnCancelarPedido.Visible = true;

                lblModalTitulo.Text = "Cancelar Pedido";
                modalPedido.Style["display"] = "block";
            }
        }

        // Salva o novo status do pedido no banco
        protected void btnSalvarStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(hdnPedidoId.Value) || string.IsNullOrEmpty(ddlStatus.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ErroStatus",
                        "setTimeout(function() { mostrarFeedback('Erro ao alterar status. Tente novamente.', 'danger'); }, 100);", true);
                    return;
                }

                int id = Convert.ToInt32(hdnPedidoId.Value);
                StatusPedido novoStatus = (StatusPedido)Convert.ToInt32(ddlStatus.SelectedValue);

                PedidoDAO.AtualizarStatus(id, novoStatus);

                modalPedido.Style["display"] = "none";
                CarregarPedidos();

                ClientScript.RegisterStartupScript(this.GetType(), "StatusAlterado",
                    "mostrarFeedback('Status do pedido alterado com sucesso!', 'success');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar status: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                    "mostrarFeedback('Erro ao alterar status do pedido. Tente novamente.', 'danger');", true);
            }
        }

        // Cancela o pedido com motivo informado
        protected void btnCancelarPedido_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(hdnPedidoId.Value);
            string motivo = txtMotivoCancelamento.Text.Trim();

            // Valida se motivo foi informado
            if (string.IsNullOrEmpty(motivo))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "MotivoObrigatorio",
                    "setTimeout(function() { mostrarFeedback('Por favor, informe o motivo do cancelamento.', 'danger'); }, 100);", true);
                return;
            }

            PedidoDAO.AtualizarStatus(id, StatusPedido.Cancelado, motivo);

            modalPedido.Style["display"] = "none";
            CarregarPedidos();

            ClientScript.RegisterStartupScript(this.GetType(), "Sucesso",
                "mostrarFeedback('Pedido cancelado com sucesso!', 'success');", true);
        }

        // Fecha o modal e limpa dados de edição
        protected void btnFechar_Click(object sender, EventArgs e)
        {
            modalPedido.Style["display"] = "none";
            ViewState["ItensEdicao"] = null;
        }

        // Exclui pedido permanentemente e notifica cliente
        private void ExcluirPedido(int id)
        {
            try
            {
                var pedido = PedidoDAO.ObterPorId(id);

                if (pedido != null)
                {
                    int clienteId = pedido.ClienteId;
                    int pedidoId = pedido.Id;

                    PedidoDAO.ExcluirPermanentemente(id);

                    // Cria notificação para o cliente informando exclusão
                    CriarNotificacaoCliente(clienteId,
                        $"Seu pedido foi removido permanentemente pelo administrador. Entre em contato conosco se tiver dúvidas.");

                    CarregarPedidos();

                    ClientScript.RegisterStartupScript(this.GetType(), "Sucesso",
                        "mostrarFeedback('Pedido excluído permanentemente com sucesso! O cliente será notificado.', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                        "mostrarFeedback('Pedido não encontrado.', 'danger');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao excluir pedido: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                    "mostrarFeedback('Erro ao excluir pedido. Tente novamente.', 'danger');", true);
            }
        }

        // Cria notificação no Application State para o cliente (máximo 50 notificações)
        private void CriarNotificacaoCliente(int clienteId, string mensagem)
        {
            try
            {
                string chaveNotificacoes = $"Notificacoes_{clienteId}";

                if (Application[chaveNotificacoes] == null)
                {
                    Application[chaveNotificacoes] = new List<string>();
                }

                var notificacoes = (List<string>)Application[chaveNotificacoes];
                notificacoes.Add($"{DateTime.Now:dd/MM/yyyy HH:mm} - {mensagem}");

                // Limita a 50 notificações (remove a mais antiga)
                if (notificacoes.Count > 50)
                {
                    notificacoes.RemoveAt(0);
                }

                Application[chaveNotificacoes] = notificacoes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar notificação: {ex.Message}");
            }
        }

        // Retorna classe CSS conforme o status do pedido (para estilização)
        public string GetStatusClass(object status)
        {
            if (status == null) return "";
            var statusEnum = (StatusPedido)status;
            switch (statusEnum)
            {
                case StatusPedido.Pendente: return "pendente";
                case StatusPedido.EmPreparo: return "preparo";
                case StatusPedido.SaiuParaEntrega: return "entrega";
                case StatusPedido.Entregue: return "entregue";
                case StatusPedido.Cancelado: return "cancelado";
                default: return "";
            }
        }

        // Retorna texto legível do status do pedido
        public string GetStatusTexto(object status)
        {
            if (status == null) return "";
            var statusEnum = (StatusPedido)status;
            switch (statusEnum)
            {
                case StatusPedido.Pendente: return "Pendente";
                case StatusPedido.EmPreparo: return "Em Preparo";
                case StatusPedido.SaiuParaEntrega: return "Saiu para Entrega";
                case StatusPedido.Entregue: return "Entregue";
                case StatusPedido.Cancelado: return "Cancelado";
                default: return "";
            }
        }

        // Controla visibilidade dos botões de ação conforme status do pedido
        protected void gridPedidos_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                var btnCancelar = e.Row.FindControl("btnCancelar") as System.Web.UI.WebControls.LinkButton;
                var btnAlterarStatus = e.Row.FindControl("btnAlterarStatus") as System.Web.UI.WebControls.LinkButton;
                var btnEditar = e.Row.FindControl("btnEditar") as System.Web.UI.WebControls.LinkButton;
                var btnExcluir = e.Row.FindControl("btnExcluir") as System.Web.UI.WebControls.LinkButton;

                if (btnCancelar != null && btnAlterarStatus != null && btnEditar != null && btnExcluir != null)
                {
                    var pedido = e.Row.DataItem as Pedido;
                    if (pedido != null)
                    {
                        // Oculta botões de cancelar, alterar status e editar se pedido já foi cancelado ou entregue
                        if (pedido.Status == StatusPedido.Cancelado || pedido.Status == StatusPedido.Entregue)
                        {
                            btnCancelar.Visible = false;
                            btnAlterarStatus.Visible = false;
                            btnEditar.Visible = false;
                        }
                    }
                }
            }
        }

        // Exibe modal de edição do pedido (permite adicionar/remover itens e alterar quantidades)
        private void MostrarEditar(int id)
        {
            var pedido = PedidoDAO.ObterPorId(id);
            if (pedido != null && pedido.Status != StatusPedido.Cancelado && pedido.Status != StatusPedido.Entregue)
            {
                hdnPedidoId.Value = pedido.Id.ToString();

                CarregarProdutosDropdown();
                CarregarItensParaEdicao(pedido);

                pnlDetalhes.Visible = false;
                pnlAlterarStatus.Visible = false;
                pnlCancelar.Visible = false;
                pnlEditarPedido.Visible = true;
                btnSalvarEdicao.Visible = true;
                btnSalvarStatus.Visible = false;
                btnCancelarPedido.Visible = false;

                lblModalTitulo.Text = "Editar Pedido";
                modalPedido.Style["display"] = "block";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ErroEdicao",
                    "mostrarFeedback('Não é possível editar pedidos cancelados ou entregues.', 'danger');", true);
            }
        }

        // Carrega produtos disponíveis no dropdown para adicionar ao pedido
        private void CarregarProdutosDropdown()
        {
            var produtos = ProdutoDAO.ObterDisponiveis();
            ddlProdutos.Items.Clear();
            ddlProdutos.Items.Add(new System.Web.UI.WebControls.ListItem("Selecione um produto...", "0"));

            foreach (var produto in produtos)
            {
                ddlProdutos.Items.Add(new System.Web.UI.WebControls.ListItem(
                    $"{produto.Nome} - R$ {produto.Preco:F2}",
                    produto.Id.ToString()));
            }
        }

        // Carrega itens do pedido no ViewState para edição
        private void CarregarItensParaEdicao(Pedido pedido)
        {
            if (pedido.Itens != null && pedido.Itens.Count > 0)
            {
                var itensEdicao = pedido.Itens.Select(i => new ItemEdicao
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.NomeProduto,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList();

                ViewState["ItensEdicao"] = itensEdicao;
                CarregarItensDoViewState();
            }
            else
            {
                ViewState["ItensEdicao"] = new List<ItemEdicao>();
                rptItensEdicao.DataSource = null;
                rptItensEdicao.DataBind();
                lblTotalEdicao.Text = "R$ 0,00";
            }
        }

        // Adiciona novo item ao pedido em edição
        protected void btnAdicionarItem_Click(object sender, EventArgs e)
        {
            try
            {
                int produtoId = Convert.ToInt32(ddlProdutos.SelectedValue);
                if (produtoId == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ErroProduto",
                        "mostrarFeedback('Por favor, selecione um produto.', 'danger');", true);
                    return;
                }

                int quantidade = Convert.ToInt32(txtQuantidadeNovo.Text);
                if (quantidade <= 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ErroQuantidade",
                        "mostrarFeedback('A quantidade deve ser maior que zero.', 'danger');", true);
                    return;
                }

                var produto = ProdutoDAO.ObterPorId(produtoId);
                if (produto == null)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ErroProdutoNaoEncontrado",
                        "mostrarFeedback('Produto não encontrado.', 'danger');", true);
                    return;
                }

                AdicionarItemAoRepeater(produtoId, produto.Nome, produto.Preco, quantidade);

                // Reseta campos após adicionar
                ddlProdutos.SelectedIndex = 0;
                txtQuantidadeNovo.Text = "1";

                CalcularTotalEdicao();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar item: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                    "mostrarFeedback('Erro ao adicionar item. Tente novamente.', 'danger');", true);
            }
        }

        // Adiciona item ao repeater de edição (ou incrementa quantidade se já existir)
        private void AdicionarItemAoRepeater(int produtoId, string nome, decimal preco, int quantidade)
        {
            List<ItemEdicao> itensAtuais = ViewState["ItensEdicao"] as List<ItemEdicao>;
            if (itensAtuais == null)
            {
                itensAtuais = new List<ItemEdicao>();

                // Recupera itens do repeater se ViewState estiver vazio
                foreach (System.Web.UI.WebControls.RepeaterItem item in rptItensEdicao.Items)
                {
                    var hdnItemId = item.FindControl("hdnItemId") as System.Web.UI.WebControls.HiddenField;
                    var hdnItemNome = item.FindControl("hdnItemNome") as System.Web.UI.WebControls.HiddenField;
                    var hdnItemPreco = item.FindControl("hdnItemPreco") as System.Web.UI.WebControls.HiddenField;
                    var txtQtd = item.FindControl("txtQuantidade") as System.Web.UI.WebControls.TextBox;

                    if (hdnItemId != null && hdnItemNome != null && hdnItemPreco != null && txtQtd != null)
                    {
                        int qtd = 0;
                        int.TryParse(txtQtd.Text, out qtd);
                        itensAtuais.Add(new ItemEdicao
                        {
                            ProdutoId = Convert.ToInt32(hdnItemId.Value),
                            NomeProduto = hdnItemNome.Value,
                            Quantidade = qtd,
                            PrecoUnitario = Convert.ToDecimal(hdnItemPreco.Value)
                        });
                    }
                }
            }

            // Verifica se produto já existe na lista
            var itemExistente = itensAtuais.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (itemExistente != null)
            {
                // Incrementa quantidade se já existir
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                // Adiciona novo item
                itensAtuais.Add(new ItemEdicao
                {
                    ProdutoId = produtoId,
                    NomeProduto = nome,
                    Quantidade = quantidade,
                    PrecoUnitario = preco
                });
            }

            ViewState["ItensEdicao"] = itensAtuais;
            CarregarItensDoViewState();
        }

        // Carrega itens do ViewState no repeater de edição
        private void CarregarItensDoViewState()
        {
            List<ItemEdicao> itens = ViewState["ItensEdicao"] as List<ItemEdicao>;
            if (itens != null && itens.Count > 0)
            {
                var itensList = itens.Select(i => new
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.NomeProduto,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Quantidade * i.PrecoUnitario
                }).ToList();

                rptItensEdicao.DataSource = itensList;
                rptItensEdicao.DataBind();
                CalcularTotalEdicao();
            }
        }

        // Classe auxiliar para armazenar itens em edição no ViewState
        [System.Serializable]
        private class ItemEdicao
        {
            public int ProdutoId { get; set; }
            public string NomeProduto { get; set; }
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }
        }

        // Processa comando de remover item do repeater de edição
        protected void rptItensEdicao_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remover")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                List<ItemEdicao> itens = ViewState["ItensEdicao"] as List<ItemEdicao>;
                if (itens != null && index >= 0 && index < itens.Count)
                {
                    itens.RemoveAt(index);
                    ViewState["ItensEdicao"] = itens;
                    CarregarItensDoViewState();
                }
            }
        }

        // Calcula total do pedido em edição somando subtotais dos itens
        private void CalcularTotalEdicao()
        {
            decimal total = 0;

            foreach (System.Web.UI.WebControls.RepeaterItem item in rptItensEdicao.Items)
            {
                var hdnItemPreco = item.FindControl("hdnItemPreco") as System.Web.UI.WebControls.HiddenField;
                var txtQtd = item.FindControl("txtQuantidade") as System.Web.UI.WebControls.TextBox;

                if (hdnItemPreco != null && txtQtd != null)
                {
                    int quantidade = 0;
                    int.TryParse(txtQtd.Text, out quantidade);
                    decimal preco = Convert.ToDecimal(hdnItemPreco.Value);
                    decimal subtotal = quantidade * preco;
                    total += subtotal;
                }
            }

            lblTotalEdicao.Text = "R$ " + total.ToString("F2");

            // Chama função JavaScript para atualizar total na interface
            ClientScript.RegisterStartupScript(this.GetType(), "CalcularTotal",
                "setTimeout(function() { calcularTotalEdicao(); }, 100);", true);
        }

        // Salva alterações do pedido editado no banco
        protected void btnSalvarEdicao_Click(object sender, EventArgs e)
        {
            try
            {
                // Atualiza ViewState com valores atuais do repeater
                AtualizarViewStateDoRepeater();

                if (string.IsNullOrEmpty(hdnPedidoId.Value))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                        "mostrarFeedback('Erro ao salvar. Tente novamente.', 'danger');", true);
                    return;
                }

                int pedidoId = Convert.ToInt32(hdnPedidoId.Value);

                var novosItens = new List<ItemPedido>();
                List<ItemEdicao> itensEdicao = ViewState["ItensEdicao"] as List<ItemEdicao>;

                if (itensEdicao != null && itensEdicao.Count > 0)
                {
                    // Atualiza quantidades dos itens do ViewState com valores do repeater
                    foreach (System.Web.UI.WebControls.RepeaterItem item in rptItensEdicao.Items)
                    {
                        var hdnItemId = item.FindControl("hdnItemId") as System.Web.UI.WebControls.HiddenField;
                        var txtQtd = item.FindControl("txtQuantidade") as System.Web.UI.WebControls.TextBox;

                        if (hdnItemId != null && txtQtd != null)
                        {
                            int produtoId = Convert.ToInt32(hdnItemId.Value);
                            int quantidade = 0;
                            int.TryParse(txtQtd.Text, out quantidade);

                            var itemEdicao = itensEdicao.FirstOrDefault(i => i.ProdutoId == produtoId);
                            if (itemEdicao != null)
                            {
                                itemEdicao.Quantidade = quantidade;
                            }
                        }
                    }

                    // Converte itens de edição para itens de pedido
                    foreach (var item in itensEdicao)
                    {
                        if (item.Quantidade > 0)
                        {
                            novosItens.Add(new ItemPedido
                            {
                                ProdutoId = item.ProdutoId,
                                NomeProduto = item.NomeProduto,
                                Quantidade = item.Quantidade,
                                PrecoUnitario = item.PrecoUnitario
                            });
                        }
                    }
                }
                else
                {
                    // Fallback: recupera itens diretamente do repeater se ViewState vazio
                    foreach (System.Web.UI.WebControls.RepeaterItem item in rptItensEdicao.Items)
                    {
                        var hdnItemId = item.FindControl("hdnItemId") as System.Web.UI.WebControls.HiddenField;
                        var hdnItemNome = item.FindControl("hdnItemNome") as System.Web.UI.WebControls.HiddenField;
                        var hdnItemPreco = item.FindControl("hdnItemPreco") as System.Web.UI.WebControls.HiddenField;
                        var txtQtd = item.FindControl("txtQuantidade") as System.Web.UI.WebControls.TextBox;

                        if (hdnItemId != null && hdnItemNome != null && hdnItemPreco != null && txtQtd != null)
                        {
                            int quantidade = 0;
                            int.TryParse(txtQtd.Text, out quantidade);
                            if (quantidade > 0)
                            {
                                novosItens.Add(new ItemPedido
                                {
                                    ProdutoId = Convert.ToInt32(hdnItemId.Value),
                                    NomeProduto = hdnItemNome.Value,
                                    Quantidade = quantidade,
                                    PrecoUnitario = Convert.ToDecimal(hdnItemPreco.Value)
                                });
                            }
                        }
                    }
                }

                // Valida se há pelo menos um item
                if (novosItens.Count == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ErroItens",
                        "mostrarFeedback('O pedido deve ter pelo menos um item.', 'danger');", true);
                    return;
                }

                // Atualiza itens do pedido no banco
                PedidoDAO.AtualizarItens(pedidoId, novosItens);

                modalPedido.Style["display"] = "none";
                CarregarPedidos();

                ClientScript.RegisterStartupScript(this.GetType(), "Sucesso",
                    "mostrarFeedback('Pedido atualizado com sucesso!', 'success');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar edição: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                    "mostrarFeedback('Erro ao salvar alterações. Tente novamente.', 'danger');", true);
            }
        }

        // Atualiza o ViewState com as quantidades atuais dos itens no repeater
        private void AtualizarViewStateDoRepeater()
        {
            List<ItemEdicao> itens = ViewState["ItensEdicao"] as List<ItemEdicao>;
            if (itens == null) return;

            // Percorre cada item do repeater e atualiza quantidade no ViewState
            foreach (System.Web.UI.WebControls.RepeaterItem item in rptItensEdicao.Items)
            {
                var hdnItemId = item.FindControl("hdnItemId") as System.Web.UI.WebControls.HiddenField;
                var txtQtd = item.FindControl("txtQuantidade") as System.Web.UI.WebControls.TextBox;

                if (hdnItemId != null && txtQtd != null)
                {
                    int produtoId = Convert.ToInt32(hdnItemId.Value);
                    int quantidade = 0;
                    int.TryParse(txtQtd.Text, out quantidade);

                    var itemEdicao = itens.FirstOrDefault(i => i.ProdutoId == produtoId);
                    if (itemEdicao != null)
                    {
                        itemEdicao.Quantidade = quantidade;
                    }
                }
            }

            ViewState["ItensEdicao"] = itens;
        }
    }
}