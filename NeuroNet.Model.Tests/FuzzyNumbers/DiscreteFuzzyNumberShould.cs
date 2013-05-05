using System;
using System.Collections.Generic;
using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Tests.FuzzyNumbers
{
    [TestFixture]
    public class DiscreteFuzzyNumberShould
    {
        [Test]
        public void BeEmptyAfterConstruction()
        {
            var number = new DiscreteFuzzyNumber();

            var result = number.LevelsCount;

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void ReturnTwoOnLevelsCount()
        {
            var levels = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };

            var number = new DiscreteFuzzyNumber(levels);

            var result = number.LevelsCount;

            Assert.That(result, Is.EqualTo(2));
        }


        [Test]
        public void ReturnActualModValue()
        {
            var levels = new Dictionary<double, IntervalD>
                {
                    {1.0, new IntervalD(2.0, 2.0)},
                };

            var number = new DiscreteFuzzyNumber(levels);

            var result = number.GetMod();

            Assert.That(result, Is.EqualTo(new PointD(2.0, 1.0)));
        }

        [Test]
        public void ReturnActualRightValue()
        {
            var levels = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                };

            var number = new DiscreteFuzzyNumber(levels);

            var result = number.GetRight();

            Assert.That(result, Is.EqualTo(new PointD(3.0, 0.0)));
        }

        [Test]
        public void ReturnActualLeftValue()
        {
            var levels = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                };

            var number = new DiscreteFuzzyNumber(levels);

            var result = number.GetRight();

            Assert.That(result, Is.EqualTo(new PointD(3.0, 0.0)));
        }

        [Test]
        public void ReturnActualAlphaLevels()
        {
            var levels = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };

            var number = new DiscreteFuzzyNumber(levels);

            var level0_0 = number.GetAlphaLevel(0.0);
            var level0_5 = number.GetAlphaLevel(0.5);
            var level1_0 = number.GetAlphaLevel(1.0);

            Assert.That(level0_0, Is.EqualTo(new IntervalD(1.0, 3.0)));
            Assert.That(level0_5, Is.EqualTo(new IntervalD(0.5, 2.5)));
            Assert.That(level1_0, Is.EqualTo(new IntervalD(2.0, 2.0)));
        }

        [Test]
        public void AfterProductionLevelsCountIsSameAsInFirstFactor()
        {
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var y = new DiscreteFuzzyNumber(levelsRight);

            var product = x.Mul(y);

            Assert.That(product.LevelsCount, Is.EqualTo(3));
        }

        [Test]
        public void CorrectlyMultiplyPositiveNumbers()
        {
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(1.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(1.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var y = new DiscreteFuzzyNumber(levelsRight);

            var product = x.Mul(y);

            Assert.That(product.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(1.0, 9.0)));
            Assert.That(product.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(2.25, 6.25)));
            Assert.That(product.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(4.0, 4.0)));
        }

        [Test]
        public void GenerateLettleNumbersInRange()
        {
            const double min = -0.5;
            const double max = 0.5;
            const int levelsCount = 100;
            var result = DiscreteFuzzyNumber.GenerateLittleNumber(min, max, levelsCount);

            Assert.That(result.LevelsCount, Is.EqualTo(levelsCount));
            
            result.ForeachLevel((alpha, level) =>
                {
                    Assert.That(result.GetAlphaLevel(0.0).X, Is.GreaterThan(min));
                    Assert.That(result.GetAlphaLevel(0.0).Y, Is.LessThan(max));
                });
        }

        [Test]
        public void CorrectlyMultiplyZeroByPositive()
        {
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(-2.0, 2.0)},
                    {0.5, new IntervalD(-1.0, 1.0)},
                    {1.0, new IntervalD(0.0, 0.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(1.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var y = new DiscreteFuzzyNumber(levelsRight);

            var product = x.Mul(y);

            Assert.That(product.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(-6.0, 6.0)));
            Assert.That(product.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(-2.5, 2.5)));
            Assert.That(product.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(0.0, 0.0)));
        }

        [Test]
        public void CorrectlyMultiplyPositiveByZero()
        {
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(1.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(-2.0, 2.0)},
                    {0.5, new IntervalD(-1.0, 1.0)},
                    {1.0, new IntervalD(0.0, 0.0)},
                };
            var y = new DiscreteFuzzyNumber(levelsRight);

            var product = x.Mul(y);

            Assert.That(product.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(-6.0, 6.0)));
            Assert.That(product.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(-2.5, 2.5)));
            Assert.That(product.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(0.0, 0.0)));
        }

        [Test]
        public void CorrectlyMultiplyPositiveByZero2()
        {
            var x = new DiscreteFuzzyNumber(new TriangularFuzzyFunction(-3.0,-1.0,2.0), 3);

            var y = new DiscreteFuzzyNumber(new TriangularFuzzyFunction(-2.0, 1.0, 4.0), 3);

            var product = x.Mul(y);

            Assert.That(product.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(-12.0, 8.0)));
            Assert.That(product.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(-5.0, 1.25)));
            Assert.That(product.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(-1.0, -1.0)));
        }

        [Test]
        public void CorrectlyMultiplyZeroByZero()
        {
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(-2.0, 2.0)},
                    {0.5, new IntervalD(-1.0, 1.0)},
                    {1.0, new IntervalD(0.0, 0.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(-2.0, 2.0)},
                    {0.5, new IntervalD(-1.0, 1.0)},
                    {1.0, new IntervalD(0.0, 0.0)},
                };
            var y = new DiscreteFuzzyNumber(levelsRight);

            var product = x.Mul(y);

            Assert.That(product.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(-4.0, 4.0)));
            Assert.That(product.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(-1.0, 1.0)));
            Assert.That(product.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(0.0, 0.0)));
        }

        [Test]
        public void CorrectlyApplyActivationFunction()
        {
            var f = new Func<double, double>(value => 1.0/(1.0 + Math.Pow(Math.E, -value)));
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(-2.0, 2.0)},
                    {0.5, new IntervalD(-1.0, 1.0)},
                    {1.0, new IntervalD(0.0, 0.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var activation = x.Apply(f);

            var valueOnLevel00 = new IntervalD(f(-2.0), f(2.0));
            var valueOnLevel05 = new IntervalD(f(-1.0), f(1.0));
            var valueOnLevel10 = new IntervalD(f(0.0), f(0.0));

            Assert.That(activation.GetAlphaLevel(0.0), Is.EqualTo(valueOnLevel00));
            Assert.That(activation.GetAlphaLevel(0.5), Is.EqualTo(valueOnLevel05));
            Assert.That(activation.GetAlphaLevel(1.0), Is.EqualTo(valueOnLevel10));
        }

        [Test]
        public void RightAnaliticalFunctionForTriangularNumbers()
        {
            double z = 2.5;

            double a1 = -2.0;
            double b1 = 0.0;
            double c1 = 2.0;

            double a2 = 1.0;
            double b2 = 2.0;
            double c2 = 3.0;

            var a = -(c1*b2 + c2*b1 - 2*c1*c2);
            var b = Math.Sqrt(Math.Pow(c1*b2-c2*b1,2) + 4*(b1-c1)*(b2-c2)*z);
            var c = 2*(b1 - c1)*(b2 - c2);

            var result = (a - b)/c;

            Assert.That(result, Is.EqualTo(0.5));
        }

        [Test]
        public void AfterSumLevelsCountIsSameAsInFirstNumber()
        {
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var y = new DiscreteFuzzyNumber(levelsRight);

            var sum = x.Sum(y);

            Assert.That(sum.LevelsCount, Is.EqualTo(3));
        }

        [Test]
        public void CorrectlySumNumbers()
        {
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var y = new DiscreteFuzzyNumber(levelsRight);

            var sum = x.Sum(y);

            Assert.That(sum.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(1.0 + 1.0, 3.0 + 3.0)));
            Assert.That(sum.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(0.5 + 0.5, 2.5 + 2.5)));
            Assert.That(sum.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(2.0 + 2.0, 2.0 + 2.0)));
        }

        [Test]
        public void AfterSubLevelsCountIsSameAsInFirstNumber()
        {
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var y = new DiscreteFuzzyNumber(levelsRight);

            var sum = x.Sub(y);

            Assert.That(sum.LevelsCount, Is.EqualTo(3));
        }

        [Test]
        public void CorrectlySubNumbers()
        {
            var levelsLeft = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levelsLeft);

            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(0.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var y = new DiscreteFuzzyNumber(levelsRight);

            var sum = x.Sub(y);

            Assert.That(sum.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(1.0 - 1.0, 3.0 - 3.0)));
            Assert.That(sum.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(0.5 - 0.5, 2.5 - 2.5)));
            Assert.That(sum.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(2.0 - 2.0, 2.0 - 2.0)));
        }

        [Test]
        public void CorrectlySumWithFlatNumber()
        {
            const double add = 10.0;
            CorretlyDoOperationWithFlatNumbers(
                (expected, actual) => expected + actual,
                (fuzzyNumber, factor) => fuzzyNumber.Sum(factor),
                add);
        }

        [Test]
        public void CorrectlySubFlatNumber()
        {
            const double sub = 10.0;
            CorretlyDoOperationWithFlatNumbers(
                (expected, actual) => expected - actual,
                (fuzzyNumber, factor) => fuzzyNumber.Sub(factor),
                sub);
        }

        [Test]
        public void CorrectlyDivideByFlatNumber()
        {
            const double div = 10.0;
            CorretlyDoOperationWithFlatNumbers(
                (expected, actual) => expected / actual,
                (fuzzyNumber, factor) => fuzzyNumber.Div(factor),
                div);
        }

        [Test]
        public void ThrowExceptionOnDivisionByZeroFlatNumber()
        {
            const double zero = 0.0;
            var levels = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(1.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levels);

            Assert.Throws<DivideByZeroException>(() => { var result = x.Div(zero); });

        }

        [Test]
        public void CopyNumbers()
        {
            var levels = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(1.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levels);

            var newLevels = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(-2.0, 2.0)},
                    {0.25, new IntervalD(-1.5, 1.5)},
                    {0.5, new IntervalD(-1.0, 1.0)},
                    {1.0, new IntervalD(0.0, 0.0)},
                };
            var y = new DiscreteFuzzyNumber(newLevels);

            x.Set(y);

            Assert.That(x.LevelsCount, Is.EqualTo(4));
            Assert.That(x.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(-2.0, 2.0)));
            Assert.That(x.GetAlphaLevel(0.25), Is.EqualTo(new IntervalD(-1.5, 1.5)));
            Assert.That(x.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(-1.0, 1.0)));
            Assert.That(x.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(0.0, 0.0)));
        }

        [Test]
        public void CorrectlyMulWithFlatNumber()
        {
            const double mul = 10.0;
            CorretlyDoOperationWithFlatNumbers(
                (expected, actual) => expected * actual,
                (fuzzyNumber, factor) => fuzzyNumber.Mul(factor),
                mul);
        }

        private static void CorretlyDoOperationWithFlatNumbers(Func<double,double, double> flatOp, Func<IFuzzyNumber, double, IFuzzyNumber> fuzzyOp, double value)
        {
            var levels = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(1.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var x = new DiscreteFuzzyNumber(levels);

            var result = fuzzyOp(x, value);

            Assert.That(result.LevelsCount, Is.EqualTo(3));
            Assert.That(result.GetAlphaLevel(0.0), Is.EqualTo(new IntervalD(flatOp(1.0, value), flatOp(3.0, value))));
            Assert.That(result.GetAlphaLevel(0.5), Is.EqualTo(new IntervalD(flatOp(1.5, value), flatOp(2.5, value))));
            Assert.That(result.GetAlphaLevel(1.0), Is.EqualTo(new IntervalD(flatOp(2.0, value), flatOp(2.0, value))));
        }
    }
}