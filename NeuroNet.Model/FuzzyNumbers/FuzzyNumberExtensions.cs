using System;

namespace NeuroNet.Model.FuzzyNumbers
{
    public class FuzzyNumberExtensions
    {
        public static IFuzzyNumber Sum(int start, int end, Func<int, IFuzzyNumber> f)
        {
            IFuzzyNumber res = f(start);
            for (int i = start + 1; i < end; i++)
            {
                res.Set(res.Sum(f(i)));
            }
            return res;
        }
    }
}