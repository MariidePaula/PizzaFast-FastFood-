<%-- Arquivo de marcação (view) que define o layout, estilização e a estrutura dinâmica (Repeaters) para listar o histórico de pedidos do usuário. --%>

<%@ Page Title="Meus Pedidos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MeusPedidos.aspx.cs" Inherits="FastPizza.MeusPedidos" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .pedidos-container {
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
        }
        .pedido-card {
            background: white;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            position: relative;
        }
        .pedido-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 15px;
        }
        .pedido-numero {
            font-size: 18px;
            font-weight: 700;
            color: #333;
            margin: 0;
        }
        .pedido-data {
            font-size: 14px;
            color: #666;
            margin-top: 5px;
        }
        .status-badge {
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }
        .status-pendente {
            background-color: #FFA726;
            color: white;
        }
        .status-preparo {
            background-color: #42A5F5;
            color: white;
        }
        .status-entrega {
            background-color: #FF9800;
            color: white;
        }
        .status-entregue {
            background-color: #66BB6A;
            color: white;
        }
        .status-cancelado {
            background-color: #EF5350;
            color: white;
        }
        .itens-section {
            margin-bottom: 15px;
        }
        .itens-titulo {
            font-size: 14px;
            font-weight: 600;
            color: #333;
            margin-bottom: 10px;
        }
        .item-lista {
            list-style: none;
            padding: 0;
            margin: 0;
        }
        .item-lista li {
            font-size: 14px;
            color: #555;
            margin-bottom: 5px;
            padding-left: 20px;
            position: relative;
        }
        .item-lista li:before {
            content: "•";
            position: absolute;
            left: 0;
            color: #999;
        }
        .pedido-total {
            font-size: 18px;
            font-weight: 700;
            color: #E64A19;
            margin-top: 10px;
        }
    </style>

    <div class="pedidos-container">
        <asp:Label ID="lblTitulo" runat="server" CssClass="fw-bold mb-4" style="color: #333; font-size: 24px; display: block;">
            Meus Pedidos
        </asp:Label>

        <asp:Panel ID="pnlNotificacoes" runat="server" Visible="false" CssClass="alert alert-warning alert-dismissible fade show mb-4" role="alert">
            <asp:Label ID="lblNotificacao" runat="server"></asp:Label>
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </asp:Panel>

        <asp:Panel ID="pnlSemPedidos" runat="server" Visible="false">
            <div class="text-center" style="min-height: 60vh; display: flex; flex-direction: column; justify-content: center; align-items: center; padding: 60px 20px;">
                <i class="fas fa-shopping-bag" style="font-size: 80px; color: #6c757d; margin-bottom: 30px;"></i>
                <h4 style="color: #6c757d; font-weight: 600; margin-bottom: 15px; font-size: 20px;">Você ainda não realizou nenhum pedido</h4>
                <p style="color: #6c757d; font-size: 16px; margin-bottom: 30px;">Que tal começar a pedir suas pizzas favoritas?</p>
                <a href="<%= ResolveUrl("~/Cardapio.aspx") %>" class="btn btn-danger btn-lg" style="background-color: #E64A19; border: none; padding: 12px 30px; font-weight: 600;">
                    <i class="fas fa-pizza-slice"></i> Ver Cardápio
                </a>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlComPedidos" runat="server" Visible="false">
            <asp:Repeater ID="rptPedidos" runat="server">
                <ItemTemplate>
                    <div class="pedido-card">
                        <div class="pedido-header">
                            <div>
                                <h3 class="pedido-numero">Pedido</h3>
                                <div class="pedido-data"><%# GetDataFormatada(Eval("DataPedido")) %></div>
                            </div>
                            <span class='status-badge <%# GetStatusBadgeClass(Eval("Status")) %>'>
                                <i class='<%# GetStatusIcon(Eval("Status")) %>'></i>
                                <%# GetStatusTexto(Eval("Status")) %>
                            </span>
                        </div>

                        <div class="itens-section">
                            <div class="itens-titulo">Itens:</div>
                            <ul class="item-lista">
                                <asp:Repeater ID="rptItens" runat="server" DataSource='<%# Eval("Itens") %>'>
                                    <ItemTemplate>
                                        <li><%# Eval("NomeProduto") %></li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>

                        <asp:Panel ID="pnlMotivoCancelamento" runat="server" Visible='<%# MostrarMotivoCancelamento(Eval("Status"), Eval("MotivoCancelamento")) %>'
                            CssClass="motivo-cancelamento" style="margin-top: 15px; padding: 12px; background-color: #FFF3CD; border-left: 4px solid #EF5350; border-radius: 4px;">
                            <div style="font-size: 13px; font-weight: 600; color: #856404; margin-bottom: 5px;">
                                <i class="fas fa-info-circle"></i> Motivo do Cancelamento:
                            </div>
                            <div style="font-size: 14px; color: #856404;">
                                <%# GetMotivoCancelamentoTexto(Eval("MotivoCancelamento")) %>
                            </div>
                        </asp:Panel>

                        <div class="pedido-total">Total: R$ <%# Eval("Total", "{0:F2}") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </div>

</asp:Content>

