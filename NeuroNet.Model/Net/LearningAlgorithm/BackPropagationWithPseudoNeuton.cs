using System;
using System.Collections.Generic;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Matrixes;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public class BackPropagationWithPseudoNeuton : BackPropagationBase
    {
        private IVector _gradient;
        private IFuzzyNumber _gradientDiffNorm;
        private IVector _prevGradient;
        private IVector _prevStep;
        private IVector _weights;
        private IMatrix _b; //pseudo-Gessian
        private double _alpha;  //eta (n)
        private List<ILink> _inputs;

        public BackPropagationWithPseudoNeuton(List<ILearningPattern> patterns, double alpha = 10.0, double errorThreshold = 0.0001): base(patterns, errorThreshold)
        {
            _alpha = alpha;
        }

        //here we have _gradient as sum of gradients, should make step in this direction
        protected override bool LearnBatch(INet net, double currentLearningCycleError)
        {
            if (_prevStep != null)
            {
                var y = _gradient.Negate().Sum(_prevGradient); //yk
                //_gradientDiffNorm = y.Norm;
                //Console.WriteLine("Gradient diff norm: {0}", _gradientDiffNorm.GetMod().X);
                //Console.WriteLine();
                //if (IsNetLearned(currentLearningCycleError))
                //    return;
                //its time to calculate b(k + 1)
                _b = CalculateInvertedPseudoGaussian(_b, _prevStep, y);
            }

            var direction = CalculateMinimizeDirection(_b, _gradient); //pk - direction of next step
            var step = MakeStep(direction, net, currentLearningCycleError); //step = alpha*pk
            if (step == null)
            {
                return false;
            }

            //Save step and grad
            _prevStep = step;
            _prevGradient = _gradient;
            //clear gradient vector
            _gradient = null;

            return true;
        }

        //here we summarize gradient of each pattern
        protected override void LearnPattern(INet net, ILearningPattern learningPattern, double currentPatternError)
        {
            PropagateErrorOnLayers(net.Layers, learningPattern.Output); //nablaF(xk)
            var currentGradient = CreateWeightsGradient(net.Layers);
            _gradient = _gradient == null ? currentGradient : _gradient.Sum(currentGradient);
        }

        protected override void PrepareToLearning(INet net)
        {
            _b = Matrix.CreateI(net.WeightsCount, net.WeightsCount, () => new RealNumber(1), () => new RealNumber(0)); //b0
            _weights = net.GetWeights(); //x0
            _inputs = net.GetLastInputsForWeights();
            _gradient = null;
            _prevGradient = null;
            _prevStep = null;
        }

        protected override bool IsNetLearned(double currentError)
        {
            return currentError < ErrorThreshold;
                //_gradientDiffNorm != null && !_gradientDiffNorm.IsGreater(0.000000000001);
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

        private IVector CreateWeightsGradient(List<ILayer> layers)
        {
            var result = new List<IFuzzyNumber>();

            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) => neuron.ForeachWeight((j, weight) => result.Add(neuron.PropagatedError)));
            var hiddenLayers = layers.Take(layers.Count - 1);
            foreach (var hiddenLayer in hiddenLayers.Reverse())
            {
                hiddenLayer.ForeachNeuron(
                    (i, neuron) => neuron.ForeachWeight((j, weight) => result.Add(neuron.PropagatedError)));
            }

            return new Vector(result.ToArray()).MemberviseMul(_inputs.ToSignalsVector());
        }

        private static IVector CalculateMinimizeDirection(IMatrix invertedPseudoGessian, IVector gradient)
        {
            return invertedPseudoGessian.Mul(gradient);
        }

        private IVector MakeStep(IVector direction, INet net, double currentError)
        {
            const int maximumNumberOfTry = 50;
            var numberOfTry = 0;
            double error;
            var step = direction.Mul(_alpha); ;

            var oldWeights = _weights;
            //can change alpha by minimizing it in f(xk + alpha*direction)
            do
            {
                if (numberOfTry > maximumNumberOfTry || _alpha < 0.000000000001)
                    break;
                
                _weights = oldWeights.Sum(step); //x(k+1) = xk + sk 
                net.SetWeights(_weights); //content of _weights now shared between net and _weights vector
                error = GetBatchError(net);
                
                _alpha /= 2.0;
                step = direction.Mul(_alpha);
                numberOfTry++;
            } while (error > currentError);

            if (numberOfTry > maximumNumberOfTry || _alpha < 0.000000000001)
            {
                Console.WriteLine("Switch to Simple. Too little alpha: {0:0.#############}.", _alpha);
                //step = direction.Mul(_alpha);
                _weights = oldWeights;
                net.SetWeights(_weights);
                //AddLittleCorrectionToWeights(net.Layers);
                _gradient = null;
                _alpha = 100.0;
                return null;
            }

            _alpha = 10.0;
            return step;
        }
    }
}