namespace Calculadora;

public class Subtrair : OperacaoMatematica
{
    bool TiraUmAnterior = false;
    public void Reset()
    {
        TiraUmAnterior = false;
    }
    public static void ExecutarSubtracaoDeArquivos(string arquivo1, string arquivo2, string resultadoTemporario, string resultadoFinal)
    {
        Subtrair operacaoDeSubtrair = new Subtrair();

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSubtrair);

        LerArquivo.InverterEEscreverResultado(resultadoTemporario, resultadoFinal, operacaoDeSubtrair);
    }

    public double Execute(double x, double y, StreamWriter streamWriter)
    {
        double subtrair = x - y - (TiraUmAnterior ? 1 : 0);
        (double resto, bool novoTiraUm) = ChecarDezena(subtrair);
        streamWriter.Write(resto.ToString("F0"));
        this.TiraUmAnterior = novoTiraUm;
        return subtrair;
    }

    public (double, bool) ChecarDezena(double x)
    {
        double resto;
        bool tiraUm;
        if (x < 0)
        {
            resto = x + 10;
            tiraUm = true;
        }
        else
        {
            resto = x;
            tiraUm = false;
        }

        return (resto, tiraUm);
    }

}