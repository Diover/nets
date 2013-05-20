using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuroNet.Model.FuzzyNumbers
{
    [Serializable]
    public class DiscreteFuzzyNumber : IFuzzyNumber
    {
        public const double Epsilon = 1E-15;
        public const int StandardAlphaLevelsCount = 101;
        private readonly Dictionary<double, IntervalD> _alphaLevels;

        public DiscreteFuzzyNumber()
        {
            _alphaLevels = new Dictionary<double, IntervalD>();
        }

        public DiscreteFuzzyNumber(Dictionary<double, IntervalD> alphaLevels)
        {
            _alphaLevels = alphaLevels;
        }

        public DiscreteFuzzyNumber(IFuzzyFunction f, int levelsCount):this(new Dictionary<double, IntervalD>())
        {
            FillLevels(f, levelsCount);
        }

        public void FillLevels(IFuzzyFunction f, int levelsCount)
        {
            if (levelsCount <= 0)
                levelsCount = StandardAlphaLevelsCount;

            double d = 1.0/(levelsCount - 1);
            for (int i = 0; i < levelsCount - 1; i++)
            {
                _alphaLevels.Add(d*i, f.GetAlphaLevel(d*i));
            }
            _alphaLevels.Add(1.0, f.GetAlphaLevel(1.0));
        }

        public void AddLevel(IntervalD level, double alpha)
        {
            if (_alphaLevels.ContainsKey(alpha))
                _alphaLevels[alpha] = level;
            else
                _alphaLevels.Add(alpha, level);
        }

        public int LevelsCount
        {
            get { return _alphaLevels.Count; }
        }

        public void ForeachLevel(Action<double, IntervalD> action)
        {
            foreach (var level in _alphaLevels)
            {
                action(level.Key, level.Value);
            }
        }

        private double GetMinAlphaLevel()
        {
            return _alphaLevels.Min(pair => pair.Key);
        }

        private double GetMaxAlphaLevel()
        {
            return _alphaLevels.Max(pair => pair.Key);
        }

        public PointD GetLeft()
        {
            var level = GetMinAlphaLevel();
            return new PointD(_alphaLevels[level].X, level);
        }

        public PointD GetRight()
        {
            var level = GetMinAlphaLevel();
            return new PointD(_alphaLevels[level].Y, level);
        }

        public PointD GetMod()
        {
            var level = GetMaxAlphaLevel();
            return new PointD(_alphaLevels[level].X, level);
        }
        
        public IntervalD GetAlphaLevel(double alpha)
        {
            if (_alphaLevels.ContainsKey(alpha))
                return _alphaLevels[alpha];
            else
                throw new ArgumentException("No such alpha-level", "alpha");
        }

        public static IFuzzyNumber GenerateLittleNumber(double min = -0.5, double max = 0.5, int levelsCount = StandardAlphaLevelsCount)
        {
            var f = PrepareLittleFuzzyFunction(min, max);

            var result = new DiscreteFuzzyNumber();
            double d = 1.0 / (levelsCount - 1);
            for (int i = 0; i < levelsCount - 1; i++)
            {
                result._alphaLevels.Add(d * i, f.GetAlphaLevel(d * i));
            }
            result._alphaLevels.Add(1.0, f.GetAlphaLevel(1.0));
            
            return result;
        }

        private static IFuzzyFunction PrepareLittleFuzzyFunction(double min, double max)
        {
            var rand = new Random();
            const int count = 4;
            var piece = (max - min) / count;

            var pieceNumber = rand.Next(count);

            var center = rand.NextDouble()*piece;
            var left = rand.NextDouble()*piece;
            var right = rand.NextDouble()*piece;

            var addShift = new Func<double, double>(x => x + min + pieceNumber * piece);
            return new TriangularFuzzyFunction(addShift(left), addShift(center), addShift(right));
        }

        public void Set(IFuzzyNumber source)
        {
            _alphaLevels.Clear();
            source.ForeachLevel((alpha, interval) => _alphaLevels.Add(alpha, new IntervalD(interval)));
        }

        public bool ContainsAlphaLevel(double alpha)
        {
            return _alphaLevels.ContainsKey(alpha);
        }

        public IFuzzyNumber Mul(IFuzzyNumber y)
        {
            return Operation((a, b) => a*b, this, y);
        }

        private static IFuzzyNumber Operation(Func<double, double, double> f, IFuzzyNumber y, IFuzzyNumber x)
        {
            var resultLevels = new Dictionary<double, IntervalD>();

            y.ForeachLevel((alpha, leftLevel) =>
                {
                    var rightLevel = x.GetAlphaLevel(alpha);

                    var leftProduct      = f(leftLevel.X, rightLevel.X);
                    var leftRightProduct = f(leftLevel.X, rightLevel.Y);
                    var rightLeftProduct = f(leftLevel.Y, rightLevel.X);
                    var rightProduct     = f(leftLevel.Y, rightLevel.Y);

                    var left = Math.Min(Math.Min(leftProduct, leftRightProduct),
                                        Math.Min(rightProduct, rightLeftProduct));
                    var right = Math.Max(Math.Max(leftProduct, leftRightProduct),
                                         Math.Max(rightProduct, rightLeftProduct));

                    resultLevels.Add(alpha, new IntervalD(left, right));
                });

            return new DiscreteFuzzyNumber(resultLevels);
        }

        public IFuzzyNumber Sum(IFuzzyNumber x)
        {
            var resultLevels = new Dictionary<double, IntervalD>();

            foreach (var leftlevel in _alphaLevels)
            {
                var rightLevel = x.GetAlphaLevel(leftlevel.Key);
                resultLevels.Add(leftlevel.Key, rightLevel + leftlevel.Value);
            }

            return new DiscreteFuzzyNumber(resultLevels);
        }

        public IFuzzyNumber Sub(IFuzzyNumber x)
        {
            var resultLevels = new Dictionary<double, IntervalD>();

            foreach (var leftlevel in _alphaLevels)
            {
                var rightLevel = x.GetAlphaLevel(leftlevel.Key);
                resultLevels.Add(leftlevel.Key, leftlevel.Value - rightLevel);
            }

            return new DiscreteFuzzyNumber(resultLevels);
        }

        public IFuzzyNumber Div(IFuzzyNumber y)
        {
            return Operation((a, b) => a / b, this, y);
        }

        public IFuzzyNumber Mul(double factor)
        {
            var resultLevels = _alphaLevels.ToDictionary(level => level.Key, level => new IntervalD(level.Value.X*factor, level.Value.Y*factor));
            return new DiscreteFuzzyNumber(resultLevels);
        }

        public IFuzzyNumber Sum(double factor)
        {
            var resultLevels = _alphaLevels.ToDictionary(level => level.Key, level => new IntervalD(level.Value.X + factor, level.Value.Y + factor));
            return new DiscreteFuzzyNumber(resultLevels);
        }

        public IFuzzyNumber Sub(double factor)
        {
            var resultLevels = _alphaLevels.ToDictionary(level => level.Key, level => new IntervalD(level.Value.X - factor, level.Value.Y - factor));
            return new DiscreteFuzzyNumber(resultLevels);
        }

        public IFuzzyNumber Div(double factor)
        {
            if(Math.Abs(factor - 0.0) < Epsilon)
                throw new DivideByZeroException("factor is equal to 0.0");
            var resultLevels = _alphaLevels.ToDictionary(level => level.Key, level => new IntervalD(level.Value.X / factor, level.Value.Y / factor));
            return new DiscreteFuzzyNumber(resultLevels);
        }

        public IFuzzyNumber Apply(Func<double, double> f)
        {
            var resultLevels = _alphaLevels.ToDictionary(level => level.Key, level => new IntervalD(f(level.Value.X), f(level.Value.Y)));
            return new DiscreteFuzzyNumber(resultLevels);
        }

        public override string ToString()
        {
            return _alphaLevels[0.0].X.ToString("0.################") + " | " +
                   _alphaLevels[1.0].X.ToString("0.################") + " | " +
                   _alphaLevels[0.0].Y.ToString("0.################");
        }
    }
}