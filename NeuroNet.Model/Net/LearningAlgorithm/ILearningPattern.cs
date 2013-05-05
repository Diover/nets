using System.Collections.Generic;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public interface ILearningPattern
    {
        List<IFuzzyNumber> Input { get; set; }
        List<IFuzzyNumber> Output { get; set; }
    }
}