using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Tests.FuzzyNumbers
{
    [TestFixture]
    public class PointDShould
    {
        [Test]
        public void ReturnTrueComparingSameValues()
        {
            const double x = 1.0001;
            var a = new PointD(x, x);
            var b = new PointD(x, x);

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void ReturnFalseComparingDifferentValues()
        {
            var a = new PointD(1, 1);
            var b = new PointD(1, 2);

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void ReturnTrueComparingValuesSmallerEpsilon()
        {
            const double epsilon = IntervalD.Epsilon;
            const double x = 1.0;
            var a = new PointD(x + epsilon / 2.0, x);
            var b = new PointD(x, x);

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void BeAnotherObjectAfterConstructionFromPointD()
        {
            var source = new PointD(0.0, 0.0);

            var result = new PointD(source);

            Assert.AreNotSame(result, source, "New point should be another object");
        }

        [Test]
        public void ContainSameValuesAfterConstructionFromPointD()
        {
            var source = new PointD(0.0, 0.0);

            var result = new PointD(source);

            Assert.That(result, Is.EqualTo(source));
        }
    }
}