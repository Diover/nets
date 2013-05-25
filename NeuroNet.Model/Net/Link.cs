using System;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net
{
    [Serializable]
    public class Link : ILink
    {
        public Link()
        {
            Signal = null;
        }

        public Link(IFuzzyNumber signal)
        {
            Signal = signal;
        }

        public IFuzzyNumber Signal { get; set; }

        public override string ToString()
        {
            return Signal.ToString();
        }
    }
}