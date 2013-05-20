using System;
using System.Collections.Generic;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Matrixes;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public class BackPropagationWithPseudoNeuton : ILearningAlgorithm
    {
        private readonly List<ILearningPattern> _patterns;
        private readonly double _errorThreshold; //Emax
        
        private IVector _gradient;
        private IVector _weights;
        private IMatrix _b; //pseudo-Gessian
        private double _alpha;  //eta (n)

        public BackPropagationWithPseudoNeuton(List<ILearningPattern> patterns, double alpha = 10.7, double errorThreshold = 0.0001)
        {
            _patterns = patterns;
            _alpha = alpha;
            _errorThreshold = errorThreshold;
        }

        public void LearnNet(INet net)
        {
            _b = Matrix.CreateI(net.WeightsCount, net.WeightsCount, () => new RealNumber(1), () => new RealNumber(0)); //b0
            _weights = CreateWeightsVector(net.Layers); //x0

            double learningCycleError;
            double previousLearningCycleError = 0.0;
            int cycle = 0;
            do
            {
                learningCycleError = 0.0;
                int algorithmStep = 0;
                foreach (var learningPattern in _patterns)
                {
                    var patternError = CalculatePatternError(net,
                                                             learningPattern.Input,
                                                             learningPattern.Output);
                    learningCycleError += patternError;

                    if (patternError > 0.0)
                    {
                        _gradient = CalculateGradientOnLayers(net.Layers, learningPattern.Output); //nablaF(xk)
                        var direction = CalculateMinimizeDirection(_b, _gradient);
                        var step = CalculateStepAndChangeAlpha(direction, net.Layers, patternError,
                                                               () => CalculatePatternError(net,
                                                                                           learningPattern.Input,
                                                                                           learningPattern.Output));
                        
                        _weights = _weights.Sum(step); //x(k+1) = xk + sk 
                        SetWeights(_weights, net.Layers); //content of _weights now shared between net and _weights vector

                        var nextGradient = CalculateGradientOnLayers(net.Layers, learningPattern.Output); //nablaF(xk + 1)
                        var y = nextGradient.Sum(_gradient.Negate()); //yk

                        //its time to calculate b(k + 1)
                        _b = CalculateInvertedPseudoGaussian(_b, step, y);
                    }

                    OnStepPerformed(cycle, algorithmStep, learningCycleError, _gradient.Norm);
                    algorithmStep++;
                }

                OnCyclePerformed(cycle, learningCycleError, _gradient.Norm);
                cycle++;
            } while (_gradient.Norm.IsGreater(_errorThreshold));
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

        public event StepPerformedEventHandler StepPerformed;
        public event CyclePerformedEventHandler CyclePerformed;

        private void OnStepPerformed(int cycle, int step, double stepError, IFuzzyNumber gradientNorm)
        {
            if (StepPerformed != null)
                StepPerformed(new StepState(cycle, step, stepError, gradientNorm));
        }

        private void OnCyclePerformed(int cycle, double cycleError, IFuzzyNumber gradientNorm)
        {
            if (CyclePerformed != null)
                CyclePerformed(new StepState(cycle, 0, cycleError, gradientNorm));
        }

        private static double CalculatePatternError(INet net, List<IFuzzyNumber> inputs, List<IFuzzyNumber> outputs)
        {
            var actualOutput = net.Propagate(inputs).First();
            var expectedOutput = outputs.First();

            var errorNumber = actualOutput.Sub(expectedOutput);

            var leftError = 0.0;
            var rightError = 0.0;
            errorNumber.ForeachLevel((alpha, level) =>
            {
                leftError += alpha * (level.X * level.X);
                rightError += alpha * (level.Y * level.Y);
            });

            return leftError / 2.0 + rightError / 2.0;
        }

        public static IVector CalculateGradientOnLayers(List<ILayer> layers, List<IFuzzyNumber> patternsOutput)
        {
            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) =>
            {
                var output = neuron.LastOutput; //Ok
                var expectedOutput = patternsOutput.ElementAt(i); //tk
                neuron.PropagatedError =
                    output.Mul(output.Apply(levelValue => 1 - levelValue))
                          .Mul(expectedOutput.Sub(output)); //Ok(1-Ok)(tk - Ok)
            });

            var hiddenLayers = layers.Take(layers.Count - 1);
            foreach (var hiddenLayer in hiddenLayers)
            {
                hiddenLayer.ForeachNeuron((i, neuron) =>
                {
                    var output = neuron.LastOutput; //Ok
                    var part = output.Mul(output.Apply(levelValue => 1 - levelValue)); //Ok(1 - Ok)
                    var sum = FuzzyNumberExtensions.Sum(0, outputLayer.NeuronsCount, j => outputLayer.GetNeuron(j)
                                                                                              .GetWeight(i)
                                                                                              .Signal
                                                                                              .Mul(
                                                                                                  outputLayer
                                                                                                      .GetNeuron(j)
                                                                                                      .PropagatedError));
                    neuron.PropagatedError = part.Mul(sum);
                });
            }

            return CreateWeightsGradient(layers);
        }

        private static void SetWeights(IVector weights, List<ILayer> layers)
        {
            var queue = weights.ToQueue();

            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) => neuron.ForeachWeight((j, weight) => weight.Signal = queue.Dequeue()));
            var hiddenLayers = layers.Take(layers.Count - 1);
            foreach (var hiddenLayer in hiddenLayers.Reverse())
            {
                hiddenLayer.ForeachNeuron(
                    (i, neuron) => neuron.ForeachWeight((j, weight) => weight.Signal = queue.Dequeue()));
            }
        }

        private static IVector CreateWeightsGradient(List<ILayer> layers)
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

            return new Vector(result.ToArray());
        }

        private static IVector CreateWeightsVector(List<ILayer> layers)
        {
            var result = new List<IFuzzyNumber>();

            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) => neuron.ForeachWeight((j, weight) => result.Add(weight.Signal)));
            var hiddenLayers = layers.Take(layers.Count - 1);
            foreach (var hiddenLayer in hiddenLayers.Reverse())
            {
                hiddenLayer.ForeachNeuron(
                    (i, neuron) => neuron.ForeachWeight((j, weight) => result.Add(weight.Signal)));
            }

            return new Vector(result.ToArray());
        }

        private static IVector CalculateMinimizeDirection(IMatrix invertedPseudoGessian, IVector gradient)
        {
            return invertedPseudoGessian.Mul(gradient.Negate());
        }

        private IVector CalculateStepAndChangeAlpha(IVector direction, List<ILayer> layers, double oldError, Func<double> calculateError)
        {
            var oldWeights = _weights;
            //can change alpha by minimizing it in f(xk + alpha*direction)
            double error;
            IVector step;
            do
            {
                step = direction.Mul(_alpha);
                _weights = oldWeights.Sum(step);
                SetWeights(_weights, layers);

                error = calculateError();
                if (Math.Abs(error - oldError) > _errorThreshold)
                    _alpha /= 2.0;

            } while (Math.Abs(error - oldError) > _errorThreshold);
            
            return step;
        }
    }
}