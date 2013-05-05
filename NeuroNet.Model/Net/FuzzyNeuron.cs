using System;
using System.Collections.Generic;
using NeuroNet.Model.FuzzyNumbers;
using System.Linq;

namespace NeuroNet.Model.Net
{
    [Serializable]
    public class FuzzyNeuron : IFuzzyNeuron
    {
        private readonly Func<double, double> _f = x => 1.0/(1.0 + Math.Pow(Math.E, -x));

        private readonly int _levelsCount;
        private readonly List<ILink> _inputs = new List<ILink>();
        private readonly List<ILink> _weights = new List<ILink>();
        private ILink _output;

        [NonSerialized] private IFuzzyNumber _propagatedError;
        [NonSerialized] private List<ILink> _weightsDeltas = new List<ILink>();

        public FuzzyNeuron(int levelsCount)
        {
            _propagatedError = null;
            _levelsCount = levelsCount;
            _f = x => 1.0/(1.0 + Math.Pow(Math.E, -x));
        }

        public FuzzyNeuron(Func<double, double> f, int levelsCount)
        {
            _propagatedError = null;
            _f = f;
            _levelsCount = levelsCount;
        }

        public IFuzzyNumber LastOutput { get { return _output.Signal; } }

        public void ForeachWeight(Action<int, ILink> action)
        {
            int i = 0;
            foreach (var weight in _weights)
            {
                action(i, weight);
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

        public void AddInput(ILink link)
        {
            _inputs.Add(link);
            _weights.Add(new Link(DiscreteFuzzyNumber.GenerateLittleNumber(levelsCount: _levelsCount)));
            _weightsDeltas.Add(new Link());
        }

        public void AddInput(ILink link, IFuzzyNumber weight)
        {
            _inputs.Add(link);
            _weights.Add(new Link(weight));
            _weightsDeltas.Add(new Link());
        }

        public void SetOutput(ILink link)
        {
            _output = link;
        }

        public void Propagate()
        {
            _output.Signal = MathExtensions.Sum(0, _inputs.Count,
                                                i => _inputs.ElementAt(i)
                                                            .Signal
                                                            .Mul(_weights.ElementAt(i).Signal))
                                           .Apply(_f);
        }
    }
}