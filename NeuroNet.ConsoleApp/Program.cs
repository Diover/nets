using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

            const int inputsCount = 2;
            const int hiddenNeuronsCount = 2;
            const int outputNeuronsCount = 1;
            //var net = new SimpleFuzzyNet(inputsCount, new[] {hiddenNeuronsCount}, () => DiscreteFuzzyNumber.GenerateLittleNumber(levelsCount: 11), levelsCount: 11);
            var net = new SimpleFuzzyNet(inputsCount, new[] { hiddenNeuronsCount }, RealNumber.GenerateLittleNumber, outputNeuronsCount: outputNeuronsCount);
            /*
            var weights = new Vector(new IFuzzyNumber[]
                {
                    //output layer
                    //one neuron
                    new RealNumber(0.032680),
                    new RealNumber(0.020701),

                    //hidden 0
                    new RealNumber(-0.082843),
                    new RealNumber(0.018629),
                    new RealNumber(-0.011006),
                    new RealNumber(-0.071407),
                });
            net.SetWeights(weights);
            */
            const string filename = "testPatternsXOR.txt";
            var numberParser = new RealNumberParser();
            //const string filename = "testPatterns.txt";
            //var numberParser = new FuzzyNumberParser();
            
            var patterns = new TestPatternPreparer(Path.Combine("../../../Misc", filename), numberParser).PreparePatterns();
            //var patterns = CreatePatterns((x, y) => Math.Abs(Math.Sin(x) + Math.Sin(y))/2.0);
            
            var bp = new BackPropagation(patterns, 2.0, 0.001);
            bp.CyclePerformed +=
                (state) =>
                {
                    //Console.ReadKey();
                    if (state.Cycle%50 == 0)
                    {
                        Console.WriteLine("cycle: " + state.Cycle +
                                          " error: " + state.CycleError.ToString("0.#####################"));
                    }
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

            BinaryFileSerializer.SaveNetState(
                "../../../Misc/LearnedNet " + Path.GetFileNameWithoutExtension(filename) +
                " " + inputsCount + "-" + hiddenNeuronsCount + "-" + outputNeuronsCount + ".net", net);

            Console.WriteLine("Finished. Press any key...");
            Console.ReadKey();
        }
    }
}
