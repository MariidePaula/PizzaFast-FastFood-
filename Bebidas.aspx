<%@ Page Title="Bebidas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Bebidas.aspx.cs" Inherits="FastPizza.Bebidas" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Modal de Login - Aparece quando usuário não autenticado tenta adicionar bebida ao carrinho -->
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
                    <!-- Label para mensagens de erro do login -->
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
                    <!-- Botão de login -->
                    <asp:Button ID="btnLoginModal" runat="server" Text="Entrar" CssClass="btn btn-danger w-100"
                        OnClick="btnLoginModal_Click" style="background-color: #E64A19; border: none; padding: 10px; font-weight: 600;" />
                </div>
            </div>
        </div>
    </div>

    <div class="container my-5">
        <!-- Cabeçalho da página com título e subtítulo -->
        <div class="row mb-4">
            <div class="col-12 text-center">
                <h1 class="display-4 fw-bold text-dark mb-3"><i class="fas fa-glass-water"></i> Bebidas</h1>
                <p class="lead text-muted">Refresque-se com nossas bebidas geladas</p>
            </div>
        </div>

        <!-- Grid de bebidas - 4 colunas em desktop, 3 em tablet, 2 em mobile -->
        <div class="row g-4">
            <!-- Repeater para renderizar lista de bebidas -->
            <asp:Repeater ID="rptBebidas" runat="server" OnItemCommand="rptBebidas_ItemCommand">
                <ItemTemplate>
                    <!-- Card individual de bebida - ID único para cada bebida -->
                    <div class="col-lg-3 col-md-4 col-sm-6" id='card-bebida-<%# Eval("Id") %>'>
                        <div class="card h-100 shadow-sm border-0">
                            <!-- Container da imagem com gradiente de fundo (tons de azul/verde) -->
                            <div class="position-relative" style="height: 200px; overflow: hidden; background: linear-gradient(135deg, #4ecdc4 0%, #44a08d 100%); display: flex; align-items: center; justify-content: center;">
                                <!-- Renderiza imagem se disponível, senão mostra ícone de copo -->
                                <%# (Eval("ImagemUrl") != null && !string.IsNullOrEmpty(Eval("ImagemUrl").ToString()) && Eval("ImagemUrl").ToString().Trim() != "") ?
                                    "<img src='" + Eval("ImagemUrl") + "' alt='" + Eval("Nome") + "' class='card-img-top h-100 w-100' style='object-fit: cover; position: absolute; top: 0; left: 0; width: 100%; height: 100%;' onerror=\"this.style.display='none'; this.parentElement.innerHTML+='<i class=\\'fas fa-glass-water\\' style=\\'font-size: 80px; color: white; opacity: 0.9;\\'></i>';\" />" :
                                    "<i class='fas fa-glass-water' style='font-size: 80px; color: white; opacity: 0.9;'></i>" %>
                                
                                <!-- Badge de categoria no canto superior direito -->
                                <span class="badge bg-info text-white position-absolute top-0 end-0 m-2" style="z-index: 10;">
                                    <%# Eval("Categoria") %>
                                </span>
                            </div>
                            
                            <!-- Conteúdo do card -->
                            <div class="card-body d-flex flex-column">
                                <!-- Nome da bebida -->
                                <h5 class="card-title fw-bold mb-2"><%# Eval("Nome") %></h5>
                                
                                <!-- Descrição da bebida - flex-grow-1 para ocupar espaço disponível -->
                                <p class="card-text text-muted small flex-grow-1"><%# Eval("Descricao") %></p>
                                
                                <!-- Linha com preço e botão de adicionar -->
                                <div class="d-flex justify-content-between align-items-center mt-auto">
                                    <!-- Preço formatado em reais -->
                                    <span class="h5 text-danger fw-bold mb-0">R$ <%# Eval("Preco", "{0:F2}") %></span>
                                    
                                    <!-- Botão adicionar ao carrinho - CommandArgument com prefixo B_ (Bebida) -->
                                    <asp:LinkButton ID="btnAdicionar" runat="server"
                                        CssClass="btn btn-danger btn-sm"
                                        CommandName="AdicionarCarrinho"
                                        data-require-login="true"
                                        CommandArgument='<%# "B_" + Eval("Id") %>'>
                                        <i class="fas fa-cart-plus"></i> Adicionar
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <!-- Label exibida quando não há bebidas disponíveis -->
            <asp:Label ID="lblSemBebidas" runat="server" CssClass="alert alert-info text-center" Visible="false">
                <i class="fas fa-info-circle"></i> Nenhuma bebida disponível no momento. Em breve teremos novidades!
            </asp:Label>
        </div>
    </div>

</asp:Content>