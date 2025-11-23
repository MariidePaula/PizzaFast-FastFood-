# Configuração de Imagens para Publish

## ✅ Configuração Realizada

Todas as imagens da pasta `/Images` foram configuradas no arquivo `FastPizza.csproj` com as seguintes propriedades:

- **Build Action**: `Content`
- **Copy to Output Directory**: `Copy Always`

Isso garante que:
1. ✅ As imagens sejam incluídas no projeto
2. ✅ As imagens sejam sempre copiadas durante o build
3. ✅ As imagens sejam incluídas ao publicar o projeto
4. ✅ As imagens apareçam corretamente quando o projeto for enviado para outra pessoa

## 📁 Estrutura de Pastas

```
Images/
├── Pizzas/
│   ├── bannerprincipal.jpg
│   ├── hero-pizzeria.jpg
│   ├── pizza-brocolis.jpg
│   ├── pizza-calabresa.jpg
│   ├── pizza-camarao.jpg
│   ├── pizza-margherita.jpg
│   ├── pizza-mussarela.jpg
│   ├── pizza-napolitana.jpg
│   ├── pizza-parma.jpg
│   ├── pizza-pepperoni.jpg
│   ├── pizza-portuguesa.jpg
│   ├── pizza-quattro-formaggi.jpg
│   ├── pizza-tomate-seco.jpg
│   └── pizza-vegetariana.jpg
└── Bebidas/
    ├── drink-agua-500ml.png
    ├── drink-agua-gas-500ml.png
    ├── drink-cha-limao.png
    ├── drink-cha-pessego.png
    ├── drink-coca-lata.png
    ├── drink-fanta-lata.png
    ├── drink-guarana-lata.png
    ├── drink-h2oh-limao.png
    ├── drink-sprite-lata.png
    ├── drink-suco-laranja.png
    └── drink-suco-uva.png
```

## 🔧 Como Adicionar Novas Imagens

Quando adicionar novas imagens à pasta `/Images`, siga estes passos:

### Opção 1: Via Visual Studio (Recomendado)
1. Clique com o botão direito na pasta `Images` no Solution Explorer
2. Selecione "Add" > "Existing Item"
3. Navegue até a imagem e adicione
4. Clique com o botão direito na imagem adicionada
5. Selecione "Properties"
6. Configure:
   - **Build Action**: `Content`
   - **Copy to Output Directory**: `Copy Always`

### Opção 2: Manualmente no .csproj
Adicione a seguinte entrada no arquivo `FastPizza.csproj` dentro do `ItemGroup` de imagens:

```xml
<Content Include="Images\NovaPasta\nova-imagem.jpg">
  <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</Content>
```

## 📝 Verificação

Para verificar se as imagens estão configuradas corretamente:

1. Abra o arquivo `FastPizza.csproj` no Visual Studio
2. Expanda a pasta `Images` no Solution Explorer
3. Selecione uma imagem
4. Nas propriedades, verifique:
   - Build Action = Content
   - Copy to Output Directory = Copy Always

## ⚠️ Importante

- **Não delete** a configuração das imagens do arquivo `.csproj`
- As imagens devem estar **incluídas no projeto** (não apenas na pasta física)
- Use caminhos relativos nas URLs: `~/Images/Pizzas/nome-imagem.jpg`

## 🌐 Uso nas Páginas

Use as imagens nas páginas com caminhos relativos:

```html
<!-- Caminho relativo (recomendado) -->
<img src="~/Images/Pizzas/pizza-mussarela.jpg" alt="Pizza Mussarela" />

<!-- Ou usando ResolveUrl -->
<img src="<%= ResolveUrl("~/Images/Pizzas/pizza-mussarela.jpg") %>" alt="Pizza Mussarela" />
```

Isso garante que as imagens funcionem tanto no desenvolvimento quanto após o publish.

