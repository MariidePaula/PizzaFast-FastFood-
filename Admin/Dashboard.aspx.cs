using System;
using System.Linq;
using FastPizza.DataAccess;

namespace FastPizza.Admin
{
    // Painel de controle do administrador com estatísticas
    public partial class Dashboard : System.Web.UI.Page
    {
        // Verifica autenticação e carrega estatísticas do sistema
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

                // Sempre carrega estatísticas para garantir dados atualizados
                // Isso garante que produtos adicionados ou removidos sejam refletidos imediatamente
                CarregarEstatisticas();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Admin/Dashboard: {ex.Message}");

                Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
            }
        }

        // Carrega estatísticas gerais do sistema (produtos, pedidos pendentes, clientes)
        private void CarregarEstatisticas()
        {
            try
            {
                // Busca TODOS os produtos (pizzas) e bebidas do banco de dados, sem filtros
                var produtos = ProdutoDAO.ObterTodos();
                var bebidas = BebidaDAO.ObterTodos();
                var pedidos = PedidoDAO.ObterTodos();
                var clientes = ClienteDAO.ObterTodos();

                // Conta TODOS os produtos (pizzas) e bebidas, independente de disponibilidade ou estoque
                int totalProdutos = 0;
                int totalBebidas = 0;
                
                if (produtos != null)
                {
                    totalProdutos = produtos.Count;
                    System.Diagnostics.Debug.WriteLine($"Total de produtos (pizzas) encontrados: {totalProdutos}");
                }
                
                if (bebidas != null)
                {
                    totalBebidas = bebidas.Count;
                    System.Diagnostics.Debug.WriteLine($"Total de bebidas encontradas: {totalBebidas}");
                }

                // Soma total de produtos e bebidas
                int totalGeral = totalProdutos + totalBebidas;
                System.Diagnostics.Debug.WriteLine($"Total geral de produtos (pizzas + bebidas): {totalGeral}");

                // Exibe contadores no dashboard
                lblTotalProdutos.Text = totalGeral.ToString();
                // Conta apenas pedidos com status Pendente
                lblPedidosPendentes.Text = pedidos != null ? pedidos.Count(p => p.Status == Models.StatusPedido.Pendente).ToString() : "0";
                lblTotalClientes.Text = clientes != null ? clientes.Count.ToString() : "0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar estatísticas: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                // Em caso de erro, exibe zeros
                lblTotalProdutos.Text = "0";
                lblPedidosPendentes.Text = "0";
                lblTotalClientes.Text = "0";
            }
        }

        // Faz logout do administrador e limpa sessão
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                // Define mensagem de logout para exibir na página de login
                Session["MensagemLogout"] = "Você saiu e foi deslogado com sucesso!";

                // Limpa sessão do admin
                Session["AdminLogado"] = false;
                Session["AdminUsuario"] = null;
                Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no logout: {ex.Message}");
                Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
            }
        }
    }
}

