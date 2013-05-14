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
        private readonly ILayer _hidden;
        private readonly ILayer _output;
        private readonly List<ILink> _outputSignals = new List<ILink>();

        public SimpleFuzzyNet(int inputNeuronsCount, int hiddenNeuronsCount, int outputNeuronsCount = 1, int levelsCount = 100)
        {
            for (int i = 0; i < inputNeuronsCount; i++)
            {
                var inputSignal = new Link();
                _inputSignals.Add(inputSignal);
            }
            _hidden = new Layer(hiddenNeuronsCount, levelsCount);
            _output = new Layer(outputNeuronsCount, levelsCount);
            Layers = new List<ILayer>{_hidden, _output};
            for (int i = 0; i < outputNeuronsCount; i++)
            {
                var outputSignal = new Link();
                _outputSignals.Add(outputSignal);
            }
            BuildStructure();
        }

        private void BuildStructure()
        {
            _hidden.ForeachNeuron((i, neuron) =>
                {
                    foreach (var inputSignal in _inputSignals)
                    {
                        neuron.AddInput(inputSignal);
                    }
                });
            _hidden.ConnectTo(_output);
            _output.ConnectTo(_outputSignals);
        }

        private void PropagateSignal()
        {
            _hidden.Propagate();
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
    }
}