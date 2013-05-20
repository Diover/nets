namespace NeuroNet.Model.FuzzyNumbers.Vectors
{
    public interface IVector
    {
        IFuzzyNumber this[int i] { get; set; }
        IFuzzyNumber Mul(IVector x);
        IVector Mul(IFuzzyNumber x);
        int Length { get; }
    }
}