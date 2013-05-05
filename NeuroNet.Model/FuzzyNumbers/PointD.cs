using System;

namespace NeuroNet.Model.FuzzyNumbers
{
    public struct PointD
    {
        public const double Epsilon = 1E-15;
        public readonly double X, Y;

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointD(PointD point)
            : this(point.X, point.Y)
        {
        }

        public bool Equals(PointD value)
        {
            return Math.Abs(X - value.X) < Epsilon && 
                   Math.Abs(Y - value.Y) < Epsilon;
        }

        public static bool operator ==(PointD a, PointD b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PointD a, PointD b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PointD && Equals((PointD)obj);
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