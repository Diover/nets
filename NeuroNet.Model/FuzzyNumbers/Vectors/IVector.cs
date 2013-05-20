using System.Collections.Generic;
using NeuroNet.Model.FuzzyNumbers.Matrixes;

namespace NeuroNet.Model.FuzzyNumbers.Vectors
{
    public interface IVector
    {
        IFuzzyNumber this[int i] { get; set; }
        IFuzzyNumber Mul(IVector x);
        IVector Mul(IFuzzyNumber x);
        IVector Mul(double x);
        int Length { get; }
        IVector Negate();
        IVector Sum(IVector x);
        IMatrix OuterMul(IVector x);

        Queue<IFuzzyNumber> ToQueue();
    }
}