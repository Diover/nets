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
        private IVector _weights;
        private List<ILink> _outputs;
        private IVector _deltas;

        public BackPropagation(List<ILearningPattern> patterns, double alpha = 0.7, double errorThreshold = 0.0001):base(patterns, errorThreshold)
        {
            _alpha = alpha;
        }

        protected override void LearnBatch(INet net, double currentLearningCycleError)
        {
            ChangeAndSetWeights(_deltas, net);
            _deltas = null;
        }

        protected override void LearnPattern(INet net, ILearningPattern learningPattern, double currentPatternError)
        {
            //call only after net.propagation()
            PropagateErrorOnLayers(net.Layers, learningPattern.Output);
            CalculateWeightDelta(net);

            //ChangeAndSetWeights(_deltas, net);
            //_deltas = null;
        }

        private void CalculateWeightDelta(INet net)
        {
            var gradient = CreateWeightsGradient(net.Layers);
            var currentDelta = gradient.Mul(_alpha);
            _deltas = _deltas == null ? currentDelta : _deltas.Sum(currentDelta);
        }

        protected override void PrepareToLearning(INet net)
        {
            _weights = net.GetWeights();
            _outputs = net.GetLastInputsForWeights();
        }

        protected override bool IsNetLearned(double currentError)
        {
            return currentError < ErrorThreshold;
        }

        private void ChangeAndSetWeights(IVector delta, INet net)
        {
            _weights = _weights.Sum(delta);
            net.SetWeights(_weights);
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

            return new Vector(result.ToArray()).MemberviseMul(_outputs.ToSignalsVector());
        }
    }
}