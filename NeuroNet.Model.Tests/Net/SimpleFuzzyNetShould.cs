using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Vectors;
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

        [Test]
        public void SetPropagatedErrorToNullAfterClear()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            var net = new SimpleFuzzyNet(inputs, hidden, () => new RealNumber(0.0));

            SetPropagatedErrorToZero(net);
            net.ClearPropagatedError();

            foreach (var layer in net.Layers)
            {
                layer.ForeachNeuron((i, neuron) => Assert.IsNull(neuron.PropagatedError));
            }
        }

        private static void SetPropagatedErrorToZero(INet net)
        {
            foreach (var layer in net.Layers)
            {
                layer.ForeachNeuron((i, neuron) =>
                {
                    neuron.PropagatedError = new RealNumber(0.0);
                });
            }
        }

        [Test]
        public void CorrectlyCreateWeightsVector()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            var net = new SimpleFuzzyNet(inputs, hidden, GenerateNumbers, outputNeuronsCount: outputs);

            var weights = net.GetWeights();
            var expectedWeights = new Vector(new IFuzzyNumber[]
                {
                    //output layer
                    //first neuron
                    new RealNumber(2.0),
                    new RealNumber(2.0),
                    //second neuron
                    new RealNumber(4.0),
                    new RealNumber(2.0),

                    //hidden 1
                    new RealNumber(1.0),
                    new RealNumber(-1.5),
                    new RealNumber(2.0),
                    new RealNumber(1.0),

                    //hidden 2
                    new RealNumber(-1.0),
                    new RealNumber(-1.0),
                    new RealNumber(2.0),
                    new RealNumber(-1.0),
                });

            for (int i = 0; i < expectedWeights.Length; i++)
            {
                Assert.That(weights[i].GetMod().X, Is.EqualTo(expectedWeights[i].GetMod().X));
            }
        }

        [Test]
        public void CorrectlySetWeightsVector()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            var net = new SimpleFuzzyNet(inputs, hidden, ()=>new RealNumber(0.0), outputNeuronsCount: outputs);
            var weights = new Vector(new IFuzzyNumber[]
                {
                    //output layer
                    //first neuron
                    new RealNumber(2.0),
                    new RealNumber(2.0),
                    //second neuron
                    new RealNumber(4.0),
                    new RealNumber(2.0),

                    //hidden 1
                    new RealNumber(1.0),
                    new RealNumber(-1.5),
                    new RealNumber(2.0),
                    new RealNumber(1.0),

                    //hidden 2
                    new RealNumber(-1.0),
                    new RealNumber(-1.0),
                    new RealNumber(2.0),
                    new RealNumber(-1.0),
                });

            //tuple: (layer, neuron, weight)
            var expectedWeights = new Dictionary<Tuple<int, int, int>, IFuzzyNumber>();
            for (int i = 0; i < weights.Length; i++)
            {
                expectedWeights.Add(new Tuple<int, int, int>(ToLayerIndex(i), ToNeuronIndex(i), ToWeightIndex(i)),
                                    weights[i]);
            }
            
            net.SetWeights(weights);

            int l = 0;
            foreach (var layer in net.Layers)
            {
                layer.ForeachNeuron((i, neuron) =>
                {
                    neuron.ForeachWeight((j, weight) =>
                        {
                            Assert.That(weight.Signal.GetMod().X,
                                        Is.EqualTo(expectedWeights[new Tuple<int, int, int>(l, i, j)].GetMod().X));
                        });
                });
                l++;
            }
        }

        private static int ToLayerIndex(int i)
        {
            if (i >= 0 && i < 4)
                return 2;
            if (i >= 4 && i < 8)
                return 0;
            if (i >= 8 && i < 12)
                return 1;
            return -1;
        }

        private static int ToWeightIndex(int i)
        {
            return i % 2;
        }

        private static int ToNeuronIndex(int i)
        {
            switch (i % 4)
            {
                case 0:
                case 1:
                    return 0;
                case 2:
                case 3:
                    return 1;
            }
            return -1;
        }

        [Test]
        public void PropagateSignalAfterWeightsChanged()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            Func<double, double> activation = x => x;
            var net = new SimpleFuzzyNet(inputs, hidden, () => new RealNumber(0.0), activation, outputs);

            var input = new List<IFuzzyNumber>
                {
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                };
            var weights = new Vector(new IFuzzyNumber[]
                {
                    //output layer
                    //first neuron
                    new RealNumber(2.0),
                    new RealNumber(2.0),
                    //second neuron
                    new RealNumber(4.0),
                    new RealNumber(2.0),

                    //hidden 1
                    new RealNumber(1.0),
                    new RealNumber(-1.5),
                    new RealNumber(2.0),
                    new RealNumber(1.0),

                    //hidden 2
                    new RealNumber(-1.0),
                    new RealNumber(-1.0),
                    new RealNumber(2.0),
                    new RealNumber(-1.0),
                });

            net.SetWeights(weights);
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
        public void CreateNulledLastInputsVectorAtStart()
        {
            const int inputs = 2;
            var hidden = new[] { 2, 2 };
            const int outputs = 2;
            Func<double, double> activation = x => x;
            var net = new SimpleFuzzyNet(inputs, hidden, () => new RealNumber(0.0), activation, outputs);

            var lastInputs = net.GetLastInputsForWeights();
            
            foreach (var input in lastInputs)
            {
                Assert.IsNull(input.Signal);
            }
        }

        [Test]
        public void ContainsCorrectLastInputsVectorAfterFirstPropagation()
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
            var lastInputs = net.GetLastInputsForWeights();
            net.Propagate(input);
            
            var expectedLastInputs = new IFuzzyNumber[]
                {
                    //output layer
                    //first neuron
                    new RealNumber(-2.0),
                    new RealNumber(-8.0),
                    //second neuron
                    new RealNumber(-2.0),
                    new RealNumber(-8.0),

                    //hidden 1
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                    new RealNumber(1.0),
                    new RealNumber(2.0),

                    //hidden 2
                    new RealNumber(-2.0),
                    new RealNumber(4.0),
                    new RealNumber(-2.0),
                    new RealNumber(4.0),
                };

            int i = 0;
            foreach (var lastInput in lastInputs)
            {
                Assert.That(lastInput.Signal.GetMod().X, Is.EqualTo(expectedLastInputs[i].GetMod().X));
                i++;
            }
        }

        [Test]
        public void ContainsCorrectLastInputsVectorAfterSecondPropagationWithWeightsChanged()
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
            var lastInputs = net.GetLastInputsForWeights();

            net.Propagate(input);
            ChangeFirstWeightsOfNeuronsToOne(net.Layers);
            net.Propagate(input);

            var expectedLastInputs = new IFuzzyNumber[]
                {
                    //output layer
                    //first neuron
                    new RealNumber(-5.0),
                    new RealNumber(-5.0),
                    //second neuron
                    new RealNumber(-5.0),
                    new RealNumber(-5.0),

                    //hidden 1
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                    new RealNumber(1.0),
                    new RealNumber(2.0),

                    //hidden 2
                    new RealNumber(-2.0),
                    new RealNumber(3.0),
                    new RealNumber(-2.0),
                    new RealNumber(3.0),
                };

            int i = 0;
            foreach (var lastInput in lastInputs)
            {
                Assert.That(lastInput.Signal.GetMod().X, Is.EqualTo(expectedLastInputs[i].GetMod().X));
                i++;
            }
        }

        [Test]
        public void CreateCorrectLastInputsVectorAfterFirstPropagation()
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
            var lastInputsLinks = net.GetLastInputsForWeights();
            net.Propagate(input);
            var lastInputs = lastInputsLinks.ToSignalsVector();

            var expectedLastInputs = new IFuzzyNumber[]
                {
                    //output layer
                    //first neuron
                    new RealNumber(-2.0),
                    new RealNumber(-8.0),
                    //second neuron
                    new RealNumber(-2.0),
                    new RealNumber(-8.0),

                    //hidden 1
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                    new RealNumber(1.0),
                    new RealNumber(2.0),

                    //hidden 2
                    new RealNumber(-2.0),
                    new RealNumber(4.0),
                    new RealNumber(-2.0),
                    new RealNumber(4.0),
                };

            for (int i = 0; i < expectedLastInputs.Length; i++)
            {
                Assert.That(lastInputs[i].GetMod().X, Is.EqualTo(expectedLastInputs[i].GetMod().X));
            }
        }

        [Test]
        public void CreateCorrectLastInputsVectorAfterSecondPropagationWithWeightsChanged()
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

            var lastInputsLinks = net.GetLastInputsForWeights();
            net.Propagate(input);
            ChangeFirstWeightsOfNeuronsToOne(net.Layers);
            net.Propagate(input);
            var lastInputs = lastInputsLinks.ToSignalsVector();

            var expectedLastInputs = new IFuzzyNumber[]
                {
                    //output layer
                    //first neuron
                    new RealNumber(-5.0),
                    new RealNumber(-5.0),
                    //second neuron
                    new RealNumber(-5.0),
                    new RealNumber(-5.0),

                    //hidden 1
                    new RealNumber(1.0),
                    new RealNumber(2.0),
                    new RealNumber(1.0),
                    new RealNumber(2.0),

                    //hidden 2
                    new RealNumber(-2.0),
                    new RealNumber(3.0),
                    new RealNumber(-2.0),
                    new RealNumber(3.0),
                };

            for (int i = 0; i < expectedLastInputs.Length; i++)
            {
                Assert.That(lastInputs[i].GetMod().X, Is.EqualTo(expectedLastInputs[i].GetMod().X));
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
                    //hidden1
                case 0:
                    //first weight in first neuron
                    result = new RealNumber(1.0);
                    break;
                case 1:
                    //second weight in first neuron
                    result = new RealNumber(-1.5);
                    break;
                case 2:
                    //first weight in second neuron
                    result = new RealNumber(2.0);
                    break;
                case 3:
                    //second weight in second neuron
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

                    //output layer
                case 8:
                    //first weight in first neuron
                    result = new RealNumber(2.0);
                    break;
                case 9:
                    //first weight in second neuron
                    result = new RealNumber(4.0);
                    break;
                case 10:
                    //second weight in first neuron
                    result = new RealNumber(2.0);
                    break;
                case 11:
                    //second weight in second neuron
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