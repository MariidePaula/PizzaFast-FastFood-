using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.Models;

namespace FastPizza.Utils
{
    // Classe utilitária para mapear nomes de produtos e bebidas para arquivos de imagem
    public static class ImageMapper
    {
        // Mapeia o nome de um produto (pizza) para o caminho da imagem correspondente
        public static string ObterImagemProduto(Produto produto)
        {
            if (produto == null || string.IsNullOrWhiteSpace(produto.Nome))
                return null;

            // Se já tiver uma imagem definida, retorna ela
            if (!string.IsNullOrWhiteSpace(produto.ImagemUrl))
                return produto.ImagemUrl;

            // Normaliza o nome do produto para buscar a imagem correspondente
            string nomeNormalizado = NormalizarNome(produto.Nome);
            // Retorna caminho relativo que será resolvido pelo ASP.NET
            string caminhoImagem = $"~/Images/Pizzas/{nomeNormalizado}.jpg";

            return caminhoImagem;
        }

        // Mapeia o nome de uma bebida para o caminho da imagem correspondente
        public static string ObterImagemBebida(Bebida bebida)
        {
            if (bebida == null || string.IsNullOrWhiteSpace(bebida.Nome))
                return null;

            // Se já tiver uma imagem definida, retorna ela
            if (!string.IsNullOrWhiteSpace(bebida.ImagemUrl))
                return bebida.ImagemUrl;

            // Normaliza o nome da bebida para buscar a imagem correspondente
            string nomeNormalizado = NormalizarNomeBebida(bebida.Nome);
            // Retorna caminho relativo que será resolvido pelo ASP.NET
            string caminhoImagem = $"~/Images/Bebidas/{nomeNormalizado}.png";

            return caminhoImagem;
        }

        // Normaliza o nome de um produto para corresponder ao nome do arquivo de imagem
        private static string NormalizarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            // Dicionário de mapeamento de nomes de produtos para nomes de arquivos
            var mapeamento = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Mussarela Tradicional", "pizza-mussarela" },
                { "Marguerita Especial", "pizza-margherita" },
                { "Napolitana", "pizza-napolitana" },
                { "Calabresa Acebolada", "pizza-calabresa" },
                { "Pizza Pepperoni", "pizza-pepperoni" },
                { "Portuguesa Completa", "pizza-portuguesa" },
                { "Quattro Formaggi (Quatro Queijos)", "pizza-quattro-formaggi" },
                { "Parma com Rúcula", "pizza-parma" },
                { "Camarão com Catupiry", "pizza-camarao" },
                { "Vegetariana Especial", "pizza-vegetariana" },
                { "Tomate Seco com Rúcula", "pizza-tomate-seco" },
                { "Brócolis com Alho", "pizza-brocolis" }
            };

            // Busca o mapeamento exato primeiro (mais rápido)
            if (mapeamento.ContainsKey(nome))
                return mapeamento[nome];

            // Se não encontrar exato, tenta correspondência parcial (busca flexível)
            string nomeLower = nome.ToLower();
            foreach (var kvp in mapeamento)
            {
                string chaveLower = kvp.Key.ToLower();
                // Verifica se o nome contém a chave ou vice-versa (busca bidirecional)
                if (nomeLower.Contains(chaveLower) || chaveLower.Contains(nomeLower))
                {
                    return kvp.Value;
                }
            }

            // Se não encontrar correspondência, normaliza o nome removendo acentos e caracteres especiais
            // Isso garante que mesmo nomes não mapeados tenham um arquivo de imagem válido
            return "pizza-" + nome.ToLower()
                .Replace(" ", "-")
                .Replace("ç", "c")
                .Replace("á", "a")
                .Replace("à", "a")
                .Replace("ã", "a")
                .Replace("â", "a")
                .Replace("é", "e")
                .Replace("ê", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ô", "o")
                .Replace("õ", "o")
                .Replace("ú", "u")
                .Replace("ü", "u");
        }

        // Normaliza o nome de uma bebida para corresponder ao nome do arquivo de imagem
        private static string NormalizarNomeBebida(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            // Dicionário de mapeamento de nomes de bebidas para nomes de arquivos
            var mapeamento = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Coca-Cola Lata", "drink-coca-lata" },
                { "Guaraná Antarctica Lata", "drink-guarana-lata" },
                { "Fanta Lata", "drink-fanta-lata" },
                { "Sprite Lata", "drink-sprite-lata" },
                { "Suco de Laranja", "drink-suco-laranja" },
                { "Suco de Uva", "drink-suco-uva" },
                { "Chá de Limão", "drink-cha-limao" },
                { "Chá de Pêssego", "drink-cha-pessego" },
                { "H2OH Limão", "drink-h2oh-limao" },
                { "Água Mineral sem Gás", "drink-agua-500ml" },
                { "Água com Gás", "drink-agua-gas-500ml" }
            };

            // Busca o mapeamento exato primeiro (mais rápido)
            if (mapeamento.ContainsKey(nome))
                return mapeamento[nome];

            // Se não encontrar exato, tenta correspondência parcial (busca flexível)
            string nomeLower = nome.ToLower();
            foreach (var kvp in mapeamento)
            {
                string chaveLower = kvp.Key.ToLower();
                // Verifica se o nome contém a chave ou vice-versa (busca bidirecional)
                if (nomeLower.Contains(chaveLower) || chaveLower.Contains(nomeLower))
                {
                    return kvp.Value;
                }
            }

            // Se não encontrar correspondência, normaliza o nome removendo acentos e caracteres especiais
            // Isso garante que mesmo nomes não mapeados tenham um arquivo de imagem válido
            return "drink-" + nome.ToLower()
                .Replace(" ", "-")
                .Replace("ç", "c")
                .Replace("á", "a")
                .Replace("à", "a")
                .Replace("ã", "a")
                .Replace("â", "a")
                .Replace("é", "e")
                .Replace("ê", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ô", "o")
                .Replace("õ", "o")
                .Replace("ú", "u")
                .Replace("ü", "u");
        }

    }
}

