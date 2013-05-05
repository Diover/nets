namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public interface INeuronWeightsChanger
    {
        void ChangeSimpleNeuronWeights(FuzzyNeuron fuzzyNeuron);
    }
}