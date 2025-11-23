using System;
using System.Collections.Generic;
using System.Linq;
using FastPizza.Models;

namespace FastPizza.DataAccess
{
    public class MolhoDAO
    {
        private static List<Molho> _molhos;
        private static int _ultimoId = 0;

        static MolhoDAO()
        {
            _molhos = new List<Molho>
            {

                new Molho {
                    Id = 1,
                    Nome="Molho de Alho",
                    Descricao="Molho cremoso de alho temperado.",
                    Categoria="Cremoso",
                    Preco=2.50m,
                    ImagemUrl="https://img.freepik.com/fotos-gratis/molho-de-alho_144627-14740.jpg",
                    Disponivel = true,
                    Estoque = 100
                },
                new Molho {
                    Id = 2,
                    Nome="Molho Cheddar",
                    Descricao="Molho cremoso de cheddar.",
                    Categoria="Cremoso",
                    Preco=3.00m,
                    ImagemUrl="https://img.freepik.com/fotos-gratis/molho-cheddar_144627-14741.jpg",
                    Disponivel = true,
                    Estoque = 80
                },
                new Molho {
                    Id = 3,
                    Nome="Molho Barbecue",
                    Descricao="Molho barbecue defumado.",
                    Categoria="Cremoso",
                    Preco=2.50m,
                    ImagemUrl="https://img.freepik.com/fotos-gratis/molho-barbecue_144627-14742.jpg",
                    Disponivel = true,
                    Estoque = 90
                },

                new Molho {
                    Id = 4,
                    Nome="Molho Picante",
                    Descricao="Molho picante com pimenta.",
                    Categoria="Picante",
                    Preco=2.00m,
                    ImagemUrl="https://img.freepik.com/fotos-gratis/molho-picante_144627-14743.jpg",
                    Disponivel = true,
                    Estoque = 70
                },
                new Molho {
                    Id = 5,
                    Nome="Molho de Pimenta",
                    Descricao="Molho extra picante.",
                    Categoria="Picante",
                    Preco=2.50m,
                    ImagemUrl="https://img.freepik.com/fotos-gratis/molho-pimenta_144627-14744.jpg",
                    Disponivel = true,
                    Estoque = 60
                },

                new Molho {
                    Id = 6,
                    Nome="Molho Agridoce",
                    Descricao="Molho doce e azedo.",
                    Categoria="Doce",
                    Preco=2.50m,
                    ImagemUrl="https://img.freepik.com/fotos-gratis/molho-agridoce_144627-14745.jpg",
                    Disponivel = true,
                    Estoque = 75
                },
                new Molho {
                    Id = 7,
                    Nome="Molho de Mel",
                    Descricao="Molho doce de mel e mostarda.",
                    Categoria="Doce",
                    Preco=3.00m,
                    ImagemUrl="https://img.freepik.com/fotos-gratis/molho-mel_144627-14746.jpg",
                    Disponivel = true,
                    Estoque = 65
                }
            };
            _ultimoId = 7;
        }

        public static List<Molho> ObterTodos()
        {
            return _molhos.ToList();
        }

        public static List<Molho> ObterDisponiveis()
        {
            return _molhos.Where(m => m.Disponivel && m.Estoque > 0).ToList();
        }

        public static List<Molho> ObterPorCategoria(string categoria)
        {
            if (string.IsNullOrEmpty(categoria) || categoria == "Todas")
            {
                return ObterDisponiveis();
            }

            return _molhos.Where(m => m.Disponivel && m.Estoque > 0 && m.Categoria == categoria).ToList();
        }

        public static Molho ObterPorId(int id)
        {
            return _molhos.FirstOrDefault(m => m.Id == id);
        }

        public static void Adicionar(Molho molho)
        {
            _ultimoId++;
            molho.Id = _ultimoId;
            _molhos.Add(molho);
        }

        public static void Atualizar(Molho molho)
        {
            var index = _molhos.FindIndex(m => m.Id == molho.Id);
            if (index >= 0)
            {
                _molhos[index] = molho;
            }
        }

        public static void Excluir(int id)
        {
            _molhos.RemoveAll(m => m.Id == id);
        }
    }
}

//não usado
