using System;
using System.Collections.Generic;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net
{
    public interface ILayer
    {
        void AddNeuron(IFuzzyNeuron neuron);
        void ForeachNeuron(Action<int, IFuzzyNeuron> action);
        void Propagate();
        void ConnectTo(ILayer nextLayer);
        void ConnectTo(List<ILink> nextLayer);
        int NeuronsCount { get; }
        List<IFuzzyNumber> LastOutput { get; }
        IFuzzyNeuron GetNeuron(int i);
    }
}