using System;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FastPizza.DataAccess;
using FastPizza.Models;

namespace FastPizza.Admin
{
    // Página de configurações do sistema (banner da página inicial)
    public partial class Configuracoes : System.Web.UI.Page
    {
        // Chave da configuração de banner no banco de dados
        private const string BANNER_IMAGE_KEY = "BannerImageUrl";

        // Verifica autenticação e carrega configuração atual
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
                    CarregarConfiguracao();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Page_Load Admin/Configuracoes: {ex.Message}");
                Response.Redirect(ResolveUrl("~/Admin/Login.aspx"));
            }
        }

        // Carrega URL do banner configurada no banco de dados
        private void CarregarConfiguracao()
        {
            try
            {
                using (var context = new FastPizzaDbContext())
                {

                    if (!context.Database.Exists())
                    {
                        txtBannerUrl.Text = string.Empty;
                        return;
                    }

                    GarantirTabelaConfiguracoes(context);

                    try
                    {
                        var config = context.Configuracoes
                            .FirstOrDefault(c => c.Chave == BANNER_IMAGE_KEY);

                        if (config != null)
                        {
                            txtBannerUrl.Text = config.Valor;
                        }
                        else
                        {
                            txtBannerUrl.Text = string.Empty;
                        }
                    }
                    catch
                    {

                        txtBannerUrl.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar configuração: {ex.Message}");
                txtBannerUrl.Text = string.Empty;
            }
        }

        // Cria tabela Configuracoes se não existir (migração automática)
        private void GarantirTabelaConfiguracoes(FastPizzaDbContext context)
        {
            try
            {
                // Tenta consultar a tabela para verificar se existe
                var test = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM Configuracoes").FirstOrDefault();
            }
            catch
            {
                // Se não existir, cria a tabela
                try
                {
                    context.Database.ExecuteSqlCommand(@"
                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Configuracoes]') AND type in (N'U'))
                        BEGIN
                            CREATE TABLE [dbo].[Configuracoes] (
                                [Id] [int] IDENTITY(1,1) NOT NULL,
                                [Chave] [nvarchar](100) NOT NULL,
                                [Valor] [nvarchar](1000) NULL,
                                [DataAtualizacao] [datetime] NOT NULL,
                                CONSTRAINT [PK_Configuracoes] PRIMARY KEY CLUSTERED ([Id] ASC)
                            )
                        END
                    ");
                }
                catch (Exception sqlEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao criar tabela Configuracoes: {sqlEx.Message}");
                    throw;
                }
            }
        }

        // Salva URL do banner no banco de dados com validações
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                string bannerUrl = txtBannerUrl.Text.Trim();

                if (!string.IsNullOrEmpty(bannerUrl))
                {

                    if (!bannerUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                        !bannerUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        ExibirMensagem("URL inválida. A URL deve começar com 'http://' ou 'https://'. Exemplo: https://exemplo.com/imagem.jpg", "danger");
                        return;
                    }

                    if (!Uri.IsWellFormedUriString(bannerUrl, UriKind.Absolute))
                    {
                        ExibirMensagem("URL inválida ou incompleta. Por favor, insira uma URL completa e válida. Exemplo: https://exemplo.com/imagem.jpg", "danger");
                        return;
                    }

                    Uri uri;
                    if (!Uri.TryCreate(bannerUrl, UriKind.Absolute, out uri))
                    {
                        ExibirMensagem("URL inválida. Verifique se a URL está completa e contém um domínio válido. Exemplo: https://exemplo.com/imagem.jpg", "danger");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(uri.Host))
                    {
                        ExibirMensagem("URL incompleta. A URL deve conter um domínio válido. Exemplo: https://exemplo.com/imagem.jpg", "danger");
                        return;
                    }
                }

                using (var context = new FastPizzaDbContext())
                {

                    if (!context.Database.Exists())
                    {
                        context.Database.Create();
                    }
                    else
                    {

                        GarantirTabelaConfiguracoes(context);
                    }

                    var config = context.Configuracoes
                        .FirstOrDefault(c => c.Chave == BANNER_IMAGE_KEY);

                    if (config != null)
                    {

                        config.Valor = bannerUrl;
                        config.DataAtualizacao = DateTime.Now;
                    }
                    else
                    {

                        config = new Configuracao
                        {
                            Chave = BANNER_IMAGE_KEY,
                            Valor = bannerUrl,
                            DataAtualizacao = DateTime.Now
                        };
                        context.Configuracoes.Add(config);
                    }

                    context.SaveChanges();
                }

                ExibirMensagem("Configuração salva com sucesso! A imagem do banner será atualizada na próxima visualização da página inicial.", "success");
            }
            catch (System.Data.Entity.Core.EntityException ex)
            {
                string erroDetalhado = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                System.Diagnostics.Debug.WriteLine($"Erro Entity ao salvar configuração: {erroDetalhado}");
                ExibirMensagem($"Erro de conexão com o banco de dados. Verifique se o SQL Server está rodando. Detalhes: {erroDetalhado}", "danger");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                string erroSQL = ex.Message;
                if (ex.Number == 208)
                {
                    erroSQL = "A tabela Configuracoes não existe no banco de dados. Tente recriar o banco ou execute o script SQL manualmente.";
                }
                System.Diagnostics.Debug.WriteLine($"Erro SQL ao salvar configuração (Número: {ex.Number}): {erroSQL}");
                ExibirMensagem($"Erro SQL: {erroSQL}", "danger");
            }
            catch (Exception ex)
            {
                string erroCompleto = ex.Message;
                if (ex.InnerException != null)
                {
                    erroCompleto += " | " + ex.InnerException.Message;
                }
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar configuração: {erroCompleto}");
                ExibirMensagem($"Erro ao salvar configuração: {ex.Message}", "danger");
            }
        }

        // Remove configuração de banner (volta ao padrão)
        protected void btnRemover_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpa valor da configuração no banco
                using (var context = new FastPizzaDbContext())
                {
                    var config = context.Configuracoes
                        .FirstOrDefault(c => c.Chave == BANNER_IMAGE_KEY);

                    if (config != null)
                    {
                        config.Valor = string.Empty;
                        config.DataAtualizacao = DateTime.Now;
                        context.SaveChanges();
                    }
                }

                txtBannerUrl.Text = string.Empty;
                ExibirMensagem("Imagem do banner removida com sucesso! O banner voltará ao padrão.", "success");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao remover imagem: {ex.Message}");
                ExibirMensagem("Erro ao remover imagem. Tente novamente.", "danger");
            }
        }

        // Exibe mensagem de feedback para o usuário
        private void ExibirMensagem(string mensagem, string tipo)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.Visible = true;

            string cssClass = tipo == "success" ? "alert-success" : "alert-danger";
            lblMensagem.CssClass = $"alert {cssClass}";

            ClientScript.RegisterStartupScript(this.GetType(), "ScrollToMessage",
                "setTimeout(function() { document.getElementById('" + lblMensagem.ClientID + "').scrollIntoView({ behavior: 'smooth', block: 'center' }); }, 100);", true);
        }

        // Método estático para obter URL do banner (usado em outras páginas)
        // Retorna URL configurada ou padrão da pasta Banners
        public static string ObterBannerUrl()
        {
            try
            {
                using (var context = new FastPizzaDbContext())
                {
                    // Se não houver banco, retorna caminho padrão da pasta Banners
                    if (!context.Database.Exists())
                    {
                        return "~/Images/Banners/hero-pizzeria.jpg";
                    }

                    try
                    {
                        // Busca configuração no banco
                        var config = context.Configuracoes
                            .FirstOrDefault(c => c.Chave == BANNER_IMAGE_KEY);

                        if (config != null && !string.IsNullOrWhiteSpace(config.Valor))
                        {
                            return config.Valor;
                        }
                        // Se não houver configuração, retorna banner padrão
                        return "~/Images/Banners/hero-pizzeria.jpg";
                    }
                    catch
                    {
                        // Em caso de erro, retorna caminho padrão da pasta Banners
                        return "~/Images/Banners/hero-pizzeria.jpg";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter URL do banner: {ex.Message}");
            }
            // Fallback: retorna caminho padrão da pasta Banners
            return "~/Images/Banners/hero-pizzeria.jpg";
        }
    }
}

