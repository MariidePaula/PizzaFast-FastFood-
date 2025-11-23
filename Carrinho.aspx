<%@ Page Title="Meu Carrinho" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Carrinho.aspx.cs" Inherits="FastPizza.Carrinho" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CONTAINER PRINCIPAL -->
    <div class="container my-5">

        <!-- TÍTULO -->
        <div class="row mb-4">
            <div class="col-12">
                <h2 class="fw-bold">
                    <i class="fas fa-shopping-cart"></i> Seu Carrinho
                </h2>
                <hr />
            </div>
        </div>

        <div class="row">

            <!-- COLUNA DA LISTA DO CARRINHO -->
            <div class="col-lg-8 col-md-7 mb-4">
                <div class="card shadow-sm">
                    <div class="card-body p-0">

                        <!-- GRID DO CARRINHO -->
                        <asp:GridView ID="gridCarrinho" runat="server"
                            CssClass="table table-hover align-middle mb-0"
                            AutoGenerateColumns="false"
                            OnRowCommand="gridCarrinho_RowCommand"
                            EmptyDataText="Seu carrinho está vazio :(">

                            <Columns>
                                <asp:TemplateField HeaderText="Item" HeaderStyle-CssClass="fw-bold ps-4" ItemStyle-CssClass="ps-4">
                                    <ItemTemplate>
                                        <span><%# Eval("NomeCompleto") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="PrecoUnitario" HeaderText="Unitário"
                                    DataFormatString="{0:C}"
                                    HeaderStyle-CssClass="fw-bold text-center"
                                    ItemStyle-CssClass="text-center" />

                                <asp:TemplateField HeaderText="Quantidade" HeaderStyle-CssClass="fw-bold text-center" ItemStyle-CssClass="text-center">
                                    <ItemTemplate>
                                        <div class="d-flex align-items-center justify-content-center gap-2">

                                            <!-- DIMINUIR QUANTIDADE -->
                                            <asp:LinkButton ID="btnMenos" runat="server"
                                                CssClass="btn btn-outline-danger btn-sm"
                                                CommandName="Diminuir"
                                                CommandArgument='<%# Eval("ProdutoId") %>'
                                                style="min-width: 35px;">
                                                <i class="fas fa-minus"></i>
                                            </asp:LinkButton>

                                            <!-- QUANTIDADE ATUAL -->
                                            <span class="fw-bold" style="min-width: 30px;">
                                                <%# Eval("Quantidade") %>
                                            </span>

                                            <!-- AUMENTAR QUANTIDADE -->
                                            <asp:LinkButton ID="btnMais" runat="server"
                                                CssClass="btn btn-outline-success btn-sm"
                                                CommandName="Aumentar"
                                                CommandArgument='<%# Eval("ProdutoId") %>'
                                                style="min-width: 35px;">
                                                <i class="fas fa-plus"></i>
                                            </asp:LinkButton>

                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="Subtotal" HeaderText="Subtotal"
                                    DataFormatString="{0:C}"
                                    HeaderStyle-CssClass="fw-bold text-end pe-4"
                                    ItemStyle-CssClass="text-end pe-4 fw-bold text-dark" />

                            </Columns>
                        </asp:GridView>

                    </div>
                </div>
            </div>

            <!-- COLUNA DO RESUMO DO PEDIDO -->
            <div class="col-lg-4 col-md-5">

                <div class="card shadow-sm sticky-top" style="top: 96px;">
                    <div class="card-body">

                        <!-- TÍTULO -->
                        <h4 class="card-title fw-bold mb-4">Resumo do Pedido</h4>

                        <!-- TOTAL -->
                        <div class="d-flex justify-content-between mb-4 pb-3 border-bottom">
                            <span class="fs-5 fw-bold">Total Geral:</span>

                            <!-- TOTAL CALCULADO -->
                            <asp:Label ID="lblTotal" runat="server"
                                Text="R$ 0,00"
                                CssClass="fs-4 fw-bold text-danger"></asp:Label>
                        </div>

                        <!-- OBSERVAÇÕES DO PEDIDO (OPCIONAL) -->
                        <div class="mb-3">
                            <asp:Label ID="lblObservacoes" runat="server"
                                AssociatedControlID="txtObservacoes"
                                CssClass="form-label fw-semibold">
                                <i class="fas fa-sticky-note"></i> Observações do Pedido
                            </asp:Label>

                            <asp:TextBox ID="txtObservacoes" runat="server"
                                CssClass="form-control"
                                TextMode="MultiLine"
                                Rows="4"
                                placeholder="Ex: Tirar cebola da pizza, sem pimenta..."
                                MaxLength="1000"
                                CausesValidation="false"></asp:TextBox>

                            <small class="text-muted">Opcional</small>
                        </div>

                        <!-- BOTÃO FINALIZAR PEDIDO (IGNORE VALIDAÇÃO) -->
                        <asp:Button ID="btnFinalizar" runat="server"
                            Text="Finalizar Pedido"
                            CssClass="btn btn-success btn-lg w-100 mb-3"
                            OnClick="btnFinalizar_Click"
                            CausesValidation="false"
                            OnClientClick="return true;" />

                        <!-- VOLTAR AO CARDÁPIO -->
                        <a href="<%= ResolveUrl("~/Default.aspx") %>" class="btn btn-outline-secondary w-100">
                            <i class="fas fa-arrow-left"></i> Continuar Comprando
                        </a>

                    </div>
                </div>

                <!-- MENSAGEM DE SUCESSO APÓS FINALIZAR PEDIDO -->
                <asp:Panel ID="pnlMensagem" runat="server"
                    Visible="false"
                    CssClass="alert alert-success mt-3 shadow-sm">
                    <h5 class="alert-heading">
                        <i class="fas fa-check-circle"></i> Pedido Finalizado!
                    </h5>
                    <p class="mb-0">Seu pedido foi enviado para a cozinha.</p>
                    <hr>
                    <p class="mb-0"><strong>Tempo estimado:</strong> 40 minutos</p>
                </asp:Panel>

            </div>
        </div>
    </div>

    <!-- SCRIPT PARA GARANTIR QUE O BOTÃO FINALIZAR FUNCIONE -->
    <script type="text/javascript">

        // Função que "libera" o botão Finalizar Pedido
        function inicializarBotaoFinalizar() {

            // Pega o botão na página
            var btnFinalizar = document.getElementById('<%= btnFinalizar.ClientID %>');

            if (btnFinalizar) {

                // Remove qualquer validação que bloqueie o clique
                btnFinalizar.removeAttribute('required');

                // Cria uma marcação para evitar falhas
                btnFinalizar.setAttribute('data-finalizar-pedido', 'true');

                // Log do clique – ajuda a debugar
                btnFinalizar.addEventListener('click', function(e) {
                    return true; // Permite postback
                });

                // Permite envio do formulário via postback
                var form = document.getElementById('form1');
                if (form) {
                    form.addEventListener('submit', function(e) {

                        var submitButton = document.activeElement || e.submitter;

                        // Só libera se o botão clicado for o Finalizar
                        if (submitButton &&
                            (submitButton.id === '<%= btnFinalizar.ClientID %>' ||
                             submitButton.name === '<%= btnFinalizar.UniqueID %>')) {

                            return true;
                        }
                    });
                }

            }
        }

        // Inicializa quando a página carrega
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', inicializarBotaoFinalizar);
        } else {
            inicializarBotaoFinalizar();
        }

        // Reaplica depois de postbacks AJAX do ASP.NET
        if (typeof Sys !== 'undefined') {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function() {
                inicializarBotaoFinalizar();
            });
        }

        // Garantia extra (caso o botão perca evento)
        setTimeout(function() {
            var btnFinalizar = document.getElementById('<%= btnFinalizar.ClientID %>');
            if (btnFinalizar && !btnFinalizar.getAttribute('data-finalizar-pedido')) {
                inicializarBotaoFinalizar();
            }
        }, 500);

    </script>

</asp:Content>
