using System;
using System.Collections.Generic;
using System.IO;

namespace NeuroNet.Model.Misc
{
    public class LocalPricesProvider : IPricesProvider
    {
        private readonly Lazy<List<KeyValuePair<DateTime, double>>> _prices;

        public LocalPricesProvider(string filename)
        {
            _prices = new Lazy<List<KeyValuePair<DateTime, double>>>(() => Load(filename));
        }

        private static List<KeyValuePair<DateTime, double>> Load(string filename)
        {
            var result = new List<KeyValuePair<DateTime, double>>();

            using (var fs = new FileStream(filename, FileMode.Open))
            using (var reader = new StreamReader(fs))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var element = ParseLine(line);
                    if (!element.HasValue)
                        continue;
                    result.Add(element.Value);
                }
            }

            return result;
        }

        private static KeyValuePair<DateTime, double>? ParseLine(string line)
        {
            int separator = line.IndexOf(';');
            string dateLine = line.Substring(0, separator);
            string valueLine = line.Substring(separator + 1, line.Length - separator - 1);

            if (valueLine == "")
                return null;

            return new KeyValuePair<DateTime, double>(DateTime.Parse(dateLine), double.Parse(valueLine));
        }

        public IEnumerable<KeyValuePair<DateTime, double>> Prices
        {
            get { return _prices.Value; }
        }
    }
}