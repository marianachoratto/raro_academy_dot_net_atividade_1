namespace Calculadora;

public class Program
{
    public static void Main()
    {
        string arquivo1 = @"C:\Users\maria\OneDrive\Desktop\Mariana\Programação\raro_e_dotnet\atividade1\numero1.txt";

        string arquivo2 = @"C:\Users\maria\OneDrive\Desktop\Mariana\Programação\raro_e_dotnet\atividade1\numero2.txt";

        string resultadoTemporario = @"C:\Users\maria\OneDrive\Desktop\Mariana\Programação\raro_e_dotnet\atividade1\resultadoTemporario.txt";

        string resultadoFinal = @"C:\Users\maria\OneDrive\Desktop\Mariana\Programação\raro_e_dotnet\atividade1\resultadoFinal.txt";

        string resultadoResto = @"C:\Users\maria\OneDrive\Desktop\Mariana\Programação\raro_e_dotnet\atividade1\resultadoResto.txt";

        int resultado;

        do
        {
            Console.WriteLine("Antes de começarmos a utilizar nossa calculadora, digite os numeros da conta respectivamente nos arquivos numero1.txt e numero2.txt");

            Console.WriteLine("Qual operação você gostaria de fazer? Digite o número da operação\n1- Somar\n2- Subtrair\n3- Multiplicar\n4- Dividir\nDigite os numeros da conta respectivamente no numero1.txt e numero2.txt");
            resultado = Convert.ToInt16(Console.ReadLine());
        }
        while (resultado != 1 && resultado != 2 && resultado != 3 && resultado != 4);


        if (resultado == 1) Somar.ExecutarSomaDeArquivos(arquivo1, arquivo2, resultadoTemporario, resultadoFinal);

        if (resultado == 2) Subtrair.ExecutarSubtracaoDeArquivos(arquivo1, arquivo2, resultadoTemporario, resultadoFinal);

        if (resultado == 3) Multiplicar.ExecutarMultiplicacaoDeArquivos(arquivo1, arquivo2, resultadoTemporario, resultadoFinal);

        if (resultado == 4) Dividir.ExecutarDivisao(arquivo1, arquivo2, resultadoFinal, resultadoResto);

    }



}


