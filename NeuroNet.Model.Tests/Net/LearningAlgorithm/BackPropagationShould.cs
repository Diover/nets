using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Net;
using NeuroNet.Model.Net.LearningAlgorithm;
using System.Collections.Generic;

namespace NeuroNet.Model.Tests.Net.LearningAlgorithm
{
    [TestFixture]
    public class BackPropagationShould
    {
        [Test, Timeout(2000)]
        public void ReturnZeroErrorOnPatternsSameAsOutput()
        {
            var levelsCount = 3;
            const double errorThreshold = 0.5;
            var patternA = new LearningPattern
                {
                    Input = new List<IFuzzyNumber>
                        {
                            new DiscreteFuzzyNumber(new TriangularFuzzyFunction(-1, 0, 1), 3),
                            new DiscreteFuzzyNumber(new TriangularFuzzyFunction(-1, 0, 1), 3),
                            new DiscreteFuzzyNumber(new TriangularFuzzyFunction(-1, 0, 1), 3),
                        },
                    Output = new List<IFuzzyNumber>(),
                };

            var patternB = new LearningPattern
            {
                Input = new List<IFuzzyNumber>
                        {
                            new DiscreteFuzzyNumber(new TriangularFuzzyFunction(0, 1, 2), 3),
                            new DiscreteFuzzyNumber(new TriangularFuzzyFunction(0, 1, 2), 3),
                            new DiscreteFuzzyNumber(new TriangularFuzzyFunction(0, 1, 2), 3),
                        },
                Output = new List<IFuzzyNumber>()
            };

            var bp = new BackPropagation(new List<ILearningPattern> { patternA, patternB }, 0.0, 0.0, errorThreshold);
            var net = new SimpleFuzzyNet(3, 2, () => DiscreteFuzzyNumber.GenerateLittleNumber(levelsCount: 3),
                                         levelsCount: 3);

            patternA.Output = net.Propagate(patternA.Input);
            patternB.Output = net.Propagate(patternB.Input);

            bp.StepPerformed += (state) => Assert.That(state.CycleError, Is.EqualTo(0.0));
            bp.LearnNet(net);
        }
    }
}