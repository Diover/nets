using System;
using System.Collections.Generic;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Misc
{
    public class MarketTick
    {
        public IFuzzyNumber Open;
        public IFuzzyNumber High;
        public IFuzzyNumber Low;
        public IFuzzyNumber Close;
        public IFuzzyNumber Volume;
        public DateTime Time;
        public readonly List<IFuzzyNumber> Indicators;

        public MarketTick(List<IFuzzyNumber> tick, DateTime time, List<IFuzzyNumber> indicators = null)
        {
            Open = tick.ElementAt(0);
            High = tick.ElementAt(1);
            Low = tick.ElementAt(2);
            Close = tick.ElementAt(3);
            Volume = tick.ElementAt(4);
            Time = time;
            Indicators = indicators ?? new List<IFuzzyNumber>();
        }
    }
}