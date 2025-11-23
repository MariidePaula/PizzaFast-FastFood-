<%@ Page Title="Gerenciar Bebidas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Bebidas.aspx.cs" Inherits="FastPizza.Admin.Bebidas" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Importação de fontes do Google Fonts (Poppins) -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600;700&display=swap" rel="stylesheet">
    <!-- Importação dos ícones do Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">

    <style>
        /* Container principal da página administrativa */
        .admin-container {
            font-family: 'Poppins', sans-serif;
            padding: 20px;
            background-color: #f5f5f5;
            min-height: 100vh; /* Garante altura mínima de toda a viewport */
        }
        
        /* Cabeçalho da página administrativa */
        .admin-header {
            background: white;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 20px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1); /* Sombra suave */
            display: flex;
            justify-content: space-between; /* Distribui título e botões nas extremidades */
            align-items: center; /* Alinha verticalmente ao centro */
        }
        
        /* Título do cabeçalho */
        .admin-header h1 {
            margin: 0;
            color: #E64A19; /* Cor laranja/vermelha da marca */
        }
        
        /* Botão "Nova Bebida" */
        .btn-novo {
            background-color: #E64A19;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-weight: 600;
        }
        
        /* Efeito hover do botão */
        .btn-novo:hover {
            background-color: #D84315; /* Tom mais escuro no hover */
        }
        
        /* Container da grid/tabela */
        .grid-container {
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }
        
        /* Estilos da GridView */
        .grid-view {
            width: 100%;
        }
        
        /* Cabeçalho da tabela */
        .grid-view th {
            background-color: #E64A19;
            color: white;
            padding: 12px;
            text-align: left;
        }
        
        /* Células da tabela */
        .grid-view td {
            padding: 10px;
            border-bottom: 1px solid #eee; /* Linha divisória entre linhas */
        }
        
        /* Botões de ação (Editar/Excluir) */
        .btn-action {
            padding: 5px 10px;
            margin: 0 3px;
            border: none;
            border-radius: 3px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
        }
        
        /* Botão Editar (azul) */
        .btn-edit {
            background-color: #2196F3;
            color: white;
        }
        
        /* Botão Excluir (vermelho) */
        .btn-delete {
            background-color: #f44336;
            color: white;
        }
        
        /* Modal de edição/criação */
        .modal {
            display: none; /* Inicialmente escondido */
            position: fixed;
            z-index: 1000; /* Fica acima de todos os elementos */
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5); /* Fundo escuro semi-transparente */
        }
        
        /* Conteúdo do modal */
        .modal-content {
            background-color: white;
            margin: 5% auto; /* Centraliza verticalmente e horizontalmente */
            padding: 30px;
            border-radius: 10px;
            width: 80%;
            max-width: 600px;
            max-height: 80vh; /* Altura máxima de 80% da viewport */
            overflow-y: auto; /* Permite rolagem se o conteúdo for muito grande */
        }
        
        /* Grupo de campo do formulário */
        .form-group {
            margin-bottom: 20px;
        }
        
        /* Label padrão dos campos */
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 600;
        }
        
        /* Label específico para checkbox */
        .form-group label.checkbox-label {
            display: flex;
            align-items: center;
            margin-bottom: 0;
            font-weight: 600;
            cursor: pointer;
            gap: 10px; /* Espaço entre checkbox e texto */
        }
        
        /* Estilo do checkbox */
        .form-group label.checkbox-label input[type="checkbox"] {
            margin: 0;
            width: 18px;
            height: 18px;
            cursor: pointer;
            flex-shrink: 0; /* Impede que o checkbox encolha */
        }
        
        /* Texto do label do checkbox */
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

        /* Efeito de foco no select */
        select.form-control:focus {
            border-color: #E64A19;
            outline: none;
            box-shadow: 0 0 0 0.2rem rgba(230, 74, 25, 0.25); /* Brilho laranja */
        }

        /* Select customizado com seta personalizada */
        select.form-control.custom-select {
            appearance: none; /* Remove a seta padrão do navegador */
            -webkit-appearance: none;
            -moz-appearance: none;
            /* Adiciona seta SVG customizada em laranja */
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23E64A19' d='M6 9L1 4h10z'/%3E%3C/svg%3E");
            background-repeat: no-repeat;
            background-position: right 10px center;
            padding-right: 35px; /* Espaço para a seta */
        }
        
        /* Opções do select */
        select.form-control.custom-select option {
            padding: 8px 10px;
            background-color: white;
            color: #333;
        }

        /* Borda laranja quando o select está em foco */
        select.form-control.custom-select:focus {
            border-color: #E64A19;
        }
        
        /* Container dos botões de ação do formulário */
        .form-actions {
            text-align: right;
            margin-top: 20px;
        }
        
        /* Badge (etiqueta) genérica */
        .badge {
            padding: 5px 10px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 600;
        }
        
        /* Badge verde (sucesso) - para "Disponível: Sim" */
        .badge-success {
            background-color: #4CAF50;
            color: white;
        }
        
        /* Badge vermelho (erro) - para "Disponível: Não" */
        .badge-danger {
            background-color: #f44336;
            color: white;
        }
    </style>

    <!-- Container principal da página -->
    <div class="admin-container">
        <!-- Cabeçalho com título e botões -->
        <div class="admin-header">
            <h1><i class="fas fa-glass-water"></i> Gerenciar Bebidas</h1>
            <div>
                <!-- Link para voltar ao Dashboard -->
                <a href="<%= ResolveUrl("~/Admin/Dashboard.aspx") %>" style="margin-right: 10px; color: #666;">← Voltar</a>
                <!-- Botão para adicionar nova bebida -->
                <asp:Button ID="btnNovo" runat="server" Text="+ Nova Bebida" CssClass="btn-novo" OnClick="btnNovo_Click"
                    CausesValidation="false" />
            </div>
        </div>

        <!-- Container da grade de bebidas -->
        <div class="grid-container">
            <!-- GridView para listar as bebidas -->
            <asp:GridView ID="gridBebidas" runat="server" CssClass="grid-view" AutoGenerateColumns="false"
                OnRowCommand="gridBebidas_RowCommand" OnRowDataBound="gridBebidas_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="Nº">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Imagem">
                        <ItemTemplate>
                            <div style="width: 60px; height: 60px; border-radius: 5px; background: linear-gradient(135deg, #4ecdc4 0%, #44a08d 100%); display: flex; align-items: center; justify-content: center; position: relative; overflow: hidden;">
                                <%# (Eval("ImagemUrl") != null && !string.IsNullOrEmpty(Eval("ImagemUrl").ToString()) && Eval("ImagemUrl").ToString().Trim() != "") ?
                                    "<img src='" + Eval("ImagemUrl") + "' alt='" + Eval("Nome") + "' style='width: 60px; height: 60px; object-fit: cover; border-radius: 5px; position: relative; z-index: 2;' onerror=\"this.style.display='none'; this.parentElement.innerHTML='<i class=\\'fas fa-glass-water\\' style=\\'font-size: 30px; color: white;\\'></i>';\" />" :
                                    "<i class='fas fa-glass-water' style='font-size: 30px; color: white;'></i>" %>
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
                    
                    <asp:TemplateField HeaderText="Ações">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-edit">
                                <i class="fas fa-edit"></i> Editar
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnExcluir" runat="server" CommandName="Excluir"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn-action btn-delete"
                                OnClientClick="return confirm('Tem certeza que deseja excluir esta bebida?');">
                                <i class="fas fa-trash"></i> Excluir
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Modal para criar/editar bebida -->
    <div id="modalBebida" runat="server" class="modal" style="display: none;">
        <div class="modal-content">
            <!-- Título do modal (muda entre "Nova Bebida" e "Editar Bebida") -->
            <h2><asp:Label ID="lblModalTitulo" runat="server"></asp:Label></h2>

            <!-- Campo hidden para armazenar o ID da bebida em edição -->
            <asp:HiddenField ID="hdnBebidaId" runat="server" />

            <!-- Campo Nome -->
            <div class="form-group">
                <label>Nome:</label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control"></asp:TextBox>
                <!-- Validador de campo obrigatório -->
                <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome"
                    ErrorMessage="Nome é obrigatório" Display="Dynamic" CssClass="text-danger" ValidationGroup="Bebida"></asp:RequiredFieldValidator>
            </div>

            <!-- Campo Descrição -->
            <div class="form-group">
                <label>Descrição:</label>
                <asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
            </div>

            <!-- Campo Categoria (DropDownList) -->
            <div class="form-group">
                <label>Categoria:</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-control custom-select">
                    <asp:ListItem Value="Refrigerante" Text="Refrigerante"></asp:ListItem>
                    <asp:ListItem Value="Suco" Text="Suco"></asp:ListItem>
                    <asp:ListItem Value="Água" Text="Água"></asp:ListItem>
                    <asp:ListItem Value="Cerveja" Text="Cerveja"></asp:ListItem>
                    <asp:ListItem Value="Chá" Text="Chá"></asp:ListItem>
                </asp:DropDownList>
                <!-- Validador de campo obrigatório -->
                <asp:RequiredFieldValidator ID="rfvCategoria" runat="server" ControlToValidate="ddlCategoria"
                    ErrorMessage="Categoria é obrigatória" Display="Dynamic" CssClass="text-danger" ValidationGroup="Bebida"></asp:RequiredFieldValidator>
            </div>

            <!-- Campo Preço -->
            <div class="form-group">
                <label>Preço:</label>
                <asp:TextBox ID="txtPreco" runat="server" CssClass="form-control" placeholder="Ex: 39,90"></asp:TextBox>
                <!-- Texto de ajuda -->
                <small class="text-muted" style="display: block; margin-top: 5px;">Use vírgula como separador decimal (ex: 39,90)</small>
                <!-- Validador de campo obrigatório -->
                <asp:RequiredFieldValidator ID="rfvPreco" runat="server" ControlToValidate="txtPreco"
                    ErrorMessage="Preço é obrigatório" Display="Dynamic" CssClass="text-danger" ValidationGroup="Bebida"></asp:RequiredFieldValidator>
            </div>

            <!-- Campo URL da Imagem -->
            <div class="form-group">
                <label>URL da Imagem:</label>
                <asp:TextBox ID="txtImagemUrl" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <!-- Campo Estoque -->
            <div class="form-group">
                <label>Estoque:</label>
                <!-- Campo numérico com valores mínimos e incremento definidos -->
                <asp:TextBox ID="txtEstoque" runat="server" CssClass="form-control" TextMode="Number" min="0" step="1"></asp:TextBox>
                <!-- Validador de campo obrigatório -->
                <asp:RequiredFieldValidator ID="rfvEstoque" runat="server" ControlToValidate="txtEstoque"
                    ErrorMessage="Estoque é obrigatório" Display="Dynamic" CssClass="text-danger" ValidationGroup="Bebida"></asp:RequiredFieldValidator>
            </div>
            
            <!-- Script para reaplica validação após postbacks AJAX -->
            <script>
                // Verifica se o ASP.NET AJAX está disponível
                if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                    // Adiciona handler para quando uma requisição AJAX terminar
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
                        aplicarValidacaoNumeroInteiro();
                    });
                }
            </script>

            <!-- Campo Disponível (Checkbox) -->
            <div class="form-group">
                <label class="checkbox-label">
                    <asp:CheckBox ID="chkDisponivel" runat="server" />
                    <span>Disponível</span>
                </label>
            </div>

            <!-- Botões de ação do formulário -->
            <div class="form-actions">
                <!-- Botão Salvar com validação -->
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" CssClass="btn-novo" OnClick="btnSalvar_Click"
                    ValidationGroup="Bebida" CausesValidation="true" OnClientClick="return ValidarFormularioBebida();" />
                <!-- Botão Cancelar -->
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // Função para validar o formulário da bebida
        function ValidarFormularioBebida() {
            // Obtém referência ao modal
            var modal = document.getElementById('<%= modalBebida.ClientID %>');
            
            // Se o modal não existe ou está escondido, não valida
            if (!modal || modal.style.display === 'none') {
                return false;
            }

            // Executa a validação client-side do ASP.NET
            if (typeof Page_ClientValidate === 'function') {
                var isValid = Page_ClientValidate('Bebida');
                return isValid;
            }

            return true;
        }

        // Quando o DOM estiver carregado
        document.addEventListener('DOMContentLoaded', function() {
            // Obtém o modal
            var modal = document.getElementById('<%= modalBebida.ClientID %>');
            
            if (modal) {
                // Cria um observer para monitorar mudanças no atributo style do modal
                var observer = new MutationObserver(function(mutations) {
                    // Verifica se o modal está visível
                    var isVisible = modal.style.display !== 'none';
                    
                    // Obtém todos os campos do formulário
                    var campos = modal.querySelectorAll('input[type="text"], input[type="number"], select');
                    
                    // Remove o atributo 'required' dos campos quando modal está visível
                    // (a validação é feita pelo ASP.NET validators)
                    campos.forEach(function(campo) {
                        if (isVisible) {
                            campo.removeAttribute('required');
                        }
                    });
                });

                // Inicia a observação do modal
                observer.observe(modal, {
                    attributes: true,
                    attributeFilter: ['style'] // Observa apenas mudanças no atributo style
                });
            }
        });
    </script>

    <script>
        // Quando o DOM estiver carregado
        document.addEventListener('DOMContentLoaded', function () {
            // Verifica se o estilo já foi adicionado (evita duplicação)
            if (!document.getElementById('select-hover-orange-style')) {
                // Cria um elemento de estilo
                var style = document.createElement('style');
                style.id = 'select-hover-orange-style';

                // Define os estilos CSS para customizar as opções do select
                style.textContent = `
                    /* Estilo padrão das opções */
                    select.form-control.custom-select option {
                        background-color: white !important;
                        color: #333 !important;
                        padding: 8px 10px !important;
                    }

                    /* Estilo das opções quando em hover, foco ou ativo */
                    select.form-control.custom-select option:hover,
                    select.form-control.custom-select option:focus,
                    select.form-control.custom-select option:active {
                        background: #E64A19 !important;
                        background-color: #E64A19 !important;
                        color: white !important;
                    }

                    /* Estilo da opção selecionada */
                    select.form-control.custom-select option:checked {
                        background-color: white !important;
                        color: #333 !important;
                    }

                    /* Garante que elementos filhos também tenham o estilo laranja no hover */
                    select.form-control.custom-select option:hover *,
                    select.form-control.custom-select option:focus *,
                    select.form-control.custom-select option:active * {
                        background-color: #E64A19 !important;
                        color: white !important;
                    }
                `;

                // Adiciona o estilo ao head do documento
                document.head.appendChild(style);
            }

            // Obtém todos os selects customizados
            var selects = document.querySelectorAll('select.form-control.custom-select');

            // Para cada select
            selects.forEach(function (select) {
                // Adiciona borda laranja quando ganha foco
                select.addEventListener('focus', function () {
                    this.style.borderColor = '#E64A19';
                });

                // Remove borda laranja quando perde foco
                select.addEventListener('blur', function () {
                    this.style.borderColor = '#ddd';
                });

                // Quando o usuário clica no select
                select.addEventListener('mousedown', function () {
                    // Cria um estilo forçado específico para este select
                    var forceStyle = document.createElement('style');
                    forceStyle.id = 'force-orange-hover-' + select.id;
                    forceStyle.textContent = `
                        #${select.id} option:hover {
                            background: #E64A19 !important;
                            background-color: #E64A19 !important;
                            color: white !important;
                        }
                    `;

                    // Adiciona o estilo se ainda não existir
                    if (!document.getElementById('force-orange-hover-' + select.id)) {
                        document.head.appendChild(forceStyle);
                    }
                });
            });

            // Verifica se o estilo de override ainda não foi adicionado
            if (!document.getElementById('override-blue-select')) {
                // Cria estilo global para sobrescrever o azul padrão do navegador
                var overrideStyle = document.createElement('style');
                overrideStyle.id = 'override-blue-select';
                overrideStyle.textContent = `
                    /* Override global para todas as opções de select */
                    select option:hover,
                    select option:focus,
                    select option:active {
                        background: #E64A19 !important;
                        background-color: #E64A19 !important;
                        color: white !important;
                    }

                    /* Garante que o background laranja seja aplicado no hover */
                    select option:hover {
                        background-color: #E64A19 !important;
                        background: #E64A19 !important;
                    }
                `;

                // Adiciona o estilo ao head do documento
                document.head.appendChild(overrideStyle);
            }
        });
    </script>
</asp:Content>
