using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroNet.Model.Misc;
using NeuroNet.Model.Net;

namespace NeuroNet.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var net = BinaryFileSerializer.LoadNetState("");
            var patterns = new TestPatternPreparer("../../../Misc/testPatternsXOR.txt", new RealNumberParser()).PreparePatterns();
            var output = patterns.Select(pattern => net.Propagate(pattern.Input)).ToList();

            for (int i = 0; i < patterns.Count; i++)
            {
                Console.WriteLine("Test: {0}, Real: {1}", string.Join(";", patterns.ElementAt(i).Output), string.Join(";", output.ElementAt(i)));
            }

            Console.WriteLine("Finished. Press any key...");
            Console.ReadKey();
        }
    }
}
