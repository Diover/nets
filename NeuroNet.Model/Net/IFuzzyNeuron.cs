using System;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net
{
    public interface IFuzzyNeuron
    {
        void AddInput(ILink link);
        void SetOutput(ILink link);
        void Propagate();
        void AddInput(ILink link, IFuzzyNumber weight);
        IFuzzyNumber LastOutput { get; }
        IFuzzyNumber PropagatedError { get; set; }
        ILink GetWeight(int i);
        ILink GetWeightDelta(int i);
        void ForeachWeight(Action<int, ILink> action);
        ILink GetLastInput(int i);
    }
}