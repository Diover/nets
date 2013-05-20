using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.FuzzyNumbers.Matrixes
{
    public interface IMatrix
    {
        IFuzzyNumber this[int i, int j] { get; set; }
        int Rows { get; }
        int Columns { get; }

        IMatrix Mul(IMatrix x);
        IMatrix Sum(IMatrix x);
        IMatrix Sub(IMatrix x);

        IVector Mul(IVector x);
        IMatrix Mul(IFuzzyNumber x);
    }
}