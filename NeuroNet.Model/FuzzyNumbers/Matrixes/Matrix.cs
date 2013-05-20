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

        public IMatrix Mul(IMatrix x)
        {
            throw new System.NotImplementedException();
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

        public IVector Mul(IVector x)
        {
            
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
    }
}