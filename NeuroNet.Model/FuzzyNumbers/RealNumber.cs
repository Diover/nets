﻿using System;

namespace NeuroNet.Model.FuzzyNumbers
{
    [Serializable]
    public class RealNumber : IFuzzyNumber
    {
        private double _value;
        [NonSerialized] private static Random _rand = new Random();

        public RealNumber(double value)
        {
            _value = value;
        }

        public PointD GetLeft()
        {
            throw new NotImplementedException();
        }

        public PointD GetRight()
        {
            throw new NotImplementedException();
        }

        public PointD GetMod()
        {
            return new PointD(_value, _value);
        }

        public IntervalD GetAlphaLevel(double a)
        {
            throw new NotImplementedException();
        }

        public IFuzzyNumber Mul(IFuzzyNumber x)
        {
            return new RealNumber(_value*x.GetMod().X);
        }

        public IFuzzyNumber Sum(IFuzzyNumber x)
        {
            return new RealNumber(_value + x.GetMod().X);
        }

        public IFuzzyNumber Sub(IFuzzyNumber x)
        {
            return new RealNumber(_value - x.GetMod().X);
        }

        public IFuzzyNumber Div(IFuzzyNumber x)
        {
            return new RealNumber(_value / x.GetMod().X);
        }

        public IFuzzyNumber Mul(double factor)
        {
            return new RealNumber(_value * factor);
        }

        public IFuzzyNumber Sum(double factor)
        {
            return new RealNumber(_value + factor);
        }

        public IFuzzyNumber Sub(double factor)
        {
            return new RealNumber(_value - factor);
        }

        public IFuzzyNumber Div(double factor)
        {
            return new RealNumber(_value / factor);
        }

        public void Set(IFuzzyNumber source)
        {
            _value = source.GetMod().X;
        }

        public bool ContainsAlphaLevel(double alpha)
        {
            throw new NotImplementedException();
        }

        public int LevelsCount { get; private set; }
        public void ForeachLevel(Action<double, IntervalD> action)
        {
            action(1.0, new IntervalD(_value, 0.0));
        }

        public IFuzzyNumber Apply(Func<double, double> f)
        {
            return new RealNumber(f(_value));
        }

        public IFuzzyNumber Negate()
        {
            return Apply(x => -x);
        }

        public bool IsGreater(double x)
        {
            return _value > x;
        }

        public static IFuzzyNumber GenerateLittleNumber()
        {
            var sign = _rand.Next(2);
            return new RealNumber(_rand.NextDouble() - sign);
        }

        public override string ToString()
        {
            return _value.ToString("0.#####################");
        }
    }
}