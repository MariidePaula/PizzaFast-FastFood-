using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using FastPizza.Models;
using System.Data.Entity.Infrastructure;

namespace FastPizza.DataAccess
{
    // Contexto do Entity Framework para acesso ao banco de dados
    // Gerencia todas as operações de leitura e escrita no banco de dados SQL Server
    public class FastPizzaDbContext : DbContext
    {
        // Construtor que inicializa o contexto usando a connection string do Web.config
        public FastPizzaDbContext() : base("FastPizzaConnection")
        {
            // Desabilita proxy creation para melhor performance (não cria proxies dinâmicos)
            this.Configuration.ProxyCreationEnabled = false;
            // Desabilita lazy loading para evitar carregamento automático de relacionamentos
            this.Configuration.LazyLoadingEnabled = false;

            // Define inicializador do banco que cria tabelas e popula dados iniciais
            Database.SetInitializer(new DatabaseInitializer());
        }

        // DbSet representa uma tabela no banco de dados
        // Cada DbSet permite operações CRUD (Create, Read, Update, Delete) na tabela correspondente
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Bebida> Bebidas { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItens { get; set; }
        public DbSet<Configuracao> Configuracoes { get; set; }

        // Configuração do modelo de dados (mapeamento Entity Framework)
        // Define como as classes C# são mapeadas para tabelas e colunas do banco de dados
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove pluralização automática de nomes de tabela
            // Mantém os nomes exatamente como definidos no ToTable()
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Configuração da entidade Cliente
            modelBuilder.Entity<Cliente>()
                .ToTable("Clientes")  // Nome da tabela no banco
                .HasKey(c => c.Id)    // Define Id como chave primária
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);  // Id é gerado automaticamente pelo banco

            modelBuilder.Entity<Cliente>()
                .Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Cliente>()
                .Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Cliente>()
                .Property(c => c.SenhaHash)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<Cliente>()
                .Property(c => c.Telefone)
                .HasMaxLength(20);

            modelBuilder.Entity<Cliente>()
                .Property(c => c.Endereco)
                .HasMaxLength(500);

            modelBuilder.Entity<Cliente>()
                .Property(c => c.Cidade)
                .HasMaxLength(100);

            modelBuilder.Entity<Cliente>()
                .Property(c => c.CEP)
                .HasMaxLength(10);

            modelBuilder.Entity<Produto>()
                .ToTable("Produtos")
                .HasKey(p => p.Id)
                .Property(p => p.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Descricao)
                .HasMaxLength(1000);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Categoria)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Preco)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Produto>()
                .Property(p => p.ImagemUrl)
                .HasMaxLength(500);

            modelBuilder.Entity<Bebida>()
                .ToTable("Bebidas")
                .HasKey(b => b.Id)
                .Property(b => b.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Bebida>()
                .Property(b => b.Nome)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Bebida>()
                .Property(b => b.Descricao)
                .HasMaxLength(1000);

            modelBuilder.Entity<Bebida>()
                .Property(b => b.Categoria)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Bebida>()
                .Property(b => b.Preco)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Bebida>()
                .Property(b => b.ImagemUrl)
                .HasMaxLength(500);

            modelBuilder.Entity<Endereco>()
                .ToTable("Enderecos")
                .HasKey(e => e.Id)
                .Property(e => e.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Endereco>()
                .Property(e => e.Rua)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Endereco>()
                .Property(e => e.Numero)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<Endereco>()
                .Property(e => e.Bairro)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Endereco>()
                .Property(e => e.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Endereco>()
                .Property(e => e.CEP)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<Pedido>()
                .ToTable("Pedidos")
                .HasKey(p => p.Id)
                .Property(p => p.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.NomeCliente)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.MotivoCancelamento)
                .HasMaxLength(500);

            // Campo de observações do pedido é opcional (pode ser null)
            modelBuilder.Entity<Pedido>()
                .Property(p => p.Observacoes)
                .IsOptional()
                .HasMaxLength(1000);

            modelBuilder.Entity<PedidoItem>()
                .ToTable("PedidoItens")
                .HasKey(i => i.Id)
                .Property(i => i.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<PedidoItem>()
                .Property(i => i.NomeProduto)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<PedidoItem>()
                .Property(i => i.Quantidade)
                .IsRequired();

            modelBuilder.Entity<PedidoItem>()
                .Property(i => i.PrecoUnitario)
                .IsRequired()
                .HasPrecision(18, 2);

            // Relacionamento: PedidoItem pertence a um Pedido
            // Cascade delete: ao excluir pedido, exclui automaticamente seus itens
            modelBuilder.Entity<PedidoItem>()
                .HasRequired(i => i.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(i => i.PedidoId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Configuracao>()
                .ToTable("Configuracoes")
                .HasKey(c => c.Id)
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Configuracao>()
                .Property(c => c.Chave)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Configuracao>()
                .Property(c => c.Valor)
                .HasMaxLength(1000);

            modelBuilder.Entity<Configuracao>()
                .Property(c => c.DataAtualizacao)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}

