using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Tests.FuzzyNumbers
{
    [TestFixture]
    public class TriangularFuzzyFunctionShould
    {
        [Test]
        public void ReturnActualRightValue()
        {
            const double left = 0.0;
            const double mod = 0.5;
            const double right = 1.0;
            var number = CreateTriangularFuzzyFunction(left, mod, right);

            var result = number.GetRight();

            Assert.That(result, Is.EqualTo(new PointD(right, 0.0)));
        }

        [Test]
        public void ReturnActualLeftValue()
        {
            const double left = 0.0;
            const double mod = 0.5;
            const double right = 1.0;
            var number = CreateTriangularFuzzyFunction(left, mod, right);

            var result = number.GetLeft();

            Assert.That(result, Is.EqualTo(new PointD(left, 0.0)));
        }

        [Test]
        public void ReturnActualModValue()
        {
            const double left = 0.0;
            const double mod = 0.5;
            const double right = 1.0;
            var number = CreateTriangularFuzzyFunction(left, mod, right);

            var result = number.GetMod();

            Assert.That(result, Is.EqualTo(new PointD(mod, 1.0)));
        }

        [Test]
        public void SwapLeftModRightValuesAfterConstruction()
        {
            const double left = 0.5;
            const double mod = -5.0;
            const double right = 0.0;
            var number = CreateTriangularFuzzyFunction(left, mod, right);

            var actualMod = number.GetMod();
            var actualLeft = number.GetLeft();
            var actualRight = number.GetRight();

            Assert.That(actualMod, Is.EqualTo(new PointD(right, 1.0)));
            Assert.That(actualLeft, Is.EqualTo(new PointD(mod, 0.0)));
            Assert.That(actualRight, Is.EqualTo(new PointD(left, 0.0)));
        }

        [Test]
        public void SwapLeftAndRightValuesAfterConstruction()
        {
            const double left = 0.5;
            const double mod = 0.0;
            const double right = -0.5;
            var number = CreateTriangularFuzzyFunction(left, mod, right);

            var actualMod = number.GetMod();
            var actualLeft = number.GetLeft();
            var actualRight = number.GetRight();

            Assert.That(actualMod, Is.EqualTo(new PointD(mod, 1.0)));
            Assert.That(actualLeft, Is.EqualTo(new PointD(right, 0.0)));
            Assert.That(actualRight, Is.EqualTo(new PointD(left, 0.0)));
        }

        [Test]
        public void ReturnCorrectLeftAndRightOnGivenAlphaLevel1()
        {
            AssertReturnValuesOnAlphaLevel(0.5, 1.0, 3.0);
        }

        [Test]
        public void ReturnCorrectLeftAndRightOnGivenAlphaLevel2()
        {
            AssertReturnValuesOnAlphaLevel(0.25, 0.5, 3.5);
        }

        private static void AssertReturnValuesOnAlphaLevel(double level, double expectedLeft, double expectedRight)
        {
            const double left = 0.0;
            const double mod = 2.0;
            const double right = 4.0;
            var number = CreateTriangularFuzzyFunction(left, mod, right);

            var result = number.GetAlphaLevel(level);

            Assert.That(result.X, Is.EqualTo(expectedLeft), "Wrong left value of level");
            Assert.That(result.Y, Is.EqualTo(expectedRight), "Wrong right value of level");
        }

        private static IFuzzyFunction CreateTriangularFuzzyFunction(double left, double mod, double right)
        {
            return new TriangularFuzzyFunction(left, mod, right);
        }
    }
}