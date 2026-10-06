using System.Globalization;
using System.Text.Json;

namespace DesafioComissao;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var caminho = args.Length > 0
            ? Path.GetFullPath(args[0])
            : Path.Combine(AppContext.BaseDirectory, "vendas.json");

        try
        {
            var arquivo = CarregarVendas(caminho);
            var resultados = ComissaoService.CalcularPorVendedor(arquivo.Vendas);

            Console.WriteLine("== Comissão por vendedor ==\n");

            foreach (var resultado in resultados)
            {
                Console.WriteLine($"Vendedor: {resultado.Vendedor}");
                Console.WriteLine($"Comissão total: {resultado.ComissaoTotal.ToString("C", CultureInfo.GetCultureInfo("pt-BR"))}");
                Console.WriteLine("Detalhes por venda:");

                foreach (var detalhe in resultado.Detalhes)
                {
                    Console.WriteLine(
                        $"  Venda: {detalhe.Valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR"))}" +
                        $" => Comissão: {detalhe.Comissao.ToString("C", CultureInfo.GetCultureInfo("pt-BR"))}");
                }

                Console.WriteLine();
            }

            return 0;
        }
        catch (Exception ex) when (ex is IOException or JsonException or ArgumentException)
        {
            Console.Error.WriteLine($"Erro ao processar as vendas: {ex.Message}");
            return 1;
        }
    }

    private static VendasArquivo CarregarVendas(string caminho)
    {
        if (!File.Exists(caminho))
        {
            throw new FileNotFoundException("Arquivo de vendas não encontrado.", caminho);
        }

        var json = File.ReadAllText(caminho);
        var arquivo = JsonSerializer.Deserialize<VendasArquivo>(json, JsonOptions.Padrao)
            ?? throw new JsonException("O JSON de vendas está vazio ou inválido.");

        if (arquivo.Vendas.Count == 0)
        {
            throw new ArgumentException("O JSON não contém vendas.");
        }

        return arquivo;
    }
}
