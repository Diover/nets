using System;
using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Tests.FuzzyNumbers
{
    [TestFixture]
    public class IntervalDShould
    {
        [Test]
        public void ReturnTrueComparingSameValues()
        {
            const double x = 1.0001;
            var a = new IntervalD(x, x);
            var b = new IntervalD(x, x);

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void SwapValuesToMakeCorrectInterval()
        {
            const double min = -1.0;
            const double max = 3.0;
            var result = new IntervalD(max, min);

            Assert.That(result.X, Is.EqualTo(min));
            Assert.That(result.Y, Is.EqualTo(max));
        }

        [Test]
        public void ReturnTrueIfContainsValue()
        {
            const double x = 99.99;
            const double left = -1.0;
            const double right = 100.0;
            var interval = new IntervalD(left, right);

            var result = interval.Contains(x);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnFalseIfNotContainsValue()
        {
            const double x = -1.001;
            const double left = -1.0;
            const double right = 100.0;
            var interval = new IntervalD(left, right);

            var result = interval.Contains(x);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnFalseComparingDifferentValues()
        {
            var a = new IntervalD(1, 1);
            var b = new IntervalD(1, 2);

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void ReturnTrueComparingValuesSmallerEpsilon()
        {
            const double epsilon = IntervalD.Epsilon;
            const double x = 1.0;
            var a = new IntervalD(x + epsilon / 2.0, x);
            var b = new IntervalD(x, x);

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void CorrectlyMultiplyIntervals()
        {
            const double a1 = -3.0;
            const double b1 = 0.5;
            var x = new IntervalD(a1, b1);
            
            const double a2 = 0.5;
            const double b2 = 2.0;
            var y = new IntervalD(a2, b2);

            var result = x*y;

            Assert.That(result, Is.EqualTo(new IntervalD(a1*a2, b1*b2)));
        }

        [Test]
        public void CorrectlySumIntervals()
        {
            const double a1 = -3.0;
            const double b1 = 0.5;
            var x = new IntervalD(a1, b1);

            const double a2 = 0.5;
            const double b2 = 2.0;
            var y = new IntervalD(a2, b2);

            var result = x + y;

            Assert.That(result, Is.EqualTo(new IntervalD(a1 + a2, b1 + b2)));
        }

        [Test]
        public void CorrectlySubstractIntervals()
        {
            const double a1 = -3.0;
            const double b1 = 0.5;
            var x = new IntervalD(a1, b1);

            const double a2 = 0.5;
            const double b2 = 2.0;
            var y = new IntervalD(a2, b2);

            var result = x - y;

            Assert.That(result, Is.EqualTo(new IntervalD(a1 - a2, b1 - b2)));
        }

        [Test]
        public void CorrectlyDivideIntervals()
        {
            const double a1 = -3.0;
            const double b1 = 0.5;
            var x = new IntervalD(a1, b1);

            const double a2 = 0.5;
            const double b2 = 2.0;
            var y = new IntervalD(a2, b2);

            var result = x / y;

            Assert.That(result, Is.EqualTo(new IntervalD(a1 / a2, b1 / b2)));
        }

        [Test]
        public void ThrowArgumentExceptionOnZeroDivision()
        {
            const double a1 = -3.0;
            const double b1 = 0.5;
            var x = new IntervalD(a1, b1);

            const double a2 = 0.5;
            const double b2 = 0.0;
            var y = new IntervalD(a2, b2);

            Assert.Throws<DivideByZeroException>(() => { var result = x / y; });
        }

        [Test]
        public void BeAnotherObjectAfterConstructionFromIntervalD()
        {
            var source = new IntervalD(0.0, 0.0);

            var result = new IntervalD(source);

            Assert.AreNotSame(result, source, "New point should be another object");
        }

        [Test]
        public void ContainSameValuesAfterConstructionFromIntervalD()
        {
            var source = new IntervalD(0.0, 0.0);

            var result = new IntervalD(source);

            Assert.That(result, Is.EqualTo(source));
        }
    }
}