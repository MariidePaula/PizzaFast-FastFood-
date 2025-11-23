using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.DataAccess;
using FastPizza.Models;

namespace FastPizza
{
    // Página que exibe histórico de pedidos do cliente logado
    public partial class MeusPedidos : System.Web.UI.Page
    {
        // Verifica autenticação e carrega pedidos do cliente
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

                // Inicializa visibilidade dos painéis
                pnlSemPedidos.Visible = false;
                pnlComPedidos.Visible = false;
                lblTitulo.Visible = true;

                if (!IsPostBack)
                {
                    // Exibe mensagem de sucesso se veio da página de carrinho
                    if (Request.QueryString["pedidoSucesso"] == "true")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "PedidoSucesso",
                            "setTimeout(function() { mostrarFeedback('Pedido finalizado com sucesso!', 'success'); }, 300);", true);
                    }

                    CarregarPedidos();
                    VerificarNotificacoes();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load MeusPedidos: {ex.Message}");
            }
        }

        // Carrega pedidos do cliente logado e controla exibição de painéis
        private void CarregarPedidos()
        {
            try
            {
                bool isAdmin = Session["AdminLogado"] != null && (bool)Session["AdminLogado"];
                bool isCliente = Session["ClienteId"] != null;

                // Admin sem sessão de cliente: redireciona para painel admin
                if (isAdmin && !isCliente)
                {
                    Response.Redirect(ResolveUrl("~/Admin/Pedidos.aspx"));
                    return;
                }

                // Não autenticado: redireciona para login
                if (!isCliente)
                {
                    Response.Redirect(ResolveUrl("~/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.Url.PathAndQuery)));
                    return;
                }

                // Busca todos os pedidos e filtra pelo cliente logado
                var todosPedidos = PedidoDAO.ObterTodos();

                int clienteId = (int)Session["ClienteId"];
                var pedidosCliente = todosPedidos != null ? todosPedidos.Where(p => p.ClienteId == clienteId).ToList() : new List<Pedido>();

                // Controla exibição: mensagem vazia ou lista de pedidos
                if (pedidosCliente == null || pedidosCliente.Count == 0)
                {
                    // Cliente sem pedidos: exibe mensagem personalizada
                    pnlSemPedidos.Visible = true;
                    pnlComPedidos.Visible = false;
                    lblTitulo.Visible = false;
                    System.Diagnostics.Debug.WriteLine("Cliente sem pedidos - exibindo mensagem personalizada");
                }
                else
                {
                    // Cliente com pedidos: exibe lista
                    pnlSemPedidos.Visible = false;
                    pnlComPedidos.Visible = true;
                    lblTitulo.Visible = true;
                    rptPedidos.DataSource = pedidosCliente;
                    rptPedidos.DataBind();
                    System.Diagnostics.Debug.WriteLine($"Cliente com {pedidosCliente.Count} pedidos");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar pedidos: {ex.Message}");

                // Em caso de erro, exibe mensagem vazia
                pnlSemPedidos.Visible = true;
                pnlComPedidos.Visible = false;
                lblTitulo.Visible = false;
            }
        }

        // Retorna classe CSS do badge de status (usado no Repeater)
        public string GetStatusBadgeClass(object status)
        {
            if (status == null) return "status-pendente";

            var statusEnum = (StatusPedido)status;
            switch (statusEnum)
            {
                case StatusPedido.Pendente:
                    return "status-pendente";
                case StatusPedido.EmPreparo:
                    return "status-preparo";
                case StatusPedido.SaiuParaEntrega:
                    return "status-entrega";
                case StatusPedido.Entregue:
                    return "status-entregue";
                case StatusPedido.Cancelado:
                    return "status-cancelado";
                default:
                    return "status-pendente";
            }
        }

        // Retorna texto legível do status do pedido
        public string GetStatusTexto(object status)
        {
            if (status == null) return "Pendente";

            var statusEnum = (StatusPedido)status;
            switch (statusEnum)
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
                    return "Pendente";
            }
        }

        // Retorna ícone Font Awesome correspondente ao status
        public string GetStatusIcon(object status)
        {
            if (status == null) return "fas fa-clock";

            var statusEnum = (StatusPedido)status;
            switch (statusEnum)
            {
                case StatusPedido.Pendente:
                    return "fas fa-clock";
                case StatusPedido.EmPreparo:
                    return "fas fa-utensils";
                case StatusPedido.SaiuParaEntrega:
                    return "fas fa-bicycle";
                case StatusPedido.Entregue:
                    return "fas fa-check-circle";
                case StatusPedido.Cancelado:
                    return "fas fa-times-circle";
                default:
                    return "fas fa-clock";
            }
        }

        // Formata data do pedido de forma amigável (Hoje, Ontem ou data completa)
        public string GetDataFormatada(object data)
        {
            if (data == null) return "";

            DateTime dataPedido = (DateTime)data;
            DateTime hoje = DateTime.Now.Date;
            DateTime ontem = hoje.AddDays(-1);

            // Formatação contextual baseada na data
            if (dataPedido.Date == hoje)
            {
                return "Hoje, " + dataPedido.ToString("HH:mm");
            }
            else if (dataPedido.Date == ontem)
            {
                return "Ontem, " + dataPedido.ToString("HH:mm");
            }
            else
            {
                return dataPedido.ToString("dd/MM/yyyy, HH:mm");
            }
        }

        // Verifica se deve exibir motivo de cancelamento (apenas para pedidos cancelados)
        public bool MostrarMotivoCancelamento(object status, object motivoCancelamento)
        {
            if (status == null) return false;

            var statusEnum = (StatusPedido)status;
            if (statusEnum == StatusPedido.Cancelado)
            {
                return true;
            }

            return false;
        }

        // Retorna texto do motivo de cancelamento ou mensagem padrão
        public string GetMotivoCancelamentoTexto(object motivoCancelamento)
        {
            if (motivoCancelamento == null || string.IsNullOrWhiteSpace(motivoCancelamento.ToString()))
            {
                return "Não informado";
            }

            return motivoCancelamento.ToString();
        }

        // Verifica e exibe notificações do cliente armazenadas em Application
        // Usado para notificar sobre mudanças de status de pedidos
        private void VerificarNotificacoes()
        {
            try
            {
                int clienteId = (int)Session["ClienteId"];
                string chaveNotificacoes = $"Notificacoes_{clienteId}";

                if (Application[chaveNotificacoes] != null)
                {
                    var notificacoes = (List<string>)Application[chaveNotificacoes];

                    if (notificacoes != null && notificacoes.Count > 0)
                    {
                        // Pega última notificação e limpa lista
                        string ultimaNotificacao = notificacoes[notificacoes.Count - 1];

                        notificacoes.Clear();
                        Application[chaveNotificacoes] = notificacoes;

                        // Exibe notificação no painel e toast
                        lblNotificacao.Text = $"<i class='fas fa-exclamation-triangle me-2'></i><strong>Atenção:</strong> {ultimaNotificacao}";
                        pnlNotificacoes.Visible = true;

                        ClientScript.RegisterStartupScript(this.GetType(), "Notificacao",
                            $"mostrarFeedback('{ultimaNotificacao.Replace("'", "\\'")}', 'danger');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar notificações: {ex.Message}");
            }
        }
    }
}

