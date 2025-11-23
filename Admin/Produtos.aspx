<%@ Page Title="Gerenciar Pizzas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Produtos.aspx.cs" Inherits="FastPizza.Admin.Produtos" %>

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

        /* Cabeçalho com título e botão de nova pizza */
        .admin-header {
            background: white;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 20px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .admin-header h1 {
            margin: 0;
            color: #E64A19;
        }

        /* Botão para adicionar nova pizza */
        .btn-novo {
            background-color: #E64A19;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-weight: 600;
        }

        .btn-novo:hover {
            background-color: #D84315;
        }

        /* Container do grid de produtos */
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

        /* Cabeçalho das colunas do grid */
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

        /* Botões de ação (editar/excluir) */
        .btn-action {
            padding: 5px 10px;
            margin: 0 3px;
            border: none;
            border-radius: 3px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
        }

        /* Botão de editar (azul) */
        .btn-edit {
            background-color: #2196F3;
            color: white;
        }

        /* Botão de excluir (vermelho) */
        .btn-delete {
            background-color: #f44336;
            color: white;
        }

        /* Modal de cadastro/edição */
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
            max-height: 80vh;
            overflow-y: auto;
        }

        /* Grupo de campos do formulário */
        .form-group {
            margin-bottom: 20px;
        }

        /* Label dos campos */
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 600;
        }

        /* Label especial para checkboxes */
        .form-group label.checkbox-label {
            display: flex;
            align-items: center;
            margin-bottom: 0;
            font-weight: 600;
            cursor: pointer;
            gap: 10px;
        }

        /* Checkbox dentro do label */
        .form-group label.checkbox-label input[type="checkbox"] {
            margin: 0;
            width: 18px;
            height: 18px;
            cursor: pointer;
            flex-shrink: 0;
        }

        .form-group label.checkbox-label span,
        .form-group label.checkbox-label label {
            margin: 0;
            cursor: pointer;
        }

        /* Campos de input, textarea e select */
        .form-group input, .form-group textarea, .form-group select {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
        }

        /* Select com foco - borda laranja */
        select.form-control:focus {
            border-color: #E64A19;
            outline: none;
            box-shadow: 0 0 0 0.2rem rgba(230, 74, 25, 0.25);
        }

        /* Select customizado com seta laranja */
        select.form-control.custom-select {
            appearance: none;
            -webkit-appearance: none;
            -moz-appearance: none;
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23E64A19' d='M6 9L1 4h10z'/%3E%3C/svg%3E");
            background-repeat: no-repeat;
            background-position: right 10px center;
            padding-right: 35px;
        }

        /* Opções do select */
        select.form-control.custom-select option {
            padding: 8px 10px;
            background-color: white;
            color: #333;
        }

        select.form-control.custom-select:focus {
            border-color: #E64A19;
        }

        /* Área de botões do formulário */
        .form-actions {
            text-align: right;
            margin-top: 20px;
        }

        /* Badges para status */
        .badge {
            padding: 5px 10px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 600;
        }

        /* Badge verde (status positivo) */
        .badge-success {
            background-color: #4CAF50;
            color: white;
        }

        /* Badge vermelho (status negativo) */
        .badge-danger {
            background-color: #f44336;
            color: white;
        }
    </style>

    <div class="admin-container">
        <!-- Cabeçalho da página administrativa -->
        <div class="admin-header">
            <h1><i class="fas fa-pizza-slice"></i> Gerenciar Pizzas</h1>
            <div>
                <!-- Link para voltar ao dashboard -->
                <a href="<%= ResolveUrl("~/Admin/Dashboard.aspx") %>" style="margin-right: 10px; color: #666;">← Voltar</a>
                <!-- Botão para adicionar nova pizza -->
                <asp:Button ID="btnNovo" runat="server" Text="+ Nova Pizza" CssClass="btn-novo" OnClick="btnNovo_Click"
                    CausesValidation="false" />
            </div>
        </div>

        <!-- Container do grid de produtos -->
        <div class="grid-container">
            <!-- GridView que lista todas as pizzas -->
            <asp:GridView ID="gridProdutos" runat="server" CssClass="grid-view" AutoGenerateColumns="false"
                OnRowCommand="gridProdutos_RowCommand" OnRowDataBound="gridProdutos_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="Nº">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Imagem">
                        <ItemTemplate>
                            <div style="width: 60px; height: 60px; border-radius: 5px; background: linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%); display: flex; align-items: center; justify-content: center; position: relative; overflow: hidden;">
                                <%# (Eval("ImagemUrl") != null && !string.IsNullOrEmpty(Eval("ImagemUrl").ToString()) && Eval("ImagemUrl").ToString().Trim() != "") ?
                                    "<img src='" + Eval("ImagemUrl") + "' alt='" + Eval("Nome") + "' style='width: 60px; height: 60px; object-fit: cover; border-radius: 5px; position: relative; z-index: 2;' onerror=\"this.style.display='none'; this.parentElement.innerHTML='<i class=\\'fas fa-pizza-slice\\' style=\\'font-size: 30px; color: white;\\'></i>';\" />" :
                                    "<i class='fas fa-pizza-slice' style='font-size: 30px; color: white;'></i>" %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:BoundField DataField="Nome" HeaderText="Nome" />
                    <asp:BoundField DataField="Categoria" HeaderText="Categoria" />
                    <asp:BoundField DataField="Preco" HeaderText="Preço" DataFormatString="{0:C}" />
                    <asp:BoundField DataField="Estoque" HeaderText="Estoque" />
                    
                    <asp:TemplateField HeaderText="Disponível">
                        <ItemTemplate>
                            <span class='badge <%# (bool)Eval("Disponivel") ? "badge-success" : "badge-danger" %>'>
                                <%# (bool)Eval("Disponivel") ? "Sim" : "Não" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Em Destaque">
                        <ItemTemplate>
                            <span class='badge <%# (bool)Eval("EmDestaque") ? "badge-success" : "badge-secondary" %>'>
                                <%# (bool)Eval("EmDestaque") ? "Sim" : "Não" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Ações">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-edit">
                                <i class="fas fa-edit"></i> Editar
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnExcluir" runat="server" CommandName="Excluir"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-delete"
                                OnClientClick="return confirm('Tem certeza que deseja excluir esta pizza?');">
                                <i class="fas fa-trash"></i> Excluir
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Modal de cadastro/edição de pizza -->
    <div id="modalProduto" runat="server" class="modal" style="display: none;">
        <div class="modal-content">
            <!-- Título dinâmico (Nova Pizza ou Editar Pizza) -->
            <h2><asp:Label ID="lblModalTitulo" runat="server"></asp:Label></h2>

            <!-- Campo oculto para armazenar ID ao editar -->
            <asp:HiddenField ID="hdnProdutoId" runat="server" />

            <!-- Campo Nome -->
            <div class="form-group">
                <label>Nome:</label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome"
                    ErrorMessage="Nome é obrigatório" Display="Dynamic" CssClass="text-danger" ValidationGroup="Pizza"></asp:RequiredFieldValidator>
            </div>

            <!-- Campo Descrição -->
            <div class="form-group">
                <label>Descrição:</label>
                <asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
            </div>

            <!-- Campo Categoria (dropdown) -->
            <div class="form-group">
                <label>Categoria:</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-control custom-select">
                    <asp:ListItem Value="Clássica" Text="Clássica"></asp:ListItem>
                    <asp:ListItem Value="Tradicional" Text="Tradicional"></asp:ListItem>
                    <asp:ListItem Value="Premium" Text="Premium"></asp:ListItem>
                    <asp:ListItem Value="Vegetariana" Text="Vegetariana"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvCategoria" runat="server" ControlToValidate="ddlCategoria"
                    ErrorMessage="Categoria é obrigatória" Display="Dynamic" CssClass="text-danger" ValidationGroup="Pizza"></asp:RequiredFieldValidator>
            </div>

            <!-- Campo Preço -->
            <div class="form-group">
                <label>Preço:</label>
                <asp:TextBox ID="txtPreco" runat="server" CssClass="form-control" placeholder="Ex: 39,90"></asp:TextBox>
                <small class="text-muted" style="display: block; margin-top: 5px;">Use vírgula como separador decimal (ex: 39,90)</small>
                <asp:RequiredFieldValidator ID="rfvPreco" runat="server" ControlToValidate="txtPreco"
                    ErrorMessage="Preço é obrigatório" Display="Dynamic" CssClass="text-danger" ValidationGroup="Pizza"></asp:RequiredFieldValidator>
            </div>

            <!-- Campo URL da Imagem -->
            <div class="form-group">
                <label>URL da Imagem:</label>
                <asp:TextBox ID="txtImagemUrl" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <!-- Campo Estoque -->
            <div class="form-group">
                <label>Estoque:</label>
                <asp:TextBox ID="txtEstoque" runat="server" CssClass="form-control" TextMode="Number" min="0" step="1"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEstoque" runat="server" ControlToValidate="txtEstoque"
                    ErrorMessage="Estoque é obrigatório" Display="Dynamic" CssClass="text-danger" ValidationGroup="Pizza"></asp:RequiredFieldValidator>
            </div>

            <!-- Script para aplicar validação após postback AJAX -->
            <script>
                if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
                        aplicarValidacaoNumeroInteiro();
                    });
                }
            </script>

            <!-- Checkbox Disponível -->
            <div class="form-group">
                <label class="checkbox-label">
                    <asp:CheckBox ID="chkDisponivel" runat="server" />
                    <span>Disponível</span>
                </label>
            </div>

            <!-- Checkbox Em Destaque -->
            <div class="form-group">
                <label class="checkbox-label">
                    <asp:CheckBox ID="chkEmDestaque" runat="server" />
                    <span>Exibir na Página Inicial</span>
                </label>
                <small class="text-muted" style="display: block; margin-top: 8px; margin-left: 28px;">Marque para exibir este produto na página inicial</small>
            </div>

            <!-- Botões de ação do formulário -->
            <div class="form-actions">
                <!-- Botão Salvar com validação -->
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" CssClass="btn-novo" OnClick="btnSalvar_Click"
                    ValidationGroup="Pizza" CausesValidation="true" OnClientClick="return ValidarFormularioPizza();" />
                <!-- Botão Cancelar -->
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" />
            </div>
        </div>
    </div>

    <!-- Script de validação do formulário -->
    <script type="text/javascript">
        // Valida o formulário apenas se o modal estiver visível
        function ValidarFormularioPizza() {
            var modal = document.getElementById('<%= modalProduto.ClientID %>');
            if (!modal || modal.style.display === 'none') {
                // Modal oculto, não valida
                return false;
            }

            // Executa validação do ASP.NET
            if (typeof Page_ClientValidate === 'function') {
                var isValid = Page_ClientValidate('Pizza');
                return isValid;
            }

            return true;
        }

        // Remove atributo required dos campos quando modal está oculto
        document.addEventListener('DOMContentLoaded', function() {
            var modal = document.getElementById('<%= modalProduto.ClientID %>');
            if (modal) {
                var observer = new MutationObserver(function(mutations) {
                    var isVisible = modal.style.display !== 'none';
                    var campos = modal.querySelectorAll('input[type="text"], input[type="number"], select');
                    campos.forEach(function(campo) {
                        if (isVisible) {
                            campo.removeAttribute('required');
                        }
                    });
                });

                observer.observe(modal, {
                    attributes: true,
                    attributeFilter: ['style']
                });
            }
        });
    </script>

    <!-- Script para estilizar o select com cores laranja ao passar mouse -->
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Adiciona estilos CSS dinâmicos para hover laranja nas opções
            if (!document.getElementById('select-hover-orange-style')) {
                var style = document.createElement('style');
                style.id = 'select-hover-orange-style';
                style.textContent = `
                    /* Estilo padrão das opções */
                    select.form-control.custom-select option {
                        background-color: white !important;
                        color: #333 !important;
                        padding: 8px 10px !important;
                    }

                    /* Hover laranja nas opções */
                    select.form-control.custom-select option:hover,
                    select.form-control.custom-select option:focus,
                    select.form-control.custom-select option:active {
                        background: #E64A19 !important;
                        background-color: #E64A19 !important;
                        color: white !important;
                    }

                    /* Opção selecionada volta ao branco */
                    select.form-control.custom-select option:checked {
                        background-color: white !important;
                        color: #333 !important;
                    }

                    select.form-control.custom-select option:hover *,
                    select.form-control.custom-select option:focus *,
                    select.form-control.custom-select option:active * {
                        background-color: #E64A19 !important;
                        color: white !important;
                    }
                `;
                document.head.appendChild(style);
            }

            // Adiciona eventos de foco/blur nos selects
            var selects = document.querySelectorAll('select.form-control.custom-select');
            selects.forEach(function (select) {
                // Borda laranja ao focar
                select.addEventListener('focus', function () {
                    this.style.borderColor = '#E64A19';
                });
                select.addEventListener('blur', function () {
                    this.style.borderColor = '#ddd';
                });

                // Força estilo laranja ao abrir dropdown
                select.addEventListener('mousedown', function () {
                    var forceStyle = document.createElement('style');
                    forceStyle.id = 'force-orange-hover-' + select.id;
                    forceStyle.textContent = `
                        #${select.id} option:hover {
                            background: #E64A19 !important;
                            background-color: #E64A19 !important;
                            color: white !important;
                        }
                    `;
                    if (!document.getElementById('force-orange-hover-' + select.id)) {
                        document.head.appendChild(forceStyle);
                    }
                });
            });

            // Sobrescreve estilo azul padrão do navegador
            if (!document.getElementById('override-blue-select')) {
                var overrideStyle = document.createElement('style');
                overrideStyle.id = 'override-blue-select';
                overrideStyle.textContent = `
                    /* Força hover laranja em todos os selects */
                    select option:hover,
                    select option:focus,
                    select option:active {
                        background: #E64A19 !important;
                        background-color: #E64A19 !important;
                        color: white !important;
                    }

                    select option:hover {
                        background-color: #E64A19 !important;
                        background: #E64A19 !important;
                    }
                `;
                document.head.appendChild(overrideStyle);
            }
        });
    </script>
</asp:Content>