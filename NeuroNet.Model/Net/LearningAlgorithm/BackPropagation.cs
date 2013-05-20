using System;
using System.Collections.Generic;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Matrixes;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public delegate void StepPerformedEventHandler(int cycle, int step, double stepError);
    public delegate void CyclePerformedEventHandler(int cycle, double cycleError);

    public class BackPropagation : ILearningAlgorithm
    {
        private readonly List<ILearningPattern> _patterns;
        private double _learningRate;  //eta (n)
        private readonly double _pulseConstant; //beta (b)
        private readonly double _errorThreshold; //Emax

        public BackPropagation(List<ILearningPattern> patterns, double learningRate = 0.7, double pulseConstant = 0.5, double errorThreshold = 0.0001)
        {
            _patterns = patterns;
            _learningRate = learningRate;
            _pulseConstant = pulseConstant;
            _errorThreshold = errorThreshold;
        }

        public void LearnNet(INet net)
        {
            double learningCycleError;
            double previousLearningCycleError = 0.0;
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
                        CalculateGradientOnLayers(net.Layers, learningPattern.Output);
                        ChangeWeights(net.Layers, patternError,
                                      () => CalculatePatternError(net,
                                                                  learningPattern.Input,
                                                                  learningPattern.Output));
                    }

                    var error = CalculatePatternError(net,
                                                             learningPattern.Input,
                                                             learningPattern.Output);

                    OnStepPerformed(cycle, step, learningCycleError);
                    step++;
                }
                //if (learningCycleError <= previousLearningCycleError)
                //    if (_learningRate < 0.5)
                //        _learningRate *= 2;
                //    else if(_learningRate > 0.01)
                //        _learningRate /= 2;
                //if (Math.Abs(previousLearningCycleError - learningCycleError) < 0.0000000001)
                //    AddLittleCorrectionToWeights(net.Layers);

                previousLearningCycleError = learningCycleError;
                //if(new Random().Next(2) == 0)
                //    _learningRate *= 1.001;
                //else
                //    _learningRate /= 2;
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
                StepPerformed(cycle, step, stepError);
        }

        private void OnCyclePerformed(int cycle, double cycleError)
        {
            if (CyclePerformed != null)
                CyclePerformed(cycle, cycleError);
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

        private void ChangeWeights(List<ILayer> layers, double currentError, Func<double> calculateError)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers.ElementAt(i);
                //foreach neuron (j) foreach weight (k): 
                //1) dw(k) = dw(k)*_b + error*_n*(input_of j)
                //2) w(k) = w(k) + dw(k)
                layer.ForeachNeuron((j, neuron) => neuron.ForeachWeight((k, weight) =>
                    {
                        var lambda = neuron.GetWeightLambda(k);
                        var newDelta = CalculateNewDeltaForWeight(neuron.PropagatedError,
                                                                  neuron.GetLastInput(k)
                                                                        .Signal);
                        //var delta = neuron.GetWeightDelta(k);
                        //delta.Signal = newDelta.Mul(lambda);

                        var oldWeight = weight.Signal;
                        weight.Signal = oldWeight.Sum(newDelta.Mul(lambda));

                        /*var er = calculateError();
                        if (currentError >= er)
                            if(lambda < 1000)
                            lambda *= 2.0;
                        else
                            if(lambda > 0.0001)
                            lambda /= 2.0;
                        
                        neuron.SetWeightLambda(k, lambda);
                        weight.Signal = oldWeight.Sum(newDelta.Mul(lambda));*/
                    }));
            }
        }

        private static IFuzzyNumber CalculateNewDeltaForWeight(IFuzzyNumber propagatedError, IFuzzyNumber output)
        {
            //if (weightDelta == null)
            //{
                return propagatedError.Mul(output);
            //}
            //return weightDelta.Mul(_pulseConstant).Sum(propagatedError.Mul(_learningRate).Mul(output));
        }

        private static IVector CreateMutableWeightsGradient(List<ILayer> layers)
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

        private static IVector CreateMutableWeightsVector(List<ILayer> layers)
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

        public static void CalculateGradientOnLayers(List<ILayer> layers, List<IFuzzyNumber> patternsOutput)
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
        }
    }
}