using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            const int inputsCount = 3;
            const int hiddenNeuronsCount = 2;
            var net = new SimpleFuzzyNet(inputsCount, hiddenNeuronsCount, levelsCount: 11);

            var patterns = new TestPatternPreparer("H:\\testPatterns.txt").PreparePatterns();

            var bp = new BackPropagation(patterns);
            bp.CyclePerformed +=
                (cycle, error) =>
                    {
                        if (cycle%50 == 0)
                            Console.WriteLine("cycle: " + cycle + 
                                              " error: " + error.ToString("0.##########################################"));
                    };
            bp.LearnNet(net);

            BinaryFileSerializer.SaveNetState("H:\\LearnedNet.net", net);
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
