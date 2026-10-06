namespace DesafioJuros;

public sealed record ResultadoJuros(
    decimal ValorOriginal,
    DateOnly Vencimento,
    DateOnly DataDoCalculo,
    int DiasEmAtraso,
    decimal Juros,
    decimal ValorFinal);

public static class JurosService
{
    public const decimal TaxaDiaria = 0.025m;

    public static ResultadoJuros Calcular(
        decimal valor,
        DateOnly vencimento,
        DateOnly? dataDoCalculo = null)
    {
        if (valor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser maior que zero.");
        }

        var dataReferencia = dataDoCalculo ?? DateOnly.FromDateTime(DateTime.Today);
        var diasEmAtraso = Math.Max(0, dataReferencia.DayNumber - vencimento.DayNumber);
        var juros = decimal.Round(
            valor * TaxaDiaria * diasEmAtraso,
            2,
            MidpointRounding.AwayFromZero);

        return new ResultadoJuros(
            valor,
            vencimento,
            dataReferencia,
            diasEmAtraso,
            juros,
            valor + juros);
    }
}
