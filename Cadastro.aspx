<%@ Page Title="Cadastro" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cadastro.aspx.cs" Inherits="FastPizza.Cadastro" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Container principal da página de cadastro -->
    <div class="container my-5">
        <!-- Row para centralizar o conteúdo -->
        <div class="row justify-content-center">
            <!-- Coluna responsiva - metade da largura em telas médias -->
            <div class="col-md-6">
                <!-- Card principal do formulário -->
                <div class="card shadow-sm border-0">
                    <!-- Corpo do card com padding grande -->
                    <div class="card-body p-5">
                        <!-- Título da página de cadastro -->
                        <h2 class="text-center mb-4 fw-bold text-dark">Criar Conta</h2>

                        <!-- Label para mensagens de erro - inicialmente oculta -->
                        <asp:Label ID="lblMensagem" runat="server" CssClass="alert alert-danger" Visible="false" style="display: block; margin-bottom: 20px;"></asp:Label>
                        <!-- Label para mensagens de sucesso - inicialmente oculta -->
                        <asp:Label ID="lblSucesso" runat="server" CssClass="alert alert-success" Visible="false" style="display: block; margin-bottom: 20px;"></asp:Label>

                        <!-- Campo Nome Completo -->
                        <div class="mb-3">
                            <label class="form-label fw-semibold">Nome Completo:</label>
                            <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" placeholder="Seu nome completo" />
                            <!-- Validação obrigatória para o campo nome -->
                            <asp:RequiredFieldValidator ID="rfvNome" runat="server"
                                ControlToValidate="txtNome"
                                ErrorMessage="O nome é obrigatório!"
                                Display="Dynamic"
                                CssClass="text-danger small"
                                ForeColor="Red" />
                        </div>

                        <!-- Campo Email -->
                        <div class="mb-3">
                            <label class="form-label fw-semibold">Email:</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="seu@email.com" TextMode="Email" />
                            <!-- Validação obrigatória para o campo email -->
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                ControlToValidate="txtEmail"
                                ErrorMessage="O email é obrigatório!"
                                Display="Dynamic"
                                CssClass="text-danger small"
                                ForeColor="Red" />
                            <!-- Validação de formato de email usando expressão regular -->
                            <asp:RegularExpressionValidator ID="revEmail" runat="server"
                                ControlToValidate="txtEmail"
                                ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
                                ErrorMessage="Por favor, insira um email válido!"
                                Display="Dynamic"
                                CssClass="text-danger small"
                                ForeColor="Red" />
                        </div>

                        <!-- Campo Telefone -->
                        <div class="mb-3">
                            <label class="form-label fw-semibold">Telefone:</label>
                            <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control" placeholder="(11) 99999-9999" />
                            <!-- Validação obrigatória para o campo telefone -->
                            <asp:RequiredFieldValidator ID="rfvTelefone" runat="server"
                                ControlToValidate="txtTelefone"
                                ErrorMessage="O telefone é obrigatório!"
                                Display="Dynamic"
                                CssClass="text-danger small"
                                ForeColor="Red" />
                        </div>
                        <!-- Script para aplicar máscara de telefone -->
                        <script>
                            // Verifica se o framework ASP.NET AJAX está disponível e configura evento
                            if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                                // Adiciona função que será executada após cada requisição AJAX (UpdatePanel)
                                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                                    aplicarMascaraTelefone(); // Reaplica a máscara após atualizações parciais
                                });
                            }
                        </script>

                        <!-- Campo Senha -->
                        <div class="mb-3">
                            <label class="form-label fw-semibold">Senha:</label>
                            <asp:TextBox ID="txtSenha" runat="server" CssClass="form-control" TextMode="Password" placeholder="Mínimo 6 caracteres" />
                            <!-- Validação obrigatória para o campo senha -->
                            <asp:RequiredFieldValidator ID="rfvSenha" runat="server"
                                ControlToValidate="txtSenha"
                                ErrorMessage="A senha é obrigatória!"
                                Display="Dynamic"
                                CssClass="text-danger small"
                                ForeColor="Red" />
                        </div>

                        <!-- Campo Confirmar Senha -->
                        <div class="mb-3">
                            <label class="form-label fw-semibold">Confirmar Senha:</label>
                            <asp:TextBox ID="txtConfirmarSenha" runat="server" CssClass="form-control" TextMode="Password" placeholder="Digite a senha novamente" />
                            <!-- Validação obrigatória para confirmação de senha -->
                            <asp:RequiredFieldValidator ID="rfvConfirmarSenha" runat="server"
                                ControlToValidate="txtConfirmarSenha"
                                ErrorMessage="A confirmação de senha é obrigatória!"
                                Display="Dynamic"
                                CssClass="text-danger small"
                                ForeColor="Red" />
                        </div>

                        <!-- Botão de cadastro - dispara o evento btnCadastrar_Click no code-behind -->
                        <asp:Button ID="btnCadastrar" runat="server" Text="Cadastrar" CssClass="btn btn-danger w-100 mb-3"
                            OnClick="btnCadastrar_Click" style="background-color: #E64A19; border: none; padding: 12px; font-size: 16px; font-weight: 600;" />

                        <!-- Link para página de login caso usuário já tenha conta -->
                        <div class="text-center">
                            <p class="text-muted">Já tem uma conta? <a href="<%= ResolveUrl("~/Login.aspx") %>" class="text-danger fw-semibold">Faça login aqui</a></p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>