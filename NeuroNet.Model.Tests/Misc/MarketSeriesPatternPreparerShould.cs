using NUnit.Framework;
using NeuroNet.Model.Misc;
using System.Linq;

namespace NeuroNet.Model.Tests.Misc
{
    [TestFixture]
    public class MarketSeriesPatternPreparerShould
    {
        [Test]
        public void Test()
        {
            var preparer = new MarketSeriesPatternPreparer("marketPatterns.txt", new RealNumberParser());
            var patterns = preparer.PreparePatterns();
            int up = 0;
            int down = 0;
            int mid = 0;
            foreach (var learningPattern in patterns)
            {
                if (learningPattern.Output.ElementAt(2).GetMod().X == 1)
                {
                    down++;
                }
                if (learningPattern.Output.ElementAt(0).GetMod().X == 1)
                {
                    up++;
                }
                if (learningPattern.Output.ElementAt(1).GetMod().X == 1)
                {
                    mid++;
                }
            }

            var max = patterns.Select(pattern => pattern.Input.ElementAt(0).GetMod().X).Max();
            var min = patterns.Select(pattern => pattern.Input.ElementAt(0).GetMod().X).Min();
        }
    }
}