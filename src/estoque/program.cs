using System.Text.Json;

namespace DesafioEstoque;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        try
        {
            var caminho = Path.Combine(AppContext.BaseDirectory, "estoque.json");
            var arquivo = CarregarEstoque(caminho);
            var servico = new EstoqueService(arquivo.Estoque);

            if (args.Contains("--demo", StringComparer.OrdinalIgnoreCase))
            {
                ExecutarDemonstracao(servico);
                return 0;
            }

            ExecutarMenu(servico);
            return 0;
        }
        catch (Exception ex) when (ex is IOException or JsonException or ArgumentException)
        {
            Console.Error.WriteLine($"Erro ao iniciar o estoque: {ex.Message}");
            return 1;
        }
    }

    private static EstoqueArquivo CarregarEstoque(string caminho)
    {
        if (!File.Exists(caminho))
        {
            throw new FileNotFoundException("Arquivo de estoque não encontrado.", caminho);
        }

        var json = File.ReadAllText(caminho);
        var arquivo = JsonSerializer.Deserialize<EstoqueArquivo>(json, JsonOptions.Padrao)
            ?? throw new JsonException("O JSON de estoque está vazio ou inválido.");

        if (arquivo.Estoque.Count == 0)
        {
            throw new ArgumentException("O JSON não contém produtos.");
        }

        return arquivo;
    }

    private static void ExecutarMenu(EstoqueService servico)
    {
        Console.WriteLine("== Movimentação de estoque ==");

        while (true)
        {
            ExibirProdutos(servico.Produtos);
            Console.Write("\nCódigo do produto (ou 0 para sair): ");

            if (!int.TryParse(Console.ReadLine(), out var codigo))
            {
                Console.WriteLine("Código inválido. Digite um número inteiro.");
                continue;
            }

            if (codigo == 0)
            {
                return;
            }

            Console.Write("Tipo [E]ntrada ou [S]aída: ");
            var tipoDigitado = Console.ReadLine()?.Trim();
            var tipo = tipoDigitado?.ToUpperInvariant() switch
            {
                "E" => TipoMovimentacao.Entrada,
                "S" => TipoMovimentacao.Saida,
                _ => (TipoMovimentacao?)null
            };

            if (tipo is null)
            {
                Console.WriteLine("Tipo inválido. Digite E ou S.");
                continue;
            }

            Console.Write("Quantidade: ");
            if (!int.TryParse(Console.ReadLine(), out var quantidade) || quantidade <= 0)
            {
                Console.WriteLine("A quantidade deve ser um número inteiro maior que zero.");
                continue;
            }

            Console.Write("Descrição da movimentação: ");
            var descricao = Console.ReadLine() ?? string.Empty;

            var resultado = servico.Movimentar(codigo, tipo.Value, quantidade, descricao);
            ExibirResultado(resultado);
        }
    }

    private static void ExecutarDemonstracao(EstoqueService servico)
    {
        Console.WriteLine("== Demonstração de movimentação de estoque ==\n");
        ExibirResultado(servico.Movimentar(102, TipoMovimentacao.Entrada, 20, "Entrada de cadernos para reposição"));
        ExibirResultado(servico.Movimentar(105, TipoMovimentacao.Saida, 15, "Saída referente ao pedido 1234"));
    }

    private static void ExibirProdutos(IEnumerable<Produto> produtos)
    {
        Console.WriteLine("\nProdutos disponíveis:");
        foreach (var produto in produtos.OrderBy(produto => produto.CodigoProduto))
        {
            Console.WriteLine($"  {produto.CodigoProduto} - {produto.DescricaoProduto} (estoque: {produto.QuantidadeEmEstoque})");
        }
    }

    private static void ExibirResultado(ResultadoMovimentacao resultado)
    {
        if (!resultado.Sucesso)
        {
            Console.WriteLine($"\nMovimentação não realizada: {resultado.Mensagem}\n");
            return;
        }

        var movimentacao = resultado.Movimentacao!;
        Console.WriteLine($"\nID: {movimentacao.Id}");
        Console.WriteLine($"Produto: {resultado.Produto!.DescricaoProduto}");
        Console.WriteLine($"Descrição: {movimentacao.Descricao}");
        Console.WriteLine($"Tipo: {(movimentacao.Tipo == TipoMovimentacao.Entrada ? "Entrada" : "Saída")}");
        Console.WriteLine($"Quantidade movimentada: {movimentacao.Quantidade}");
        Console.WriteLine($"Estoque final: {resultado.EstoqueFinal}\n");
    }
}
