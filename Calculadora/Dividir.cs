namespace Calculadora;

using System.Text;

public class Dividir
{
    public static void ExecutarDivisao(string arquivoDividendo, string arquivoDivisor, string resultadoQuociente, string resultadoResto)
    {
        DivideStringsFileBased(arquivoDividendo, arquivoDivisor, resultadoQuociente, resultadoResto);
    }
    private static void DivideStringsFileBased(string arquivoDividendo, string arquivoDivisor, string resultadoFinal, string resultadoResto)
    {
        if (!File.Exists(arquivoDividendo) || !File.Exists(arquivoDivisor))
        {
            throw new FileNotFoundException("Arquivo do dividendo ou divisor não encontrado.");
        }

        string divisorStr = File.ReadAllText(arquivoDivisor).TrimStart('0');
        if (string.IsNullOrEmpty(divisorStr) || divisorStr == "0")
        {
            throw new DivideByZeroException("O divisor não pode ser zero.");
        }

        ChecarArquivo(resultadoFinal);
        ChecarArquivo(resultadoResto);

        int comparacaoInicial = CompareFileNumbers_StreamBased(arquivoDividendo, arquivoDivisor);
        if (comparacaoInicial < 0)
        {
            File.WriteAllText(resultadoFinal, "0");
            File.Copy(arquivoDividendo, resultadoResto, true);
            return;
        }

        List<string> tempFiles = new List<string>();
        string arquivoDividendoParcial = Path.GetTempFileName();
        tempFiles.Add(arquivoDividendoParcial);

        try
        {
            File.WriteAllText(resultadoFinal, "");
            long divisorLength = new FileInfo(arquivoDivisor).Length;

            using (StreamReader dividendoReader = new StreamReader(arquivoDividendo))
            {
                char[] buffer = new char[(int)divisorLength];
                int bytesRead = dividendoReader.Read(buffer, 0, buffer.Length);
                File.WriteAllText(arquivoDividendoParcial, new string(buffer, 0, bytesRead));

                if (CompareFileNumbers_StreamBased(arquivoDividendoParcial, arquivoDivisor) < 0 && !dividendoReader.EndOfStream)
                {
                    int nextChar = dividendoReader.Read();
                    if (nextChar != -1)
                    {
                        File.AppendAllText(arquivoDividendoParcial, ((char)nextChar).ToString());
                    }
                }

                int loopCount = 0;
                while (true)
                {
                    loopCount++;

                    int digitoQuociente = EncontrarDigitoQuociente(arquivoDividendoParcial, arquivoDivisor, tempFiles);

                    File.AppendAllText(resultadoFinal, digitoQuociente.ToString());

                    if (digitoQuociente > 0)
                    {
                        string produtoTemp = Path.GetTempFileName();
                        string digitoTemp = Path.GetTempFileName();
                        tempFiles.Add(produtoTemp);
                        tempFiles.Add(digitoTemp);
                        File.WriteAllText(digitoTemp, digitoQuociente.ToString());
                        Multiplicar.KaratsubaFileBased(arquivoDivisor, digitoTemp, produtoTemp);
                        string restoDaSubtracao = Path.GetTempFileName();
                        tempFiles.Add(restoDaSubtracao);
                        SubtractFileNumbers(arquivoDividendoParcial, produtoTemp, restoDaSubtracao);
                        File.Copy(restoDaSubtracao, arquivoDividendoParcial, true);
                    }

                    if (dividendoReader.EndOfStream)
                    {
                        break;
                    }

                    int proximoDigitoChar = dividendoReader.Read();
                    if (proximoDigitoChar == -1)
                    {
                        break;
                    }

                    string dividendoParcialAtual = File.ReadAllText(arquivoDividendoParcial).TrimStart('0');

                    if (dividendoParcialAtual == "0")
                    {
                        dividendoParcialAtual = "";
                    }

                    string novoDividendoParcial = dividendoParcialAtual + (char)proximoDigitoChar;

                    File.WriteAllText(arquivoDividendoParcial, novoDividendoParcial);

                }

            }
        }
        finally
        {
            foreach (var file in tempFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
    }

    private static int EncontrarDigitoQuociente(string arquivoDividendoParcial, string arquivoDivisor, List<string> tempFiles)
    {
        int menor = 0;
        int maior = 9;
        int digitoCandidato = 0;

        while (menor <= maior)
        {
            int meio = menor + (maior - menor) / 2;
            if (meio == 0)
            {
                menor = meio + 1;
                continue;
            }

            string produtoTemp = Path.GetTempFileName();
            string digitoTemp = Path.GetTempFileName();
            tempFiles.Add(produtoTemp);
            tempFiles.Add(digitoTemp);

            File.WriteAllText(digitoTemp, meio.ToString());
            Multiplicar.KaratsubaFileBased(arquivoDivisor, digitoTemp, produtoTemp);

            int comparacao = CompareFileNumbers_StreamBased(produtoTemp, arquivoDividendoParcial);

            if (comparacao <= 0)
            {
                digitoCandidato = meio;
                menor = meio + 1;
            }
            else
            {
                maior = meio - 1;
            }
        }
        return digitoCandidato;
    }
    private static void SubtractFileNumbers(string arquivoNum1, string arquivoNum2, string resultadoPath)
    {
        string num1 = File.ReadAllText(arquivoNum1).TrimStart('0');
        string num2 = File.ReadAllText(arquivoNum2).TrimStart('0');

        StringBuilder resultado = new StringBuilder();
        int n1 = num1.Length;
        int n2 = num2.Length;
        int emprestimo = 0;

        int i = n1 - 1, j = n2 - 1;

        while (j >= 0)
        {
            int sub = ((num1[i] - '0') - (num2[j] - '0') - emprestimo);
            if (sub < 0)
            {
                sub += 10;
                emprestimo = 1;
            }
            else
            {
                emprestimo = 0;
            }
            resultado.Insert(0, sub);
            i--;
            j--;
        }

        while (i >= 0)
        {
            int sub = ((num1[i] - '0') - emprestimo);
            if (sub < 0)
            {
                sub += 10;
                emprestimo = 1;
            }
            else
            {
                emprestimo = 0;
            }
            resultado.Insert(0, sub);
            i--;
        }

        File.WriteAllText(resultadoPath, resultado.ToString().TrimStart('0'));
    }
    private static void ChecarArquivo(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static int CompareFileNumbers_StreamBased(string arquivo1, string arquivo2)
    {
        long length1 = new FileInfo(arquivo1).Length;
        long length2 = new FileInfo(arquivo2).Length;

        if (length1 > length2) return 1;
        if (length1 < length2) return -1;

        const int BLOCK_SIZE = 8192; // Bloco de 8KB
        char[] buffer1 = new char[BLOCK_SIZE];
        char[] buffer2 = new char[BLOCK_SIZE];

        using (StreamReader reader1 = new StreamReader(arquivo1))
        using (StreamReader reader2 = new StreamReader(arquivo2))
        {
            while (!reader1.EndOfStream)
            {
                int bytesRead1 = reader1.Read(buffer1, 0, BLOCK_SIZE);
                int bytesRead2 = reader2.Read(buffer2, 0, BLOCK_SIZE);

                for (int i = 0; i < bytesRead1; i++)
                {
                    if (buffer1[i] > buffer2[i]) return 1;
                    if (buffer1[i] < buffer2[i]) return -1;
                }
            }
        }

        return 0;
    }
}