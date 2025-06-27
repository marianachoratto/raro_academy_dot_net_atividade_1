using System;
using System.IO;
using System.Text;
using Calculadora;

public class LerArquivo
{
    static public void LerArquivoTrasParaFrente(string arquivo1, string arquivo2, string arquivoTemporario, OperacaoMatematica operacao)
    {
        ChecarArquivoTemporario(arquivoTemporario);

        using (FileStream fs1 = new FileStream(arquivo1, FileMode.Open, FileAccess.Read))
        {
            using (FileStream fs2 = new FileStream(arquivo2, FileMode.Open, FileAccess.Read))
            {
                using (StreamWriter swSaidaTemp = new StreamWriter(arquivoTemporario, false))
                {
                    long posicaoAtual1 = fs1.Length - 1;
                    long posicaoAtual2 = fs2.Length - 1;

                    Validacoes.ChecarTamanhoNumero(fs1, fs2);


                    while (posicaoAtual1 >= 0 || posicaoAtual2 >= 0)
                    {
                        int caracter1 = -1;
                        int caracter2 = -1;

                        double numero1 = 0;
                        double numero2 = 0;

                        if (posicaoAtual1 >= 0)
                        {
                            fs1.Seek(posicaoAtual1, SeekOrigin.Begin);
                            caracter1 = fs1.ReadByte();
                            char valorCorreto1 = (char)caracter1;
                            numero1 = char.GetNumericValue(valorCorreto1);

                            if (numero1 == -1) numero1 = 0;

                        }

                        if (posicaoAtual2 >= 0)
                        {
                            fs2.Seek(posicaoAtual2, SeekOrigin.Begin);
                            caracter2 = fs2.ReadByte();
                            char valorCorreto2 = (char)caracter2;
                            numero2 = char.GetNumericValue(valorCorreto2);

                            if (numero2 == -1) numero2 = 0;

                        }


                        double resultadoOperacao = operacao.Execute(numero1, numero2, swSaidaTemp);

                        posicaoAtual1--;
                        posicaoAtual2--;

                    }
                    if (operacao is Somar somarOperacao)
                    {
                        if (somarOperacao.GetVaiUmAnterior())
                        {
                            swSaidaTemp.Write("1");
                        }
                    }

                }
            }
        }

    }

    static public void InverterEEscreverResultado(string arquivoTemporario, string resultadoFinal, OperacaoMatematica operacao)
    {
        if (File.Exists(resultadoFinal))
        {
            File.Delete(resultadoFinal);
        }

        using (FileStream fsTempRead = new FileStream(arquivoTemporario, FileMode.Open, FileAccess.Read))
        {
            using (StreamWriter swResultadoFinal = new StreamWriter(resultadoFinal, false))
            {
                long posicaoAtualNoArqTemp = fsTempRead.Length - 1;

                bool pulandoZerosIniciais = true;

                while (posicaoAtualNoArqTemp >= 0)
                {
                    fsTempRead.Seek(posicaoAtualNoArqTemp, SeekOrigin.Begin);
                    int byteLido = fsTempRead.ReadByte();

                    if (byteLido != -1)
                    {
                        char charDecodificado = (char)byteLido;

                        if (operacao is Subtrair)
                        {
                            if (pulandoZerosIniciais && charDecodificado == '0' && posicaoAtualNoArqTemp > 0)
                            {
                            }
                            else
                            {
                                pulandoZerosIniciais = false;
                                swResultadoFinal.Write(charDecodificado);
                            }
                        }
                        else
                        {
                            swResultadoFinal.Write(charDecodificado);
                        }
                    }
                    posicaoAtualNoArqTemp--;
                }

                if (operacao is Subtrair && pulandoZerosIniciais && swResultadoFinal.BaseStream.Length == 0)
                {
                    swResultadoFinal.Write('0');
                }

                swResultadoFinal.Close();
            }
        }
    }

    static private void ChecarArquivoTemporario(string arquivoTemporario)
    {
        if (File.Exists(arquivoTemporario))
        {
            File.Delete(arquivoTemporario);
        }
    }

    static private double LerDigitoAtual(FileStream fs, long posicaoAtual)
    {
        fs.Seek(posicaoAtual, SeekOrigin.Begin);
        int caracter = fs.ReadByte();
        char valorCorreto = (char)caracter;
        double numero = char.GetNumericValue(valorCorreto);
        return numero;
    }
}
