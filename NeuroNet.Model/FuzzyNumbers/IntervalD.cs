using System;

namespace NeuroNet.Model.FuzzyNumbers
{
    [Serializable]
    public struct IntervalD
    {
        public const double Epsilon = 1E-15;
        public readonly double X, Y;

        public IntervalD(double x, double y)
        {
            if (x > y)
            {
                X = y;
                Y = x;
            }
            else
            {
                X = x;
                Y = y;
            }
        }

        public IntervalD(IntervalD interval)
            : this(interval.X, interval.Y)
        {
        }

        public bool Contains(double x)
        {
            return (X <= x) && (x <= Y);
        }

        public static IntervalD operator *(IntervalD x, IntervalD y)
        {
            return new IntervalD(x.X*y.X, x.Y*y.Y);
        }

        public static IntervalD operator +(IntervalD x, IntervalD y)
        {
            return new IntervalD(x.X + y.X, x.Y + y.Y);
        }

        public static IntervalD operator /(IntervalD x, IntervalD y)
        {
            if (Math.Abs(y.X - 0.0) <= Epsilon || Math.Abs(y.Y - 0.0) <= Epsilon)
                throw new DivideByZeroException("y.X or y.Y is equal to 0.0");

            return new IntervalD(x.X / y.X, x.Y / y.Y);
        }

        public static IntervalD operator -(IntervalD x, IntervalD y)
        {
            return new IntervalD(x.X - y.X, x.Y - y.Y);
        }

        public bool Equals(IntervalD value)
        {
            return Math.Abs(X - value.X) < Epsilon && 
                   Math.Abs(Y - value.Y) < Epsilon;
        }

        public static bool operator ==(IntervalD a, IntervalD b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(IntervalD a, IntervalD b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IntervalD && Equals((IntervalD)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
}