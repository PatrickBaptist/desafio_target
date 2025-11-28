using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DesafioEstoque {
    public class Produto
    {
        public int codigoProduto { get; set; }
        public string descricaoProduto { get; set; }
        public int estoque { get; set; }
    }

    public class RootEstoque
    {
        public List<Produto> estoque { get; set; }
    }

    public class Movimentacao
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
        public bool Entrada { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "estoque", "estoque.json");
            path = Path.GetFullPath(path);

            if (!File.Exists(path))
            {
                Console.WriteLine("Arquivo estoque.json não encontrado: " + path);
                return;
            }

            var json = File.ReadAllText(path);
            var dados = JsonSerializer.Deserialize<RootEstoque>(json);

            if (dados?.estoque == null)
            {
                Console.WriteLine("Erro ao carregar estoque do JSON");
                return;
            }

            Console.WriteLine("== Movimentação de Estoque ==\n");

            var mov1 = new Movimentacao { Descricao = "Entrada de cadernos (reposição)", Quantidade = 20, Entrada = true };
            MovimentarProduto(dados.estoque, 102, mov1);

            var mov2 = new Movimentacao { Descricao = "Venda - pedido #1234", Quantidade = 15, Entrada = false };
            MovimentarProduto(dados.estoque, 105, mov2);

            var mov3 = new Movimentacao { Descricao = "Venda grande", Quantidade = 1000, Entrada = false };
            MovimentarProduto(dados.estoque, 103, mov3);
        }

        public static void MovimentarProduto(List<Produto> estoque, int codigo, Movimentacao mov)
        {
            var produto = estoque.Find(p => p.codigoProduto == codigo);

            if (produto == null)
            {
                Console.WriteLine($"Produto com código {codigo} não encontrado.\n");
                return;
            }

            if (!mov.Entrada && mov.Quantidade > produto.estoque)
            {
                Console.WriteLine(
                    $"Erro: estoque insuficiente para '{produto.descricaoProduto}' " +
                    $"(Solicitado: {mov.Quantidade}, Disponível: {produto.estoque}). " +
                    $"ID Movimentação: {mov.ID}\n"
                );
                return;
            }

            produto.estoque += mov.Entrada ? mov.Quantidade : -mov.Quantidade;

            Console.WriteLine($"ID: {mov.ID}");
            Console.WriteLine($"Produto: {produto.descricaoProduto}");
            Console.WriteLine($"Descrição: {mov.Descricao}");
            Console.WriteLine($"Tipo: {(mov.Entrada ? "Entrada" : "Saída")}");
            Console.WriteLine($"Quantidade: {mov.Quantidade}");
            Console.WriteLine($"Estoque final: {produto.estoque}\n");        
        }
    }
}