namespace CalculadoraTestes;

public class TestesCalculadora
{
    string arquivo1 = @"C:\Users\maria\OneDrive\Desktop\Mariana\Programação\raro_e_dotnet\atividade1\numero1.txt";

    string arquivo2 = @"C:\Users\maria\OneDrive\Desktop\Mariana\Programação\raro_e_dotnet\atividade1\numero2.txt";

    string resultadoTemporario = @"C:\Users\maria\OneDrive\Desktop\Mariana\Programação\raro_e_dotnet\atividade1\resultadoTemporario.txt";

    string resultadoFinal = @"C:\Users\maria\OneDrive\Desktop\Mariana\Programação\raro_e_dotnet\atividade1\resultadoFinal.txt";

    string numero1 = "9";
    string numero2 = "7";

    [Fact]
    public void DeveDarErroSe1MaiorQue2()
    {
        Somar operacaoDeSoma = new Somar();

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero2);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero1);

        const string exception = "Número 2 é maior do que número 1";

        Exception thrownException = Assert.Throws<Exception>(() =>
    {

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSoma);
    });

        Assert.Contains(exception, thrownException.Message);



    }

    [Fact]
    public void DeveEscreverResultadoDaSomaNoArquivoTemporario()
    {
        Somar operacaoDeSoma = new Somar();

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero1);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero2);

        double resultadoSoma = Convert.ToDouble(numero1) + Convert.ToDouble(numero2);

        string resultadoInvertido = MetodosAuxiliares.InversorDeResultado(resultadoSoma);


        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSoma);


        string lerTextoTemporario;
        using (StreamReader sr = new StreamReader(resultadoTemporario))
        {
            lerTextoTemporario = sr.ReadToEnd();
        }

        Assert.Equal(Convert.ToString(resultadoInvertido), lerTextoTemporario);
    }

    [Fact]
    public void DeveSomarCorretamenteNoResultadoFinal()
    {
        Somar operacaoDeSoma = new Somar();

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero1);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero2);
        double resultadoSomaNumerico = Convert.ToDouble(numero1) + Convert.ToDouble(numero2);
        string resultadoEsperadoFinal = resultadoSomaNumerico.ToString("F0");

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSoma);
        LerArquivo.InverterEEscreverResultado(resultadoTemporario, resultadoFinal, operacaoDeSoma);

        string textoLidoDoFinal;
        using (StreamReader sr = new StreamReader(resultadoFinal))
        {
            textoLidoDoFinal = sr.ReadToEnd();
        }

        Assert.Equal(resultadoEsperadoFinal, textoLidoDoFinal);
    }

    [Fact]
    public void DeveSomarNumerosComTamanhosDiferentesNoResultadoFinal()
    {
        Somar operacaoDeSoma = new Somar();
        operacaoDeSoma.Reset();

        string numero1Str = "123";
        string numero2Str = "45";

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero1Str);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero2Str);

        string resultadoEsperadoFinal = "168";

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSoma);
        LerArquivo.InverterEEscreverResultado(resultadoTemporario, resultadoFinal, operacaoDeSoma);

        string textoLidoDoFinal = File.ReadAllText(resultadoFinal);
        Assert.Equal(resultadoEsperadoFinal, textoLidoDoFinal);
    }

    [Fact]
    public void DeveSomarComVaiUmNoResultadoFinal()
    {
        Somar operacaoDeSoma = new Somar();
        operacaoDeSoma.Reset();

        string numero1Str = "99";
        string numero2Str = "1";

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero1Str);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero2Str);

        string resultadoEsperadoFinal = "100";

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSoma);
        LerArquivo.InverterEEscreverResultado(resultadoTemporario, resultadoFinal, operacaoDeSoma);

        string textoLidoDoFinal = File.ReadAllText(resultadoFinal);
        Assert.Equal(resultadoEsperadoFinal, textoLidoDoFinal);
    }

    [Fact]
    public void DeveDarErroSeNumero2MaiorQue1()
    {
        Subtrair operacaoDeSubtracao = new Subtrair();

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero2);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero1);

        const string exception = "Número 2 é maior do que número 1";

        Exception thrownException = Assert.Throws<Exception>(() =>
        {
            LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSubtracao);
        });

        Assert.Contains(exception, thrownException.Message);
    }

    [Fact]
    public void DeveEscreverResultadoDaSubtracaoNoArquivoTemporario()
    {
        Subtrair operacaoDeSubtracao = new Subtrair();

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero1);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero2);

        double resultadoSubtracao = Convert.ToDouble(numero1) - Convert.ToDouble(numero2);
        string resultadoInvertido = MetodosAuxiliares.InversorDeResultado(resultadoSubtracao);

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSubtracao);

        string lerTextoTemporario;
        using (StreamReader sr = new StreamReader(resultadoTemporario))
        {
            lerTextoTemporario = sr.ReadToEnd();
        }

        Assert.Equal(Convert.ToString(resultadoInvertido), lerTextoTemporario);
    }

    [Fact]
    public void DeveSubtrairCorretamenteNoResultadoFinal()
    {
        Subtrair operacaoDeSubtracao = new Subtrair();

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero1);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero2);

        double resultadoSubtracaoNumerico = Convert.ToDouble(numero1) - Convert.ToDouble(numero2);
        string resultadoEsperadoFinal = resultadoSubtracaoNumerico.ToString("F0");

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSubtracao);
        LerArquivo.InverterEEscreverResultado(resultadoTemporario, resultadoFinal, operacaoDeSubtracao);

        string textoLidoDoFinal;
        using (StreamReader sr = new StreamReader(resultadoFinal))
        {
            textoLidoDoFinal = sr.ReadToEnd();
        }

        Assert.Equal(resultadoEsperadoFinal, textoLidoDoFinal);
    }


    [Fact]
    public void DeveSubtrairNumerosComTamanhosDiferentesNoResultadoFinal()
    {
        Subtrair operacaoDeSubtracao = new Subtrair();

        string numero1Str = "123";
        string numero2Str = "45";

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero1Str);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero2Str);

        string resultadoEsperadoFinal = (Convert.ToDouble(numero1Str) - Convert.ToDouble(numero2Str)).ToString("F0");

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSubtracao);
        LerArquivo.InverterEEscreverResultado(resultadoTemporario, resultadoFinal, operacaoDeSubtracao);

        string textoLidoDoFinal = File.ReadAllText(resultadoFinal);
        Assert.Equal(resultadoEsperadoFinal, textoLidoDoFinal);
    }

    [Fact]
    public void DeveSubtrairComEmprestimoNoResultadoFinal()
    {
        Subtrair operacaoDeSubtracao = new Subtrair();

        string numero1Str = "100";
        string numero2Str = "1";

        MetodosAuxiliares.EscreverNoArquivo(arquivo1, numero1Str);
        MetodosAuxiliares.EscreverNoArquivo(arquivo2, numero2Str);


        string resultadoEsperadoFinal = (Convert.ToDouble(numero1Str) - Convert.ToDouble(numero2Str)).ToString("F0");

        LerArquivo.LerArquivoTrasParaFrente(arquivo1, arquivo2, resultadoTemporario, operacaoDeSubtracao);
        LerArquivo.InverterEEscreverResultado(resultadoTemporario, resultadoFinal, operacaoDeSubtracao);

        string textoLidoDoFinal = File.ReadAllText(resultadoFinal);
        Assert.Equal(resultadoEsperadoFinal, textoLidoDoFinal);
    }
}

public static class MetodosAuxiliares
{
    public static void EscreverNoArquivo(string arquivo1, string numero)
    {
        using (FileStream fs = new FileStream(arquivo1, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter escritor = new StreamWriter(fs))
            {
                escritor.Write(numero);
            }
        }
    }

    public static string InversorDeResultado(double soma)
    {
        string resultadoStr = soma.ToString();
        char[] charArray = resultadoStr.ToCharArray();
        Array.Reverse(charArray);
        string resultadoInvertido = new string(charArray);

        return resultadoInvertido;
    }
}