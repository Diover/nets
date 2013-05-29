using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NeuroNet.Model.Net.LearningAlgorithm;

namespace NeuroNet.Model.Misc
{
    public class TestPatternPreparer: IPatternPreparer
    {
        private readonly INumberParser _parser;
        private readonly List<ILearningPattern> _patterns;

        private const char _inputOutputSeparator = ' ';
        private const char _numbersSeparator = ';';

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
            if(line[0] == '/' && line[1] == '/')
                return null;

            int inputOutputSeparatorPosition = line.IndexOf(' ');
            string inputsPart = line.Substring(0, inputOutputSeparatorPosition);
            string outputsPart = line.Substring(inputOutputSeparatorPosition + 1, line.Length - inputOutputSeparatorPosition - 1);

            if (inputsPart == "" || outputsPart == "")
                return null;

            const char numbersSeparator = _numbersSeparator;
            var inputsNumbers = inputsPart.Split(numbersSeparator);
            var inputs = inputsNumbers.Select(_parser.Parse).ToList();
            var outputsNumbers = outputsPart.Split(numbersSeparator);
            var outputs = outputsNumbers.Select(_parser.Parse).ToList();

            return new LearningPattern(inputs, outputs);
        }

        private string PackLine(LearningPattern pattern)
        {
            /*var sb = new StringBuilder();
            foreach(var input in pattern.)
            int inputOutputSeparator = line.IndexOf(_inputOutputSeparator);
            string inputsPart = line.Substring(0, inputOutputSeparator);
            string outputsPart = line.Substring(inputOutputSeparator + 1, line.Length - inputOutputSeparator - 1);

            if (inputsPart == "" || outputsPart == "")
                return null;

            const char numbersSeparator = _numbersSeparator;
            var inputsNumbers = inputsPart.Split(numbersSeparator);
            var inputs = inputsNumbers.Select(_parser.Parse).ToList();
            var outputsNumbers = outputsPart.Split(numbersSeparator);
            var outputs = outputsNumbers.Select(_parser.Parse).ToList();

            return new LearningPattern(inputs, outputs);*/
            return "";
        }

        public List<ILearningPattern> PreparePatterns()
        {
            return _patterns;
        }
    }
}