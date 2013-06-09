using System.Collections.Generic;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public class MarketTickPattern : ILearningPattern
    {
        public MarketTickPattern()
        {
            Input = null;
            Output = null;
        }

        public MarketTickPattern(List<IFuzzyNumber> input, List<IFuzzyNumber> output)
        {
            Input = input;
            Output = output;
        }

        public List<IFuzzyNumber> Input { get; set; }
        public List<IFuzzyNumber> Output { get; set; }
    }
}