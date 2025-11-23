using System;
using System.Web;
using FastPizza.DataAccess;

namespace FastPizza
{
    // Classe global da aplicação - inicializa banco de dados na inicialização
    public class Global : HttpApplication
    {
        // Executado uma vez quando a aplicação inicia
        // Inicializa banco de dados e cria coluna Observacoes se não existir
        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                // Define inicializador do banco (cria tabelas e dados iniciais)
                System.Data.Entity.Database.SetInitializer(new DataAccess.DatabaseInitializer());

                using (var context = new DataAccess.FastPizzaDbContext())
                {
                    // Força inicialização do banco
                    context.Database.Initialize(true);

                    // Migração automática: cria coluna Observacoes se não existir
                    // Necessário para compatibilidade com bancos antigos
                    try
                    {
                        context.Database.ExecuteSqlCommand(@"
                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Pedidos]') AND name = 'Observacoes')
                            BEGIN
                                ALTER TABLE [dbo].[Pedidos] ADD [Observacoes] NVARCHAR(1000) NULL
                            END");
                    }
                    catch (Exception exColuna)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao verificar/adicionar coluna Observacoes: {exColuna.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar banco de dados: {ex.Message}");
            }
        }
    }
}

