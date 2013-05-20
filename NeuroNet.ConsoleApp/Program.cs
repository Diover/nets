using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Misc;
using NeuroNet.Model.Net;
using NeuroNet.Model.Net.LearningAlgorithm;

namespace NeuroNet.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const int inputsCount = 2;
            const int hiddenNeuronsCount = 2;
            //var net = new SimpleFuzzyNet(inputsCount, new[] {hiddenNeuronsCount}, () => DiscreteFuzzyNumber.GenerateLittleNumber(levelsCount: 3) levelsCount: 11);
            var net = new SimpleFuzzyNet(inputsCount, new[] {hiddenNeuronsCount}, RealNumber.GenerateLittleNumber);
            
            //var patterns = new TestPatternPreparer("testPatterns.txt", new FuzzyNumberParser()).PreparePatterns();
            var patterns = new TestPatternPreparer("testPatternsReal.txt", new RealNumberParser()).PreparePatterns();

            /*var bp = new BackPropagation(patterns);
            bp.CyclePerformed +=
                (state) =>
                {
                    //Console.ReadKey();
                    if (state.Cycle % 100 == 0)
                        Console.WriteLine("cycle: " + state.Cycle +
                                          " error: " + state.CycleError.ToString("0.#####################"));
                };*/

            var bp = new BackPropagationWithPseudoNeuton(patterns);
            bp.CyclePerformed +=
                (state) =>
                    {
                        //Console.ReadKey();
                        if (state.Cycle % 100 == 0)
                            Console.WriteLine("cycle: " + state.Cycle +
                                              " error: " + state.CycleError.ToString("0.#########################") +
                                              " grad: " + state.GradientNorm.ToString());
                    };

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
