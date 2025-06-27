public class Multiplicar
{
    private const int MAX_DIGIT_BLOCK_SIZE = 9;

    public static void ExecutarMultiplicacaoDeArquivos(string arquivo1, string arquivo2, string resultadoTemporario, string resultadoFinal)
    {
        CleanUpFixedTempFiles();

        IgualarTamanhoNumerosNosArquivos(arquivo1, arquivo2);

        KaratsubaFileBased(arquivo1, arquivo2, resultadoFinal);

        CleanUpFixedTempFiles();
    }

    public double Execute(double x, double y, StreamWriter streamWriter)
    {
        long num1 = (long)x;
        long num2 = (long)y;
        long multiplicacao = checked(num1 * num2);
        streamWriter.Write(multiplicacao.ToString());
        return (double)multiplicacao;

    }

    private static void KaratsubaFileBased(string pathNum1, string pathNum2, string pathResult)
    {
        List<string> tempFilesToCleanUp = new List<string>();
        string currentCallId = Path.GetRandomFileName().Substring(0, 4);

        try
        {

            string num1Content = File.ReadAllText(pathNum1);
            long n = num1Content.Length;

            if (n <= MAX_DIGIT_BLOCK_SIZE)
            {

                string num1ContentTemp = File.ReadAllText(pathNum1);
                string num2ContentTemp = File.ReadAllText(pathNum2);

                long num1 = long.Parse(num1ContentTemp);
                long num2 = long.Parse(num2ContentTemp);
                long product = checked(num1 * num2);
                File.WriteAllText(pathResult, product.ToString());
                return;
            }

            if (n % 2 != 0)
            {
                string paddedNum1Path = Path.GetTempFileName();
                string paddedNum2Path = Path.GetTempFileName();
                tempFilesToCleanUp.Add(paddedNum1Path);
                tempFilesToCleanUp.Add(paddedNum2Path);

                PrependZeroToFile(pathNum1, paddedNum1Path);
                PrependZeroToFile(pathNum2, paddedNum2Path);

                KaratsubaFileBased(paddedNum1Path, paddedNum2Path, pathResult);
                return;
            }

            long half = n / 2;

            // Dividir os números em A, B, C, D e salvá-los em arquivos temporários.
            string tempA_Path = Path.GetTempFileName();
            string tempB_Path = Path.GetTempFileName();
            string tempC_Path = Path.GetTempFileName();
            string tempD_Path = Path.GetTempFileName();

            tempFilesToCleanUp.Add(tempA_Path); tempFilesToCleanUp.Add(tempB_Path);
            tempFilesToCleanUp.Add(tempC_Path); tempFilesToCleanUp.Add(tempD_Path);
            SplitNumberFile(pathNum1, tempA_Path, tempB_Path, half);
            SplitNumberFile(pathNum2, tempC_Path, tempD_Path, half);


            // Chamadas recursivas para P1 = Karatsuba(A, C) e P0 = Karatsuba(B, D).
            string tempAC_Path = Path.GetTempFileName();
            string tempBD_Path = Path.GetTempFileName();

            tempFilesToCleanUp.Add(tempAC_Path); tempFilesToCleanUp.Add(tempBD_Path);
            KaratsubaFileBased(tempA_Path, tempC_Path, tempAC_Path); // Calcula P1
            KaratsubaFileBased(tempB_Path, tempD_Path, tempBD_Path); // Calcula P0

            // 3. Calcular (A+B) e (C+D) e salvar os resultados em arquivos temporários.
            string tempSumAB_Path = Path.GetTempFileName();
            string tempSumCD_Path = Path.GetTempFileName();

            tempFilesToCleanUp.Add(tempSumAB_Path); tempFilesToCleanUp.Add(tempSumCD_Path);
            SumFileNumbers(tempA_Path, tempB_Path, tempSumAB_Path);
            SumFileNumbers(tempC_Path, tempD_Path, tempSumCD_Path);

            // 4. Chamada recursiva para P2 = Karatsuba((A+B), (C+D)).
            string tempP2Path = Path.GetTempFileName();
            tempFilesToCleanUp.Add(tempP2Path);
            KaratsubaFileBased(tempSumAB_Path, tempSumCD_Path, tempP2Path);

            // 5. Calcular ad_plus_bc = P2 - P1 - P0.
            string p1Str = File.ReadAllText(tempAC_Path);
            string p0Str = File.ReadAllText(tempBD_Path);
            string p2Str = File.ReadAllText(tempP2Path);

            string diffP2P1 = SubtractStrings(p2Str, p1Str);
            string ad_plus_bc_str = SubtractStrings(diffP2P1, p0Str);


            // 6. Combinar os resultados finais usando a fórmula de Karatsuba:
            string ac_str = p1Str;
            string bd_str = p0Str;

            string term1 = MultiplyStringByPowerOfTen(ac_str, n);
            string term2 = MultiplyStringByPowerOfTen(ad_plus_bc_str, half);

            string tempSum1 = AddStrings(term1, term2);
            string finalResult = AddStrings(tempSum1, bd_str);

            File.WriteAllText(pathResult, finalResult);
        }
        finally
        {
            foreach (string filePath in tempFilesToCleanUp)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }

    private static void SplitNumberFile(string sourcePath, string part1Path, string part2Path, long halfLength)
    {
        string sourceContent = File.ReadAllText(sourcePath);

        if (halfLength > sourceContent.Length)
        {
            halfLength = sourceContent.Length;
        }

        string part1Content = sourceContent.Substring(0, (int)halfLength);
        string part2Content = sourceContent.Substring((int)halfLength);

        File.WriteAllText(part1Path, part1Content);
        File.WriteAllText(part2Path, part2Content);
    }

    private static void SumFileNumbers(string pathNum1, string pathNum2, string pathResult)
    {
        string num1Str = File.ReadAllText(pathNum1);
        string num2Str = File.ReadAllText(pathNum2);

        string sumResult = AddStrings(num1Str, num2Str);
        File.WriteAllText(pathResult, sumResult);

    }

    private static void IgualarTamanhoNumerosNosArquivos(string arquivo1, string arquivo2)
    {
        string num1Content = File.ReadAllText(arquivo1);
        string num2Content = File.ReadAllText(arquivo2);

        long len1 = num1Content.Length;
        long len2 = num2Content.Length;

        string sourceFileToPadPath;
        string contentToPad;
        long diff = Math.Abs(len1 - len2);

        if (len1 < len2)
        {
            sourceFileToPadPath = arquivo1;
            contentToPad = num1Content;
        }
        else
        {
            sourceFileToPadPath = arquivo2;
            contentToPad = num2Content;
        }

        string paddingZeros = new string('0', (int)diff);
        string paddedContent = paddingZeros + contentToPad;
        File.WriteAllText(sourceFileToPadPath, paddedContent);

    }

    private static void PrependZeroToFile(string sourcePath, string destinationPath)
    {
        string content = File.ReadAllText(sourcePath);
        string newContent = "0" + content;
        File.WriteAllText(destinationPath, newContent);
    }

    private static string PowerOfTen(long exponent)
    {
        return "1" + new string('0', (int)exponent);
    }

    private static string MultiplyStringByPowerOfTen(string num, long exponent)
    {
        if (num == "0") return "0";
        return num + new string('0', (int)exponent);
    }

    private static string AddStrings(string num1, string num2)
    {
        string originalNum1 = num1;
        string originalNum2 = num2;

        num1 = num1.TrimStart('0');
        num2 = num2.TrimStart('0');

        if (string.IsNullOrEmpty(num1)) num1 = "0";
        if (string.IsNullOrEmpty(num2)) num2 = "0";


        if (num1.Length < num2.Length)
        {
            string temp = num1;
            num1 = num2;
            num2 = temp;
        }

        char[] result = new char[num1.Length + 1];
        int carry = 0;
        int i = num1.Length - 1;
        int j = num2.Length - 1;
        int k = result.Length - 1;

        while (i >= 0)
        {

            int digit1 = num1[i] - '0';
            int digit2 = (j >= 0) ? (num2[j] - '0') : 0;

            int sum = digit1 + digit2 + carry;
            result[k] = (char)((sum % 10) + '0');
            carry = sum / 10;

            i--;
            j--;
            k--;
        }

        if (carry > 0)
        {
            result[k] = (char)(carry + '0');
        }
        else
        {
            k++;
        }

        string finalResult = new string(result, k, result.Length - k).TrimStart('0');
        if (string.IsNullOrEmpty(finalResult)) finalResult = "0";

        return finalResult;
    }

    private static string SubtractStrings(string num1, string num2)
    {
        string originalNum1 = num1;
        string originalNum2 = num2;

        num1 = num1.TrimStart('0');
        num2 = num2.TrimStart('0');
        if (string.IsNullOrEmpty(num1)) num1 = "0";
        if (string.IsNullOrEmpty(num2)) num2 = "0";

        if (IsGreaterThan(num2, num1))
        {
            return "0";
        }

        num2 = num2.PadLeft(num1.Length, '0');

        char[] result = new char[num1.Length];
        int borrow = 0;

        for (int i = num1.Length - 1; i >= 0; i--)
        {

            int digit1 = num1[i] - '0';
            int digit2 = num2[i] - '0';

            int diff = digit1 - digit2 - borrow;

            if (diff < 0)
            {
                diff += 10;
                borrow = 1;
            }
            else
            {
                borrow = 0;
            }
            result[i] = (char)(diff + '0');
        }

        string finalResult = new string(result).TrimStart('0');
        if (string.IsNullOrEmpty(finalResult)) finalResult = "0";

        return finalResult;
    }

    private static bool IsGreaterThan(string num1, string num2)
    {
        num1 = num1.TrimStart('0');
        num2 = num2.TrimStart('0');
        if (num1.Length > num2.Length) return true;
        if (num1.Length < num2.Length) return false;
        return string.CompareOrdinal(num1, num2) > 0;
    }

    private static void CleanUpFixedTempFiles()
    {
        string[] fixedTempFileNames = {
            "TEMP_FILE_A", "TEMP_FILE_B", "TEMP_FILE_C", "TEMP_FILE_D",
            "TEMP_FILE_AC", "TEMP_FILE_BD", "TEMP_FILE_ADBC",
            "TEMP_FILE_SUM_AB", "TEMP_FILE_SUM_CD"
        };

        foreach (string fileName in fixedTempFileNames)
        {

            if (File.Exists(fileName + ".txt"))
            {
                File.Delete(fileName + ".txt");
            }
            else if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}
