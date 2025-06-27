using System;

namespace Calculadora;

public interface OperacaoMatematica
{
    public double Execute(double x, double y, StreamWriter streamWriter);

}
