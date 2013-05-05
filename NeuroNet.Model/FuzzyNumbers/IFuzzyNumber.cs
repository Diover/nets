using System;

namespace NeuroNet.Model.FuzzyNumbers
{
    public interface IFuzzyNumber : IFuzzyFunction
    {
        IFuzzyNumber Mul(IFuzzyNumber x);
        IFuzzyNumber Sum(IFuzzyNumber x);
        IFuzzyNumber Sub(IFuzzyNumber x);
        IFuzzyNumber Div(IFuzzyNumber x);

        IFuzzyNumber Mul(double factor);
        IFuzzyNumber Sum(double factor);
        IFuzzyNumber Sub(double factor);
        IFuzzyNumber Div(double factor);

        void Set(IFuzzyNumber source);

        bool ContainsAlphaLevel(double alpha);
        int LevelsCount { get; }
        void ForeachLevel(Action<double, IntervalD> action);
        IFuzzyNumber Operation(Func<double, double, double> f, double factor);
        IFuzzyNumber Apply(Func<double, double> f);
    }
}