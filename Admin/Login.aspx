<%-- Arquivo de marcação visual (HTML e ASP.NET Markup) para o Login da Área Administrativa, apresentando um formulário centralizado para inserção de usuário e senha, com um script JavaScript para desabilitar a validação automática do HTML5 no campo de usuário. --%>
<%@ Page 
    Title="Login Administrador"         
    Language="C#"                        
    MasterPageFile="~/Site.Master"       
    AutoEventWireup="true"               
    CodeBehind="Login.aspx.cs"           
    Inherits="FastPizza.Admin.Login"     
    EnableViewState="false"              
%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <%-- Caixa branca central com sombra (formulário de login) --%>
    <div style="max-width: 400px; margin: 100px auto; padding: 30px; background: white; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">

        <%-- Título do login --%>
        <h2 style="text-align: center; color: #E64A19; margin-bottom: 30px;">
            <i class="fas fa-lock"></i> Área do Administrador
        </h2>

        <%-- Label para mostrar erros (ex: senha errada). Começa invisível. --%>
        <asp:Label ID="lblMensagem" runat="server"
            CssClass="alert alert-danger"
            Visible="false"
            style="display: block; margin-bottom: 20px;">
        </asp:Label>

        <%-- Campo de Usuário (email do admin) --%>
        <div style="margin-bottom: 20px;">
            <label style="display: block; margin-bottom: 5px; font-weight: 600;">Usuário:</label>

            <%-- Caixa de texto para digitar o usuário --%>
            <asp:TextBox ID="txtUsuario" runat="server"
                CssClass="form-control"
                placeholder="admin@email.com"
                TextMode="SingleLine" />
        </div>

        <%-- Campo de Senha --%>
        <div style="margin-bottom: 20px;">
            <label style="display: block; margin-bottom: 5px; font-weight: 600;">Senha:</label>

            <%-- Caixa de texto em modo senha --%>
            <asp:TextBox ID="txtSenha" runat="server"
                TextMode="Password"
                CssClass="form-control"
                placeholder="••••••••" />
        </div>

        <div style="text-align: center;">
            <asp:Button ID="btnLogin" runat="server"
                Text="Entrar"
                CssClass="btn btn-primary"
                OnClick="btnLogin_Click"
                style="background-color: #E64A19; border: none; padding: 12px; font-size: 16px; font-weight: 600; min-width: 200px;" />
        </div>

        <%-- Mensagem de rodapé --%>
        <div style="margin-top: 20px; text-align: center; color: #666; font-size: 12px;">
            <p><i class="fas fa-shield-alt"></i> Área restrita para administradores</p>
        </div>
    </div>

    <%-- Script para desativar validações automáticas do HTML5 --%>
    <script type="text/javascript">
        (function () {
            function configurarValidacao() {

                // Pega o campo usuário gerado pelo ASP.NET
                var txtUsuario = document.getElementById('<%= txtUsuario.ClientID %>');

                // Pega o formulário principal da página
                var form = document.getElementById('form1');

                // Se achou os elementos
                if (txtUsuario && form) {

                    // Define o campo como tipo texto (não 'email')
                    txtUsuario.type = 'text';

                    // Remove qualquer validação automática do HTML5
                    txtUsuario.removeAttribute('inputmode');
                    txtUsuario.removeAttribute('pattern');
                    txtUsuario.removeAttribute('required');

                    // Desativa validação do formulário
                    if (!form.hasAttribute('novalidate')) {
                        form.setAttribute('novalidate', 'novalidate');
                    }

                    // Remove mensagens de erro que o campo pode gerar
                    txtUsuario.addEventListener('input', function (e) {
                        e.target.setCustomValidity('');
                    });

                    txtUsuario.addEventListener('blur', function (e) {
                        e.target.setCustomValidity('');
                    });
                }
            }

            // Executa quando a página terminar de carregar
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', configurarValidacao);
            } else {
                setTimeout(configurarValidacao, 50);
            }
        })();
    </script>

</asp:Content>