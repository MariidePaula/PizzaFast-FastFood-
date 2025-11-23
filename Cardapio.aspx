<%@ Page Title="Cardápio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cardapio.aspx.cs" Inherits="FastPizza.Cardapio" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Modal de Login - Exibido quando usuário não autenticado tenta adicionar item ao carrinho -->
    <div class="modal fade" id="modalLogin" tabindex="-1" aria-labelledby="modalLoginLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false" data-bs-show="false">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalLoginLabel">
                        <i class="fas fa-lock"></i> Faça Login para Continuar
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
                </div>
                <div class="modal-body">
                    <!-- Label para exibir mensagens de erro do login -->
                    <asp:Label ID="lblMensagemModal" runat="server" CssClass="alert alert-danger" Visible="false" style="display: block; margin-bottom: 20px;"></asp:Label>

                    <!-- Campo de email -->
                    <div class="mb-3">
                        <label class="form-label fw-semibold">Email:</label>
                        <asp:TextBox ID="txtEmailModal" runat="server" CssClass="form-control" placeholder="seu@email.com" TextMode="Email" />
                    </div>

                    <!-- Campo de senha -->
                    <div class="mb-3">
                        <label class="form-label fw-semibold">Senha:</label>
                        <asp:TextBox ID="txtSenhaModal" runat="server" CssClass="form-control" TextMode="Password" placeholder="••••••••" />
                    </div>

                    <!-- Link para página de cadastro -->
                    <div class="text-center mb-3">
                        <p class="text-muted small">Não tem uma conta? <a href="<%= ResolveUrl("~/Cadastro.aspx") %>" class="text-danger fw-semibold">Cadastre-se aqui</a></p>
                    </div>
                </div>
                <div class="modal-footer">
                    <!-- Botão para processar o login -->
                    <asp:Button ID="btnLoginModal" runat="server" Text="Entrar" CssClass="btn btn-danger w-100"
                        OnClick="btnLoginModal_Click" style="background-color: #E64A19; border: none; padding: 10px; font-weight: 600;" />
                </div>
            </div>
        </div>
    </div>

    <!-- Importação de fontes e ícones -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">

    <style>
        /* Container principal do cardápio */
        .menu-container {
            font-family: 'Poppins', sans-serif;
            background-color: #FFFBF7;
            padding-bottom: 60px;
            color: #333;
            min-height: 100vh;
        }

        /* Seção de cabeçalho com título e descrição */
        .header-section {
            text-align: center;
            padding: 50px 20px 30px 20px;
        }

        .main-title {
            font-size: 2.5rem;
            font-weight: 700;
            color: #111;
            margin-bottom: 10px;
            line-height: 1.2;
        }

        .header-title {
            font-size: 1.1rem;
            color: #666;
            max-width: 600px;
            margin: 0 auto;
            line-height: 1.5;
        }

        /* Barra de filtros por categoria */
        .filter-bar {
            display: flex;
            justify-content: center;
            gap: 15px;
            flex-wrap: wrap;
            margin-bottom: 40px;
        }

        /* Botões de filtro de categoria */
        .filter-btn {
            background-color: #fdfdfd;
            border: 1px solid #eee;
            padding: 8px 25px;
            border-radius: 50px;
            font-weight: 600;
            color: #777;
            cursor: pointer;
            transition: all 0.3s ease;
            text-decoration: none;
            font-size: 0.9rem;
        }

        /* Estado ativo e hover dos botões de filtro */
        .filter-btn.active, .filter-btn:hover {
            background-color: #fff;
            color: #333;
            border-color: #ccc;
            box-shadow: 0 4px 6px rgba(0,0,0,0.05);
            font-weight: 700;
        }

        /* Grid responsivo de produtos (3 colunas em desktop) */
        .pizza-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 30px;
            padding: 0 20px;
            max-width: 1200px;
            margin: 0 auto;
        }

        /* Grid em 2 colunas para tablets */
        @media (max-width: 992px) {
            .pizza-grid {
                grid-template-columns: repeat(2, 1fr);
            }
        }

        /* Grid em 1 coluna para mobile */
        @media (max-width: 768px) {
            .pizza-grid {
                grid-template-columns: 1fr;
            }
        }

        /* Card de produto individual */
        .pizza-card {
            background: white;
            border-radius: 20px;
            overflow: hidden;
            box-shadow: 0 10px 20px rgba(0,0,0,0.05);
            transition: transform 0.3s ease;
            border: 1px solid #f0f0f0;
            display: flex;
            flex-direction: column;
            max-width: 100%;
        }

        /* Animação ao passar mouse sobre o card */
        .pizza-card:hover {
            transform: translateY(-5px);
        }

        /* Container da imagem do produto com gradiente de fundo */
        .card-img-wrapper {
            height: 200px;
            overflow: hidden;
            background: linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            position: relative;
        }

        /* Gradiente diferente para bebidas */
        .card-img-wrapper.bebida-wrapper {
            background: linear-gradient(135deg, #4ecdc4 0%, #44a08d 100%);
        }

        /* Estilo da imagem do produto */
        .card-img-wrapper img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        /* Ícone fallback quando não há imagem */
        .card-img-wrapper .icon-fallback {
            font-size: 80px;
            color: white;
            opacity: 0.9;
        }

        /* Conteúdo do card (nome, descrição, preço) */
        .card-content {
            padding: 20px;
            display: flex;
            flex-direction: column;
            flex-grow: 1;
        }

        /* Linha do cabeçalho com nome e badge */
        .card-header-row {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 8px;
        }

        /* Nome do produto */
        .pizza-name {
            font-size: 1.1rem;
            font-weight: 700;
            margin: 0;
            color: #111;
        }

        /* Badge de categoria do produto */
        .pizza-badge {
            background-color: #FFA726;
            color: white;
            font-size: 0.7rem;
            font-weight: 600;
            padding: 4px 10px;
            border-radius: 12px;
            text-transform: uppercase;
            white-space: nowrap;
            margin-left: 10px;
        }

        /* Descrição do produto */
        .pizza-desc {
            font-size: 0.85rem;
            color: #777;
            margin-bottom: 20px;
            line-height: 1.4;
            flex-grow: 1;
        }

        /* Preço do produto */
        .pizza-price {
            font-size: 1.4rem;
            font-weight: 700;
            color: #E64A19;
            margin-bottom: 15px;
            display: block;
        }

        /* Botão de adicionar ao carrinho */
        .btn-add-full {
            background-color: #E64A19;
            color: white;
            width: 100%;
            border: none;
            padding: 12px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 0.9rem;
            cursor: pointer;
            transition: background 0.3s;
            text-align: center;
            text-decoration: none;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
        }

        /* Estado hover do botão */
        .btn-add-full:hover {
            background-color: #BF360C;
            color: white;
            text-decoration: none;
        }
    </style>

    <div class="menu-container">

        <!-- Cabeçalho da página com título e descrição -->
        <div class="header-section">
            <h1 class="main-title">Nosso Cardápio</h1>
            <p class="header-title">
                Explore nossas deliciosas pizzas artesanais feitas com ingredientes selecionados
            </p>
        </div>

        <!-- Barra de filtros por categoria - Links aplicam classe "active" dinamicamente -->
        <div class="filter-bar">
            <!-- Filtro "Todas" - ativo quando não há categoria ou categoria=Todas -->
            <a href="<%= ResolveUrl("~/Cardapio.aspx") %>"
               class='<%= string.IsNullOrEmpty(Request.QueryString["categoria"]) || Request.QueryString["categoria"] == "Todas" ? "filter-btn active" : "filter-btn" %>'>
               Todas
            </a>
            <!-- Filtro "Bebidas" -->
            <a href="<%= ResolveUrl("~/Cardapio.aspx?categoria=Bebidas") %>"
               class='<%= Request.QueryString["categoria"] == "Bebidas" ? "filter-btn active" : "filter-btn" %>'>
               Bebidas
            </a>
            <!-- Filtro "Tradicional" -->
            <a href="<%= ResolveUrl("~/Cardapio.aspx?categoria=Tradicional") %>"
               class='<%= Request.QueryString["categoria"] == "Tradicional" ? "filter-btn active" : "filter-btn" %>'>
               Tradicionais
            </a>
            <!-- Filtro "Premium" -->
            <a href="<%= ResolveUrl("~/Cardapio.aspx?categoria=Premium") %>"
               class='<%= Request.QueryString["categoria"] == "Premium" ? "filter-btn active" : "filter-btn" %>'>
               Premium
            </a>
            <!-- Filtro "Vegetariana" -->
            <a href="<%= ResolveUrl("~/Cardapio.aspx?categoria=Vegetariana") %>"
               class='<%= Request.QueryString["categoria"] == "Vegetariana" ? "filter-btn active" : "filter-btn" %>'>
               Vegetarianas
            </a>
        </div>

        <!-- UpdatePanel para atualizações parciais da página (AJAX) -->
        <asp:UpdatePanel ID="upProdutos" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="pizza-grid">
                    <!-- Repeater que renderiza cada produto do cardápio -->
                    <asp:Repeater ID="rptCardapio" runat="server" OnItemCommand="rptCardapio_ItemCommand">
                        <ItemTemplate>
                            <!-- Card individual do produto - ID diferente para bebida ou pizza -->
                            <div class="pizza-card" id='<%# Request.QueryString["categoria"] == "Bebidas" ? "card-bebida-" + Eval("Id") : "card-produto-" + Eval("Id") %>'>
                                
                                <!-- Container da imagem - gradiente diferente para bebidas -->
                                <div class='card-img-wrapper <%= Request.QueryString["categoria"] == "Bebidas" ? "bebida-wrapper" : "" %>'>
                                    <!-- Renderiza imagem se disponível, senão mostra ícone fallback -->
                                    <%# (Eval("ImagemUrl") != null && !string.IsNullOrEmpty(Eval("ImagemUrl").ToString()) && Eval("ImagemUrl").ToString().Trim() != "") ?
                                        "<img src='" + Eval("ImagemUrl") + "' alt='" + Eval("Nome") + "' style='width: 100%; height: 100%; object-fit: cover; position: relative; z-index: 2;' onerror=\"this.style.display='none'; var icon = document.createElement('i'); icon.className = 'fas " + (Request.QueryString["categoria"] == "Bebidas" ? "fa-glass-water" : "fa-pizza-slice") + " icon-fallback'; this.parentElement.appendChild(icon);\" />" :
                                        "<i class='fas " + (Request.QueryString["categoria"] == "Bebidas" ? "fa-glass-water" : "fa-pizza-slice") + " icon-fallback'></i>" %>
                                </div>
                                
                                <!-- Conteúdo do card -->
                                <div class="card-content">
                                    <!-- Nome e badge de categoria -->
                                    <div class="card-header-row">
                                        <h3 class="pizza-name"><%# Eval("Nome") %></h3>
                                        <span class="pizza-badge"><%# Eval("Categoria") %></span>
                                    </div>
                                    
                                    <!-- Descrição do produto -->
                                    <p class="pizza-desc"><%# Eval("Descricao") %></p>
                                    
                                    <!-- Preço formatado em reais -->
                                    <span class="pizza-price">R$ <%# Eval("Preco", "{0:F2}") %></span>
                                    
                                    <!-- Botão adicionar ao carrinho - CommandArgument com prefixo B_ para bebidas ou P_ para pizzas -->
                                    <asp:LinkButton ID="btnAdicionar" runat="server"
                                        CssClass="btn-add-full"
                                        CommandName="AdicionarCarrinho"
                                        CommandArgument='<%# Request.QueryString["categoria"] == "Bebidas" ? "B_" + Eval("Id") : "P_" + Eval("Id") %>'
                                        data-require-login="true">
                                        <i class="fas fa-shopping-cart"></i> Adicionar ao Carrinho
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <!-- Label para exibir mensagens de sucesso (ex: "Item adicionado ao carrinho") -->
        <div style="max-width: 600px; margin: 20px auto;">
            <asp:Label ID="lblMensagem" runat="server" CssClass="alert alert-success" Visible="false" style="display:block; text-align:center;"></asp:Label>
        </div>

    </div>

</asp:Content>