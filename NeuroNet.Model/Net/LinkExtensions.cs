using System.Collections.Generic;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.Net
{
    public static class LinkExtensions
    {
        public static IVector ToVector(this IEnumerable<ILink> links)
        {
            var res = links.Select(link => link.Signal).ToArray();
            return new Vector(res);
        }
    }
}