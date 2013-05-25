using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.Tests.FuzzyNumbers.Vectors
{
    [TestFixture]
    public class VectorShould
    {
        [Test]
        public void CorrectlyAccessElements()
        {
            var values = new[] {new RealNumber(1.0), new RealNumber(2.0), new RealNumber(-1.0)};
            var vector = new Vector(values);

            Assert.That(vector.Length, Is.EqualTo(3));
            for (int i = 0; i < vector.Length; i++)
            {
                Assert.That(vector[i].GetMod().X, Is.EqualTo(values[i].GetMod().X));
            }
        }

        [Test]
        public void CorrectlyMultiplyByFuzzyNumber()
        {
            var values = new[] { new RealNumber(1.0), new RealNumber(2.0), new RealNumber(-1.0) };
            IVector vector = new Vector(values);
            var multiplier = new RealNumber(3.0);

            vector = vector.Mul(multiplier);

            Assert.That(vector.Length, Is.EqualTo(3));
            Assert.That(vector[0].GetMod().X, Is.EqualTo(new RealNumber(3.0).GetMod().X));
            Assert.That(vector[1].GetMod().X, Is.EqualTo(new RealNumber(6.0).GetMod().X));
            Assert.That(vector[2].GetMod().X, Is.EqualTo(new RealNumber(-3.0).GetMod().X));

        }

        [Test]
        public void CorrectlyMultiplyByAnotherVector()
        {
            var valuesX = new[] { new RealNumber(1.0), new RealNumber(2.0), new RealNumber(-1.0) };
            IVector vectorX = new Vector(valuesX);
            var valuesY = new[] { new RealNumber(4.0), new RealNumber(-2.0), new RealNumber(0.0) };
            IVector vectorY = new Vector(valuesY);

            var result = vectorX.Mul(vectorY);

            Assert.That(result.GetMod().X, Is.EqualTo(new RealNumber(0.0).GetMod().X));
        }

        [Test]
        public void ProvideCommutativeMultiplication()
        {
            var valuesX = new[] { new RealNumber(1.0), new RealNumber(2.0), new RealNumber(-1.0) };
            IVector vectorX = new Vector(valuesX);
            var valuesY = new[] { new RealNumber(4.0), new RealNumber(-2.0), new RealNumber(0.0) };
            IVector vectorY = new Vector(valuesY);

            var resultXY = vectorX.Mul(vectorY);
            var resultYX = vectorY.Mul(vectorX);

            Assert.That(resultXY.GetMod().X, Is.EqualTo(resultYX.GetMod().X));
        }

        [Test]
        public void CorrectlyCalculateOuterProduct()
        {
            var valuesX = new[] { new RealNumber(1.0), new RealNumber(2.0), new RealNumber(3.0) };
            IVector vectorX = new Vector(valuesX);
            var valuesY = new[] { new RealNumber(1.0), new RealNumber(2.0)};
            IVector vectorY = new Vector(valuesY);
            var expected = new[]
                {
                    new[] {new RealNumber(1.0), new RealNumber(2.0)},
                    new[] {new RealNumber(2.0), new RealNumber(4.0)},
                    new[] {new RealNumber(3.0), new RealNumber(6.0)},
                };

            var result = vectorX.OuterMul(vectorY);

            for (int i = 0; i < valuesX.Length; i++)
                for (int j = 0; j < valuesY.Length; j++)
                    Assert.That(result[i, j].GetMod().X, Is.EqualTo(expected[i][j].GetMod().X));
        }

        [Test]
        public void CorrectlySumWithAnotherVector()
        {
            var valuesX = new[] { new RealNumber(1.0), new RealNumber(2.0), new RealNumber(-1.0) };
            IVector vectorX = new Vector(valuesX);
            var valuesY = new[] { new RealNumber(4.0), new RealNumber(-2.0), new RealNumber(0.0) };
            IVector vectorY = new Vector(valuesY);

            var result = vectorX.Sum(vectorY);
            var expected = new[]
                {
                    new RealNumber(5.0),
                    new RealNumber(0.0),
                    new RealNumber(-1.0),
                };

            Assert.That(result.Length, Is.EqualTo(valuesX.Length));
            for (int i = 0; i < valuesX.Length; i++)
                Assert.That(result[i].GetMod().X, Is.EqualTo(expected[i].GetMod().X));
        }

        [Test]
        public void CorrectlyMemberviseMulWithAnotherVector()
        {
            var valuesX = new[] { new RealNumber(1.0), new RealNumber(2.0), new RealNumber(-1.0) };
            IVector vectorX = new Vector(valuesX);
            var valuesY = new[] { new RealNumber(4.0), new RealNumber(-2.0), new RealNumber(0.0) };
            IVector vectorY = new Vector(valuesY);

            var result = vectorX.MemberviseMul(vectorY);
            var expected = new[]
                {
                    new RealNumber(4.0),
                    new RealNumber(-4.0),
                    new RealNumber(0.0),
                };

            Assert.That(result.Length, Is.EqualTo(valuesX.Length));
            for (int i = 0; i < valuesX.Length; i++)
                Assert.That(result[i].GetMod().X, Is.EqualTo(expected[i].GetMod().X));
        }

        [Test]
        public void CorrectlyNegateVector()
        {
            var valuesX = new[] { new RealNumber(1.0), new RealNumber(0.0), new RealNumber(-1.0) };
            IVector vectorX = new Vector(valuesX);

            var result = vectorX.Negate();
            var expected = new[]
                {
                    new RealNumber(-1.0),
                    new RealNumber(0.0),
                    new RealNumber(1.0),
                };

            Assert.That(result.Length, Is.EqualTo(valuesX.Length));
            for (int i = 0; i < valuesX.Length; i++)
                Assert.That(result[i].GetMod().X, Is.EqualTo(expected[i].GetMod().X));
        }
    }
}