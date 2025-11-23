<%-- Diretiva de página ASP.NET (Web Forms) que define configurações para a página "Meu Perfil", incluindo o título, o idioma (C#), o arquivo Master Page, e o arquivo code-behind (MeuPerfil.aspx.cs). --%>

<%@ Page Title="Meu Perfil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MeuPerfil.aspx.cs" Inherits="FastPizza.MeuPerfil" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container my-5">
        <h1 class="mb-4 fw-bold text-dark">Meu Perfil</h1>

        <asp:Label ID="lblMensagem" runat="server" CssClass="alert alert-success" Visible="false" style="display: block; margin-bottom: 20px;"></asp:Label>
        <asp:Label ID="lblErro" runat="server" CssClass="alert alert-danger" Visible="false" style="display: block; margin-bottom: 20px;"></asp:Label>

        <div class="row">

            <div class="col-lg-8 mb-4">
                <div class="card shadow-sm border-0">
                    <div class="card-body p-4">
                        <h3 class="mb-3 fw-bold text-dark">Informações Pessoais</h3>
                        <p class="text-muted mb-4">Gerencie suas informações de conta e preferências</p>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">
                                <i class="fas fa-user"></i> Nome Completo
                            </label>
                            <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" placeholder="Seu nome" />
                            <asp:RequiredFieldValidator ID="rfvNome" runat="server"
                                ControlToValidate="txtNome"
                                ErrorMessage="O nome é obrigatório!"
                                Display="Dynamic"
                                CssClass="text-danger small"
                                ForeColor="Red" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">
                                <i class="fas fa-envelope"></i> Email
                            </label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="seu@email.com" TextMode="Email" />
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                ControlToValidate="txtEmail"
                                ErrorMessage="O email é obrigatório!"
                                Display="Dynamic"
                                CssClass="text-danger small"
                                ForeColor="Red" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">
                                <i class="fas fa-phone"></i> Telefone
                            </label>
                            <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control" placeholder="(11) 99999-9999" />
                            <asp:RequiredFieldValidator ID="rfvTelefone" runat="server"
                                ControlToValidate="txtTelefone"
                                ErrorMessage="O telefone é obrigatório!"
                                Display="Dynamic"
                                CssClass="text-danger small"
                                ForeColor="Red" />
                        </div>
                        <script>

                            if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
                                    aplicarMascaraTelefone();
                                });
                            }
                        </script>

                        <div class="d-flex gap-2">
                            <asp:Button ID="btnSalvar" runat="server" Text="Salvar Alterações" CssClass="btn btn-danger"
                                OnClick="BtnSalvar_Click" style="background-color: #E64A19; border: none; padding: 10px 20px; font-weight: 600;" />
                            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary"
                                OnClick="BtnCancelar_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-4 mb-4">
                <div class="card shadow-sm border-0">
                    <div class="card-body p-4">
                        <h4 class="mb-3 fw-bold text-dark">Alterar Senha</h4>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">Senha Atual</label>
                            <asp:TextBox ID="txtSenhaAtual" runat="server" CssClass="form-control" TextMode="Password" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">Nova Senha</label>
                            <asp:TextBox ID="txtNovaSenha" runat="server" CssClass="form-control" TextMode="Password" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">Confirmar Nova Senha</label>
                            <asp:TextBox ID="txtConfirmarNovaSenha" runat="server" CssClass="form-control" TextMode="Password" />
                        </div>

                        <asp:Button ID="btnAlterarSenha" runat="server" Text="Alterar Senha" CssClass="btn btn-outline-danger w-100"
                            OnClick="BtnAlterarSenha_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-4">
            <div class="col-12">
                <div class="card shadow-sm border-0">
                    <div class="card-body p-4">
                        <div class="d-flex justify-content-between align-items-center mb-4">
                            <h3 class="mb-0 fw-bold text-dark">Meus Endereços</h3>
                            <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalEndereco"
                                style="background-color: #E64A19; border: none;">
                                <i class="fas fa-plus"></i> Adicionar Endereço
                            </button>
                        </div>

                        <asp:Repeater ID="rptEnderecos" runat="server" OnItemCommand="RptEnderecos_ItemCommand">
                            <ItemTemplate>
                                <div class="card mb-3 border">
                                    <div class="card-body">
                                        <div class="d-flex justify-content-between align-items-start">
                                            <div class="flex-grow-1">
                                                <%# Eval("EnderecoCompleto") %>
                                                <asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("EnderecoPadrao") %>'>
                                                    <span class="badge bg-danger ms-2">Padrão</span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("Referencia") != null && !string.IsNullOrEmpty(Eval("Referencia").ToString()) %>'>
                                                    <br/><small class="text-muted"><i class="fas fa-info-circle"></i> <%# Eval("Referencia") %></small>
                                                </asp:PlaceHolder>
                                            </div>
                                            <div class="btn-group">
                                                <asp:LinkButton ID="btnEditar" runat="server"
                                                    CssClass="btn btn-sm btn-outline-primary"
                                                    CommandName="Editar"
                                                    CommandArgument='<%# Eval("Id") %>'>
                                                    <i class="fas fa-edit"></i>
                                                </asp:LinkButton>
                                                <%# !(bool)Eval("EnderecoPadrao") ? "<asp:LinkButton ID='btnPadrao' runat='server' CssClass='btn btn-sm btn-outline-success' CommandName='DefinirPadrao' CommandArgument='" + Eval("Id") + "'><i class='fas fa-star'></i></asp:LinkButton>" : "" %>
                                                <asp:LinkButton ID="btnExcluir" runat="server"
                                                    CssClass="btn btn-sm btn-outline-danger"
                                                    CommandName="Excluir"
                                                    CommandArgument='<%# Eval("Id") %>'
                                                    OnClientClick="return confirm('Tem certeza que deseja excluir este endereço?');">
                                                    <i class="fas fa-trash"></i>
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <asp:Panel ID="pnlSemEnderecos" runat="server" Visible="false">
                            <div class="text-center py-5">
                                <i class="fas fa-map-marker-alt fa-3x text-muted mb-3"></i>
                                <p class="text-muted">Você ainda não cadastrou nenhum endereço.</p>
                                <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalEndereco"
                                    style="background-color: #E64A19; border: none;">
                                    <i class="fas fa-plus"></i> Adicionar Primeiro Endereço
                                </button>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalEndereco" tabindex="-1" aria-labelledby="modalEnderecoLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalEnderecoLabel">
                        <asp:Label ID="lblTituloModal" runat="server" Text="Adicionar Endereço"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnEnderecoId" runat="server" Value="0" />

                    <div class="mb-3">
                        <label class="form-label fw-semibold">Rua:</label>
                        <asp:TextBox ID="txtRua" runat="server" CssClass="form-control" placeholder="Nome da rua" />
                    </div>

                    <div class="row mb-3">
                        <div class="col-md-6">
                            <label class="form-label fw-semibold">Número:</label>
                            <asp:TextBox ID="txtNumero" runat="server" CssClass="form-control" placeholder="123" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-semibold">Complemento:</label>
                            <asp:TextBox ID="txtComplemento" runat="server" CssClass="form-control" placeholder="Apto, Bloco, etc" />
                        </div>
                    </div>

                    <div class="mb-3">
                        <label class="form-label fw-semibold">Bairro:</label>
                        <asp:TextBox ID="txtBairro" runat="server" CssClass="form-control" placeholder="Nome do bairro" />
                    </div>

                    <div class="row mb-3">
                        <div class="col-md-8">
                            <label class="form-label fw-semibold">Cidade:</label>
                            <asp:TextBox ID="txtCidade" runat="server" CssClass="form-control" placeholder="Cidade" />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label fw-semibold">CEP:</label>
                            <asp:TextBox ID="txtCEP" runat="server" CssClass="form-control" placeholder="00000-000" />
                        </div>
                    </div>

                    <div class="mb-3">
                        <label class="form-label fw-semibold">Referência:</label>
                        <asp:TextBox ID="txtReferencia" runat="server" CssClass="form-control" placeholder="Ponto de referência (opcional)" />
                    </div>

                    <div class="form-check mb-3" style="display: flex; align-items: center; gap: 10px;">
                        <asp:CheckBox ID="chkEnderecoPadrao" runat="server" CssClass="form-check-input" style="margin: 0; width: 18px; height: 18px; flex-shrink: 0; cursor: pointer;" />
                        <label class="form-check-label" for="<%= chkEnderecoPadrao.ClientID %>" style="margin: 0; cursor: pointer;">
                            Definir como endereço padrão
                        </label>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnSalvarEndereco" runat="server" Text="Salvar" CssClass="btn btn-danger"
                        OnClick="BtnSalvarEndereco_Click" style="background-color: #E64A19; border: none;" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

