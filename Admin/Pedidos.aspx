<%@ Page Title="Gerenciar Pedidos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pedidos.aspx.cs" Inherits="FastPizza.Admin.Pedidos" %>

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

        /* Cabeçalho da página */
        .admin-header {
            background: white;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 20px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .admin-header h1 {
            margin: 0;
            color: #E64A19;
        }

        /* Container do grid de pedidos */
        .grid-container {
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        /* Estilos do GridView */
        .grid-view {
            width: 100%;
        }

        /* Cabeçalho das colunas */
        .grid-view th {
            background-color: #E64A19;
            color: white;
            padding: 12px;
            text-align: left;
        }

        /* Células do grid */
        .grid-view td {
            padding: 10px;
            border-bottom: 1px solid #eee;
        }

        /* Badges de status - estilos base */
        .badge {
            padding: 5px 10px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 600;
        }

        /* Badges específicos por status */
        .badge-pendente { background-color: #FF9800; color: white; }
        .badge-preparo { background-color: #2196F3; color: white; }
        .badge-entrega { background-color: #FF9800; color: white; }
        .badge-entregue { background-color: #4CAF50; color: white; }
        .badge-cancelado { background-color: #f44336; color: white; }

        /* Botões de ação do grid */
        .btn-action {
            padding: 5px 10px;
            margin: 0 3px;
            border: none;
            border-radius: 3px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            font-size: 12px;
        }

        .btn-primary { background-color: #2196F3; color: white; }
        .btn-danger { background-color: #f44336; color: white; }

        /* Modal de detalhes/edição */
        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
        }

        /* Conteúdo do modal */
        .modal-content {
            background-color: white;
            margin: 5% auto;
            padding: 30px;
            border-radius: 10px;
            width: 80%;
            max-width: 600px;
        }

        /* Grupos de campos do formulário */
        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 600;
        }

        .form-group select, .form-group textarea {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
        }

        /* Container de itens do pedido */
        .itens-pedido {
            margin-top: 20px;
            padding: 15px;
            background-color: #f9f9f9;
            border-radius: 5px;
        }

        /* Item individual do pedido */
        .item-pedido {
            display: flex;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid #ddd;
        }

        .btn-success {
            background-color: #4CAF50;
            color: white;
        }
    </style>

    <!-- Script JavaScript para calcular total do pedido em edição -->
    <script>
        // Calcula o total somando subtotais de todos os itens
        function calcularTotalEdicao() {
            var total = 0;
            var itens = document.querySelectorAll('.item-pedido');

            // Percorre cada item e calcula subtotal (quantidade * preço)
            itens.forEach(function (itemDiv) {
                var precoSpan = itemDiv.querySelector('.subtotal-item');
                var qtdInput = itemDiv.querySelector('input[type="number"].qtd-item');

                if (precoSpan && qtdInput) {
                    var preco = parseFloat(precoSpan.getAttribute('data-preco')) || 0;
                    var quantidade = parseInt(qtdInput.value) || 0;
                    var subtotal = quantidade * preco;

                    // Atualiza subtotal do item
                    precoSpan.textContent = subtotal.toFixed(2);
                    total += subtotal;
                }
            });

            // Atualiza total geral
            var lblTotal = document.getElementById('<%= lblTotalEdicao.ClientID %>');
            if (lblTotal) {
                lblTotal.textContent = 'R$ ' + total.toFixed(2);
            }
        }

        // Inicializa eventos de mudança de quantidade nos inputs
        function inicializarCalculoTotal() {
            var inputs = document.querySelectorAll('input[type="number"].qtd-item');
            inputs.forEach(function (input) {
                // Remove eventos antigos para evitar duplicação
                input.removeEventListener('change', calcularTotalEdicao);
                input.removeEventListener('input', calcularTotalEdicao);
                // Adiciona eventos para recalcular ao mudar quantidade
                input.addEventListener('change', calcularTotalEdicao);
                input.addEventListener('input', calcularTotalEdicao);
            });
            calcularTotalEdicao();
        }

        // Inicializa ao carregar página
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', inicializarCalculoTotal);
        } else {
            inicializarCalculoTotal();
        }
    </script>

    <div class="admin-container">
        <!-- Cabeçalho da página -->
        <div class="admin-header">
            <h1><i class="fas fa-shopping-bag"></i> Gerenciar Pedidos</h1>
            <a href="<%= ResolveUrl("~/Admin/Dashboard.aspx") %>" style="color: #666;">← Voltar ao Dashboard</a>
        </div>

        <!-- Grid de pedidos -->
        <div class="grid-container">
            <asp:GridView ID="gridPedidos" runat="server" CssClass="grid-view" AutoGenerateColumns="false"
                OnRowCommand="gridPedidos_RowCommand" OnRowDataBound="gridPedidos_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" />
                    
                    <asp:BoundField DataField="DataPedido" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='badge badge-<%# GetStatusClass(Eval("Status")) %>'>
                                <%# GetStatusTexto(Eval("Status")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" />
                    
                    <asp:TemplateField HeaderText="Ações">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDetalhes" runat="server" CommandName="Detalhes"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-primary">
                                <i class="fas fa-eye"></i> Detalhes
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-primary">
                                <i class="fas fa-edit"></i> Editar
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnAlterarStatus" runat="server" CommandName="AlterarStatus"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-primary">
                                <i class="fas fa-edit"></i> Status
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnCancelar" runat="server" CommandName="Cancelar"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-danger"
                                OnClientClick="return confirm('Tem certeza que deseja cancelar este pedido?');">
                                <i class="fas fa-times"></i> Cancelar
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnExcluir" runat="server" CommandName="Excluir"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-danger"
                                OnClientClick="return confirm('Tem certeza que deseja EXCLUIR este pedido? Esta ação não pode ser desfeita!');">
                                <i class="fas fa-trash"></i> Excluir
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Modal multifuncional (detalhes, editar, alterar status, cancelar) -->
    <div id="modalPedido" runat="server" class="modal" style="display: none;">
        <div class="modal-content">
            <!-- Título dinâmico do modal -->
            <h2><asp:Label ID="lblModalTitulo" runat="server"></asp:Label></h2>

            <!-- Campo oculto para armazenar ID do pedido -->
            <asp:HiddenField ID="hdnPedidoId" runat="server" />

            <!-- Painel de Detalhes do Pedido -->
            <asp:Panel ID="pnlDetalhes" runat="server">
                <div class="form-group">
                    <label>Cliente:</label>
                    <asp:Label ID="lblCliente" runat="server"></asp:Label>
                </div>
                <div class="form-group">
                    <label>Data do Pedido:</label>
                    <asp:Label ID="lblDataPedido" runat="server"></asp:Label>
                </div>
                <div class="form-group">
                    <label>Status Atual:</label>
                    <asp:Label ID="lblStatusAtual" runat="server"></asp:Label>
                </div>
                <div class="form-group">
                    <label>Total:</label>
                    <asp:Label ID="lblTotal" runat="server" style="font-size: 20px; font-weight: 700; color: #E64A19;"></asp:Label>
                </div>

                <!-- Painel de observações (visível apenas se houver observações) -->
                <asp:Panel ID="pnlObservacoes" runat="server" Visible="false" CssClass="form-group" style="background-color: #fff3cd; padding: 15px; border-radius: 5px; border-left: 4px solid #ffc107; margin-bottom: 20px;">
                    <label style="font-weight: 700; color: #856404;">
                        <i class="fas fa-sticky-note"></i> Observações do Cliente:
                    </label>
                    <asp:Label ID="lblObservacoes" runat="server" style="display: block; margin-top: 8px; color: #856404; font-style: italic; white-space: pre-wrap;"></asp:Label>
                </asp:Panel>

                <!-- Lista de itens do pedido -->
                <div class="itens-pedido">
                    <h4>Itens do Pedido:</h4>
                    
                    <!-- Mensagem quando não há itens -->
                    <asp:PlaceHolder ID="phItensVazios" runat="server" Visible="false">
                        <div class="item-pedido">
                            <p class="text-muted">Nenhum item encontrado para este pedido.</p>
                        </div>
                    </asp:PlaceHolder>
                    
                    <!-- Repeater de itens do pedido -->
                    <asp:Repeater ID="rptItens" runat="server">
                        <ItemTemplate>
                            <div class="item-pedido">
                                <div>
                                    <strong><%# Eval("NomeProduto") %></strong><br />
                                    Quantidade: <%# Eval("Quantidade") %> x R$ <%# Eval("PrecoUnitario", "{0:F2}") %>
                                </div>
                                <div>
                                    <strong>R$ <%# Eval("Subtotal", "{0:F2}") %></strong>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <!-- Painel de Alterar Status -->
            <asp:Panel ID="pnlAlterarStatus" runat="server" Visible="false">
                <div class="form-group">
                    <label>Novo Status:</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                        <asp:ListItem Value="1" Text="Pendente"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Em Preparo"></asp:ListItem>
                        <asp:ListItem Value="3" Text="Saiu para Entrega"></asp:ListItem>
                        <asp:ListItem Value="4" Text="Entregue"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </asp:Panel>

            <!-- Painel de Cancelamento -->
            <asp:Panel ID="pnlCancelar" runat="server" Visible="false">
                <div class="form-group">
                    <label>Motivo do Cancelamento:</label>
                    <asp:TextBox ID="txtMotivoCancelamento" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                </div>
            </asp:Panel>

            <!-- Painel de Edição do Pedido -->
            <asp:Panel ID="pnlEditarPedido" runat="server" Visible="false">
                <div class="form-group">
                    <label>Itens do Pedido:</label>
                    <div id="divItensEdicao" runat="server">
                        <!-- Repeater de itens em edição -->
                        <asp:Repeater ID="rptItensEdicao" runat="server" OnItemCommand="rptItensEdicao_ItemCommand">
                            <ItemTemplate>
                                <div class="item-pedido" style="display: flex; align-items: center; gap: 10px; margin-bottom: 10px; padding: 10px; background: #f9f9f9; border-radius: 5px;">
                                    <!-- Nome e preço do produto -->
                                    <div style="flex: 1;">
                                        <strong><%# Eval("NomeProduto") %></strong><br />
                                        <small>R$ <%# Eval("PrecoUnitario", "{0:F2}") %> cada</small>
                                    </div>
                                    
                                    <!-- Controles de quantidade e remoção -->
                                    <div style="display: flex; align-items: center; gap: 10px;">
                                        <label>Qtd:</label>
                                        <!-- Input de quantidade com eventos JavaScript -->
                                        <asp:TextBox ID="txtQuantidade" runat="server" Text='<%# Eval("Quantidade") %>'
                                            TextMode="Number" min="1" style="width: 60px; padding: 5px;"
                                            CssClass="qtd-item" data-index='<%# Container.ItemIndex %>' />
                                        
                                        <!-- HiddenFields para armazenar dados do item -->
                                        <asp:HiddenField ID="hdnItemId" runat="server" Value='<%# Eval("ProdutoId") %>' />
                                        <asp:HiddenField ID="hdnItemNome" runat="server" Value='<%# Eval("NomeProduto") %>' />
                                        <asp:HiddenField ID="hdnItemPreco" runat="server" Value='<%# Eval("PrecoUnitario") %>' />
                                        
                                        <!-- Botão remover item -->
                                        <asp:LinkButton ID="btnRemoverItem" runat="server" CommandName="Remover"
                                            CommandArgument='<%# Container.ItemIndex %>'
                                            CssClass="btn-action btn-danger" style="padding: 5px 10px;">
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </div>
                                    
                                    <!-- Subtotal do item (atualizado via JavaScript) -->
                                    <div style="min-width: 100px; text-align: right;">
                                        <strong>R$ <span class="subtotal-item" data-preco='<%# Eval("PrecoUnitario") %>' data-index='<%# Container.ItemIndex %>'><%# Eval("Subtotal", "{0:F2}") %></span></strong>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <!-- Seção para adicionar novos produtos -->
                <div class="form-group">
                    <label>Adicionar Produto:</label>
                    <div style="display: flex; gap: 10px; margin-bottom: 10px;">
                        <!-- Dropdown de produtos disponíveis -->
                        <asp:DropDownList ID="ddlProdutos" runat="server" CssClass="form-control" style="flex: 1;"></asp:DropDownList>
                        <!-- Input de quantidade do novo produto -->
                        <asp:TextBox ID="txtQuantidadeNovo" runat="server" TextMode="Number" min="1"
                            Text="1" style="width: 80px; padding: 5px;" />
                        <!-- Botão adicionar -->
                        <asp:Button ID="btnAdicionarItem" runat="server" Text="Adicionar"
                            CssClass="btn-action btn-primary" OnClick="btnAdicionarItem_Click" />
                    </div>
                </div>

                <!-- Total do pedido em edição -->
                <div class="form-group" style="border-top: 2px solid #E64A19; padding-top: 15px; margin-top: 20px;">
                    <div style="display: flex; justify-content: space-between; align-items: center;">
                        <label style="font-size: 18px; font-weight: 700;">Total do Pedido:</label>
                        <asp:Label ID="lblTotalEdicao" runat="server" style="font-size: 24px; font-weight: 700; color: #E64A19;"></asp:Label>
                    </div>
                </div>
            </asp:Panel>

            <!-- Botões de ação do modal (visibilidade controlada pelo code-behind) -->
            <div style="text-align: right; margin-top: 20px;">
                <!-- Botão salvar edição -->
                <asp:Button ID="btnSalvarEdicao" runat="server" Text="Salvar Alterações" CssClass="btn-action btn-primary" OnClick="btnSalvarEdicao_Click" Visible="false" />
                <!-- Botão salvar status -->
                <asp:Button ID="btnSalvarStatus" runat="server" Text="Salvar" CssClass="btn-action btn-primary" OnClick="btnSalvarStatus_Click" />
                <!-- Botão confirmar cancelamento -->
                <asp:Button ID="btnCancelarPedido" runat="server" Text="Confirmar Cancelamento" CssClass="btn-action btn-danger" OnClick="btnCancelarPedido_Click" />
                <!-- Botão fechar modal -->
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" CssClass="btn-action" OnClick="btnFechar_Click" />
            </div>
        </div>
    </div>
</asp:Content>