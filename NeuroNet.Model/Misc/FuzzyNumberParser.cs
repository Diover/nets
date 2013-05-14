using System.Globalization;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Misc
{
    public class FuzzyNumberParser : INumberParser
    {
        public IFuzzyNumber Parse(string stringWithNumber)
        {
            const char doublesSeparator = ',';
            var stringWithDoubles = stringWithNumber.Split(doublesSeparator);
            var doubles = stringWithDoubles.Select(s => double.Parse(s, CultureInfo.InvariantCulture));

            if (doubles.Count() != 3)
                return null;

            var left = doubles.ElementAt(0);
            var mod = doubles.ElementAt(1);
            var right = doubles.ElementAt(2);

            return new DiscreteFuzzyNumber(new TriangularFuzzyFunction(left, mod, right), 11);
        }
    }
}