using System;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net
{
    public interface IFuzzyNeuron
    {
        void AddInput(ILink link);
        void AddInput(ILink link, IFuzzyNumber weight);
        void SetOutput(ILink link);
        void Propagate();

        IFuzzyNumber LastOutput { get; }
        IFuzzyNumber PropagatedError { get; set; }

        ILink GetWeight(int i);
        ILink GetWeightDelta(int i);
        void ForeachWeight(Action<int, ILink> action);
        ILink GetLastInput(int i);
        void SetWeightLambda(int i, double newValue);
        double GetWeightLambda(int i);
    }
}