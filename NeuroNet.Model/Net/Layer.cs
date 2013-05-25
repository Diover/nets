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

        public Layer(int neuronsCount, int levelsCount, Func<IFuzzyNumber> littleFuzzyNumberGenerator, Func<double, double> activationFunction = null)
            : this()
        {
            for (int i = 0; i < neuronsCount; i++)
                AddNeuron(new FuzzyNeuron(activationFunction, littleFuzzyNumberGenerator));
        }

        public void AddNeuron(IFuzzyNeuron neuron)
        {
            _neurons.Add(neuron);
        }

        public static void Test()
        {
            var m = Matrix.Create(1, 10, 1.0);
            var v = Matrix.Create(10, 1, 1.0);
            
            var p = m.Multiply(v);

            Func<double[], double> f = (x) =>
                                       -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5*Math.Pow(x[1] - 2, 2));
            Func<double[], double[]> g = (x) => new[]
                {
                    // df/dx = {-2 e^(-    (x-1)^2) (x-1)} 
                    2*Math.Exp(-Math.Pow(x[0] - 1, 2))*(x[0] - 1),

                    // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
                    Math.Exp(-0.5*Math.Pow(x[1] - 2, 2))*(x[1] - 2)
                };

            var lbfgs = new BroydenFletcherGoldfarbShanno(2, f, g);
            lbfgs.Progress += (sender, args) =>
                {
                    //var grad = args.Gradient;
                    //var sol = args.Solution;
                    //var step = args.Step;
                    Console.WriteLine("Iteration: {0}, solution: {1}, {2}", args.Iteration, args.Solution[0], args.Solution[1]);
                };
            bool runAgain;
            do
            {
                try
                {
                    lbfgs.Minimize();
                    runAgain = false;
                }
                catch (LineSearchFailedException)
                {
                    Console.WriteLine("Failed");
                    runAgain = true;
                }    
            } while (runAgain);

            double minValue = lbfgs.Value;
            double[] solution = lbfgs.Solution;

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

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != null)
                    throw new ArgumentException("Cannot set Name twice");
                _name = value;
            }
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