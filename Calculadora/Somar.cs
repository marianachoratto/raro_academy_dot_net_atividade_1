namespace Calculadora;

public class Somar : OperacaoMatematica
{
    bool vaiUmAnterior = false;

    public void Reset()
    {
        vaiUmAnterior = false;
    }
    public static void ExecutarSomaDeArquivos(string arquivo1, string arquivo2, string resultadoTemporario, string resultadoFinal)
    {
        Somar operacaoDeSoma = new Somar();
        operacaoDeSoma.Reset();

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSoma);

        LerArquivo.InverterEEscreverResultado(resultadoTemporario, resultadoFinal, operacaoDeSoma);
    }

    public double Execute(double x, double y, StreamWriter streamWriter)
    {
        double soma = x + y + (vaiUmAnterior ? 1 : 0);
        (double resto, bool novoVaiUm) = ChecarDezena(soma);
        streamWriter.Write(resto.ToString("F0"));
        this.vaiUmAnterior = novoVaiUm;

        return soma;
    }
    public (double, bool) ChecarDezena(double x)
    {
        double resto;
        bool vaiUm;
        if (x >= 10)
        {
            resto = x - 10;
            vaiUm = true;
        }
        else
        {
            resto = x;
            vaiUm = false;
        }

        return (resto, vaiUm);
    }

    public bool GetVaiUmAnterior()
    {
        return vaiUmAnterior;
    }

}