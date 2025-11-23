# 🚀 Guia de Setup - Fast Pizza

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- ✅ **Visual Studio 2019 ou superior** (Community, Professional ou Enterprise)
- ✅ **.NET Framework 4.8** (geralmente vem com Visual Studio)
- ✅ **SQL Server LocalDB** (vem com Visual Studio, mas pode ser instalado separadamente)

## 🔧 Instalação Passo a Passo

### 1. Baixar/Clonar o Projeto

```bash
# Se usar Git
git clone [url-do-repositorio]
cd pizzafast

# Ou extraia o arquivo ZIP em uma pasta
```

### 2. Abrir o Projeto no Visual Studio

1. Abra o Visual Studio
2. File → Open → Project/Solution
3. Navegue até a pasta do projeto
4. Selecione `FastPizza.sln`

### 3. Restaurar Pacotes NuGet

**Opção A - Pelo Visual Studio:**
1. Clique com botão direito na solução no Solution Explorer
2. Selecione "Restore NuGet Packages"

**Opção B - Pela Linha de Comando:**
```bash
# No Package Manager Console do Visual Studio
Update-Package -reinstall

# Ou via NuGet CLI
nuget restore FastPizza.sln
```

### 4. Verificar Connection String

Abra o arquivo `Web.config` e verifique a connection string na linha 8:

**Padrão (LocalDB):**
```xml
<add name="FastPizzaConnection" 
     connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=FastPizzaDB;Integrated Security=True;MultipleActiveResultSets=True" 
     providerName="System.Data.SqlClient" />
```

**Se LocalDB não funcionar, use SQL Server Express:**
```xml
<add name="FastPizzaConnection" 
     connectionString="Data Source=(local)\SQLEXPRESS;Initial Catalog=FastPizzaDB;Integrated Security=True;MultipleActiveResultSets=True" 
     providerName="System.Data.SqlClient" />
```

### 5. Compilar o Projeto

1. Pressione `Ctrl+Shift+B` ou
2. Build → Build Solution

### 6. Executar o Projeto

1. Pressione `F5` ou
2. Debug → Start Debugging

O Visual Studio irá:
- Compilar o projeto
- Iniciar o IIS Express
- Abrir o navegador automaticamente
- Criar o banco de dados na primeira execução

## 🔍 Verificando se Funcionou

Após executar, você deve ver:

1. ✅ O navegador abrindo automaticamente
2. ✅ A página inicial do Fast Pizza carregando
3. ✅ Sem erros no console do Visual Studio

## 🐛 Troubleshooting

### Erro: "Cannot open database"

**Causa:** LocalDB não está instalado ou não está rodando.

**Solução:**
```bash
# Verificar se LocalDB está instalado
sqllocaldb info

# Iniciar LocalDB
sqllocaldb start MSSQLLocalDB

# Se não estiver instalado, instale:
# https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb
```

### Erro: "Port already in use"

**Causa:** Outra aplicação está usando a porta.

**Solução:**
1. Feche outras aplicações web rodando
2. Ou altere a porta nas propriedades do projeto:
   - Clique com botão direito no projeto
   - Properties → Web
   - Altere a porta ou use "Auto-assign Port"

### Erro: "NuGet packages are missing"

**Causa:** Pacotes não foram restaurados.

**Solução:**
```bash
# No Package Manager Console
Update-Package -reinstall

# Ou delete a pasta packages/ e restaure novamente
```

### Erro: "MachineKey validation failed"

**Causa:** MachineKey diferente entre ambientes.

**Solução:**
1. Gere um novo MachineKey:
   - https://www.allkeysgenerator.com/Random/Security-Encryption-Key-Generator.aspx
   - Selecione "Machine Key"
2. Substitua no `Web.config`

### Erro: "Access denied" ou "Permission denied"

**Causa:** Permissões de pasta ou banco de dados.

**Solução:**
1. Execute o Visual Studio como Administrador
2. Verifique permissões da pasta do projeto
3. Verifique permissões do SQL Server

## 📝 Notas Importantes

### Banco de Dados

- O banco de dados é criado **automaticamente** na primeira execução
- Localização padrão: `C:\Users\[SeuUsuario]\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB`
- Nome do banco: `FastPizzaDB`

### Portas

- O Visual Studio escolhe automaticamente uma porta disponível
- Geralmente entre 50000-65535
- Você verá a porta na barra de endereços do navegador

### Primeira Execução

Na primeira vez que executar:
1. O banco será criado automaticamente
2. As tabelas serão criadas automaticamente
3. Um usuário admin será criado (se configurado no DatabaseInitializer)

## 🔐 Credenciais Padrão

Após a primeira execução, use:

**Administrador:**
- Usuário: `admin` ou `admin@email.com`
- Senha: `admin123`

## 📚 Próximos Passos

1. ✅ Projeto compilando sem erros
2. ✅ Aplicação rodando no navegador
3. ✅ Login funcionando
4. ✅ Banco de dados criado

## 💡 Dicas

- Use `Ctrl+F5` para executar sem debug (mais rápido)
- Use `F5` para executar com debug (permite breakpoints)
- Verifique a janela "Output" para mensagens de build
- Verifique a janela "Error List" para erros de compilação

## 🆘 Precisa de Ajuda?

1. Verifique o arquivo `REVISAO_PORTABILIDADE.md` para problemas comuns
2. Verifique os logs no Visual Studio (Output window)
3. Verifique o Event Viewer do Windows para erros do sistema

