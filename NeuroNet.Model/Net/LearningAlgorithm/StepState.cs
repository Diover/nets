using NeuroNet.Model.FuzzyNumbers;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public struct StepState
    {
        public StepState(int cycle, int step, double cycleError, IFuzzyNumber gradientNorm = null) : this()
        {
            Cycle = cycle;
            Step = step;
            CycleError = cycleError;
            GradientNorm = gradientNorm;
        }

        public int Cycle { get; private set; }
        public int Step { get; private set; }
        public double CycleError { get; private set; }
        public IFuzzyNumber GradientNorm { get; private set; }
    }
}