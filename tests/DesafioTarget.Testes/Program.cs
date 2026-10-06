using DesafioComissao;
using DesafioEstoque;
using DesafioJuros;

var testes = new (string Nome, Action Executar)[]
{
    ("Comissão respeita os limites das faixas", TestarFaixasDeComissao),
    ("Comissão rejeita valor negativo", TestarVendaNegativa),
    ("Estoque aplica entrada e saída", TestarMovimentacoesValidas),
    ("Estoque bloqueia saída maior que o saldo", TestarEstoqueInsuficiente),
    ("Movimentações recebem identificadores diferentes", TestarIdsUnicos),
    ("Juros usam 2,5% simples por dia de atraso", TestarJurosEmAtraso),
    ("Juros não são cobrados antes do vencimento", TestarVencimentoFuturo)
};

var falhas = 0;

foreach (var teste in testes)
{
    try
    {
        teste.Executar();
        Console.WriteLine($"[OK] {teste.Nome}");
    }
    catch (Exception ex)
    {
        falhas++;
        Console.WriteLine($"[FALHOU] {teste.Nome}: {ex.Message}");
    }
}

Console.WriteLine($"\n{testes.Length - falhas} de {testes.Length} testes passaram.");
return falhas == 0 ? 0 : 1;

static void TestarFaixasDeComissao()
{
    Igual(0m, ComissaoService.CalcularComissao(99.99m));
    Igual(1m, ComissaoService.CalcularComissao(100m));
    Igual(5m, ComissaoService.CalcularComissao(499.99m));
    Igual(25m, ComissaoService.CalcularComissao(500m));
    Igual(4.01m, ComissaoService.CalcularComissao(400.50m));
}

static void TestarVendaNegativa()
{
    Lanca<ArgumentOutOfRangeException>(() => ComissaoService.CalcularComissao(-1m));
}

static void TestarMovimentacoesValidas()
{
    var servico = CriarEstoque();
    var entrada = servico.Movimentar(102, TipoMovimentacao.Entrada, 20, "Reposição");
    var saida = servico.Movimentar(102, TipoMovimentacao.Saida, 15, "Pedido 1234");

    Verdadeiro(entrada.Sucesso, entrada.Mensagem);
    Igual(95, entrada.EstoqueFinal);
    Verdadeiro(saida.Sucesso, saida.Mensagem);
    Igual(80, saida.EstoqueFinal);
}

static void TestarEstoqueInsuficiente()
{
    var servico = CriarEstoque();
    var resultado = servico.Movimentar(103, TipoMovimentacao.Saida, 1_000, "Venda");

    Verdadeiro(!resultado.Sucesso, "A saída deveria ter sido bloqueada.");
    Igual(200, resultado.Produto?.QuantidadeEmEstoque);
    Igual(0, servico.Movimentacoes.Count);
}

static void TestarIdsUnicos()
{
    var servico = CriarEstoque();
    var primeira = servico.Movimentar(102, TipoMovimentacao.Entrada, 1, "Primeira");
    var segunda = servico.Movimentar(102, TipoMovimentacao.Entrada, 1, "Segunda");

    Verdadeiro(primeira.Movimentacao?.Id != segunda.Movimentacao?.Id, "Os IDs deveriam ser únicos.");
}

static void TestarJurosEmAtraso()
{
    var hoje = new DateOnly(2026, 10, 5);
    var resultado = JurosService.Calcular(1_000m, new DateOnly(2026, 9, 25), hoje);

    Igual(10, resultado.DiasEmAtraso);
    Igual(250m, resultado.Juros);
    Igual(1_250m, resultado.ValorFinal);
}

static void TestarVencimentoFuturo()
{
    var hoje = new DateOnly(2026, 10, 5);
    var resultado = JurosService.Calcular(500m, new DateOnly(2026, 10, 10), hoje);

    Igual(0, resultado.DiasEmAtraso);
    Igual(0m, resultado.Juros);
    Igual(500m, resultado.ValorFinal);
}

static EstoqueService CriarEstoque()
{
    return new EstoqueService(
    [
        new Produto(102, "Caderno Universitário", 75),
        new Produto(103, "Borracha Branca", 200)
    ]);
}

static void Igual<T>(T esperado, T atual)
{
    if (!EqualityComparer<T>.Default.Equals(esperado, atual))
    {
        throw new InvalidOperationException($"Esperado: {esperado}; obtido: {atual}.");
    }
}

static void Verdadeiro(bool condicao, string mensagem)
{
    if (!condicao)
    {
        throw new InvalidOperationException(mensagem);
    }
}

static void Lanca<TException>(Action acao) where TException : Exception
{
    try
    {
        acao();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"Era esperada a exceção {typeof(TException).Name}.");
}
