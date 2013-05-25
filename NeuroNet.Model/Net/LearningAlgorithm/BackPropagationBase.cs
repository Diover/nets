﻿using System;
using System.Collections.Generic;
using System.Linq;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.FuzzyNumbers.Vectors;

namespace NeuroNet.Model.Net.LearningAlgorithm
{
    public delegate void StepPerformedEventHandler(StepState state);
    public delegate void CyclePerformedEventHandler(StepState state);

    public abstract class BackPropagationBase
    {
        private readonly List<ILearningPattern> _patterns;
        private readonly double _errorThreshold;

        protected BackPropagationBase(List<ILearningPattern> patterns, double errorThreshold = 0.0001)
        {
            _patterns = patterns;
            _errorThreshold = errorThreshold;
        }

        public double ErrorThreshold
        {
            get { return _errorThreshold; }
        }

        public void LearnNet(INet net)
        {
            PrepareToLearning(net);

            var algorithmCycle = 0;
            do
            {
                var learningCycleError = 0.0;
                var algorithmStep = 0;
                foreach (var learningPattern in _patterns)
                {
                    var patternError = CalculatePatternError(net, learningPattern.Input, learningPattern.Output);
                    learningCycleError += patternError;

                    if (patternError > 0.0)
                    {
                        LearnPattern(net, learningPattern, patternError);
                    }

                    OnStepPerformed(algorithmCycle, algorithmStep, patternError);
                    algorithmStep++;
                }
                if(learningCycleError > 0.0)
                {
                    LearnBatch(net, learningCycleError);
                }

                OnCyclePerformed(algorithmCycle, learningCycleError);
                algorithmCycle++;
            } while (!IsNetLearned());
        }

        protected abstract void LearnBatch(INet net, double currentLearningCycleError);
        protected abstract void LearnPattern(INet net, ILearningPattern learningPattern, double currentPatternError);
        protected abstract void PrepareToLearning(INet net);
        protected abstract bool IsNetLearned();
        protected virtual double CalculatePatternError(INet net, List<IFuzzyNumber> input, List<IFuzzyNumber> output)
        {
            return GetPatternError(net, input, output);
        }

        public event StepPerformedEventHandler StepPerformed;
        public event CyclePerformedEventHandler CyclePerformed;

        private void OnStepPerformed(int cycle, int step, double stepError)
        {
            if (StepPerformed != null)
                StepPerformed(new StepState(cycle, step, stepError));
        }

        private void OnCyclePerformed(int cycle, double cycleError)
        {
            if (CyclePerformed != null)
                CyclePerformed(new StepState(cycle, 0, cycleError));
        }

        protected static double GetPatternError(INet net, List<IFuzzyNumber> input, List<IFuzzyNumber> output)
        {
            var actual = net.Propagate(input);
            var expected = output;

            var errors = new List<IFuzzyNumber>();
            var i = 0;
            foreach (var actualNumber in actual)
            {
                errors.Add(actualNumber.Sub(expected.ElementAt(i)));
                i++;
            }

            var patternError = 0.0;
            foreach (var errorNumber in errors)
            {
                var leftError = 0.0;
                var rightError = 0.0;
                errorNumber.ForeachLevel((alpha, level) =>
                {
                    leftError += alpha * (level.X * level.X);
                    rightError += alpha * (level.Y * level.Y);
                });

                var currentOutputError = leftError + rightError;
                patternError += currentOutputError;
            }
            
            return patternError / 2.0;
        }

        protected static IVector CreateWeightsVector(List<ILayer> layers)
        {
            var result = new List<IFuzzyNumber>();

            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) => neuron.ForeachWeight((j, weight) => result.Add(weight.Signal)));
            var hiddenLayers = layers.Take(layers.Count - 1);
            foreach (var hiddenLayer in hiddenLayers.Reverse())
            {
                hiddenLayer.ForeachNeuron(
                    (i, neuron) => neuron.ForeachWeight((j, weight) => result.Add(weight.Signal)));
            }

            return new Vector(result.ToArray());
        }

        protected static void SetWeights(IVector weights, List<ILayer> layers)
        {
            var queue = weights.ToQueue();

            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) => neuron.ForeachWeight((j, weight) => weight.Signal = queue.Dequeue()));
            var hiddenLayers = layers.Take(layers.Count - 1);
            foreach (var hiddenLayer in hiddenLayers.Reverse())
            {
                hiddenLayer.ForeachNeuron(
                    (i, neuron) => neuron.ForeachWeight((j, weight) => weight.Signal = queue.Dequeue()));
            }
        }

        protected static List<ILink> CreateOutputsDeltas(List<ILayer> layers)
        {
            var outputs = new List<ILink>();
            
            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) => neuron.ForeachWeight((j, weight) => outputs.Add(neuron.GetLastInput(j))));
            var hiddenLayers = layers.Take(layers.Count - 1);
            foreach (var hiddenLayer in hiddenLayers.Reverse())
            {
                hiddenLayer.ForeachNeuron(
                    (i, neuron) => neuron.ForeachWeight((j, weight) => outputs.Add(neuron.GetLastInput(j))));
            }

            return outputs;
        }
    }
}