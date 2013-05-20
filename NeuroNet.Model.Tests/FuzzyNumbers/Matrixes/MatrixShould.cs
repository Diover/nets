using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Matrixes;

namespace NeuroNet.Model.Tests.FuzzyNumbers.Matrixes
{
    [TestFixture]
    public class MatrixShould
    {
        [Test]
        public void CorrectlyConstructsRowsAndColumns1()
        {
            var values = new[]
                {
                    new[] {new RealNumber(1.0), new RealNumber(2.0), new RealNumber(3.0)},
                    new[] {new RealNumber(4.0), new RealNumber(5.0), new RealNumber(6.0)},
                    new[] {new RealNumber(7.0), new RealNumber(8.0), new RealNumber(9.0)},
                    new[] {new RealNumber(1.0), new RealNumber(1.0), new RealNumber(1.0)},
                };
            var matrix = new Matrix(values);

            Assert.That(matrix.Rows, Is.EqualTo(4));
            Assert.That(matrix.Columns, Is.EqualTo(3));
        }

        [Test]
        public void CorrectlyConstructsRowsAndColumns2()
        {
            var matrix = new Matrix(4, 3);

            Assert.That(matrix.Rows, Is.EqualTo(4));
            Assert.That(matrix.Columns, Is.EqualTo(3));
        }

        [Test]
        public void CorrectlyAccessElements()
        {
            var values = new[]
                {
                    new[] {new RealNumber(1.0), new RealNumber(2.0), new RealNumber(3.0)},
                    new[] {new RealNumber(4.0), new RealNumber(5.0), new RealNumber(6.0)},
                    new[] {new RealNumber(7.0), new RealNumber(8.0), new RealNumber(9.0)},
                    new[] {new RealNumber(1.0), new RealNumber(1.0), new RealNumber(1.0)},
                };
            var matrix = new Matrix(values);

            for(int i=0; i<matrix.Rows; i++)
                for(int j=0; j<matrix.Columns; j++)
                    Assert.That(matrix[i,j].GetMod().X, Is.EqualTo(values[i][j].GetMod().X));
        }

        [Test]
        public void ContainNullElementsAfterSimpleConstruct()
        {
            var matrix = new Matrix(4, 3);

            for (int i = 0; i < matrix.Rows; i++)
                for (int j = 0; j < matrix.Columns; j++)
                    Assert.That(matrix[i, j], Is.Null);
        }

        [Test]
        public void CorrectlySumMatrixes()
        {
            var valuesX = new[]
                {
                    new[] {new RealNumber(1.0), new RealNumber(2.0), new RealNumber(3.0)},
                    new[] {new RealNumber(-4.0), new RealNumber(-5.0), new RealNumber(-6.0)},
                    new[] {new RealNumber(7.0), new RealNumber(-8.0), new RealNumber(9.0)},
                    new[] {new RealNumber(-1.0), new RealNumber(1.0), new RealNumber(-1.0)},
                };
            var matrixX = new Matrix(valuesX);
            var valuesY = new[]
                {
                    new[] {new RealNumber(1.0), new RealNumber(2.0), new RealNumber(3.0)},
                    new[] {new RealNumber(4.0), new RealNumber(5.0), new RealNumber(6.0)},
                    new[] {new RealNumber(7.0), new RealNumber(8.0), new RealNumber(9.0)},
                    new[] {new RealNumber(1.0), new RealNumber(1.0), new RealNumber(1.0)},
                };
            var matrixY = new Matrix(valuesY);
            var results = new[]
                {
                    new[] {new RealNumber(2.0), new RealNumber(4.0), new RealNumber(6.0)},
                    new[] {new RealNumber(0.0), new RealNumber(0.0), new RealNumber(0.0)},
                    new[] {new RealNumber(14.0), new RealNumber(0.0), new RealNumber(18.0)},
                    new[] {new RealNumber(0.0), new RealNumber(2.0), new RealNumber(0.0)},
                };

            var result = matrixX.Sum(matrixY);

            for (int i = 0; i < matrixX.Rows; i++)
                for (int j = 0; j < matrixX.Columns; j++)
                    Assert.That(result[i, j].GetMod().X, Is.EqualTo(results[i][j].GetMod().X));
        }

