using System.Collections.Generic;
using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Net;
using System.Linq;

namespace NeuroNet.Model.Tests.Misc
{
    [TestFixture]
    public class BinarySerializerShould
    {
        private const string _filename = "binarySerializerTest.net";

        [Test]
        public void SaveAndLoadNetState()
        {
            const int inputsCount = 3;
            var net = new SimpleFuzzyNet(inputsCount, new[] {2},
                                         levelsCount: 3,
                                         littleFuzzyNumberGenerator:
                                             () => DiscreteFuzzyNumber.GenerateLittleNumber(levelsCount: 3));

            var first = new DiscreteFuzzyNumber(new TriangularFuzzyFunction(-1, 0, 1), 3);
            var second = new DiscreteFuzzyNumber(new TriangularFuzzyFunction(-1, -0.5, 0), 3);
            var third = new DiscreteFuzzyNumber(new TriangularFuzzyFunction(0, 0.5, 1), 3);
            
            var inputs = new List<IFuzzyNumber>
                {
                    first,
                    second,
                    third,
                };

            var expectedOutput = net.Propagate(inputs).First();

            BinaryFileSerializer.SaveNetState(_filename, net);
            var loadedNet = BinaryFileSerializer.LoadNetState(_filename);

            var actualOutput = loadedNet.PropagateLastInput().First();

            expectedOutput.ForeachLevel((alpha, level) => Assert.That(level, Is.EqualTo(actualOutput.GetAlphaLevel(alpha))));
        }
    }
}