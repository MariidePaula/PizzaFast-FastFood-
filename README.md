# Fast Pizza - Sistema de Pedidos Online (Fast Food)

Sistema web completo de pedidos de pizza desenvolvido em ASP.NET Web Forms com CRUD de produtos e bebidas, gerenciamento de clientes, pedidos, bloqueio de usuários e painel administrativo completo.

---

# 🎯 Objetivo do Projeto (Atividade Prática)

Este projeto foi desenvolvido para atender **integralmente** aos requisitos da atividade "E-commerce de Fast Food", incluindo:

- Área do cliente com cadastro, login, cardápio, carrinho, pedidos e perfil.
- Área administrativa com CRUD de produtos, bebidas, clientes e pedidos.
- Controle de sessão, bloqueio de clientes e segurança com hashing.
- Organização em camadas (Apresentação, Negócio, Dados).
- Banco de dados com Entity Framework e migração automática.

---

# 📌 Decisões Técnicas

As principais escolhas técnicas foram:

### 🔹 ASP.NET Web Forms (.NET Framework 4.8)
Escolhido por ser o framework solicitado na atividade e permitir desenvolvimento rápido com páginas .aspx.

### 🔹 Entity Framework 6.5.1
Usado como ORM por facilitar:
- criação automática do banco,
- mapping simples,
- inicialização com dados (seed),
- consultas com LINQ.

### 🔹 Arquitetura em Camadas
O projeto foi dividido em:
- **Presentation:** Páginas ASPX e MasterPage  
- **Business:** Regras de negócio e validações  
- **DataAccess:** DAOs e DbContext  
- **Models:** Entidades  
- **Utils:** Funções auxiliares (hash, imagem)  

Isso melhora organização e mantém responsabilidades separadas.

### 🔹 Hash de Senha (SHA256)
Implementado para cumprir os requisitos de segurança e garantir que senhas nunca fiquem em texto puro.

### 🔹 Sessão para Controle de Acesso
- `Session["Admin"]` controla acesso às páginas administrativas.  
- `Session["Cliente"]` controla login do cliente.  
- Clientes bloqueados têm login impedido.

---

# 🧩 Fluxo Geral do Sistema

### 👤 Cliente
1. Cadastra-se ou faz login  
2. Acessa cardápio de pizzas e bebidas  
3. Adiciona itens ao carrinho  
4. Finaliza pedido (simulado)  
5. Acompanha status:  
   - Pendente → Em Preparo → Entregue  
6. Gerencia seu perfil e endereços  
7. Consulta histórico de pedidos  

### 🛠️ Administrador
1. Faz login na área admin  
2. Acessa o dashboard com estatísticas  
3. Gerencia produtos e bebidas (CRUD)  
4. Gerencia pedidos  
5. Altera status dos pedidos  
6. Cancela pedidos com motivo  
7. Edita clientes  
8. Bloqueia / desbloqueia clientes  
9. Altera banner da página inicial  

---

# 📋 Requisitos para Execução

- Visual Studio 2019 ou superior  
- .NET Framework 4.8  
- SQL Server LocalDB ou SQL Express  
- Navegador atualizado  

---

# 🚀 Como Rodar o Projeto

1. Abra o arquivo `FastPizza.sln` no Visual Studio  
2. Clique com botão direito → **Restore NuGet Packages**  
3. Abra `Web.config` e confirme a connection string  
4. Pressione **F5** para executar o projeto  
5. O banco será criado automaticamente com dados iniciais  

---

# 🔐 Credenciais de Acesso

### Administrador:
- **Email:** admin@email.com  
- **Senha:** admin123  
- **URL:** /Admin/Login.aspx  

### Clientes de Teste:
- joao@email.com — 123456  
- maria@email.com — maria123  

OBS: Também é possível fazer seu cadastro!
---

# 📁 Estrutura do Projeto

```
FastPizza/
├── Admin/               # Área administrativa
├── Business/            # Regras de negócio
├── DataAccess/          # EF + DAOs
├── Models/              # Entidades
├── Images/              # Imagens
├── Utils/               # Hash, mapeador
├── Default.aspx         # Página inicial
├── Cardapio.aspx        # Cardápio
├── Login.aspx           # Login do cliente
├── Cadastro.aspx        # Cadastro
└── MeusPedidos.aspx     # Histórico
```

---

# 🧠 Requisitos da Atividade (Checklist)

### ✔️ CRUD Completo
- Produtos  
- Bebidas  
- Clientes  
- Pedidos  

### ✔️ Usabilidade e Interface
- Layout responsivo com Bootstrap 5  
- Interface clara para cliente e admin  

### ✔️ Organização e Arquitetura
- Projeto em camadas  
- Pastas nomeadas corretamente  
- DAOs separados  

### ✔️ Segurança
- Hash SHA256  
- Verificação de sessão  
- Bloqueio de clientes  
- Emails únicos  

### ✔️ Documentação
- README completo  
- Estrutura explicada  
- Fluxos e decisões técnicas descritos  

---

# 🔒 Segurança Implementada

- Senhas com hashing SHA256  
- Bloqueio de login para clientes bloqueados  
- Proteção de páginas admin por sessão  
- Validações em formulários  
- Email único por cliente  
- Sanitização de inputs simples  

---

# 🗄️ Banco de Dados

Criado automaticamente via EF.

**Tabelas:**  
Clientes, Produtos, Bebidas, Pedidos, PedidoItens, Enderecos, Configuracoes  

**Dados iniciais:**  
- 12 pizzas  
- 11 bebidas  
- 2 clientes de teste  

---

# 📸 Imagens e Mapeamento

- Images/Pizzas/  
- Images/Bebidas/  
- Images/Banners/  

O ImageMapper encontra a imagem automaticamente pelo nome.

---

# 🛠️ Tecnologias Utilizadas

- ASP.NET Web Forms  
- C# (Framework 4.8)  
- Entity Framework 6.5.1  
- SQL Server LocalDB  
- Bootstrap 5.3  
- Font Awesome  
- SHA256  

---

# 🛟 Suporte e Problemas Comuns

Verificar:
- NuGet restaurado  
- LocalDB instalado  
- Porta não utilizada  
- ConnectionString correta  
- Pastas de imagens presentes  

Se o projeto não rodar em outro PC:
- excluir **bin/**, **obj/**, **.vs/** e `.csproj.user`  

---

**Projeto desenvolvido para fins educacionais.** 🎓
