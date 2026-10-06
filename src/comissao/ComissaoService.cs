using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesafioComissao;

public sealed class Venda
{
    [JsonPropertyName("vendedor")]
    public string Vendedor { get; init; } = string.Empty;

    [JsonPropertyName("valor")]
    public decimal Valor { get; init; }
}

public sealed class VendasArquivo
{
    [JsonPropertyName("vendas")]
    public List<Venda> Vendas { get; init; } = [];
}

public sealed record DetalheComissao(decimal Valor, decimal Comissao);

public sealed record ResultadoComissao(
    string Vendedor,
    decimal ComissaoTotal,
    IReadOnlyList<DetalheComissao> Detalhes);

public static class JsonOptions
{
    public static JsonSerializerOptions Padrao { get; } = new()
    {
        PropertyNameCaseInsensitive = true
    };
}

public static class ComissaoService
{
    public static decimal CalcularComissao(decimal valor)
    {
        if (valor < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor da venda não pode ser negativo.");
        }

        var percentual = valor switch
        {
            < 100m => 0m,
            < 500m => 0.01m,
            _ => 0.05m
        };

        return decimal.Round(valor * percentual, 2, MidpointRounding.AwayFromZero);
    }

    public static IReadOnlyList<ResultadoComissao> CalcularPorVendedor(IEnumerable<Venda> vendas)
    {
        ArgumentNullException.ThrowIfNull(vendas);

        return vendas
            .Select(venda =>
            {
                if (string.IsNullOrWhiteSpace(venda.Vendedor))
                {
                    throw new ArgumentException("Toda venda deve informar um vendedor.");
                }

                return venda;
            })
            .GroupBy(venda => venda.Vendedor.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(grupo =>
            {
                var detalhes = grupo
                    .Select(venda => new DetalheComissao(venda.Valor, CalcularComissao(venda.Valor)))
                    .ToList();

                return new ResultadoComissao(
                    grupo.First().Vendedor.Trim(),
                    detalhes.Sum(detalhe => detalhe.Comissao),
                    detalhes);
            })
            .OrderByDescending(resultado => resultado.ComissaoTotal)
            .ThenBy(resultado => resultado.Vendedor, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
