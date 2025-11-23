<%@ Page Title="Fast Pizza" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FastPizza._Default" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* Estilo do banner principal/hero - altura mínima, centralização e configurações de background */
        .hero-banner {
            position: relative;
            min-height: 550px;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            background-size: cover;
            background-position: center;
            background-attachment: scroll;
            background-repeat: no-repeat;
            overflow: hidden;
            margin: 0;
            padding: 0;
        }

        /* Overlay com gradientes coloridos sobre o banner - camada decorativa */
        .hero-banner::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background:
                linear-gradient(135deg, rgba(230, 74, 25, 0.75) 0%, rgba(255, 107, 107, 0.7) 100%),
                radial-gradient(circle at 20% 30%, rgba(255, 165, 0, 0.3) 0%, transparent 50%),
                radial-gradient(circle at 80% 70%, rgba(255, 140, 0, 0.2) 0%, transparent 50%);
            pointer-events: none;
            z-index: 1;
        }

        /* Estilo para quando não há imagem de fundo - usa apenas gradientes */
        .hero-banner.no-image {
            background:
                linear-gradient(135deg, rgba(230, 74, 25, 0.75) 0%, rgba(255, 107, 107, 0.7) 100%),
                radial-gradient(circle at 20% 30%, rgba(255, 165, 0, 0.3) 0%, transparent 50%),
                radial-gradient(circle at 80% 70%, rgba(255, 140, 0, 0.2) 0%, transparent 50%),
                linear-gradient(180deg, rgba(139, 69, 19, 0.1) 0%, rgba(160, 82, 45, 0.15) 100%);
            background-size: cover, cover, cover, cover;
            background-position: center, left top, right bottom, center;
        }

        /* Overlay adicional com blur para banner sem imagem */
        .hero-banner.no-image::before {
            background:
                radial-gradient(ellipse at 25% 40%, rgba(255, 140, 0, 0.15) 0%, transparent 60%),
                radial-gradient(ellipse at 75% 60%, rgba(230, 74, 25, 0.12) 0%, transparent 60%);
            filter: blur(40px);
        }

        /* Padrão de textura diagonal sobre o banner */
        .hero-banner::after {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-image:
                repeating-linear-gradient(45deg, transparent, transparent 2px, rgba(255, 255, 255, 0.03) 2px, rgba(255, 255, 255, 0.03) 4px);
            pointer-events: none;
            z-index: 1;
        }

        /* Container do conteúdo do hero - centralizado e com espaçamento */
        .hero-content {
            position: relative;
            z-index: 2;
            text-align: center;
            padding: 60px 20px;
            max-width: 800px;
            margin: 0 auto;
        }

        /* Garante que todo conteúdo direto do hero fique acima dos overlays */
        .hero-banner > * {
            position: relative;
            z-index: 2;
        }

        /* Badge/emblema no topo do hero com efeito glassmorphism */
        .hero-badge {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            background: rgba(255, 255, 255, 0.2);
            backdrop-filter: blur(15px);
            -webkit-backdrop-filter: blur(15px);
            border: 2px solid rgba(255, 255, 255, 0.4);
            border-radius: 50px;
            padding: 12px 28px;
            margin-bottom: 35px;
            font-size: 1.05rem;
            font-weight: 600;
            color: white;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
            transition: all 0.3s ease;
        }

        /* Efeito hover no badge - levanta e aumenta sombra */
        .hero-badge:hover {
            background: rgba(255, 255, 255, 0.25);
            transform: translateY(-2px);
            box-shadow: 0 6px 25px rgba(0, 0, 0, 0.2);
        }

        /* Ícone dentro do badge - cor dourada com sombra */
        .hero-badge i {
            font-size: 1.4rem;
            color: #FFD700;
            filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.2));
        }

        /* Container do título principal do hero */
        .hero-title {
            font-family: 'Poppins', sans-serif;
            font-weight: 700;
            line-height: 1.2;
            margin-bottom: 25px;
        }

        /* Primeira linha do título - texto branco com sombra */
        .hero-title-line1 {
            display: block;
            font-size: 4.5rem;
            color: #ffffff;
            text-shadow: 3px 3px 12px rgba(0, 0, 0, 0.4), 0 0 20px rgba(0, 0, 0, 0.2);
            margin-bottom: 8px;
            letter-spacing: -1px;
        }

        /* Segunda linha do título - texto dourado com sombra brilhante */
        .hero-title-line2 {
            display: block;
            font-size: 4.5rem;
            color: #FFD700;
            text-shadow: 3px 3px 12px rgba(0, 0, 0, 0.4), 0 0 25px rgba(255, 215, 0, 0.3);
            letter-spacing: -1px;
        }

        /* Texto de descrição abaixo do título */
        .hero-description {
            font-size: 1.25rem;
            color: rgba(255, 255, 255, 0.98);
            line-height: 1.7;
            margin-bottom: 40px;
            text-shadow: 1px 1px 4px rgba(0, 0, 0, 0.25);
            max-width: 650px;
            margin-left: auto;
            margin-right: auto;
            font-weight: 400;
        }

        /* Container do botão call-to-action */
        .hero-cta {
            margin-top: 30px;
        }

        /* Estilo do botão principal do hero - gradiente dourado */
        .btn-hero {
            background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
            color: #1a1a1a;
            font-weight: 700;
            font-size: 1.15rem;
            padding: 16px 45px;
            border-radius: 50px;
            border: none;
            box-shadow: 0 6px 25px rgba(255, 215, 0, 0.45), 0 2px 10px rgba(0, 0, 0, 0.1);
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 12px;
        }

        /* Efeito hover no botão hero - levanta e clareia */
        .btn-hero:hover {
            transform: translateY(-3px);
            box-shadow: 0 10px 30px rgba(255, 215, 0, 0.55), 0 4px 15px rgba(0, 0, 0, 0.15);
            color: #1a1a1a;
            background: linear-gradient(135deg, #FFE44D 0%, #FFB84D 100%);
        }

        /* Efeito de clique no botão */
        .btn-hero:active {
            transform: translateY(-1px);
        }

        /* Ícone dentro do botão hero */
        .btn-hero i {
            font-size: 1.2rem;
        }

        /* Media query para tablets - reduz tamanhos */
        @media (max-width: 768px) {
            .hero-banner {
                min-height: 450px;
            }

            .hero-title-line1,
            .hero-title-line2 {
                font-size: 2.5rem;
            }

            .hero-description {
                font-size: 1rem;
                padding: 0 15px;
            }

            .hero-badge {
                font-size: 0.9rem;
                padding: 10px 20px;
            }

            .btn-hero {
                font-size: 1rem;
                padding: 12px 30px;
            }
        }

        /* Media query para celulares - reduz ainda mais os tamanhos */
        @media (max-width: 576px) {
            .hero-title-line1,
            .hero-title-line2 {
                font-size: 2rem;
            }

            .hero-description {
                font-size: 0.95rem;
            }
        }

        /* Estilo dos cards de pizza na home */
        .pizza-card-home {
            border-radius: 15px !important;
            overflow: hidden;
            transition: all 0.3s ease;
            cursor: pointer;
        }

        /* Efeito hover nos cards - levanta e adiciona sombra laranja */
        .pizza-card-home:hover {
            transform: translateY(-5px);
            box-shadow: 0 10px 25px rgba(230, 74, 25, 0.2) !important;
            border: 1px solid #E64A19;
        }

        /* Área da imagem do card com bordas arredondadas no topo */
        .pizza-card-home .position-relative {
            border-radius: 15px 15px 0 0;
        }

        /* Corpo do card com bordas arredondadas na base */
        .pizza-card-home .card-body {
            border-radius: 0 0 15px 15px;
        }

        /* Efeito hover no botão do card - escurece e aumenta */
        .pizza-card-home .btn-danger:hover {
            background-color: #D84315 !important;
            border-color: #D84315 !important;
            transform: scale(1.05);
        }

        /* Estilo padrão do botão danger no card */
        .pizza-card-home .btn-danger {
            background-color: #E64A19 !important;
            border-color: #E64A19 !important;
            transition: all 0.3s ease;
        }
    </style>

    <!-- Modal de Login - aparece quando usuário tenta adicionar ao carrinho sem estar logado -->
    <div class="modal fade" id="modalLogin" tabindex="-1" aria-labelledby="modalLoginLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false" data-bs-show="false">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <!-- Cabeçalho do modal com título e botão fechar -->
                <div class="modal-header">
                    <h5 class="modal-title" id="modalLoginLabel">
                        <i class="fas fa-lock"></i> Faça Login para Continuar
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
                </div>
                <!-- Corpo do modal com formulário de login -->
                <div class="modal-body">
                    <!-- Label para mensagens de erro no modal -->
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
                <!-- Rodapé do modal com botão de login -->
                <div class="modal-footer">
                    <asp:Button ID="btnLoginModal" runat="server" Text="Entrar" CssClass="btn btn-danger w-100"
                        OnClick="btnLoginModal_Click" style="background-color: #E64A19; border: none; padding: 10px; font-weight: 600;" />
                </div>
            </div>
        </div>
    </div>

    <!-- Seção Hero/Banner principal - adiciona classe 'no-image' se não houver URL de imagem -->
    <section class="hero-banner <%= string.IsNullOrEmpty(BannerImageUrl) ? "no-image" : "" %>"
        style="margin-bottom: 3rem;<%= !string.IsNullOrEmpty(BannerImageUrl) ? " background-image: url('" + BannerImageUrl + "');" : "" %>">
        <div class="hero-content">
            <!-- Badge com ícone de pizza -->
            <div class="hero-badge">
                <i class="fas fa-pizza-slice"></i>
                <span>As melhores pizzas da cidade</span>
            </div>
            <!-- Título principal dividido em duas linhas -->
            <h1 class="hero-title">
                <span class="hero-title-line1">Sabor Autêntico,</span>
                <span class="hero-title-line2">Entrega Rápida</span>
            </h1>
            <!-- Descrição/subtítulo -->
            <p class="hero-description">
                Pizzas artesanais feitas com ingredientes frescos e massa tradicional.
                Peça agora e receba quentinha em casa!
            </p>
            <!-- Botão call-to-action que leva ao cardápio -->
            <div class="hero-cta">
                <a href="<%= ResolveUrl("~/Cardapio.aspx") %>" class="btn btn-hero">
                    <i class="fas fa-shopping-bag"></i> Ver Cardápio
                </a>
            </div>
        </div>
    </section>

    <!-- Container principal com as pizzas em destaque -->
    <div class="container my-5">
        <!-- Cabeçalho da seção -->
        <div class="row mb-4">
            <div class="col-12 text-center">
                <h1 class="display-4 fw-bold text-dark mb-3">Pizzas em Destaque</h1>
                <p class="lead text-muted">Nossas pizzas mais pedidas e amadas</p>
            </div>
        </div>

        <!-- UpdatePanel para atualização parcial da página (AJAX) -->
        <asp:UpdatePanel ID="upProdutos" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <!-- Grid de produtos -->
                <div class="row g-4">
                    <!-- Repeater que itera sobre as pizzas disponíveis -->
                    <asp:Repeater ID="rptPizzas" runat="server" OnItemCommand="rptPizzas_ItemCommand">
                        <ItemTemplate>
                            <!-- Coluna responsiva para cada pizza -->
                            <div class="col-lg-3 col-md-4 col-sm-6" id='card-produto-<%# Eval("Id") %>'>
                                <!-- Card individual da pizza -->
                                <div class="card h-100 shadow-sm border-0 pizza-card-home">
                                    <!-- Container da imagem com gradiente de fundo como fallback -->
                                    <div class="position-relative" style="height: 200px; overflow: hidden; background: linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%); display: flex; align-items: center; justify-content: center; border-radius: 15px 15px 0 0;">
                                        <!-- Lógica condicional: se houver imagem, exibe, senão mostra ícone -->
                                        <%# (Eval("ImagemUrl") != null && !string.IsNullOrEmpty(Eval("ImagemUrl").ToString()) && Eval("ImagemUrl").ToString().Trim() != "") ?
                                            "<img src='" + Eval("ImagemUrl") + "' alt='" + Eval("Nome") + "' class='card-img-top h-100 w-100' style='object-fit: cover; position: absolute; top: 0; left: 0; width: 100%; height: 100%;' onerror=\"this.style.display='none'; this.parentElement.innerHTML+='<i class=\\'fas fa-pizza-slice\\' style=\\'font-size: 80px; color: white; opacity: 0.9;\\'></i>';\" />" :
                                            "<i class='fas fa-pizza-slice' style='font-size: 80px; color: white; opacity: 0.9;'></i>" %>
                                        <!-- Badge com a categoria da pizza -->
                                        <span class="badge bg-warning text-dark position-absolute top-0 end-0 m-2" style="z-index: 10;">
                                            <%# Eval("Categoria") %>
                                        </span>
                                    </div>
                                    <!-- Corpo do card com informações da pizza -->
                                    <div class="card-body d-flex flex-column">
                                        <!-- Nome da pizza -->
                                        <h5 class="card-title fw-bold mb-2"><%# Eval("Nome") %></h5>
                                        <!-- Descrição da pizza -->
                                        <p class="card-text text-muted small flex-grow-1"><%# Eval("Descricao") %></p>
                                        <!-- Rodapé do card com preço e botão -->
                                        <div class="d-flex justify-content-between align-items-center mt-auto">
                                            <!-- Preço formatado -->
                                            <span class="h5 text-danger fw-bold mb-0">R$ <%# Eval("Preco", "{0:F2}") %></span>
                                            <!-- Botão para adicionar ao carrinho - requer login -->
                                    <asp:LinkButton ID="btnAdicionar" runat="server"
                                        CssClass="btn btn-danger btn-sm"
                                        CommandName="AdicionarCarrinho"
                                        CommandArgument='<%# "P_" + Eval("Id") %>'
                                        data-require-login="true">
                                        <i class="fas fa-cart-plus"></i> Adicionar
                                    </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <!-- Label exibida quando não há pizzas disponíveis -->
                    <asp:Label ID="lblSemProdutos" runat="server" CssClass="alert alert-info text-center" Visible="false">
                        <i class="fas fa-info-circle"></i> Nenhuma pizza disponível no momento. Em breve teremos novidades!
                    </asp:Label>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</asp:Content>