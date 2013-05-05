using System;
using System.Collections.Generic;
using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Net;

namespace NeuroNet.Model.Tests.Net
{
    [TestFixture]
    public class FuzzyNeuronShould
    {
        [Test]
        public void PropagateSignal()
        {
            var levelsRight = new Dictionary<double, IntervalD>
                {
                    {0.0, new IntervalD(1.0, 3.0)},
                    {0.5, new IntervalD(1.5, 2.5)},
                    {1.0, new IntervalD(2.0, 2.0)},
                };
            var weight = new DiscreteFuzzyNumber(levelsRight);
            var input = new Link(weight);
            var output = new Link();
            var f = new Func<double, double>(x => 1.0 / (1.0 + Math.Pow(Math.E, -x)));

            var neuron = new FuzzyNeuron(f, 3);
            neuron.AddInput(input, weight);
            neuron.SetOutput(output);
            
            neuron.Propagate();

            var expectedLevel00 = new IntervalD(f(1), f(9));
            var expectedLevel05 = new IntervalD(f(2.25), f(6.25));
            var expectedLevel10 = new IntervalD(f(4.0), f(4.0));

            Assert.NotNull(output.Signal);
            Assert.That(output.Signal.GetAlphaLevel(0.0), Is.EqualTo(expectedLevel00));
            Assert.That(output.Signal.GetAlphaLevel(0.5), Is.EqualTo(expectedLevel05));
            Assert.That(output.Signal.GetAlphaLevel(1.0), Is.EqualTo(expectedLevel10));
        }
    }
}