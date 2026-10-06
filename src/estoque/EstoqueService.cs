using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesafioEstoque;

public sealed class Produto
{
    public Produto()
    {
    }

    public Produto(int codigoProduto, string descricaoProduto, int quantidadeEmEstoque)
    {
        CodigoProduto = codigoProduto;
        DescricaoProduto = descricaoProduto;
        QuantidadeEmEstoque = quantidadeEmEstoque;
    }

    [JsonPropertyName("codigoProduto")]
    public int CodigoProduto { get; init; }

    [JsonPropertyName("descricaoProduto")]
    public string DescricaoProduto { get; init; } = string.Empty;

    [JsonPropertyName("estoque")]
    [JsonInclude]
    public int QuantidadeEmEstoque { get; private set; }

    public void Adicionar(int quantidade) => QuantidadeEmEstoque += quantidade;

    public void Retirar(int quantidade) => QuantidadeEmEstoque -= quantidade;
}

public sealed class EstoqueArquivo
{
    [JsonPropertyName("estoque")]
    public List<Produto> Estoque { get; init; } = [];
}

public enum TipoMovimentacao
{
    Entrada,
    Saida
}

public sealed record Movimentacao(
    long Id,
    int CodigoProduto,
    TipoMovimentacao Tipo,
    int Quantidade,
    string Descricao,
    DateTimeOffset CriadaEm);

public sealed record ResultadoMovimentacao(
    bool Sucesso,
    string Mensagem,
    Produto? Produto = null,
    Movimentacao? Movimentacao = null,
    int? EstoqueFinal = null);

public static class JsonOptions
{
    public static JsonSerializerOptions Padrao { get; } = new()
    {
        PropertyNameCaseInsensitive = true
    };
}

public sealed class EstoqueService
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly List<Movimentacao> _movimentacoes = [];
    private long _proximoId;

    public EstoqueService(IEnumerable<Produto> produtos)
    {
        ArgumentNullException.ThrowIfNull(produtos);
        _produtos = produtos.ToDictionary(produto => produto.CodigoProduto);
    }

    public IReadOnlyCollection<Produto> Produtos => _produtos.Values;
    public IReadOnlyList<Movimentacao> Movimentacoes => _movimentacoes;

    public ResultadoMovimentacao Movimentar(
        int codigoProduto,
        TipoMovimentacao tipo,
        int quantidade,
        string descricao)
    {
        if (!_produtos.TryGetValue(codigoProduto, out var produto))
        {
            return new ResultadoMovimentacao(false, $"Produto com código {codigoProduto} não encontrado.");
        }

        if (quantidade <= 0)
        {
            return new ResultadoMovimentacao(false, "A quantidade deve ser maior que zero.", produto);
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            return new ResultadoMovimentacao(false, "A descrição da movimentação é obrigatória.", produto);
        }

        if (tipo == TipoMovimentacao.Saida && quantidade > produto.QuantidadeEmEstoque)
        {
            return new ResultadoMovimentacao(
                false,
                $"Estoque insuficiente. Solicitado: {quantidade}; disponível: {produto.QuantidadeEmEstoque}.",
                produto);
        }

        if (tipo == TipoMovimentacao.Entrada)
        {
            produto.Adicionar(quantidade);
        }
        else
        {
            produto.Retirar(quantidade);
        }

        var movimentacao = new Movimentacao(
            Interlocked.Increment(ref _proximoId),
            codigoProduto,
            tipo,
            quantidade,
            descricao.Trim(),
            DateTimeOffset.Now);

        _movimentacoes.Add(movimentacao);

        return new ResultadoMovimentacao(
            true,
            "Movimentação realizada com sucesso.",
            produto,
            movimentacao,
            produto.QuantidadeEmEstoque);
    }
}
