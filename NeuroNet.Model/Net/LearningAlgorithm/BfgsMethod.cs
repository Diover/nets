using System;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Matrixes;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public class BfgsMethod
    {
        private readonly double _errorThreshold; //Emax
        private double _alpha;  //eta (n)
        private IVector _gradient;
        private IVector _x;
        private IMatrix _b;
        private Func<IVector, double> _f;

        public BfgsMethod(double alpha = 10, double errorThreshold = 0.000001)
        {
            _alpha = alpha;
            _errorThreshold = errorThreshold;
        }

        public void Minimize(Func<IVector, double> f, Func<IVector, IVector> gradient, int n)
        {
            _b = Matrix.CreateI(n, n, () => new RealNumber(1), () => new RealNumber(0));
            _x = new Vector(new[] {new RealNumber(0), new RealNumber(0)});
            _f = f;
            do
            {
                _gradient = gradient(_x);
                var direction = CalculateMinimizeDirection(_b, _gradient);
                var step = CalculateStepAndChangeAlpha(direction, f);

                var nextGradient = gradient(_x); //nablaF(xk + 1)
                var y = nextGradient.Sum(_gradient.Negate()); //yk

                //its time to calculate b(k + 1)
                _b = CalculateInvertedPseudoGaussian(_b, step, y);
            } while (_gradient.Norm.IsGreater(_errorThreshold));
        }

        public IVector Values
        {
            get { return _x; }
        }

        public double Minimum
        {
            get { return _f(_x); }
        }

        private static IVector CalculateMinimizeDirection(IMatrix invertedPseudoGessian, IVector gradient)
        {
            return invertedPseudoGessian.Mul(gradient.Negate());
        }

        private IVector CalculateStepAndChangeAlpha(IVector direction, Func<IVector, double> f)
        {
            IVector step;
            IVector newX;
            //do
            //{
                step = direction.Mul(_alpha);
                newX = _x.Sum(step);
                //_alpha /= 2.0;
                //_alpha *= 2.0;
                if (f(newX) > f(_x))
                    _alpha /= 2.0;
                else
                    _alpha *= 2.0;
            //} while (f(newX) > f(_x));
            
            _x = newX;
            return step;
        }

        private static IMatrix CalculateInvertedPseudoGaussian(IMatrix b, IVector s, IVector y)
        {
            var syNumber = s.Mul(y);

            var syNumberSqr = syNumber.Mul(syNumber);
            var yByNumber = y.Mul(b.Mul(y));
            var ssMatrix = s.OuterMul(s);
            var second = ssMatrix.Mul(syNumber.Sum(yByNumber)).Div(syNumberSqr);

            var ysMatrix = y.OuterMul(s);
            var syMatrix = s.OuterMul(y);
            var third = b.Mul(ysMatrix).Sum(syMatrix.Mul(b)).Div(syNumber);

            return b.Sum(second).Sub(third);
        }
    }
}