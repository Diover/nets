using System.Collections.Generic;

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

        Queue<IFuzzyNumber> ToQueue();
    }
}