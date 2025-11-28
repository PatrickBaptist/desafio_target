using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DesafioComissao {
    public class Venda{
        public string vendedor { get; set; } = "";
        public double valor { get; set; }
    }


    public class Root {
        public List<Venda>? vendas { get; set; } = new();
    }

    class Program {
        static void Main(string[] args) {
            var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "comissao", "vendas.json");
            path = Path.GetFullPath(path);

            if (!File.Exists(path)) {
                Console.WriteLine($"Arquivo não encontrado: {path}");
                return;
            }

        var json = File.ReadAllText(path);
        var dados = JsonSerializer.Deserialize<Root>(json);

        var comissoes = dados!.vendas
            .GroupBy(v => v.vendedor)
            .Select(g => new {
                Vendedor = g.Key,
                ComissaoTotal = g.Sum(v => CalcularComissao(v.valor)),
                Detalhes = g.Select(v => new { Valor = v.valor, Comissao = CalcularComissao(v.valor) }).ToList()
            })
            .OrderByDescending(x => x.ComissaoTotal);

        Console.WriteLine("== Comissão por Vendedor ==\n");
        foreach (var c in comissoes) {
            Console.WriteLine($"Vendedor: {c.Vendedor}");
            Console.WriteLine($"Comissão total: R${c.ComissaoTotal:F2}");
            Console.WriteLine("Detalhes por venda:");

            foreach (var d in c.Detalhes) {
                Console.WriteLine($" Venda: R${d.Valor:F2} => Comissão: R${d.Comissao:F2}");
            }

            Console.WriteLine();
        }
    }

        public static double CalcularComissao(double valor) {
            if (valor < 100) return 0;
            if (valor < 500) return valor * 0.01;
            return valor * 0.05;
        }
    }
}