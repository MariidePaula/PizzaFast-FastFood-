<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="FastPizza.Admin.Dashboard" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Importação de fontes e ícones -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">

    <style>
        /* Container principal da área administrativa */
        .admin-container {
            font-family: 'Poppins', sans-serif;
            padding: 20px;
            background-color: #f5f5f5;
            min-height: 100vh;
        }

        /* Cabeçalho do dashboard com título e botão sair */
        .admin-header {
            background: white;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 30px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .admin-header h1 {
            margin: 0;
            color: #E64A19;
        }

        /* Grid de cards do menu - responsivo com auto-fit */
        .admin-menu {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 40px;
            margin-bottom: 30px;
            row-gap: 40px;
            column-gap: 40px;
        }

        /* Card individual do menu */
        .menu-card {
            background: white;
            padding: 40px 35px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            text-align: center;
            transition: transform 0.3s;
            text-decoration: none;
            color: #333;
            display: block;
            margin: 0;
        }

        /* Animação ao passar mouse - levanta card */
        .menu-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
            text-decoration: none;
            color: #333;
        }

        /* Ícone do card (laranja) */
        .menu-card i {
            font-size: 48px;
            color: #E64A19;
            margin-bottom: 15px;
        }

        .menu-card h3 {
            margin: 0;
            color: #333;
        }

        /* Grid de estatísticas - responsivo */
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 30px;
            margin-bottom: 40px;
        }

        /* Card individual de estatística */
        .stat-card {
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        /* Título da estatística */
        .stat-card h4 {
            margin: 0 0 10px 0;
            color: #666;
            font-size: 14px;
        }

        /* Valor da estatística (grande e laranja) */
        .stat-card .value {
            font-size: 32px;
            font-weight: 700;
            color: #E64A19;
        }
    </style>

    <div class="admin-container">
        <!-- Cabeçalho com boas-vindas e botão de logout -->
        <div class="admin-header">
            <h1><i class="fa-solid fa-pizza-slice"></i> Dashboard Administrativo</h1>
            <div>
                <!-- Exibe nome do usuário administrador da sessão -->
                <span>Bem-vindo, <strong><%: Session["AdminUsuario"] %></strong></span>
                <!-- Botão para fazer logout -->
                <asp:Button ID="btnLogout" runat="server" Text="Sair" CssClass="btn btn-danger"
                    OnClick="btnLogout_Click" style="margin-left: 15px;" />
            </div>
        </div>

        <!-- Grid de estatísticas rápidas -->
        <div class="stats-grid">
            <!-- Card: Total de Produtos -->
            <div class="stat-card">
                <h4>Total de Produtos</h4>
                <div class="value"><asp:Label ID="lblTotalProdutos" runat="server" Text="0"></asp:Label></div>
            </div>
            
            <!-- Card: Pedidos Pendentes -->
            <div class="stat-card">
                <h4>Pedidos Pendentes</h4>
                <div class="value"><asp:Label ID="lblPedidosPendentes" runat="server" Text="0"></asp:Label></div>
            </div>
            
            <!-- Card: Total de Clientes -->
            <div class="stat-card">
                <h4>Total de Clientes</h4>
                <div class="value"><asp:Label ID="lblTotalClientes" runat="server" Text="0"></asp:Label></div>
            </div>
        </div>

        <!-- Menu principal com cards clicáveis -->
        <div class="admin-menu">
            <!-- Card: Gerenciar Produtos (Pizzas) -->
            <a href="<%= ResolveUrl("~/Admin/Produtos.aspx") %>" class="menu-card">
                <i class="fas fa-pizza-slice"></i>
                <h3>Gerenciar Produtos</h3>
                <p>Cadastrar, editar e remover produtos</p>
            </a>
            
            <!-- Card: Gerenciar Bebidas -->
            <a href="<%= ResolveUrl("~/Admin/Bebidas.aspx") %>" class="menu-card">
                <i class="fas fa-glass-water"></i>
                <h3>Gerenciar Bebidas</h3>
                <p>Cadastrar, editar e remover bebidas</p>
            </a>
            
            <!-- Card: Gerenciar Pedidos -->
            <a href="<%= ResolveUrl("~/Admin/Pedidos.aspx") %>" class="menu-card">
                <i class="fas fa-shopping-bag"></i>
                <h3>Gerenciar Pedidos</h3>
                <p>Visualizar e atualizar status</p>
            </a>
            
            <!-- Card: Gerenciar Clientes -->
            <a href="<%= ResolveUrl("~/Admin/Clientes.aspx") %>" class="menu-card">
                <i class="fas fa-users"></i>
                <h3>Gerenciar Clientes</h3>
                <p>Listar, editar e bloquear</p>
            </a>
            
            <!-- Card: Configurações -->
            <a href="<%= ResolveUrl("~/Admin/Configuracoes.aspx") %>" class="menu-card">
                <i class="fas fa-cog"></i>
                <h3>Configurações</h3>
                <p>Gerenciar banner e configurações</p>
            </a>
        </div>
    </div>
</asp:Content>