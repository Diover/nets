using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Net;

namespace NeuroNet.Model.Tests.Net
{
    [TestFixture]
    public class SimpleFuzzyNetShould
    {
        [SetUp]
        public void SetupTests()
        {
            _layer = 0;
        }
        private static int _layer;

        [Test]
        public void CorrectlyReturnNumberOfWeights()
        {
            const int inputs = 5;
            var hidden = new[] {7, 2};
            const int output = 1;
            var net = new SimpleFuzzyNet(inputs, hidden, RealNumber.GenerateLittleNumber);

            var weightsCount = net.WeightsCount;

            Assert.That(weightsCount, Is.EqualTo(inputs * hidden[0] + hidden[0] * hidden[1] + hidden[1] * output));
        }

        [Test]
        public void PropagateSignalOnce()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            Func<double, double> activation = x => x;
            var net = new SimpleFuzzyNet(inputs, hidden, GenerateNumbers, activation, outputs);

            var input = new List<IFuzzyNumber>
                {
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                };

            var output = net.Propagate(input);

            var expected = new List<IFuzzyNumber>
                {
                    new RealNumber(-20.0),
                    new RealNumber(-24.0),
                };

            int i = 0;
            foreach (var expectedNumber in expected)
            {
                Assert.That(output.ElementAt(i).GetMod().X, Is.EqualTo(expectedNumber.GetMod().X));
                i++;
            }
        }

        [Test]
        public void PropagateSignalTwice()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            Func<double, double> activation = x => x;
            var net = new SimpleFuzzyNet(inputs, hidden, GenerateNumbers, activation, outputs);

            var input = new List<IFuzzyNumber>
                {
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                };

            net.Propagate(input);
            var output = net.Propagate(input);

            var expected = new List<IFuzzyNumber>
                {
                    new RealNumber(-20.0),
                    new RealNumber(-24.0),
                };

            int i = 0;
            foreach (var expectedNumber in expected)
            {
                Assert.That(output.ElementAt(i).GetMod().X, Is.EqualTo(expectedNumber.GetMod().X));
                i++;
            }
        }

        [Test]
        public void PropagateSignalAfterSomeWeightsChangedToZero()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            Func<double, double> activation = x => x;
            var net = new SimpleFuzzyNet(inputs, hidden, GenerateNumbers, activation, outputs);

            var input = new List<IFuzzyNumber>
                {
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                };

            net.Propagate(input);
            ChangeFirstWeightsOfNeuronsToZero(net.Layers);
            var output = net.Propagate(input);

            var expected = new List<IFuzzyNumber>
                {
                    new RealNumber(-4.0),
                    new RealNumber(-4.0),
                };

            int i = 0;
            foreach (var expectedNumber in expected)
            {
                Assert.That(output.ElementAt(i).GetMod().X, Is.EqualTo(expectedNumber.GetMod().X));
                i++;
            }
        }

        [Test]
        public void PropagateSignalAfterSomeWeightsChangedToOne()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            Func<double, double> activation = x => x;
            var net = new SimpleFuzzyNet(inputs, hidden, GenerateNumbers, activation, outputs);

            var input = new List<IFuzzyNumber>
                {
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                };

            net.Propagate(input);
            ChangeFirstWeightsOfNeuronsToOne(net.Layers);
            var output = net.Propagate(input);

            var expected = new List<IFuzzyNumber>
                {
                    new RealNumber(-15.0),
                    new RealNumber(-15.0),
                };

            int i = 0;
            foreach (var expectedNumber in expected)
            {
                Assert.That(output.ElementAt(i).GetMod().X, Is.EqualTo(expectedNumber.GetMod().X));
                i++;
            }
        }

        private static void ChangeFirstWeightsOfNeuronsToZero(IEnumerable<ILayer> layers)
        {
            foreach (var layer in layers)
            {
                layer.ForeachNeuron((n, neuron) => neuron.ForeachWeight((w, weight) =>
                {
                    if (w == 0)
                        weight.Signal = new RealNumber(0.0);
                }));
            }
        }

        private static void ChangeFirstWeightsOfNeuronsToOne(IEnumerable<ILayer> layers)
        {
            foreach (var layer in layers)
            {
                layer.ForeachNeuron((n, neuron) => neuron.ForeachWeight((w, weight) =>
                {
                    if (w == 0)
                        weight.Signal = new RealNumber(1.0);
                }));
            }
        }

        [Test]
        public void PropagateSignalWithSquaredFunction()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            Func<double, double> activation = x => x*x;
            var net = new SimpleFuzzyNet(inputs, hidden, GenerateNumbers, activation, outputs);

            var input = new List<IFuzzyNumber>
                {
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                };

            var output = net.Propagate(input);

            var expected = new List<IFuzzyNumber>
                {
                    new RealNumber(861184.0),
                    new RealNumber(2985984.0),
                };

            int i = 0;
            foreach (var expectedNumber in expected)
            {
                Assert.That(output.ElementAt(i).GetMod().X, Is.EqualTo(expectedNumber.GetMod().X));
                i++;
            }
        }

        [Test]
        public void PropagateSignalWithXMinusOneFunction()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            Func<double, double> activation = x => x - 1;
            var net = new SimpleFuzzyNet(inputs, hidden, GenerateNumbers, activation, outputs);

            var input = new List<IFuzzyNumber>
                {
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                };

            var output = net.Propagate(input);

            var expected = new List<IFuzzyNumber>
                {
                    new RealNumber(-23.0),
                    new RealNumber(-25.0),
                };

            int i = 0;
            foreach (var expectedNumber in expected)
            {
                Assert.That(output.ElementAt(i).GetMod().X, Is.EqualTo(expectedNumber.GetMod().X));
                i++;
            }
        }


        [Test]
        public void PropagateSignalWithZeroWeights()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            var net = new SimpleFuzzyNet(inputs, hidden, () => new RealNumber(0.0), outputNeuronsCount: outputs);

            var input = new List<IFuzzyNumber>
                {
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                };

            var output = net.Propagate(input);

            var expected = new List<IFuzzyNumber>
                {
                    new RealNumber(0.5),
                    new RealNumber(0.5),
                };

            int i = 0;
            foreach (var expectedNumber in expected)
            {
                Assert.That(output.ElementAt(i).GetMod().X, Is.EqualTo(expectedNumber.GetMod().X));
                i++;
            }
        }

        /// <summary>
        /// This function is used to generate appropriate weights for net
        /// </summary>
        /// <returns></returns>
        private static IFuzzyNumber GenerateNumbers()
        {
            IFuzzyNumber result;

            switch (_layer)
            {
                case 0:
                    result = new RealNumber(1.0);
                    break;
                case 1:
                    result = new RealNumber(-1.5);
                    break;
                case 2:
                    result = new RealNumber(2.0);
                    break;
                case 3:
                    result = new RealNumber(1.0);
                    break;


                case 4:
                    result = new RealNumber(-1.0);
                    break;
                case 5:
                    result = new RealNumber(2.0);
                    break;
                case 6:
                    result = new RealNumber(-1.0);
                    break;
                case 7:
                    result = new RealNumber(-1.0);
                    break;


                case 8:
                    result = new RealNumber(2.0);
                    break;
                case 9:
                    result = new RealNumber(4.0);
                    break;
                case 10:
                    result = new RealNumber(2.0);
                    break;
                case 11:
                    result = new RealNumber(2.0);
                    break;


                default:
                    result = new RealNumber(0.0);
                    break;
            }
            _layer++;

            return result;
        }
    }
}