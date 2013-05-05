using System;
using System.Collections.Generic;

namespace NeuroNet.Model.Misc
{
    public interface IPricesProvider
    {
        IEnumerable<KeyValuePair<DateTime, double>> Prices { get; }
    }
}