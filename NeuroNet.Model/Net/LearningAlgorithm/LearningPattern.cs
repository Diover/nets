using System.Collections.Generic;
using System.Text;
using NeuroNet.Model.FuzzyNumbers;
using System.Linq;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public class LearningPattern : ILearningPattern
    {
        public LearningPattern()
        {
            Input = null;
            Output = null;
        }

        public LearningPattern(List<IFuzzyNumber> input, List<IFuzzyNumber> output)
        {
            Input = input;
            Output = output;
        }

        public List<IFuzzyNumber> Input { get; set; }
        public List<IFuzzyNumber> Output { get; set; }

        public override string ToString()
        {
            return string.Join(",", Input.Select(number => number.ToString())) + ": " + string.Join(",", Output.Select(number => number.ToString()));
        }
    }
}