        [Test]
        public void ProvideCommutativeSum()
        {
            var valuesX = new[]
                {
                    new[] {new RealNumber(1.0), new RealNumber(2.0), new RealNumber(3.0)},
                    new[] {new RealNumber(-4.0), new RealNumber(-5.0), new RealNumber(-6.0)},
                    new[] {new RealNumber(7.0), new RealNumber(-8.0), new RealNumber(9.0)},
                    new[] {new RealNumber(-1.0), new RealNumber(1.0), new RealNumber(-1.0)},
                };
            var matrixX = new Matrix(valuesX);
            var valuesY = new[]
                {
                    new[] {new RealNumber(1.0), new RealNumber(2.0), new RealNumber(3.0)},
                    new[] {new RealNumber(4.0), new RealNumber(5.0), new RealNumber(6.0)},
                    new[] {new RealNumber(7.0), new RealNumber(8.0), new RealNumber(9.0)},
                    new[] {new RealNumber(1.0), new RealNumber(1.0), new RealNumber(1.0)},
                };
            var matrixY = new Matrix(valuesY);
            var results = new[]
                {
                    new[] {new RealNumber(2.0), new RealNumber(4.0), new RealNumber(6.0)},
                    new[] {new RealNumber(0.0), new RealNumber(0.0), new RealNumber(0.0)},
                    new[] {new RealNumber(14.0), new RealNumber(0.0), new RealNumber(18.0)},
                    new[] {new RealNumber(0.0), new RealNumber(2.0), new RealNumber(0.0)},
                };

            var resultXY = matrixX.Sum(matrixY);
            var resultYX = matrixY.Sum(matrixX);


            for (int i = 0; i < matrixX.Rows; i++)
                for (int j = 0; j < matrixX.Columns; j++)
                    Assert.That(resultXY[i, j].GetMod().X, Is.EqualTo(resultYX[i, j].GetMod().X));
        }

        [Test]
        public void CorrectlySubMatrixes()
        {
            var valuesX = new[]
                {
                    new[] {new RealNumber(1.0), new RealNumber(2.0), new RealNumber(3.0)},
                    new[] {new RealNumber(4.0), new RealNumber(5.0), new RealNumber(6.0)},
                    new[] {new RealNumber(7.0), new RealNumber(-8.0), new RealNumber(9.0)},
                    new[] {new RealNumber(-1.0), new RealNumber(1.0), new RealNumber(-1.0)},
                };
            var matrixX = new Matrix(valuesX);
            var valuesY = new[]
                {
                    new[] {new RealNumber(1.0), new RealNumber(2.0), new RealNumber(3.0)},
                    new[] {new RealNumber(4.0), new RealNumber(5.0), new RealNumber(6.0)},
                    new[] {new RealNumber(7.0), new RealNumber(8.0), new RealNumber(9.0)},
                    new[] {new RealNumber(1.0), new RealNumber(1.0), new RealNumber(1.0)},
                };
            var matrixY = new Matrix(valuesY);
            var results = new[]
                {
                    new[] {new RealNumber(0.0), new RealNumber(0.0), new RealNumber(0.0)},
                    new[] {new RealNumber(0.0), new RealNumber(0.0), new RealNumber(0.0)},
                    new[] {new RealNumber(0.0), new RealNumber(-16.0), new RealNumber(0.0)},
                    new[] {new RealNumber(-2.0), new RealNumber(0.0), new RealNumber(-2.0)},
                };

            var result = matrixX.Sub(matrixY);

            for (int i = 0; i < matrixX.Rows; i++)
                for (int j = 0; j < matrixX.Columns; j++)
                    Assert.That(result[i, j].GetMod().X, Is.EqualTo(results[i][j].GetMod().X));
        }

        [Test]
        public void CorrectlyMultiplyByFuzzyNumber()
        {
            var values = new[]
                {
                    new[] {new RealNumber(2.0), new RealNumber(2.0), new RealNumber(4.0)},
                    new[] {new RealNumber(4.0), new RealNumber(6.0), new RealNumber(6.0)},
                    new[] {new RealNumber(8.0), new RealNumber(8.0), new RealNumber(10.0)},
                    new[] {new RealNumber(10.0), new RealNumber(12.0), new RealNumber(12.0)},
                };
            var matrix = new Matrix(values);
            var multiplier = new RealNumber(-0.5);
            var results = new[]
                {
                    new[] {new RealNumber(-1.0), new RealNumber(-1.0), new RealNumber(-2.0)},
                    new[] {new RealNumber(-2.0), new RealNumber(-3.0), new RealNumber(-3.0)},
                    new[] {new RealNumber(-4.0), new RealNumber(-4.0), new RealNumber(-5.0)},
                    new[] {new RealNumber(-5.0), new RealNumber(-6.0), new RealNumber(-6.0)},
                };

            var result = matrix.Mul(multiplier);

            for (int i = 0; i < matrix.Rows; i++)
                for (int j = 0; j < matrix.Columns; j++)
                    Assert.That(result[i, j].GetMod().X, Is.EqualTo(results[i][j].GetMod().X));
        }
    }
}