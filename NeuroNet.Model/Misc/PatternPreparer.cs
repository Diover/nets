using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Net.LearningAlgorithm;

namespace NeuroNet.Model.Misc
{
    public class TestPatternPreparer: IPatternPreparer
    {
        private readonly List<ILearningPattern> _patterns;

        public TestPatternPreparer(string filename)
        {
            _patterns = Load(filename);
        }

        private static List<ILearningPattern> Load(string filename)
        {
            var result = new List<ILearningPattern>();

            using (var fs = new FileStream(filename, FileMode.Open))
            using (var reader = new StreamReader(fs))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var element = ParseLine(line);
                    if (element == null)
                        continue;
                    result.Add(element);
                }
            }

            return result;
        }

        private static LearningPattern ParseLine(string line)
        {
            int inputOutputSeparator = line.IndexOf(' ');
            string inputsPart = line.Substring(0, inputOutputSeparator);
            string outputsPart = line.Substring(inputOutputSeparator + 1, line.Length - inputOutputSeparator - 1);

            if (inputsPart == "" || outputsPart == "")
                return null;

            const char numbersSeparator = ';';
            var inputsNumbers = inputsPart.Split(numbersSeparator);
            var inputs = inputsNumbers.Select(ParseSrtingWithFuzzyNumber).ToList();
            var outputsNumbers = outputsPart.Split(numbersSeparator);
            var outputs = outputsNumbers.Select(ParseSrtingWithFuzzyNumber).ToList();

            return new LearningPattern(inputs, outputs);
        }

        private static IFuzzyNumber ParseSrtingWithFuzzyNumber(string lineWithNumber)
        {
            const char doublesSeparator = ',';
            var stringWithDoubles = lineWithNumber.Split(doublesSeparator);
            var doubles = stringWithDoubles.Select(s=>double.Parse(s, CultureInfo.InvariantCulture));

            if (doubles.Count() != 3)
                return null;

            var left = doubles.ElementAt(0);
            var mod = doubles.ElementAt(1);
            var right = doubles.ElementAt(2);

            return new DiscreteFuzzyNumber(new TriangularFuzzyFunction(left, mod, right), 11);
        }

        public List<ILearningPattern> PreparePatterns()
        {
            return _patterns;
        }
    }
}