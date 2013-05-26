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
            var learningCycleError = 0.0;
            do
            {
                learningCycleError = 0.0;
                var algorithmStep = 0;
                foreach (var learningPattern in _patterns)
                {
                    var patternError = CalculatePatternError(net, learningPattern);
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
                    if (!LearnBatch(net, learningCycleError))
                    {
                        Console.WriteLine("SWITCH TO SIMPLE");
                        var bp = new BackPropagation(_patterns, 0.3, ErrorThreshold);
                        bp.LearnNet(net);
                        bp.CyclePerformed +=
                            (state) =>
                                {
                                    //Console.ReadKey();
                                    if (state.Cycle%50 == 0)
                                    {
                                        //Console.ReadKey();
                                        Console.WriteLine("cycle: " + state.Cycle +
                                                          " error: " +
                                                          state.CycleError.ToString("0.#####################"));
                                    }
                                };
                    }
                }

                OnCyclePerformed(algorithmCycle, learningCycleError);
                algorithmCycle++;
            } while (!IsNetLearned(learningCycleError));
        }

        protected abstract bool LearnBatch(INet net, double currentLearningCycleError);
        protected abstract void LearnPattern(INet net, ILearningPattern learningPattern, double currentPatternError);
        protected abstract void PrepareToLearning(INet net);
        protected abstract bool IsNetLearned(double currentError);
        protected virtual double CalculatePatternError(INet net, ILearningPattern pattern)
        {
            return GetPatternError(net, pattern);
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

        protected static double GetPatternError(INet net, ILearningPattern pattern)
        {
            var actual = net.Propagate(pattern.Input);
            var expected = pattern.Output;

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

        private void AddLittleCorrectionToWeights(List<ILayer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers.ElementAt(i);
                layer.ForeachNeuron((j, neuron) => neuron.ForeachWeight((k, weight) =>
                {
                    //weight.Signal = weight.Signal.Sum(DiscreteFuzzyNumber.GenerateLittleNumber(levelsCount: 11));
                    weight.Signal = weight.Signal.Sum(RealNumber.GenerateLittleNumber());
                }));
            }
        }

        protected static void PropagateErrorOnLayers(List<ILayer> layers, List<IFuzzyNumber> patternsOutput)
        {
            var outputLayer = layers.Last();
            outputLayer.ForeachNeuron((i, neuron) =>
                {
                    var output = neuron.LastOutput; //Ok
                    var expectedOutput = patternsOutput.ElementAt(i); //tk
                    var error = output.Mul(output.Apply(levelValue => 1 - levelValue))
                                      .Mul(expectedOutput.Sub(output))
                                      .Mul(4.0); //Ok(1-Ok)(tk - Ok)*alpha
                    //neuron.PropagatedError = neuron.PropagatedError == null ? error : neuron.PropagatedError.Sum(error);
                    neuron.PropagatedError = error;
                });

            //from last to first
            for (int i = layers.Count - 2; i >= 0; i--)
            {
                var layer = layers.ElementAt(i);
                var nextLayer = layers.ElementAt(i + 1);

                layer.ForeachNeuron((neuronIndex, neuron) =>
                    {
                        var output = neuron.LastOutput; //Ok
                        var part = output.Mul(output.Apply(levelValue => 1 - levelValue)).Mul(4.0); //Ok(1 - Ok)*alpha
                        //var part = output.Mul(output.Apply(levelValue => 1 - levelValue)); //Ok(1 - Ok)
                        var sum = FuzzyNumberExtensions.Sum(0, nextLayer.NeuronsCount,
                                                            j => nextLayer.GetNeuron(j).GetWeight(neuronIndex).Signal
                                                                          .Mul(nextLayer.GetNeuron(j).PropagatedError));
                        //neuron.PropagatedError = neuron.PropagatedError == null
                        //                             ? part.Mul(sum)
                        //                             : neuron.PropagatedError.Sum(part.Mul(sum));
                        neuron.PropagatedError = part.Mul(sum);
                    });
            }
        }

        protected double GetBatchError(INet net)
        {
            return _patterns.Sum(learningPattern => CalculatePatternError(net, learningPattern));
        }
    }
}