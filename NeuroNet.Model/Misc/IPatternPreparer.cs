using System;
using System.Collections.Generic;
using NeuroNet.Model.Net.LearningAlgorithm;

namespace NeuroNet.Model.Misc
{
    public interface IPatternPreparer
    {
        List<ILearningPattern> PreparePatterns();
    }
}