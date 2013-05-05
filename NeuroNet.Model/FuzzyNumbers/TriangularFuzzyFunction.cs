using System;

namespace NeuroNet.Model.FuzzyNumbers
{
    public class TriangularFuzzyFunction : IFuzzyFunction
    {
        private readonly double _left;
        private readonly double _mod;
        private readonly double _right;

        public TriangularFuzzyFunction(double left, double mod, double right)
        {
            _left = Math.Min(left, Math.Min(mod, right));
            _mod = mod < right ? (mod > left ? mod : Math.Min(left, right)) : (right > left ? right : Math.Min(mod, left));
            _right = Math.Max(left, Math.Max(mod, right));
        }

        public PointD GetLeft()
        {
            return new PointD(_left, 0.0);
        }

        public PointD GetRight()
        {
            return new PointD(_right, 0.0);
        }

        public PointD GetMod()
        {
            return new PointD(_mod, 1.0);
        }

        public IntervalD GetAlphaLevel(double a)
        {
            return new IntervalD(GetLeftValue(a), GetRightValue(a));
        }

        private double GetRightValue(double a)
        {
            return a*(_mod - _right) + _right;
        }

        private double GetLeftValue(double a)
        {
            return a*(_mod - _left) + _left;
        }
    }
}