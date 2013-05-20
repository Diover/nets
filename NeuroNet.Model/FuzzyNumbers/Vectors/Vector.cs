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

        public int Length
        {
            get { return _values.Length; }
        }
    }
}