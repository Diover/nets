using System.Collections.Generic;
using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Net;
using System.Linq;

namespace NeuroNet.Model.Tests.Net
{
    [TestFixture]
    public class MathExtensionsShould
    {
        [Test]
        public void SumDiscreteNumbers()
        {
            var source = new List<IFuzzyNumber>
                {
                    new DiscreteFuzzyNumber(new Dictionary<double, IntervalD>
                        {
                            {0.0, new IntervalD(1.0, 3.0)},
                            {0.5, new IntervalD(1.5, 2.5)},
                            {1.0, new IntervalD(2.0, 2.0)},
                        }),
                    new DiscreteFuzzyNumber(new Dictionary<double, IntervalD>
                        {
                            {0.0, new IntervalD(1.0, 3.0)},
                            {0.5, new IntervalD(1.5, 2.5)},
                            {1.0, new IntervalD(2.0, 2.0)},
                        }),
                };

            var result = FuzzyNumberExtensions.Sum(0, source.Count, i => source.ElementAt(i).Mul(1.0));

            Assert.That(result.LevelsCount, Is.EqualTo(3));
            Assert.That(result.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(2.0, 6.0)));
            Assert.That(result.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(3.0, 5.0)));
            Assert.That(result.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(4.0, 4.0)));
        }

        [Test]
        public void SumRealNumbers()
        {
            var source = new List<IFuzzyNumber>
                {
                    new RealNumber(2),
                    new RealNumber(3),
                };

            var result = FuzzyNumberExtensions.Sum(0, source.Count, i => source.ElementAt(i).Mul(1.0));

            Assert.That(result.GetMod().X, Is.EqualTo(5.0));
        }
    }
}