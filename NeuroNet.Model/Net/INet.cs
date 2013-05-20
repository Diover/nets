using System;
using System.Collections.Generic;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Net.LearningAlgorithm;

namespace NeuroNet.Model.Net
{
    public interface INet
    {
        List<IFuzzyNumber> Propagate(List<IFuzzyNumber> inputs);
        List<IFuzzyNumber> PropagateLastInput();
        List<IFuzzyNumber> LastOutput { get; }
        List<ILayer> Layers { get; }
        int WeightsCount { get; }
    }
}