/*-----------------------------|
|Author     Ioan Steffan Thomas|
|Product Date        15/03/2024|
|-----------------------------*/
using System;
using MlApiNet.Snake;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MathNet.Numerics.Data.Text;
using ImplementationExamples;

namespace ImplementingExample
{
    public partial class Brains
    {
        private static Vector<double> Sigmoid(Vector<double> weightedInput)
        {
            return 1.0 / (1.0 + (-weightedInput).PointwiseExp());
        }

        public class NNBrain : IBrain, ITrainable, ILoadable
        {
            public int[] Sizes { get; set; }
            public int NumberOfLayers { get => Sizes.Length; }
            public Matrix<double>[] Weights { get; set; }
            public double Fitness = 0;

            public NNBrain()
            {
                Sizes = new int[]
                {
                    7,
                    28,
                    3
                };
                InitialiseWeights();
            }

            private void InitialiseWeights()
            {
                Weights = new Matrix<double>[NumberOfLayers - 1];
                Normal normal = new Normal();
                for (int layerIndex = 0; layerIndex < NumberOfLayers - 1; layerIndex++)
                {
                    int columns = Sizes[layerIndex];
                    int rows = Sizes[layerIndex + 1];
                    Weights[layerIndex] = Matrix<double>.Build.Random(rows, columns, normal);// / Math.Sqrt(columns)
                }
            }

            private Vector<double> FeedForward(Vector<double> inputVector)
            {
                for (int layerIndex = 0; layerIndex < NumberOfLayers - 1; layerIndex++)
                {
                    inputVector = Sigmoid(Weights[layerIndex] * inputVector);
                }
                return inputVector;
            }

            private static double[] GetInputVector(Game.GameState gameState)
            {
                return new double[]
                {
                    gameState.HeadDirectionNESW.Row,
                    gameState.HeadDirectionNESW.Column,
                    gameState.Apple.Row - gameState.Head.Row,
                    gameState.Apple.Column - gameState.Head.Column,
                    BrainUtilities.CheckInFrontOfHeadBlocked(gameState) ? 1.0 : 0.0,
                    BrainUtilities.CheckToRightOfHeadBlocked(gameState) ? 1.0 : 0.0,
                    BrainUtilities.CheckToLeftOfHeadBlocked(gameState) ? 1.0 : 0.0
                };

            }

            public int DecideNextMove(Game.GameState gameState)
            {
                switch (FeedForward(Vector<double>.Build.DenseOfArray(GetInputVector(gameState))).MaximumIndex())
                {
                    case 0:
                        return -1;
                    case 2:
                        return 1;
                    default:
                        return 0;
                }
            }

            private void AssignInstanceToThis(NNBrain brain)
            {
                Sizes = brain.Sizes;
                Weights = brain.Weights;
                Fitness = brain.Fitness;
            }

            public static double GetFitness(int moveLimit, NNBrain brain, int numberOfGames = 10)
            {
                double sum = 0;
                for (int i = 0; i < numberOfGames; i++)
                {
                    Game.GameState gameState = new Game(brain).RunGameForTraining(moveLimit);
                    sum += gameState.Score * (gameState.Score + 1) / 2;
                    if (gameState.MoveCount != moveLimit)
                    {
                        sum -= 0.01 * gameState.Score;
                    }
                }
                return sum / numberOfGames;
            }

            private static NNBrain[] GetRandomParentPair(NNBrain[] parents)
            {
                Random random = new Random();
                NNBrain[] parentPair = new NNBrain[2];
                List<NNBrain> parentList = new List<NNBrain>(parents);
                parentPair[0] = parentList[random.Next(0, parentList.Count)];
                parentList.Remove(parentPair[0]);
                parentPair[1] = parentList[random.Next(0, parentList.Count)];
                return parentPair;
            }

            // AKA CrossOver
            private static NNBrain Reproduce(NNBrain[] parentPair)
            {
                Random random = new Random();
                NNBrain child = new NNBrain();

                for (int weightLayerIndex = 0; weightLayerIndex < child.Weights.Length; weightLayerIndex++)
                {
                    for (int i = 0; i < child.Weights[weightLayerIndex].RowCount; i++)
                    {
                        for (int j = 0; j < child.Weights[weightLayerIndex].ColumnCount; j++)
                        {
                            child.Weights[weightLayerIndex][i, j] = parentPair[random.Next(0, 2)].Weights[weightLayerIndex][i, j];
                        }
                    }
                }
                return child;
            }

            private void Mutate()
            {
                Random random = new Random();
                Normal normal = new Normal();
                Matrix<double> weightsMatrix = Weights[random.Next(0, Weights.Length)];
                weightsMatrix[random.Next(0, weightsMatrix.RowCount), random.Next(0, weightsMatrix.ColumnCount)] = normal.Sample();// / Math.Sqrt(weightsMatrix.ColumnCount)
            }

