using System;
using System.Collections.Generic;
using NeuroNet.Model.FuzzyNumbers;
using System.Linq;

namespace NeuroNet.Model.Net
{
    [Serializable]
    public class FuzzyNeuron : IFuzzyNeuron
    {
        private readonly Func<double, double> _f = x => 1.0/(1.0 + Math.Pow(Math.E, -0.5*x));
        private readonly Func<IFuzzyNumber> _generateLittleFuzzyNumber;
        private readonly List<ILink> _inputs = new List<ILink>();
        private readonly List<ILink> _weights = new List<ILink>();
        private ILink _output;

        [NonSerialized] private IFuzzyNumber _propagatedError;
        [NonSerialized] private readonly List<ILink> _weightsDeltas = new List<ILink>();
        [NonSerialized] private readonly List<double> _weightsLambdas = new List<double>();
        private const double _defaultWeightLambda = 1.0;

        public FuzzyNeuron(Func<IFuzzyNumber> littleFuzzyNumberGenerator)
        {
            _propagatedError = null;
            _generateLittleFuzzyNumber = littleFuzzyNumberGenerator;
        }

        public FuzzyNeuron(Func<double, double> f, Func<IFuzzyNumber> littleFuzzyNumberGenerator)
        {
            _propagatedError = null;
            _f = f;
            _generateLittleFuzzyNumber = littleFuzzyNumberGenerator;
        }

        public IFuzzyNumber LastOutput { get { return _output.Signal; } }

        public void ForeachWeight(Action<int, ILink> action)
        {
            int i = 0;
            foreach (var weight in _weights)
            {
                action(i, weight);
                i++;
            }
        }

        public IFuzzyNumber PropagatedError
        {
            get { return _propagatedError; }
            set { _propagatedError = value; }
        }

        public ILink GetWeight(int i)
        {
            return _weights.ElementAt(i);
        }

        public ILink GetWeightDelta(int i)
        {
            return _weightsDeltas.ElementAt(i);
        }

        public ILink GetLastInput(int i)
        {
            return _inputs.ElementAt(i);
        }

        public void SetWeightLambda(int i, double newValue)
        {
            _weightsLambdas[i] = newValue;
        }

        public double GetWeightLambda(int i)
        {
            return _weightsLambdas.ElementAt(i);
        }

        public void AddInput(ILink link)
        {
            _inputs.Add(link);
            _weights.Add(new Link(generateLittleFuzzyNumber()));
            _weightsDeltas.Add(new Link());
            _weightsLambdas.Add(_defaultWeightLambda);
        }

        public void AddInput(ILink link, IFuzzyNumber weight)
        {
            _inputs.Add(link);
            _weights.Add(new Link(weight));
            _weightsDeltas.Add(new Link());
            _weightsLambdas.Add(_defaultWeightLambda);
        }

        public void SetOutput(ILink link)
        {
            _output = link;
        }

        public void Propagate()
        {
            _output.Signal = FuzzyNumberExtensions.Sum(0, _inputs.Count,
                                                i => _inputs.ElementAt(i)
                                                            .Signal
                                                            .Mul(_weights.ElementAt(i).Signal))
                                           .Apply(_f);
        }
    }
}