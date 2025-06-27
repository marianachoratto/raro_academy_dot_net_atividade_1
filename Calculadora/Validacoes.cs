using System;

namespace Calculadora;

public class Validacoes
{
    public static void ChecarTamanhoNumero(FileStream fs1, FileStream fs2)
    {
        if (fs1.Length < fs2.Length)
        {
            LancarExcecao();
        }
        else if (fs1.Length == fs2.Length)
        {
            int caracter1 = fs1.ReadByte();
            int caracter2 = fs2.ReadByte();
            char valorCorreto1 = (char)caracter1;
            char valorCorreto2 = (char)caracter2;
            double numero1 = char.GetNumericValue(valorCorreto1);
            double numero2 = char.GetNumericValue(valorCorreto2);

            if (numero2 > numero1)
            {
                LancarExcecao();
            }
        }
    }

    private static void LancarExcecao()
    {
        throw new Exception("Número 2 é maior do que número 1");
    }
}
