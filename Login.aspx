<%-- Arquivo de marcação (view) que define o layout e a estrutura do formulário de Login, com campos para email, senha e botões de ação. --%>

<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="FastPizza.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container my-5">
        <div class="row justify-content-center">
            <div class="col-md-5">
                <div class="card shadow-sm border-0">
                    <div class="card-body p-5">
                        <h2 class="text-center mb-4 fw-bold text-dark">Entrar</h2>

                        <asp:Label ID="lblMensagem" runat="server" CssClass="alert alert-danger" Visible="false" style="display: block; margin-bottom: 20px;"></asp:Label>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">Email:</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="seu@email.com" TextMode="Email" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">Senha:</label>
                            <asp:TextBox ID="txtSenha" runat="server" CssClass="form-control" TextMode="Password" placeholder="••••••••" />
                        </div>

                        <asp:Button ID="btnLogin" runat="server" Text="Entrar" CssClass="btn btn-danger w-100 mb-3"
                            OnClick="btnLogin_Click" style="background-color: #E64A19; border: none; padding: 12px; font-size: 16px; font-weight: 600;" />

                        <div class="text-center mb-3">
                            <p class="text-muted">Não tem uma conta? <a href="<%= ResolveUrl("~/Cadastro.aspx") %>" class="text-danger fw-semibold">Cadastre-se aqui</a></p>
                        </div>

                        <hr class="my-4" />

                        <div class="text-center">
                            <asp:Button ID="btnAdmin" runat="server" Text="Área do Administrador" CssClass="btn btn-outline-secondary w-100"
                                OnClick="btnAdmin_Click" style="padding: 10px; font-size: 14px;" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

