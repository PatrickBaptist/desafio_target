using System.Globalization;

namespace DesafioJuros;

internal static class Program
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

    private static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("== Cálculo de juros de 2,5% ao dia ==\n");

        if (args.Contains("--demo", StringComparer.OrdinalIgnoreCase))
        {
            ExibirResultado(JurosService.Calcular(1_000m, DateOnly.FromDateTime(DateTime.Today.AddDays(-10))));
            return 0;
        }

        if (args.Length >= 2)
        {
            if (!TentarLerValor(args[0], out var valor) || !TentarLerData(args[1], out var vencimento))
            {
                Console.Error.WriteLine("Argumentos inválidos. Use: dotnet run -- 1000,00 20/09/2026");
                return 1;
            }

            ExibirResultado(JurosService.Calcular(valor, vencimento));
            return 0;
        }

        Console.Write("Valor original: R$ ");
        if (!TentarLerValor(Console.ReadLine(), out var valorInformado))
        {
            Console.Error.WriteLine("Valor inválido. Informe um número maior que zero.");
            return 1;
        }

        Console.Write("Data de vencimento (dd/MM/aaaa): ");
        if (!TentarLerData(Console.ReadLine(), out var vencimentoInformado))
        {
            Console.Error.WriteLine("Data inválida. Use o formato dd/MM/aaaa.");
            return 1;
        }

        ExibirResultado(JurosService.Calcular(valorInformado, vencimentoInformado));
        return 0;
    }

    private static bool TentarLerValor(string? texto, out decimal valor)
    {
        return decimal.TryParse(texto, NumberStyles.Number, Cultura, out valor) && valor > 0;
    }

    private static bool TentarLerData(string? texto, out DateOnly data)
    {
        var formatos = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
        return DateOnly.TryParseExact(texto, formatos, Cultura, DateTimeStyles.None, out data);
    }

    private static void ExibirResultado(ResultadoJuros resultado)
    {
        Console.WriteLine($"\nValor original: {resultado.ValorOriginal.ToString("C", Cultura)}");
        Console.WriteLine($"Vencimento: {resultado.Vencimento:dd/MM/yyyy}");
        Console.WriteLine($"Dias em atraso: {resultado.DiasEmAtraso}");
        Console.WriteLine($"Juros: {resultado.Juros.ToString("C", Cultura)}");
        Console.WriteLine($"Valor final: {resultado.ValorFinal.ToString("C", Cultura)}");
    }
}
