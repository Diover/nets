using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Net.LearningAlgorithm;

namespace NeuroNet.Model.Misc
{
    public class MarketSeriesPatternPreparer : IPatternPreparer
    {
        private readonly INumberParser _parser;
        private readonly int _seriesLength;
        private readonly int _forecastLength;
        private readonly int _priceShift;
        private readonly List<ILearningPattern> _patterns;

        private const char _inputOutputSeparator = '|';
        private const char _valuesSeparator = ';';

        public MarketSeriesPatternPreparer(string filename, INumberParser parser, int seriesLength = 5, int forecastLength = 3, int priceShift = -10000)
        {
            _parser = parser;
            _seriesLength = seriesLength;
            _forecastLength = forecastLength;
            _priceShift = priceShift;
            _patterns = Load(filename);
        }

        private List<ILearningPattern> Load(string filename)
        {
            var ticks = LoadFromFile(filename);
            var result = new List<ILearningPattern>();

            var ticksNumbers = new List<int>();

            for (int i = _seriesLength - 1; i < ticks.Count - _forecastLength - 1; i++)
            {
                var inputs = new List<IFuzzyNumber>();  //input of (t(k - _seriesLength), ..., tk)
                for (int tick = i; tick > i - _seriesLength; tick--)
                {
                    inputs.AddRange(ticks.ElementAt(tick).Input);
                }

                var patternLength = ticks.ElementAt(i).Input.Count;
                //compute average of prices (assumed, that price is first item in pattern)
                var average = inputs.Where((number, index) => index % patternLength == 0).Average(number => number.GetMod().X);

                //assumed, that output contains only one element
                List<IFuzzyNumber> outputs;// = new List<IFuzzyNumber>(); //output of (tk, ..., t(k + forecastLength))
                int stagnation = 0;
                int up = 0;
                int down = 0;
                int stagnationChannelSize = 30;
                int upDownChannelSize = 70;
                for (int tick = i + 1; tick < i + _forecastLength + 1; tick++)
                {
                    var value = ticks.ElementAt(tick).Output.ElementAt(0).GetMod().X;
                    if (Math.Abs(value - average) < stagnationChannelSize)
                        stagnation++;
                    if (value < average - upDownChannelSize)
                        down++;
                    if (value > average + upDownChannelSize)
                        up++;
                }

                if (stagnation == _forecastLength)
                {
                    outputs = new List<IFuzzyNumber>{new RealNumber(0), new RealNumber(1), new RealNumber(0)};
                    result.Add(new LearningPattern(inputs, outputs));
                    ticksNumbers.Add(i);
                }
                if (up == _forecastLength)
                {
                    outputs = new List<IFuzzyNumber>{new RealNumber(1), new RealNumber(0), new RealNumber(0)};
                    result.Add(new LearningPattern(inputs, outputs));
                    ticksNumbers.Add(i);
                }
                if (down == _forecastLength)
                {
                    outputs = new List<IFuzzyNumber>{new RealNumber(0), new RealNumber(0), new RealNumber(1)};
                    result.Add(new LearningPattern(inputs, outputs));
                    ticksNumbers.Add(i);
                }
            }

            return NormalizeInputData(result);
        }

        private List<ILearningPattern> NormalizeInputData(List<ILearningPattern> patterns)
        {
            var result = new List<ILearningPattern>();

            var upCategory = patterns.Where(pattern => Math.Abs(pattern.Output.ElementAt(0).GetMod().X - 1) < 0.1).ToList();
            var downCategory = patterns.Where(pattern => Math.Abs(pattern.Output.ElementAt(2).GetMod().X - 1) < 0.1).ToList();
            var channelCategory = patterns.Where(pattern => Math.Abs(pattern.Output.ElementAt(1).GetMod().X - 1) < 0.1).ToList();

            result.AddRange(NormalizeCategory(upCategory));
            result.AddRange(NormalizeCategory(downCategory));
            result.AddRange(NormalizeCategory(channelCategory));


            return result;
        }

        private List<ILearningPattern> NormalizeCategory(List<ILearningPattern> category)
        {
            var result = new List<ILearningPattern>();
            
            var inputSet = category.SelectMany(pattern => pattern.Input).ToList();
            var expectedValue = ExpectedValue(inputSet);
            var variance = SquaredVariance(inputSet, expectedValue).Apply(Math.Sqrt);

            foreach (var pattern in category)
            {
                var normalizedInput = pattern.Input.Select(number => number.Sub(expectedValue).Div(variance)).ToList();
                result.Add(new LearningPattern(normalizedInput, pattern.Output.ToList()));
            }

            var max = result.SelectMany(pattern => pattern.Input).Max(number => number.GetMod().X);
            var min = result.SelectMany(pattern => pattern.Input).Min(number => number.GetMod().X);

            return result;
        }

        private IFuzzyNumber ExpectedValue(List<IFuzzyNumber> set)
        {
            return set.Sum(number => number).Div(set.Count);
        }

        private IFuzzyNumber SquaredVariance(List<IFuzzyNumber> set, IFuzzyNumber expectedValue)
        {
            return set.Sum(number => number.Sub(expectedValue)
                                           .Apply(x => x*x))
                      .Div(set.Count - 1);
        }

        private List<ILearningPattern> LoadFromFile(string filename)
        {
            var result = new List<ILearningPattern>();

            using (var fs = new FileStream(filename, FileMode.Open))
            using (var reader = new StreamReader(fs))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var element = ParseLineWithTick(line);
                    if (element == null)
                        continue;
                    result.Add(element);
                }
            }

            return result;
        }

        private ILearningPattern ParseLineWithTick(string line)
        {
            if (line[0] == '/' && line[1] == '/')
                return null;

            int inputOutputSeparatorPosition = line.IndexOf(_inputOutputSeparator);
            string inputsPart = line.Substring(0, inputOutputSeparatorPosition);
            string outputsPart = line.Substring(inputOutputSeparatorPosition + 1, line.Length - inputOutputSeparatorPosition - 1);

            if (inputsPart == "" || outputsPart == "")
                return null;

            var inputValues = inputsPart.Split(_valuesSeparator);
            var time = DateTime.Parse(inputValues.ElementAt(0));

            var inputs = inputValues.Skip(1).Select(_parser.Parse).ToList();
            //var prices = inputs.Take(5).ToList(); //open, high, low, close, volume
            //var indicators = inputs.Skip(5).ToList();

            var outputsNumbers = outputsPart.Split(_valuesSeparator);
            var outputs = outputsNumbers.Select(_parser.Parse).ToList();

            return new LearningPattern(inputs, outputs);
        }

        public List<ILearningPattern> PreparePatterns()
        {
            return _patterns;
        }
    }
}