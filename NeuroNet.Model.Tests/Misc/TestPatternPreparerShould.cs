using NUnit.Framework;
using NeuroNet.Model.Misc;

namespace NeuroNet.Model.Tests.Misc
{
    [TestFixture]
    public class TestPatternPreparerShould
    {
        private const string _filename = "H:\\testPatterns.txt";
        [Test]
        public void ParseFile()
        {
            var testPatternsPreparer = new TestPatternPreparer(_filename);
            var patterns = testPatternsPreparer.PreparePatterns();

            foreach (var learningPattern in patterns)
            {
                Assert.That(learningPattern.Input.Count, Is.EqualTo(3));
                Assert.That(learningPattern.Output.Count, Is.EqualTo(1));
            }
            Assert.That(patterns.Count, Is.EqualTo(20));
        }
    }
}