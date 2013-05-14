using System.Globalization;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Misc
{
    public class RealNumberParser : INumberParser
    {
        public IFuzzyNumber Parse(string stringWithNumber)
        {
            var value = double.Parse(stringWithNumber, CultureInfo.InvariantCulture);
            return new RealNumber(value);
        }
    }
}