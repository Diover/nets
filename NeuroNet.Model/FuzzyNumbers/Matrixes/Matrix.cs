using System;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.FuzzyNumbers.Matrixes
{
    public class Matrix : IMatrix
    {
        private readonly IFuzzyNumber[][] _values;

        public Matrix(IFuzzyNumber[][] values)
        {
            _values = values;
        }

        public Matrix(int rows, int columns)
        {
            _values = new IFuzzyNumber[rows][];
            for (int i = 0; i < rows; i++)
            {
                _values[i] = new IFuzzyNumber[columns];
            }
        }

        public IFuzzyNumber this[int i, int j]
        {
            get { return _values[i][j]; }
            set { _values[i][j] = value; }
        }

        public int Rows
        {
            get { return _values.Length; }
        }

        public int Columns
        {
            get { return _values[0].Length; }
        }

        public IMatrix Sum(IMatrix x)
        {
            var values = new IFuzzyNumber[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                values[i] = new IFuzzyNumber[Columns];
            }

            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    values[i][j] = _values[i][j].Sum(x[i, j]);

            return new Matrix(values);
        }

        public IMatrix Sub(IMatrix x)
        {
            var values = new IFuzzyNumber[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                values[i] = new IFuzzyNumber[Columns];
            }

            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    values[i][j] = _values[i][j].Sub(x[i, j]);

            return new Matrix(values);
        }

        public IMatrix Mul(IMatrix x)
        {
            if (Columns != x.Rows)
                throw new ArgumentException("matrixes dimensions are different");

            var values = new IFuzzyNumber[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                values[i] = new IFuzzyNumber[x.Columns];
            }

            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < x.Columns; j++)
                {
                    var locali = i;
                    var localj = j;
                    values[i][j] = FuzzyNumberExtensions.Sum(0, Columns, k => _values[locali][k].Mul(x[k, localj]));
                }

            return new Matrix(values);
        }

        public IMatrix Div(IFuzzyNumber x)
        {
            var values = new IFuzzyNumber[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                values[i] = new IFuzzyNumber[Columns];
            }

            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    values[i][j] = _values[i][j].Div(x);

            return new Matrix(values);
        }

        public IVector Mul(IVector x)
        {
            if(x.Length != Columns)
                throw new ArgumentException("Vector and matrix dimensions are different");

            var result = new Vector(Rows);

            for (int i = 0; i < result.Length; i++)
                result[i] = x.Mul(new Vector(_values[i]));

            return result;
        }

        public IMatrix Mul(IFuzzyNumber x)
        {
            var values = new IFuzzyNumber[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                values[i] = new IFuzzyNumber[Columns];
            }

            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    values[i][j] = _values[i][j].Mul(x);

            return new Matrix(values);
        }

        public static IMatrix CreateI(int rows, int columns, Func<IFuzzyNumber> primeGenerator, Func<IFuzzyNumber> zeroGenerator)
        {
            var result = new Matrix(rows, columns);
            for (int i = 0; i < result.Rows; i++)
                for (int j = 0; j < result.Columns; j++)
                    if (i == j)
                        result[i, j] = primeGenerator();
                    else
                        result[i, j] = zeroGenerator();
            return result;
        }
    }
}