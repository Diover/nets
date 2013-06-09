using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuroNet.Model.FuzzyNumbers
{
    public static class FuzzyNumberExtensions
    {
        public static IFuzzyNumber Sum(int start, int end, Func<int, IFuzzyNumber> f)
        {
            IFuzzyNumber res = f(start).Sum(0.0);
            for (int i = start + 1; i < end; i++)
            {
                res.Set(res.Sum(f(i)));
            }
            return res;
        }

        public static IFuzzyNumber Sum(this List<IFuzzyNumber> list, Func<IFuzzyNumber, IFuzzyNumber> f)
        {
            IFuzzyNumber res = f(list[0]).Sum(0.0);
            foreach (var number in list.Skip(1))
            {
                res.Set(res.Sum(f(number)));
            }
            return res;
        }
    }
}