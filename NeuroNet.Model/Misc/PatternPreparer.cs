using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeuroNet.Model.Net.LearningAlgorithm;

namespace NeuroNet.Model.Misc
{
    public class TestPatternPreparer: IPatternPreparer
    {
        private readonly INumberParser _parser;
        private readonly List<ILearningPattern> _patterns;

        public TestPatternPreparer(string filename, INumberParser parser)
        {
            _parser = parser;
            _patterns = Load(filename);
        }

        private List<ILearningPattern> Load(string filename)
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

        private LearningPattern ParseLine(string line)
        {
            int inputOutputSeparator = line.IndexOf(' ');
            string inputsPart = line.Substring(0, inputOutputSeparator);
            string outputsPart = line.Substring(inputOutputSeparator + 1, line.Length - inputOutputSeparator - 1);

            if (inputsPart == "" || outputsPart == "")
                return null;

            const char numbersSeparator = ';';
            var inputsNumbers = inputsPart.Split(numbersSeparator);
            var inputs = inputsNumbers.Select(_parser.Parse).ToList();
            var outputsNumbers = outputsPart.Split(numbersSeparator);
            var outputs = outputsNumbers.Select(_parser.Parse).ToList();

            return new LearningPattern(inputs, outputs);
        }

        public List<ILearningPattern> PreparePatterns()
        {
            return _patterns;
        }
    }
}