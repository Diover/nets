using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Misc
{
    public interface INumberParser
    {
        IFuzzyNumber Parse(string stringWithNumber);
    }
}