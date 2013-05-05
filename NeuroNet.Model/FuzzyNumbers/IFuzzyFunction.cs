using System.Collections.Generic;

namespace NeuroNet.Model.FuzzyNumbers
{
    public interface IFuzzyFunction
    {
        PointD GetLeft();
        PointD GetRight();
        PointD GetMod();
        IntervalD GetAlphaLevel(double a);
    }
}