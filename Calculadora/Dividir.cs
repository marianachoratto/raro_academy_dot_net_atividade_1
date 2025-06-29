namespace Calculadora;

using System.Text;

public class Dividir
{
    public static void ExecutarDivisao(string arquivoDividendo, string arquivoDivisor, string resultadoQuociente, string resultadoResto)
    {
        DivideStringsFileBased(arquivoDividendo, arquivoDivisor, resultadoQuociente, resultadoResto);
    }

    /// Implementa a divisão longa de números armazenados em arquivos.
    private static void DivideStringsFileBased(string arquivoDividendo, string arquivoDivisor, string resultadoFinal, string resultadoResto)
    {
        Console.WriteLine("--- INICIANDO DIVISÃO ---");
        Console.WriteLine($"Dividendo: {File.ReadAllText(arquivoDividendo)} ({arquivoDividendo})");
        Console.WriteLine($"Divisor: {File.ReadAllText(arquivoDivisor)} ({arquivoDivisor})");
        Console.WriteLine("---------------------------\n");

        // 1. Validações Iniciais
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
        Console.WriteLine($"Comparação inicial (Dividendo vs Divisor): {comparacaoInicial}");
        if (comparacaoInicial < 0)
        {
            Console.WriteLine("CASO SIMPLES: Dividendo é menor que o divisor. Quociente = 0.");
            File.WriteAllText(resultadoFinal, "0");
            File.Copy(arquivoDividendo, resultadoResto, true);
            return;
        }

        List<string> tempFiles = new List<string>();
        string arquivoDividendoParcial = Path.GetTempFileName();
        tempFiles.Add(arquivoDividendoParcial);
        Console.WriteLine($"Criado arquivo de dividendo parcial temporário: {arquivoDividendoParcial}");

        try
        {
            // 2. Preparação para a Divisão
            File.WriteAllText(resultadoFinal, "");
            long divisorLength = new FileInfo(arquivoDivisor).Length;
            Console.WriteLine($"\n--- FASE DE PREPARAÇÃO ---");
            Console.WriteLine($"Comprimento do divisor: {divisorLength}");

            using (StreamReader dividendoReader = new StreamReader(arquivoDividendo))
            {
                // 3. Pega a parte inicial do dividendo
                char[] buffer = new char[(int)divisorLength];
                int bytesRead = dividendoReader.Read(buffer, 0, buffer.Length);
                File.WriteAllText(arquivoDividendoParcial, new string(buffer, 0, bytesRead));
                Console.WriteLine($"Pegou a parte inicial do dividendo: '{File.ReadAllText(arquivoDividendoParcial)}'");

                // Se a parte inicial for menor que o divisor, pega mais um dígito
                if (CompareFileNumbers_StreamBased(arquivoDividendoParcial, arquivoDivisor) < 0 && !dividendoReader.EndOfStream)
                {
                    Console.WriteLine("Dividendo parcial inicial é menor que o divisor. Pegando próximo dígito...");
                    int nextChar = dividendoReader.Read();
                    if (nextChar != -1)
                    {
                        File.AppendAllText(arquivoDividendoParcial, ((char)nextChar).ToString());
                        Console.WriteLine($"Anexou '{(char)nextChar}'. Novo dividendo parcial: '{File.ReadAllText(arquivoDividendoParcial)}'");
                    }
                }
                Console.WriteLine($"FIM DA PREPARAÇÃO. Posição no dividendo: EndOfStream = {dividendoReader.EndOfStream}");

                // 4. Loop Principal da Divisão Longa
                Console.WriteLine("\n--- INÍCIO DO LOOP PRINCIPAL ---\n");
                int loopCount = 0;
                while (true)
                {
                    loopCount++;
                    Console.WriteLine($"--- Iteração do Loop #{loopCount} ---");
                    Console.WriteLine($"Estado atual: Quociente='{File.ReadAllText(resultadoFinal)}', Dividendo Parcial='{File.ReadAllText(arquivoDividendoParcial)}'");

                    int digitoQuociente = EncontrarDigitoQuociente(arquivoDividendoParcial, arquivoDivisor, tempFiles);
                    Console.WriteLine($"==> Digito do quociente encontrado: {digitoQuociente}");

                    File.AppendAllText(resultadoFinal, digitoQuociente.ToString());
                    Console.WriteLine($"Anexado ao quociente. Quociente agora: '{File.ReadAllText(resultadoFinal)}'");

                    if (digitoQuociente > 0)
                    {
                        Console.WriteLine("Atualizando dividendo parcial (subtração)...");
                        string produtoTemp = Path.GetTempFileName();
                        string digitoTemp = Path.GetTempFileName();
                        tempFiles.Add(produtoTemp);
                        tempFiles.Add(digitoTemp);
                        File.WriteAllText(digitoTemp, digitoQuociente.ToString());
                        Multiplicar.KaratsubaFileBased(arquivoDivisor, digitoTemp, produtoTemp);
                        Console.WriteLine($"    - Produto (dígito * divisor): '{File.ReadAllText(produtoTemp)}'");
                        string restoDaSubtracao = Path.GetTempFileName();
                        tempFiles.Add(restoDaSubtracao);
                        SubtractFileNumbers(arquivoDividendoParcial, produtoTemp, restoDaSubtracao);
                        File.Copy(restoDaSubtracao, arquivoDividendoParcial, true);
                        Console.WriteLine($"    - Novo dividendo parcial (resto da subtração): '{File.ReadAllText(arquivoDividendoParcial)}'");
                    }

                    // if (dividendoReader.EndOfStream)
                    // {
                    //     Console.WriteLine("Fim do arquivo de dividendo principal alcançado. Saindo do loop.");
                    //     break;
                    // }

                    // Console.WriteLine("Pegando próximo dígito do dividendo principal...");
                    // int proximoDigitoChar = dividendoReader.Read();
                    // if (proximoDigitoChar == -1)
                    // {
                    //     Console.WriteLine("Não há mais dígitos para ler. Saindo do loop.");
                    //     break;
                    // }

                    // string dividendoParcialAtual = File.ReadAllText(arquivoDividendoParcial);

                    if (dividendoReader.EndOfStream)
                    {
                        Console.WriteLine("FIM DO FLUXO: Fim do arquivo de dividendo principal alcançado. Saindo do loop.");
                        break;
                    }

                    // Lê o próximo dígito do arquivo principal
                    int proximoDigitoChar = dividendoReader.Read();
                    if (proximoDigitoChar == -1)
                    {
                        Console.WriteLine("FIM DO FLUXO: Não há mais dígitos para ler. Saindo do loop.");
                        break;
                    }

                    // Pega o resto atual (que está no arquivo do dividendo parcial)
                    string dividendoParcialAtual = File.ReadAllText(arquivoDividendoParcial).TrimStart('0');

                    // Se o resto for "0", tratamos como uma string vazia para não gerar "04" em vez de "4"
                    if (dividendoParcialAtual == "0")
                    {
                        dividendoParcialAtual = "";
                    }

                    // Forma o novo dividendo parcial JUNTANDO o resto com o novo dígito
                    string novoDividendoParcial = dividendoParcialAtual + (char)proximoDigitoChar;

                    // Escreve o novo valor de volta no arquivo temporário
                    File.WriteAllText(arquivoDividendoParcial, novoDividendoParcial);

                    // Log para confirmar a operação
                    Console.WriteLine($"Anexou '{(char)proximoDigitoChar}'. Novo dividendo parcial para próxima iteração: '{novoDividendoParcial}'");
                    Console.WriteLine("---------------------------\n");

                }

            }
        }
        catch (Exception ex)
        {

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
            { // Evita multiplicar por zero desnecessariamente
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

            if (comparacao <= 0) // produto <= dividendoParcial
            {
                digitoCandidato = meio;
                menor = meio + 1;
            }
            else // produto > dividendoParcial
            {
                maior = meio - 1;
            }
        }
        return digitoCandidato;
    }

    /// <summary>
    /// Subtrai dois números grandes representados em arquivos (num1 - num2).
    /// Assume que num1 >= num2.
    /// </summary>
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

    /// <summary>
    /// Deleta um arquivo se ele existir, para garantir que o resultado comece limpo.
    /// </summary>
    private static void ChecarArquivo(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// Compara dois números representados em arquivos.
    /// Retorna -1 se num1 < num2, 0 se num1 == num2, 1 se num1 > num2.
    /// </summary>
    private static int CompareFileNumbers_StreamBased(string arquivo1, string arquivo2)
    {
        // Etapa 1: Comparar pelo comprimento do arquivo (número de dígitos)
        // Esta é a otimização mais importante e rápida.
        long length1 = new FileInfo(arquivo1).Length;
        long length2 = new FileInfo(arquivo2).Length;

        // Ignora zeros à esquerda, o que complica a comparação de tamanho puro.
        // Uma forma de lidar com isso é ler apenas os primeiros caracteres para checar.
        // No nosso algoritmo, os arquivos temporários raramente terão zeros à esquerda significativos.

        if (length1 > length2) return 1;
        if (length1 < length2) return -1;

        // Etapa 2: Se os comprimentos são iguais, compare bloco a bloco
        const int BLOCK_SIZE = 8192; // Bloco de 8KB, um bom tamanho padrão para I/O
        char[] buffer1 = new char[BLOCK_SIZE];
        char[] buffer2 = new char[BLOCK_SIZE];

        using (StreamReader reader1 = new StreamReader(arquivo1))
        using (StreamReader reader2 = new StreamReader(arquivo2))
        {
            while (!reader1.EndOfStream) // Se um não terminou, o outro também não (tamanhos iguais)
            {
                int bytesRead1 = reader1.Read(buffer1, 0, BLOCK_SIZE);
                int bytesRead2 = reader2.Read(buffer2, 0, BLOCK_SIZE);

                // Compara os blocos lidos caractere por caractere
                for (int i = 0; i < bytesRead1; i++)
                {
                    if (buffer1[i] > buffer2[i]) return 1;
                    if (buffer1[i] < buffer2[i]) return -1;
                }
            }
        }

        // Se todos os blocos foram idênticos, os números são iguais
        return 0;
    }
}