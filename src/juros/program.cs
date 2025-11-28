using System;

namespace DesafioJuros
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("== Cálculo de Juros (2.5% ao dia) ==\n");

            double valor;
            DateTime vencimento;

            if (args.Length >= 2 && double.TryParse(args[0], out valor) && DateTime.TryParse(args[1], out vencimento))
            {
                ImprimeResultado(valor, vencimento);
                return;
            }

            valor = 1000.00;
            vencimento = new DateTime(DateTime.Today.Year, DateTime.Today.Month, Math.Max(1, DateTime.Today.Day - 10));

            ImprimeResultado(valor, vencimento);


            Console.WriteLine("\nExemplo com vencimento futuro (sem juros):");
            ImprimeResultado(500, DateTime.Today.AddDays(5));
        }


        static void ImprimeResultado(double valor, DateTime vencimento)
        {
            double juros = CalcularJuros(valor, vencimento);
            Console.WriteLine($"Valor original: R${valor:F2}");
            Console.WriteLine($"Vencimento: {vencimento:yyyy-MM-dd}");
            Console.WriteLine($"Dias em atraso: {Math.Max(0, (DateTime.Today - vencimento).Days)}");
            Console.WriteLine($"Juros aplicados: R${juros:F2}");
            Console.WriteLine($"Valor final: R${valor + juros:F2}");
        }

        public static double CalcularJuros(double valor, DateTime vencimento)
        {
            int diasAtraso = (DateTime.Today - vencimento).Days;
            if (diasAtraso <= 0) return 0;


            double multaDiaria = 0.025;
            return valor * multaDiaria * diasAtraso;
        }
    }
}