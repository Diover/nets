using System;
using System.Collections.Generic;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Matrixes;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public delegate void StepPerformedEventHandler(StepState state);
    public delegate void CyclePerformedEventHandler(StepState state);

    public class BackPropagation : ILearningAlgorithm
    {
        private readonly List<ILearningPattern> _patterns;
        private double _alpha;  //eta (n)
        private readonly double _errorThreshold; //Emax
        private IVector _gradient;
        private IVector _weights;
        private List<ILink> _outputDeltas;

        public BackPropagation(List<ILearningPattern> patterns, double alpha = 0.7, double errorThreshold = 0.0001)
        {
            _patterns = patterns;
            _alpha = alpha;
            _errorThreshold = errorThreshold;
        }

        public void LearnNet(INet net)
        {
            _weights = CreateWeightsVector(net.Layers);
            _outputDeltas = CreateOutputsDeltas(net.Layers);

            double learningCycleError;
            double previousLearningCycleError = 0.0;
            int cycle = 0;
            do
            {
                learningCycleError = 0.0;
                int algoStep = 0;
                foreach (var learningPattern in _patterns)
                {
                    var patternError = CalculatePatternError(net,
                                                             learningPattern.Input,
                                                             learningPattern.Output);
                    learningCycleError += patternError;

                    if (patternError > 0.0)
                    {
                        _gradient = CalculateGradientOnLayers(net.Layers, learningPattern.Output);
                        var direction = _gradient.Negate();
                        CalculateStepAndChangeAlpha(direction, net.Layers, patternError,
                                                    () => CalculatePatternError(net,
                                                                                learningPattern.Input,
                                                                                learningPattern.Output));
                    }

                    var error = CalculatePatternError(net,
                                                             learningPattern.Input,
                                                             learningPattern.Output);

                    OnStepPerformed(cycle, algoStep, learningCycleError);
                    algoStep++;
                }

                previousLearningCycleError = learningCycleError;

                OnCyclePerformed(cycle, learningCycleError);
                cycle++;
            } while (learningCycleError > _errorThreshold);
            int k = 0;
        }

        public event StepPerformedEventHandler StepPerformed;
        public event CyclePerformedEventHandler CyclePerformed;

        private void OnStepPerformed(int cycle, int step, double stepError)
        {
            if (StepPerformed != null)
                StepPerformed(new StepState(cycle, step, stepError));
        }

        private void OnCyclePerformed(int cycle, double cycleError)
        {
            if (CyclePerformed != null)
                CyclePerformed(new StepState(cycle, 0, cycleError));
        }

        private static double CalculatePatternError(INet net, List<IFuzzyNumber> patterns, List<IFuzzyNumber> outputs)
        {
            var actualOutput = net.Propagate(patterns).First();
            var expectedOutput = outputs.First();

            var errorNumber = actualOutput.Sub(expectedOutput);

            var leftError = 0.0;
            var rightError = 0.0;
            errorNumber.ForeachLevel((alpha, level) =>
                {
                    leftError += alpha*(level.X*level.X);
                    rightError += alpha*(level.Y*level.Y);
                });

            return leftError/2.0 + rightError/2.0;
        }

        private void AddLittleCorrectionToWeights(List<ILayer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers.ElementAt(i);
                layer.ForeachNeuron((j, neuron) => neuron.ForeachWeight((k, weight) =>
                {
                    //weight.Signal = weight.Signal.Sum(DiscreteFuzzyNumber.GenerateLittleNumber(levelsCount: 11));
                    weight.Signal = weight.Signal.Sum(RealNumber.GenerateLittleNumber());
                }));
            }
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

        private static List<ILink> CreateOutputsDeltas(List<ILayer> layers)
        {
            var outputs = new List<ILink>();
            
            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) => neuron.ForeachWeight((j, weight) => outputs.Add(neuron.GetLastInput(j))));
            var hiddenLayers = layers.Take(layers.Count - 1);
            foreach (var hiddenLayer in hiddenLayers.Reverse())
            {
                hiddenLayer.ForeachNeuron(
                    (i, neuron) => neuron.ForeachWeight((j, weight) => outputs.Add(neuron.GetLastInput(j))));
            }

            return outputs;
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

            return new Vector(result.ToArray()).MemberviseMul(ToVector(_outputDeltas));
        }

        private static IVector ToVector(IEnumerable<ILink> links)
        {
            var res = links.Select(link => link.Signal).ToArray();
            return new Vector(res);
        }

        public IVector CalculateGradientOnLayers(List<ILayer> layers, List<IFuzzyNumber> patternsOutput)
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
    }
}