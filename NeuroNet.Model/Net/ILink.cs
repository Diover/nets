using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net
{
    public interface ILink
    {
        IFuzzyNumber Signal { get; set; }
    }
}