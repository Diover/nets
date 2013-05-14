using System;
using System.Collections.Generic;
using Accord.Math;
using Accord.Math.Optimization;
using NeuroNet.Model.FuzzyNumbers;
using System.Linq;

namespace NeuroNet.Model.Net
{
    [Serializable]
    public class Layer : ILayer
    {
        private readonly List<IFuzzyNeuron> _neurons;
        private List<ILink> _forwardLinks; 

        public Layer()
        {
            _neurons = new List<IFuzzyNeuron>();
            _forwardLinks = new List<ILink>();
        }

        public Layer(int neuronsCount, int levelsCount)
            : this()
        {
            for (int i = 0; i < neuronsCount; i++)
                AddNeuron(new FuzzyNeuron(levelsCount));
        }

        public void AddNeuron(IFuzzyNeuron neuron)
        {
            _neurons.Add(neuron);
        }

        public void ForeachNeuron(Action<int, IFuzzyNeuron> action)
        {
            int i = 0;
            foreach (var neuron in _neurons)
            {
                action(i, neuron);
                i++;
            }
        }

        public void ConnectTo(ILayer nextLayer)
        {
            _forwardLinks.Clear();
            foreach (var neuron in _neurons)
            {
                var output = new Link();
                _forwardLinks.Add(output);
                neuron.SetOutput(output);
                nextLayer.ForeachNeuron((i, nextNeuron) => nextNeuron.AddInput(output));
            }
        }

        public void ConnectTo(List<ILink> nextLayer)
        {
            if(nextLayer.Count != _neurons.Count)
                throw new ArgumentException("Next layer dimension not equals this layer neurons count");

            _forwardLinks = nextLayer;
            int i = 0;
            foreach (var neuron in _neurons)
            {
                neuron.SetOutput(_forwardLinks.ElementAt(i));
                i++;
            }
        }
        
        public IFuzzyNeuron GetNeuron(int i)
        {
            return _neurons.ElementAt(i);
        }

        public int NeuronsCount { get { return _neurons.Count; } }
        
        public List<IFuzzyNumber> LastOutput 
        { 
            get { return _forwardLinks.Select(link => link.Signal).ToList(); }
        }

        public void Propagate()
        {
            foreach (var neuron in _neurons)
            {
                neuron.Propagate();
            }
        }
    }
}