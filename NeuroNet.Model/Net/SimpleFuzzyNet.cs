using System;
using System.Collections.Generic;
using NeuroNet.Model.FuzzyNumbers;
using System.Linq;
using NeuroNet.Model.Net.LearningAlgorithm;

namespace NeuroNet.Model.Net
{
    [Serializable]
    public class SimpleFuzzyNet : INet
    {
        private readonly List<ILink> _inputSignals = new List<ILink>();
        private readonly List<ILayer> _hiddens = new List<ILayer>();
        private readonly ILayer _output;
        private readonly List<ILink> _outputSignals = new List<ILink>();

        public SimpleFuzzyNet(int inputNeuronsCount, int[] hiddenNeuronsCount, Func<IFuzzyNumber> littleFuzzyNumberGenerator, Func<double, double> activationFunction = null,  int outputNeuronsCount = 1, int levelsCount = 100)
        {
            for (int i = 0; i < inputNeuronsCount; i++)
            {
                var inputSignal = new Link();
                _inputSignals.Add(inputSignal);
            }
            for (int i = 0; i < hiddenNeuronsCount.Length; i++)
            {
                _hiddens.Add(new Layer(hiddenNeuronsCount[i],
                                       levelsCount,
                                       littleFuzzyNumberGenerator,
                                       activationFunction) {Name = "Hidden layer " + i});
            }

            _output = new Layer(outputNeuronsCount, levelsCount, littleFuzzyNumberGenerator, activationFunction){Name = "Output layer"};
            Layers = new List<ILayer>(_hiddens) {_output};
            for (int i = 0; i < outputNeuronsCount; i++)
            {
                var outputSignal = new Link();
                _outputSignals.Add(outputSignal);
            }
            BuildStructure();
        }

        private void BuildStructure()
        {
            _hiddens.First().ForeachNeuron((i, neuron) =>
                {
                    foreach (var inputSignal in _inputSignals)
                    {
                        neuron.AddInput(inputSignal);
                    }
                });
            for (int i = 0; i < _hiddens.Count - 1; i++)
            {
                _hiddens[i].ConnectTo(_hiddens[i + 1]);
            }
            _hiddens.Last().ConnectTo(_output);
            _output.ConnectTo(_outputSignals);
        }

        private void PropagateSignal()
        {
            foreach (var hidden in _hiddens)
            {
                hidden.Propagate();
            }
            _output.Propagate();
        }

        public List<IFuzzyNumber> Propagate(List<IFuzzyNumber> inputs)
        {
            if(inputs.Count != _inputSignals.Count)
                throw new ArgumentException("Given inputs count not equals net inputs count");

            SetInputSignals(inputs);
            PropagateSignal();
            return _outputSignals.Select(output => output.Signal).ToList();
        }

        public List<IFuzzyNumber> PropagateLastInput()
        {
            PropagateSignal();
            return _outputSignals.Select(output => output.Signal).ToList();
        }

        public List<IFuzzyNumber> LastOutput
        {
            get { return _outputSignals.Select(output => output.Signal).ToList(); }
        }

        public List<ILayer> Layers { get; private set; }

        private void SetInputSignals(List<IFuzzyNumber> inputs)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                _inputSignals.ElementAt(i).Signal = inputs.ElementAt(i);
            }
        }

        public void Learn(BackPropagation bp)
        {
            bp.LearnNet(this);
        }

        public int WeightsCount
        {
            get
            {
                var count = 0;
                Layers.ForEach(
                    layer => layer.ForeachNeuron((i, neuron) => neuron.ForeachWeight((j, weight) => count += 1)));
                return count;
            }
        }
    }
}