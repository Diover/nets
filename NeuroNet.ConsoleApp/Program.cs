using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Vectors;
using NeuroNet.Model.Misc;
using NeuroNet.Model.Net;
using NeuroNet.Model.Net.LearningAlgorithm;

namespace NeuroNet.ConsoleApp
{
    class Program
    {
        static List<ILearningPattern> CreatePatterns(Func<double, double, double> f)
        {
            var patternsCount = 5;
            var step = 0.5;

            var a = -100.0;
            var b = 100.0;

            var result = new List<ILearningPattern>();
            for (double i = a; i < a + step * patternsCount; i += step)
                for (double j = b; j < b + step*patternsCount; j += step)
                {
                    if(f(i, j) > 1.0 || f(i, j) < 0.0)
                        throw new ArgumentException("f(" + i + ", " + j + ") = " + f(i, j) + " is out of [0,1]");
                    
                    result.Add(new LearningPattern(new List<IFuzzyNumber> {new RealNumber(i), new RealNumber(j)},
                                                   new List<IFuzzyNumber> {new RealNumber(f(i, j))}));
                }
            return result;
        }

        private static RealNumber GenerateNumber()
        {
            return new RealNumber(0.5);
        }

        static void Main(string[] args)
        {
            /*var b = new BfgsMethod();
            var f =
                new Func<IVector, double>(
                    vector =>
                    -Math.Exp(-Math.Pow(vector[0].GetMod().X - 1, 2)) -
                    Math.Exp(-0.5*Math.Pow(vector[1].GetMod().X - 2, 2)));

            Func<IVector, IVector> g = vector => new Vector(new[]
                {
                    // df/dx = {-2 e^(-    (x-1)^2) (x-1)} 
                    new RealNumber(2*Math.Exp(-Math.Pow(vector[0].GetMod().X - 1, 2))*(vector[0].GetMod().X - 1)),

                    // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
                    new RealNumber(Math.Exp(-0.5*Math.Pow(vector[1].GetMod().X - 2, 2))*(vector[1].GetMod().X - 2))
                });

            b.Minimize(f, g, 2);

            var x = b.Values;
            var fx = b.Minimum;
            */

            const int inputsCount = 1;
            const int hiddenNeuronsCount = 2;
            //var net = new SimpleFuzzyNet(inputsCount, new[] {hiddenNeuronsCount}, () => DiscreteFuzzyNumber.GenerateLittleNumber(levelsCount: 3) levelsCount: 11);
            //var net = new SimpleFuzzyNet(inputsCount, new[] { hiddenNeuronsCount}, RealNumber.GenerateLittleNumber);
            var net = new SimpleFuzzyNet(inputsCount, new[] { hiddenNeuronsCount }, GenerateNumber);
            
            //var patterns = new TestPatternPreparer("testPatterns.txt", new FuzzyNumberParser()).PreparePatterns();
            var patterns = new TestPatternPreparer("testPatternsReal.txt", new RealNumberParser()).PreparePatterns();
            //var patterns = CreatePatterns((x, y) => Math.Abs(Math.Sin(x) + Math.Sin(y))/2.0);

            var bp = new BackPropagation(patterns);
            bp.CyclePerformed +=
                (state) =>
                {
                    Console.ReadKey();
                    //if (state.Cycle % 100 == 0)
                        Console.WriteLine("cycle: " + state.Cycle +
                                          " error: " + state.CycleError.ToString("0.#####################"));
                };

            /*var bp = new BackPropagationWithPseudoNeuton(patterns);
            bp.CyclePerformed +=
                (state) =>
                    {
                        //Console.ReadKey();
                        if (state.Cycle % 100 == 0)
                            Console.WriteLine("cycle: " + state.Cycle +
                                              " error: " + state.CycleError.ToString("0.#########################") +
                                              " grad: " + state.GradientNorm.ToString());
                    };*/

            bp.LearnNet(net);

            BinaryFileSerializer.SaveNetState("LearnedNet.net", net);
            
            /*
            var net = BinaryFileSerializer.LoadNetState("LearnedNet.net");
            var patterns = new TestPatternPreparer("testPatternsReal.txt", new RealNumberParser()).PreparePatterns();
            foreach (var pattern in patterns)
            {
                var o = net.Propagate(pattern.Input);
            }*/
            
            //var inputs = new List<IFuzzyNumber>();
            //for (int i = 0; i < inputsCount; i++)
            //    inputs.Add(DiscreteFuzzyNumber.GenerateLittleNumber());

            //var net = BinaryFileSerializer.LoadNetState("H:\\test.net");

            //var s = new Stopwatch();
            //s.Start();
            //var output = net.PropagateLastInput();
            //s.Stop();
            //var time = s.Elapsed;

            //output.First()
            //      .ForeachLevel((alpha, interval) => Console.WriteLine("alpha: " + alpha + " levels:" + interval));
            //Console.WriteLine("Time of signal propagation: " + time);

            //BinaryFileSerializer.SaveNetState("H:\\test.net", net);
            Console.WriteLine("Finished. Press any key...");
            Console.ReadKey();
        }
    }
}
