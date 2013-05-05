using System.Collections.Generic;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public delegate void StepPerformedEventHandler(int cycle, int step, double error);
    public class BackPropagation
    {
        private readonly List<ILearningPattern> _patterns;
        private readonly double _learningRate;  //eta (n)
        private readonly double _pulseConstant; //beta (b)
        private readonly double _errorThreshold; //Emax

        private List<IFuzzyNumber> _outputNeuronsErrors;
        private List<IFuzzyNumber> _hiddenNeuronsErrors;

        public BackPropagation(List<ILearningPattern> patterns, double learningRate = 0.7, double pulseConstant = 0.5, double errorThreshold = 0.0001)
        {
            _patterns = patterns;
            _learningRate = learningRate;
            _pulseConstant = pulseConstant;
            _errorThreshold = errorThreshold;

            _outputNeuronsErrors = new List<IFuzzyNumber>();
            _hiddenNeuronsErrors = new List<IFuzzyNumber>();
        }

        public void LearnNet(INet net)
        {
            double learningCycleError;
            int cycle = 0;
            do
            {
                learningCycleError = 0.0;
                int step = 0;
                foreach (var learningPattern in _patterns)
                {
                    var patternError = CalculatePatternError(net,
                                                             learningPattern.Input,
                                                             learningPattern.Output);
                    learningCycleError += patternError;

                    if (patternError > 0.0)
                    {
                        PropagateErrorOnLayers(net.Layers, learningPattern.Output);
                        ChangeWeights(net.Layers);
                    }

                    StepPerformed(cycle, step, learningCycleError);
                    step++;
                }
                cycle++;
            } while (learningCycleError > _errorThreshold);
        }

        public event StepPerformedEventHandler StepPerformed;

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

        private void ChangeWeights(List<ILayer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers.ElementAt(i);
                layer.Foreach((j, neuron) => neuron.ForeachWeight((k, weight) =>
                    {
                        var delta = neuron.GetWeightDelta(k);
                        delta.Signal = CalculateNewDeltaForWeight(delta.Signal, neuron.PropagatedError,
                                                                  neuron.GetLastInput(k)
                                                                        .Signal);
                        weight.Signal = weight.Signal.Sum(delta.Signal);
                    }));
            }
        }

        private IFuzzyNumber CalculateNewDeltaForWeight(IFuzzyNumber weightDelta, IFuzzyNumber propagatedError, IFuzzyNumber output)
        {
            if (weightDelta == null)
            {
                return propagatedError.Mul(_learningRate).Mul(output);
            }
            return weightDelta.Mul(_pulseConstant).Sum(propagatedError.Mul(_learningRate).Mul(output));
        }

        public static void PropagateErrorOnLayers(List<ILayer> layers, List<IFuzzyNumber> patternsOutput)
        {
            var outputLayer = layers.Last();
            outputLayer.Foreach((i, neuron) =>
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
                hiddenLayer.Foreach((i, neuron) =>
                    {
                        var output = neuron.LastOutput; //Ok
                        var part = output.Mul(output.Apply(levelValue => 1 - levelValue)); //Oj(1 - Oj)
                        var sum = MathExtensions.Sum(0, outputLayer.NeuronsCount, j => outputLayer.GetNeuron(j)
                                                                                                  .GetWeight(i)
                                                                                                  .Signal
                                                                                                  .Mul(
                                                                                                      outputLayer
                                                                                                          .GetNeuron(j)
                                                                                                          .PropagatedError));
                        neuron.PropagatedError = part.Mul(sum);
                    });
            }
        }
    }
}