            private static NNBrain[] Repopulate(NNBrain[] population, int numberOfParents, int mutationPercentageProbability, int maxMutationsPerIndividual = 1)
            {
                Random random = new Random();
                for (int i = numberOfParents; i < population.Length; i++)
                {
                    population[i] = Reproduce(GetRandomParentPair(population.Take(numberOfParents).ToArray()));
                    int mutationCount = 0;
                    while (mutationCount != maxMutationsPerIndividual && random.Next(0, 100) < mutationPercentageProbability)
                    {
                        population[i].Mutate();
                        mutationCount++;
                    }
                }
                return population;
            }

            private static NNBrain[] GetInitialPopulation(int populationSize)
            {
                NNBrain[] population = new NNBrain[populationSize];
                for (int i = 0; i < population.Length; i++)
                {
                    population[i] = new NNBrain();
                    //population[i].LoadToThis("Generation30000"); // stage 2
                }
                return population;
            }

            public void Train()
            {
                const int FitnessMajorThreshold = 30_000;
                const int NumberOfParents = 4;
                const int MoveMultiplierIncrement = 1; // stage 2: 3
                const int NumberOfGamesForFitness = 9;
                const int MutationProbability = 75;
                const int MaxMutationsPerIndividual = 150;
                const int PopulationSize = 64;
                NNBrain[] population;
                int fitnessScaling = 1; // stage 2: 43
                int fitnessMinorThreshold = 1; // stage 2: 946
                int moveMultiplier = 20; // stage 2: 28 => 35
                int generation = 0; // stage 2: 30_001
                int thresholdReachedCount = 0;

                population = GetInitialPopulation(PopulationSize);

                while (population[0].Fitness < FitnessMajorThreshold)
                {
                    if (population[0].Fitness >= fitnessMinorThreshold)
                    {
                        thresholdReachedCount++;
                    }

                    if (thresholdReachedCount == 50)
                    {
                        thresholdReachedCount = 0;
                        if (fitnessScaling % 5 == 0)
                        {
                            // for incremental saves based on fitness
                            population[0].Save($"Fitness{fitnessMinorThreshold}Generation{generation}");

                            moveMultiplier += MoveMultiplierIncrement;
                        }
                        fitnessScaling++;
                        fitnessMinorThreshold += fitnessScaling;
                    }

                    // maybe put this section into an AssessPopulationFitness?
                    for (int individual = 0; individual < population.Length; individual++)
                    {
                        population[individual].Fitness = GetFitness(moveMultiplier * fitnessScaling, population[individual], NumberOfGamesForFitness);
                    }
                    population = population.OrderByDescending(individual => individual.Fitness).ToArray();

                    population = Repopulate(population, NumberOfParents, MutationProbability, MaxMutationsPerIndividual);

                    // for incremental saves based on generation
                    if (generation % 5000 == 0)
                    {
                        population[0].Save($"Generation{generation}");
                    }

                    // used to keep track of training progress
                    if (generation % 250 == 0)
                    {
                        using (StreamWriter consoleLogger = new StreamWriter("ConsoleLogDelta.txt", true))
                        {
                            consoleLogger.WriteLine($"Generation: {generation, 9} | Fitness: {population[0].Fitness}");
                        }
                        Console.WriteLine($"[LOGGED] Generation: {generation, 9} | Fitness: {population[0].Fitness}");
                    }
                    generation++;
                }
                AssignInstanceToThis(population[0]);
            }

            // add to these implementations (for ILoadable) the number of layers as first entry to file in future trainings
            public void Save(string fileName)
            {
                using (StreamWriter streamWriter = new StreamWriter(Loader.FormatFileName(fileName, typeof(NNBrain), ".txt"), false))
                {
                    foreach (int layerSize in Sizes)
                    {
                        streamWriter.WriteLine(layerSize);
                    }
                    streamWriter.WriteLine((int)Fitness);
                }
                for (int i = 0; i < NumberOfLayers - 1; i++)
                {
                    using (StreamWriter streamWriterWeights = new StreamWriter($"WeightsLayer{i}" + Loader.FormatFileName(fileName, typeof(NNBrain), ".txt"), false))
                    {
                        DelimitedWriter.Write<double>(streamWriterWeights, Weights[i]);
                    }
                }
            }

            public void LoadToThis(string fileName)
            {
                using (StreamReader streamReader = new StreamReader(Loader.FormatFileName(fileName, typeof(NNBrain), ".txt")))
                {
                    for (int i = 0; i < Sizes.Length; i++)
                    {
                        Sizes[i] = Convert.ToInt32(streamReader.ReadLine());
                    }
                    Fitness = Convert.ToInt32(streamReader.ReadLine());
                }
                Weights = new Matrix<double>[NumberOfLayers - 1];
                for (int i = 0; i < NumberOfLayers - 1; i++)
                {
                    using (StreamReader streamReaderWeights = new StreamReader($"WeightsLayer{i}" + Loader.FormatFileName(fileName, typeof(NNBrain), ".txt")))
                    {
                        Weights[i] = DelimitedReader.Read<double>(streamReaderWeights);
                    }
                }
            }
        }
    }
}