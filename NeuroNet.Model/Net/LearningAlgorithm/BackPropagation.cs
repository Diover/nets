using System;
using System.Collections.Generic;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Matrixes;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public class BackPropagation : BackPropagationBase
    {
        private double _alpha;
        private IVector _gradient;
        private IVector _weights;
        private List<ILink> _outputDeltas;

        public BackPropagation(List<ILearningPattern> patterns, double alpha = 0.7, double errorThreshold = 0.0001):base(patterns, errorThreshold)
        {
            _alpha = alpha;
        }

        protected override void LearnBatch(INet net, double currentLearningCycleError)
        {
            //_gradient = CreateWeightsGradient(net.Layers);
            //ChangeWeights(_gradient, net);
            //net.ClearPropagatedError();
        }

        protected override void LearnPattern(INet net, ILearningPattern learningPattern, double currentPatternError)
        {
            //call only after net.propagation()
            CalculateGradientOnLayers(net.Layers, learningPattern.Output);
            _gradient = CreateWeightsGradient(net.Layers);
            ChangeWeights(_gradient, net);
            net.ClearPropagatedError();
        }

        protected override void PrepareToLearning(INet net)
        {
            _weights = net.GetWeights();
            _outputDeltas = net.GetLastInputsForWeights();
        }

        protected override bool IsNetLearned(double currentError)
        {
            return currentError < ErrorThreshold;
        }

        private IVector ChangeWeights(IVector weightsDelta, INet net)
        {
            var oldWeights = _weights;

            var step = weightsDelta.Mul(_alpha);
            _weights = oldWeights.Sum(step);
            net.SetWeights(_weights);

            return step;
        }

        private IVector CreateWeightsGradient(List<ILayer> layers)
        {
            var result = new List<IFuzzyNumber>();

            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) => neuron.ForeachWeight((j, weight) => result.Add(neuron.PropagatedError)));
            var hiddenLayers = layers.Take(layers.Count - 1);
            foreach (var hiddenLayer in hiddenLayers)
            {
                hiddenLayer.ForeachNeuron(
                    (i, neuron) => neuron.ForeachWeight((j, weight) => result.Add(neuron.PropagatedError)));
            }

            return new Vector(result.ToArray()).MemberviseMul(_outputDeltas.ToSignalsVector());
        }

        private static void CalculateGradientOnLayers(List<ILayer> layers, List<IFuzzyNumber> patternsOutput)
        {
            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) =>
                {
                    var output = neuron.LastOutput; //Ok
                    var expectedOutput = patternsOutput.ElementAt(i); //tk
                    var error = output.Mul(output.Apply(levelValue => 1 - levelValue))
                                      .Mul(expectedOutput.Sub(output))
                                      .Mul(4.0); //Ok(1-Ok)(tk - Ok)*alpha
                    neuron.PropagatedError = neuron.PropagatedError == null ? error : neuron.PropagatedError.Sum(error);
                });
            
            //from last to first
            for (int i = layers.Count - 2; i >= 0; i--)
            {
                var layer = layers.ElementAt(i);
                var nextLayer = layers.ElementAt(i + 1);

                layer.ForeachNeuron((neuronIndex, neuron) =>
                {
                    var output = neuron.LastOutput; //Ok
                    var part = output.Mul(output.Apply(levelValue => 1 - levelValue)).Mul(4.0); //Ok(1 - Ok)*alpha
                    //var part = output.Mul(output.Apply(levelValue => 1 - levelValue)); //Ok(1 - Ok)
                    var sum = FuzzyNumberExtensions.Sum(0, nextLayer.NeuronsCount,
                                                        j => nextLayer.GetNeuron(j).GetWeight(neuronIndex).Signal
                                                                      .Mul(nextLayer.GetNeuron(j).PropagatedError));
                    neuron.PropagatedError = neuron.PropagatedError == null
                                                 ? part.Mul(sum)
                                                 : neuron.PropagatedError.Sum(part.Mul(sum));
                });
            }
        }
    }
}