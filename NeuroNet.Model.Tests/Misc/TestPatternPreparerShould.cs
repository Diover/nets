using NUnit.Framework;
using NeuroNet.Model.Misc;

namespace NeuroNet.Model.Tests.Misc
{
    [TestFixture]
    public class TestPatternPreparerShould
    {
        private const string _filenameFuzzy = "testPatterns.txt";
        private const string _filenameReal = "testPatternsReal.txt";

        [Test]
        public void ParseFileWithFuzzyNumbers()
        {
            var testPatternsPreparer = new TestPatternPreparer(_filenameFuzzy, new FuzzyNumberParser());
            var patterns = testPatternsPreparer.PreparePatterns();

            foreach (var learningPattern in patterns)
            {
                Assert.That(learningPattern.Input.Count, Is.EqualTo(3));
                Assert.That(learningPattern.Output.Count, Is.EqualTo(1));
            }
            Assert.That(patterns.Count, Is.EqualTo(20));
        }

        [Test]
        public void ParseFileWithRealNumbers()
        {
            var testPatternsPreparer = new TestPatternPreparer(_filenameReal, new RealNumberParser());
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