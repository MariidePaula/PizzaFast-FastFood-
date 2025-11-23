using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.DataAccess;
using FastPizza.Models;
using FastPizza.Business;

namespace FastPizza.Admin
{
    // Página administrativa para gerenciar clientes
    public partial class Clientes : System.Web.UI.Page
    {
        // Verifica autenticação de admin e carrega lista de clientes
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Redireciona para login se não for admin
                if (Session["AdminLogado"] == null || !(bool)Session["AdminLogado"])
                {
                    Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
                    return;
                }

                if (!IsPostBack)
                {
                    CarregarClientes();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Admin/Clientes: {ex.Message}");
                Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
            }
        }

        // Carrega todos os clientes no GridView
        private void CarregarClientes()
        {
            gridClientes.DataSource = ClienteDAO.ObterTodos();
            gridClientes.DataBind();
        }

        // Processa ações do GridView (Editar, Bloquear, Excluir)
        protected void gridClientes_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Editar")
            {
                EditarCliente(id);
            }
            else if (e.CommandName == "Bloquear")
            {
                BloquearDesbloquearCliente(id);
            }
            else if (e.CommandName == "Excluir")
            {
                ExcluirCliente(id);
            }
        }

        // Carrega dados do cliente no formulário de edição e abre modal
        private void EditarCliente(int id)
        {
            var cliente = ClienteDAO.ObterPorId(id);
            if (cliente != null)
            {
                // Preenche campos do formulário
                hdnClienteId.Value = cliente.Id.ToString();
                txtNome.Text = cliente.Nome;
                txtEmail.Text = cliente.Email;
                txtTelefone.Text = cliente.Telefone;
                txtEndereco.Text = cliente.Endereco;
                txtCidade.Text = cliente.Cidade;
                txtCEP.Text = cliente.CEP;

                // Carrega histórico de pedidos do cliente
                CarregarPedidosCliente(id);

                modalCliente.Style["display"] = "block";
            }
        }

        // Carrega pedidos do cliente ordenados por data (mais recentes primeiro)
        private void CarregarPedidosCliente(int clienteId)
        {
            var pedidos = PedidoDAO.ObterPorCliente(clienteId);
            var listaPedidos = pedidos != null ? pedidos.OrderByDescending(p => p.DataPedido).ToList() : new List<Pedido>();

            if (listaPedidos.Count > 0)
            {
                // Exibe lista de pedidos
                rptPedidosCliente.DataSource = listaPedidos;
                rptPedidosCliente.DataBind();
                phPedidosVazios.Visible = false;
            }
            else
            {
                // Exibe mensagem se não houver pedidos
                rptPedidosCliente.DataSource = null;
                rptPedidosCliente.DataBind();
                phPedidosVazios.Visible = true;
            }
        }

        protected void rptPedidosCliente_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int pedidoId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarPedido")
            {
                EditarPedido(pedidoId);
            }
            else if (e.CommandName == "VerDetalhes")
            {
                VerDetalhesPedido(pedidoId);
            }
        }

        private void EditarPedido(int pedidoId)
        {
            var pedido = PedidoDAO.ObterPorId(pedidoId);
            if (pedido != null)
            {
                hdnPedidoId.Value = pedido.Id.ToString();
                ddlStatusPedido.SelectedValue = ((int)pedido.Status).ToString();
                txtTotalPedido.Text = pedido.Total.ToString("F2");
                txtMotivoCancelamento.Text = pedido.MotivoCancelamento ?? "";

                modalPedido.Style["display"] = "block";
            }
        }

        private void VerDetalhesPedido(int pedidoId)
        {

            Response.Redirect(ResolveUrl($"~/Admin/Pedidos.aspx?pedidoId={pedidoId}"));
        }

        // Atualiza status e total de um pedido (usado no modal de edição)
        protected void btnSalvarPedido_Click(object sender, EventArgs e)
        {
            try
            {
                int pedidoId = Convert.ToInt32(hdnPedidoId.Value);
                var pedido = PedidoDAO.ObterPorId(pedidoId);

                if (pedido != null)
                {
                    // Atualiza status do pedido
                    StatusPedido novoStatus = (StatusPedido)Convert.ToInt32(ddlStatusPedido.SelectedValue);
                    string motivo = txtMotivoCancelamento.Text.Trim();

                    // Se cancelado sem motivo, define motivo padrão
                    if (novoStatus == StatusPedido.Cancelado && string.IsNullOrEmpty(motivo))
                    {
                        motivo = "Cancelado pelo administrador";
                    }

                    PedidoDAO.AtualizarStatus(pedidoId, novoStatus, motivo);

                    // Atualiza total se foi alterado
                    decimal novoTotal = Convert.ToDecimal(txtTotalPedido.Text);
                    if (novoTotal != pedido.Total)
                    {
                        AtualizarTotalPedido(pedidoId, novoTotal);
                    }

                    // Recarrega lista de pedidos do cliente
                    int clienteId = Convert.ToInt32(hdnClienteId.Value);
                    CarregarPedidosCliente(clienteId);

                    modalPedido.Style["display"] = "none";

                    ClientScript.RegisterStartupScript(this.GetType(), "Sucesso",
                        "mostrarFeedback('Pedido atualizado com sucesso!', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar pedido: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "Erro",
                    "mostrarFeedback('Erro ao atualizar pedido. Tente novamente.', 'danger');", true);
            }
        }

        // Atualiza total do pedido diretamente no banco (ajuste manual pelo admin)
        private void AtualizarTotalPedido(int pedidoId, decimal novoTotal)
        {
            using (var context = new FastPizzaDbContext())
            {
                var pedido = context.Pedidos.Find(pedidoId);
                if (pedido != null)
                {
                    pedido.Total = novoTotal;
                    context.SaveChanges();
                }
            }
        }

        protected void btnCancelarPedido_Click(object sender, EventArgs e)
        {
            modalPedido.Style["display"] = "none";
        }

        public string GetStatusClass(object status)
        {
            if (status == null) return "badge-secondary";

            StatusPedido statusPedido = (StatusPedido)status;
            switch (statusPedido)
            {
                case StatusPedido.Pendente:
                    return "badge-warning";
                case StatusPedido.EmPreparo:
                    return "badge-info";
                case StatusPedido.SaiuParaEntrega:
                    return "badge-primary";
                case StatusPedido.Entregue:
                    return "badge-success";
                case StatusPedido.Cancelado:
                    return "badge-danger";
                default:
                    return "badge-secondary";
            }
        }

        public string GetStatusTexto(object status)
        {
            if (status == null) return "Desconhecido";

            StatusPedido statusPedido = (StatusPedido)status;
            switch (statusPedido)
            {
                case StatusPedido.Pendente:
                    return "Pendente";
                case StatusPedido.EmPreparo:
                    return "Em Preparo";
                case StatusPedido.SaiuParaEntrega:
                    return "Saiu para Entrega";
                case StatusPedido.Entregue:
                    return "Entregue";
                case StatusPedido.Cancelado:
                    return "Cancelado";
                default:
                    return "Desconhecido";
            }
        }

        // Bloqueia ou desbloqueia cliente (alterna status atual)
        // Cliente bloqueado não pode fazer login nem adicionar itens ao carrinho
        private void BloquearDesbloquearCliente(int id)
        {
            try
            {
                var cliente = ClienteDAO.ObterPorId(id);
                if (cliente != null)
                {
                    // Inverte status de bloqueio atual
                    bool novoStatus = !cliente.Bloqueado;
                    ClienteDAO.Bloquear(id, novoStatus);
                    CarregarClientes();

                    string acao = novoStatus ? "bloqueado" : "desbloqueado";
                    ClientScript.RegisterStartupScript(this.GetType(), "ClienteBloqueado",
                        $"mostrarFeedback('Cliente {acao} com sucesso!', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao bloquear/desbloquear cliente: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "ErroBloquear",
                    "mostrarFeedback('Erro ao alterar status do cliente. Tente novamente.', 'danger');", true);
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            var cliente = new Cliente();
            cliente.Id = Convert.ToInt32(hdnClienteId.Value);
            cliente.Nome = txtNome.Text;
            cliente.Email = txtEmail.Text;
            cliente.Telefone = txtTelefone.Text;
            cliente.Endereco = txtEndereco.Text;
            cliente.Cidade = txtCidade.Text;
            cliente.CEP = txtCEP.Text;

            var clienteExistente = ClienteDAO.ObterPorId(cliente.Id);
            if (clienteExistente != null)
            {
                cliente.Bloqueado = clienteExistente.Bloqueado;
                cliente.DataCadastro = clienteExistente.DataCadastro;
            }

            ClienteDAO.Atualizar(cliente);

            modalCliente.Style["display"] = "none";
            CarregarClientes();

            ClientScript.RegisterStartupScript(this.GetType(), "Sucesso",
                "mostrarFeedback('Cliente atualizado com sucesso!', 'success');", true);
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            modalCliente.Style["display"] = "none";
        }

        private void ExcluirCliente(int id)
        {
            try
            {
                var cliente = ClienteDAO.ObterPorId(id);
                if (cliente != null)
                {
                    string nomeCliente = cliente.Nome;
                    ClienteDAO.Excluir(id);
                    CarregarClientes();

                    ClientScript.RegisterStartupScript(this.GetType(), "ClienteExcluido",
                        $"mostrarFeedback('Cliente {nomeCliente} excluído permanentemente do sistema!', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao excluir cliente: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "ErroExcluir",
                    "mostrarFeedback('Erro ao excluir cliente. Tente novamente.', 'danger');", true);
            }
        }
    }
}

