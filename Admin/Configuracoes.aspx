<%-- Diretiva da Página: Define as configurações básicas da página ASP.NET --%>
<%@ Page 
    Title="Configurações" 
    Language="C#" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="Configuracoes.aspx.cs" 
    Inherits="FastPizza.Admin.Configuracoes" 
%>

<%-- Bloco Content: Conteúdo específico desta página inserido na Master Page --%>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <%-- Importação de fontes e ícones --%>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">

    <%-- Estilos CSS customizados --%>
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
            margin-bottom: 30px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .admin-header h1 {
            margin: 0;
            color: #E64A19; /* Cor laranja da marca */
        }
        
        /* Card de configurações */
        .config-card {
            background: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }

        .config-card h3 {
            color: #333;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #E64A19; /* Linha divisória laranja */
        }
        
        /* Grupo de campos do formulário */
        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            margin-bottom: 8px;
            font-weight: 600;
            color: #333;
        }

        /* Inputs de texto e URL */
        .form-group input[type="text"],
        .form-group input[type="url"] {
            width: 100%;
            padding: 12px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
        }

        /* Efeito de foco nos inputs */
        .form-group input:focus {
            outline: none;
            border-color: #E64A19;
            box-shadow: 0 0 0 3px rgba(230, 74, 25, 0.1);
        }
        
        /* Área de pré-visualização do banner */
        .preview-banner {
            margin-top: 20px;
            border: 2px dashed #ddd; /* Borda tracejada */
            border-radius: 10px;
            padding: 20px;
            text-align: center;
            background-color: #f9f9f9;
            min-height: 200px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        /* Imagem dentro da preview */
        .preview-banner img {
            max-width: 100%;
            max-height: 300px;
            border-radius: 5px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        /* Mensagem quando não há imagem */
        .preview-banner .no-image {
            color: #999;
            font-style: italic;
        }

        /* Botão principal "Salvar" */
        .btn-save {
            background-color: #E64A19;
            color: white;
            padding: 12px 30px;
            border: none;
            border-radius: 5px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        /* Animação do botão ao passar mouse */
        .btn-save:hover {
            background-color: #c0391a;
            transform: translateY(-2px);
            box-shadow: 0 4px 10px rgba(230, 74, 25, 0.3);
        }

        /* Texto de ajuda e dicas */
        .help-text {
            font-size: 12px;
            color: #666;
            margin-top: 5px;
        }
    </style>

    <%-- Conteúdo principal das Configurações --%>
    <div class="admin-container">
        
        <%-- Cabeçalho da página com link de retorno --%>
        <div class="admin-header">
            <h1><i class="fas fa-cog"></i> Configurações</h1>
            <a href="<%= ResolveUrl("~/Admin/Dashboard.aspx") %>" style="color: #666;">← Voltar ao Dashboard</a>
        </div>

        <%-- Card de configuração do banner --%>
        <div class="config-card">
            <h3><i class="fas fa-image"></i> Banner da Página Inicial</h3>

            <%-- Campo para URL da imagem do banner --%>
            <div class="form-group">
                <label for="txtBannerUrl">URL da Imagem do Banner:</label>
                <%-- TextBox com validação HTML5 de URL --%>
                <asp:TextBox ID="txtBannerUrl" runat="server" CssClass="form-control"
                    placeholder="https://exemplo.com/imagem.jpg" TextMode="Url" />
                
                <%-- Área de erro de validação (controlada por JavaScript) --%>
                <div id="urlError" class="help-text" style="color: #dc3545; display: none; margin-top: 5px;">
                    <i class="fas fa-exclamation-circle"></i>
                    <span id="urlErrorText"></span>
                </div>
                
                <%-- Texto explicativo de ajuda --%>
                <p class="help-text">
                    <i class="fas fa-info-circle"></i>
                    Cole aqui a URL completa da imagem que deseja usar como fundo do banner.
                    A URL deve começar com 'http://' ou 'https://' e ser completa.
                    A imagem será exibida com um overlay laranja para manter a legibilidade do texto.
                </p>
                <p class="help-text" style="color: #dc3545; font-weight: 600;">
                    <i class="fas fa-exclamation-triangle"></i>
                    <strong>Importante:</strong> A URL deve estar completa e válida. Exemplo: https://exemplo.com/imagem.jpg
                </p>
            </div>

            <%-- Área de pré-visualização da imagem --%>
            <div class="form-group">
                <label>Preview:</label>
                <div class="preview-banner" id="previewBanner">
                    <div class="no-image">Nenhuma imagem configurada</div>
                </div>
            </div>

            <%-- Botões de ação --%>
            <div class="form-group">
                <%-- Botão Salvar (chama método no code-behind) --%>
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar Configuração"
                    CssClass="btn btn-save" OnClick="btnSalvar_Click" />
                
                <%-- Botão Remover (limpa configuração do banner) --%>
                <asp:Button ID="btnRemover" runat="server" Text="Remover Imagem"
                    CssClass="btn btn-secondary" OnClick="btnRemover_Click"
                    style="margin-left: 10px; background-color: #6c757d; color: white; padding: 12px 30px; border: none; border-radius: 5px; font-weight: 600;" />
            </div>

            <%-- Label para exibir mensagens de feedback (sucesso/erro) --%>
            <asp:Label ID="lblMensagem" runat="server" Visible="false"
                style="display: block; margin-top: 15px; padding: 10px; border-radius: 5px;"></asp:Label>
        </div>
    </div>

    <%-- Script JavaScript para validação e preview dinâmico --%>
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            // Obtém referências aos elementos DOM (usa ClientID para controles ASP.NET)
            var txtBannerUrl = document.getElementById('<%= txtBannerUrl.ClientID %>');
            var previewBanner = document.getElementById('previewBanner');
            var urlError = document.getElementById('urlError');
            var urlErrorText = document.getElementById('urlErrorText');

            // Função para validar URL no front-end
            function validarURL(url) {
                // URL vazia é válida (permite salvar vazio)
                if (!url || url.trim() === '') {
                    return { valido: true, mensagem: '' };
                }

                url = url.trim();

                // Validação 1: Deve começar com http:// ou https://
                if (!url.startsWith('http://') && !url.startsWith('https://')) {
                    return {
                        valido: false,
                        mensagem: 'A URL deve começar com "http://" ou "https://"'
                    };
                }

                // Validação 2: Deve conter pelo menos um ponto (domínio)
                if (url.indexOf('.') === -1) {
                    return {
                        valido: false,
                        mensagem: 'URL incompleta. A URL deve conter um domínio válido (ex: exemplo.com)'
                    };
                }

                // Validação 3: Verifica se há conteúdo após o protocolo
                var partes = url.split('://');
                if (partes.length < 2 || partes[1].trim() === '') {
                    return {
                        valido: false,
                        mensagem: 'URL incompleta. Verifique se a URL está completa'
                    };
                }

                // Validação 4: Verifica tamanho mínimo do domínio
                var dominio = partes[1].split('/')[0];
                if (dominio.length < 3) {
                    return {
                        valido: false,
                        mensagem: 'URL incompleta. O domínio parece estar incompleto'
                    };
                }

                return { valido: true, mensagem: '' };
            }

            // Verifica se elementos existem antes de adicionar listeners
            if (txtBannerUrl && previewBanner && urlError && urlErrorText) {

                // Listener para atualizar preview e validação ao digitar
                txtBannerUrl.addEventListener('input', function () {
                    var url = this.value.trim();
                    var validacao = validarURL(url);

                    // Exibe ou oculta mensagens de erro
                    if (!validacao.valido) {
                        urlError.style.display = 'block';
                        urlErrorText.textContent = validacao.mensagem;
                        txtBannerUrl.style.borderColor = '#dc3545'; // Borda vermelha
                    } else {
                        urlError.style.display = 'none';
                        urlErrorText.textContent = '';
                        txtBannerUrl.style.borderColor = '#ddd'; // Borda padrão
                    }

                    // Atualiza preview da imagem
                    if (url && validacao.valido) {
                        // Insere imagem com fallback para erro
                        previewBanner.innerHTML = '<img src="' + url + '" alt="Preview do Banner" onerror="this.parentElement.innerHTML=\'<div class=\\'no - image\\'>Imagem não encontrada ou URL inválida</div>\';" />';
                    } else {
                        // Mostra mensagem padrão
                        previewBanner.innerHTML = '<div class="no-image">Nenhuma imagem configurada</div>';
                    }
                });

                // Atualiza preview na carga inicial (se já houver valor)
                if (txtBannerUrl.value) {
                    var url = txtBannerUrl.value.trim();
                    var validacao = validarURL(url);
                    if (url && validacao.valido) {
                        previewBanner.innerHTML = '<img src="' + url + '" alt="Preview do Banner" onerror="this.parentElement.innerHTML=\'<div class=\\'no - image\\'>Imagem não encontrada ou URL inválida</div>\';" />';
                    }
                }
            }
        });
    </script>
</asp:Content>