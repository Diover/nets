using System.Collections.Generic;
using NeuroNet.Model.FuzzyNumbers.Matrixes;

namespace NeuroNet.Model.FuzzyNumbers.Vectors
{
    public class Vector : IVector
    {
        private readonly IFuzzyNumber[] _values;

        public Vector(IFuzzyNumber[] values)
        {
            _values = values;
        }

        public Vector(int length)
        {
            _values = new IFuzzyNumber[length];
        }

        public IFuzzyNumber this[int i]
        {
            get { return _values[i]; }
            set { _values[i] = value; }
        }

        public IFuzzyNumber Mul(IVector x)
        {
            var res = this[0].Mul(x[0]);
            for (int i = 1; i < _values.Length; i++)
            {
                res.Set(res.Sum(this[i].Mul(x[i])));
            }
            return res;
        }

        public IVector Mul(IFuzzyNumber x)
        {
            var values = new IFuzzyNumber[Length];
            for (int i = 0; i < _values.Length; i++)
            {
                values[i] = _values[i].Mul(x);
            }
            return new Vector(values);
        }

        public IVector Mul(double x)
        {
            var values = new IFuzzyNumber[Length];
            for (int i = 0; i < _values.Length; i++)
            {
                values[i] = _values[i].Mul(x);
            }
            return new Vector(values);
        }

        public IVector Negate()
        {
            var values = new IFuzzyNumber[Length];
            for (int i = 0; i < _values.Length; i++)
            {
                values[i] = _values[i].Negate();
            }
            return new Vector(values);
        }

        public IVector Sum(IVector x)
        {
            var values = new IFuzzyNumber[Length];
            for (int i = 0; i < _values.Length; i++)
            {
                values[i] = _values[i].Sum(x[i]);
            }
            return new Vector(values);
        }

        public IMatrix OuterMul(IVector x)
        {
            var values = new IFuzzyNumber[Length][];
            for (int i = 0; i < Length; i++)
            {
                values[i] = new IFuzzyNumber[x.Length];
            }

            for (int i = 0; i < Length; i++)
                for (int j = 0; j < x.Length; j++)
                    values[i][j] = _values[i].Mul(x[j]);

            return new Matrix(values);
        }

        public Queue<IFuzzyNumber> ToQueue()
        {
            var result = new Queue<IFuzzyNumber>();

            foreach (var t in _values)
            {
                result.Enqueue(t);
            }

            return result;
        }

        public int Length
        {
            get { return _values.Length; }
        }
    }
}