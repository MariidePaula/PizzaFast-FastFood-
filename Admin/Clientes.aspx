<%@ Page Title="Gerenciar Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="FastPizza.Admin.Clientes" %>

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

        /* Container do grid de clientes */
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

        /* Badges de status - estilo base */
        .badge {
            padding: 5px 10px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 600;
        }

        /* Badge verde (Ativo) */
        .badge-success {
            background-color: #4CAF50;
            color: white;
        }

        /* Badge vermelho (Bloqueado) */
        .badge-danger {
            background-color: #f44336;
            color: white;
        }

        /* Botões de ação no grid */
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

        /* Botão editar (azul) */
        .btn-edit {
            background-color: #2196F3;
            color: white;
        }

        /* Botão bloquear (laranja) */
        .btn-block {
            background-color: #FF9800;
            color: white;
        }

        /* Botão desbloquear (verde) */
        .btn-unblock {
            background-color: #4CAF50;
            color: white;
        }

        /* Botão excluir (vermelho) */
        .btn-delete {
            background-color: #f44336;
            color: white;
        }

        /* Modal de edição */
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
            margin: 3% auto;
            padding: 30px;
            border-radius: 10px;
            width: 90%;
            max-width: 900px;
            max-height: 90vh;
            overflow-y: auto;
        }

        /* Grupo de campos do formulário */
        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 600;
        }

        .form-group input {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
        }

        /* Área de botões do formulário */
        .form-actions {
            text-align: right;
            margin-top: 20px;
        }

        /* Badges adicionais para status de pedidos */
        .badge-warning {
            background-color: #FF9800;
            color: white;
        }

        .badge-info {
            background-color: #2196F3;
            color: white;
        }

        .badge-primary {
            background-color: #2196F3;
            color: white;
        }

        .badge-secondary {
            background-color: #6c757d;
            color: white;
        }

        /* Linha divisória laranja */
        hr {
            border: none;
            border-top: 2px solid #E64A19;
            margin: 30px 0;
        }
    </style>

    <div class="admin-container">
        <!-- Cabeçalho da página -->
        <div class="admin-header">
            <h1><i class="fas fa-users"></i> Gerenciar Clientes</h1>
            <a href="<%= ResolveUrl("~/Admin/Dashboard.aspx") %>" style="color: #666;">← Voltar ao Dashboard</a>
        </div>

        <!-- Grid de clientes -->
        <div class="grid-container">
            <asp:GridView ID="gridClientes" runat="server" CssClass="grid-view" AutoGenerateColumns="false"
                OnRowCommand="gridClientes_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Nome" HeaderText="Nome" />
                    
                    <asp:BoundField DataField="Email" HeaderText="E-mail" />
                    
                    <asp:BoundField DataField="Telefone" HeaderText="Telefone" />
                    
                    <asp:BoundField DataField="Cidade" HeaderText="Cidade" />
                    
                    <asp:BoundField DataField="DataCadastro" HeaderText="Data Cadastro" DataFormatString="{0:dd/MM/yyyy}" />
                    
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='badge <%# (bool)Eval("Bloqueado") ? "badge-danger" : "badge-success" %>'>
                                <%# (bool)Eval("Bloqueado") ? "Bloqueado" : "Ativo" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Ações">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-edit">
                                <i class="fas fa-edit"></i> Editar
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnBloquear" runat="server" CommandName="Bloquear"
                                CommandArgument='<%# Eval("Id") %>'
                                CssClass='<%# (bool)Eval("Bloqueado") ? "btn-action btn-unblock" : "btn-action btn-block" %>'
                                Text='<%# (bool)Eval("Bloqueado") ? "<i class=\"fas fa-unlock\"></i> Desbloquear" : "<i class=\"fas fa-lock\"></i> Bloquear" %>'>
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnExcluir" runat="server" CommandName="Excluir"
                                CommandArgument='<%# Eval("Id") %>'
                                CssClass="btn-action btn-delete"
                                OnClientClick="return confirm('Tem certeza que deseja excluir permanentemente este cliente? Esta ação não pode ser desfeita e excluirá todos os pedidos e endereços associados.');">
                                <i class="fas fa-trash"></i> Excluir
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Modal de edição de cliente -->
    <div id="modalCliente" runat="server" class="modal" style="display: none;">
        <div class="modal-content">
            <h2>Editar Cliente</h2>

            <!-- Campo oculto para armazenar ID do cliente -->
            <asp:HiddenField ID="hdnClienteId" runat="server" />

            <!-- Campo Nome -->
            <div class="form-group">
                <label>Nome:</label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" required="true"></asp:TextBox>
            </div>

            <!-- Campo E-mail -->
            <div class="form-group">
                <label>E-mail:</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" required="true"></asp:TextBox>
            </div>

            <!-- Campo Telefone com máscara -->
            <div class="form-group">
                <label>Telefone:</label>
                <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control" placeholder="(11) 99999-9999"></asp:TextBox>
            </div>

            <!-- Script para aplicar máscara após postback AJAX -->
            <script>
                if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                        aplicarMascaraTelefone();
                    });
                }
            </script>

            <!-- Campo Endereço -->
            <div class="form-group">
                <label>Endereço:</label>
                <asp:TextBox ID="txtEndereco" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <!-- Campo Cidade -->
            <div class="form-group">
                <label>Cidade:</label>
                <asp:TextBox ID="txtCidade" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <!-- Campo CEP -->
            <div class="form-group">
                <label>CEP:</label>
                <asp:TextBox ID="txtCEP" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <!-- Linha divisória -->
            <hr style="margin: 30px 0; border-color: #ddd;" />

            <!-- Seção de pedidos do cliente -->
            <h3 style="margin-bottom: 20px; color: #E64A19;">
                <i class="fas fa-shopping-bag"></i> Pedidos do Cliente
            </h3>

            <!-- Área scrollável com lista de pedidos -->
            <div style="max-height: 600px; min-height: 400px; overflow-y: auto; border: 1px solid #ddd; border-radius: 5px; padding: 15px;">
                <!-- Mensagem quando não há pedidos -->
                <asp:PlaceHolder ID="phPedidosVazios" runat="server" Visible="false">
                    <p class="text-muted" style="text-align: center; padding: 20px;">
                        <i class="fas fa-inbox"></i><br />
                        Este cliente ainda não possui pedidos.
                    </p>
                </asp:PlaceHolder>

                <!-- Repeater de pedidos do cliente -->
                <asp:Repeater ID="rptPedidosCliente" runat="server" OnItemCommand="rptPedidosCliente_ItemCommand">
                    <ItemTemplate>
                        <!-- Card de pedido individual -->
                        <div style="border: 1px solid #eee; border-radius: 5px; padding: 15px; margin-bottom: 10px; background-color: #f9f9f9;">
                            <!-- Cabeçalho do pedido com data, status e total -->
                            <div style="display: flex; justify-content: space-between; align-items: start; margin-bottom: 10px;">
                                <div>
                                    <strong>Pedido</strong><br />
                                    <small class="text-muted">Data: <%# Eval("DataPedido", "{0:dd/MM/yyyy HH:mm}") %></small>
                                </div>
                                <div style="text-align: right;">
                                    <!-- Badge de status do pedido -->
                                    <span class='badge <%# GetStatusClass(Eval("Status")) %>' style="margin-bottom: 5px; display: block;">
                                        <%# GetStatusTexto(Eval("Status")) %>
                                    </span>
                                    <!-- Total do pedido -->
                                    <strong style="color: #E64A19;">R$ <%# Eval("Total", "{0:F2}") %></strong>
                                </div>
                            </div>

                            <!-- Botões de ação do pedido -->
                            <div style="margin-top: 10px;">
                                <!-- Botão Editar Pedido -->
                                <asp:LinkButton ID="btnEditarPedido" runat="server"
                                    CommandName="EditarPedido"
                                    CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn-action btn-edit"
                                    style="font-size: 12px; padding: 5px 10px;">
                                    <i class="fas fa-edit"></i> Editar Pedido
                                </asp:LinkButton>
                                
                                <!-- Botão Ver Detalhes -->
                                <asp:LinkButton ID="btnVerDetalhes" runat="server"
                                    CommandName="VerDetalhes"
                                    CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn-action"
                                    style="font-size: 12px; padding: 5px 10px; background-color: #6c757d; color: white;">
                                    <i class="fas fa-eye"></i> Ver Detalhes
                                </asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <!-- Botões de ação do modal de cliente -->
            <div class="form-actions">
                <!-- Botão Salvar -->
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar Alterações" CssClass="btn-action btn-edit" OnClick="btnSalvar_Click" />
                <!-- Botão Cancelar -->
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn-action" OnClick="btnCancelar_Click" />
            </div>
        </div>
    </div>

    <!-- Modal de edição rápida de pedido -->
    <div id="modalPedido" runat="server" class="modal" style="display: none;">
        <div class="modal-content" style="max-width: 700px;">
            <h2>Editar Pedido</h2>

            <!-- Campo oculto para armazenar ID do pedido -->
            <asp:HiddenField ID="hdnPedidoId" runat="server" />

            <!-- Campo Status do Pedido -->
            <div class="form-group">
                <label>Status:</label>
                <asp:DropDownList ID="ddlStatusPedido" runat="server" CssClass="form-control">
                    <asp:ListItem Value="1" Text="Pendente"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Em Preparo"></asp:ListItem>
                    <asp:ListItem Value="3" Text="Saiu para Entrega"></asp:ListItem>
                    <asp:ListItem Value="4" Text="Entregue"></asp:ListItem>
                    <asp:ListItem Value="5" Text="Cancelado"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <!-- Campo Total do Pedido -->
            <div class="form-group">
                <label>Total do Pedido:</label>
                <asp:TextBox ID="txtTotalPedido" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
            </div>

            <!-- Campo Motivo de Cancelamento (opcional) -->
            <div class="form-group">
                <label>Motivo do Cancelamento (se aplicável):</label>
                <asp:TextBox ID="txtMotivoCancelamento" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>

            <!-- Botões de ação do modal de pedido -->
            <div class="form-actions">
                <!-- Botão Salvar Pedido -->
                <asp:Button ID="btnSalvarPedido" runat="server" Text="Salvar Pedido" CssClass="btn-action btn-edit" OnClick="btnSalvarPedido_Click" />
                <!-- Botão Cancelar -->
                <asp:Button ID="btnCancelarPedido" runat="server" Text="Cancelar" CssClass="btn-action" OnClick="btnCancelarPedido_Click" />
            </div>
        </div>
    </div>
</asp:Content>