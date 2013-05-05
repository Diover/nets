using System;
using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net
{
    public class MathExtensions
    {
        public static IFuzzyNumber Sum(int start, int end, Func<int, IFuzzyNumber> f)
        {
            IFuzzyNumber res = new DiscreteFuzzyNumber();
            res.Set(f(start));
            for (int i = start + 1; i < end; i++)
            {
                res.Set(res.Sum(f(i)));
            }
            return res;
        }
    }
}