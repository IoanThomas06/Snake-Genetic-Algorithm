/*-----------------------------|
|Author     Ioan Steffan Thomas|
|Product Date        15/03/2024|
|-----------------------------*/
using System;
using System.IO;
using System.Linq;
using MlApiNet.Snake;
using static ImplementingExample.Brains;

namespace ImplementingExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // standard program loop example
            bool continueProgram = true;
            while (continueProgram == true)
            {
                Console.Clear();

                Game game = Game.Setup(typeof(Brains));
                game.RunGameForDisplay(1000, 0.25);

                Console.WriteLine("Enter 'c' or 'cont' to run another program;\nLeave blank or enter alternative text to exit program");
                if ((new string[] { "c", "cont" }).Contains(Console.ReadLine().ToLower()) == false)
                {
                    continueProgram = false;
                }
            }

            //// getting log as csv
            //string[] generationData = File.ReadAllLines("ConsoleLogDelta.txt");
            //int[,] integerData = new int[generationData.Length, 2];
            //for (int i = 0; i < generationData.Length; i++)
            //{
            //    string genData = generationData[i];
            //    generationData[i] = Convert.ToString((int)Convert.ToDouble(genData.Split('|')[0].Substring(11).Trim(' '))); // gen
            //    generationData[i] += "," + Convert.ToString((int)Convert.ToDouble(genData.Split('|')[1].Substring(9).Trim(' '))); // fitness
            //}
            //File.WriteAllLines("ConsoleLogDelta.csv", generationData);

            //// there can be only one
            //for (int count = 0; count < 5; count++)
            //{
            //    double highestFitness = -1.0;
            //    int highestFitnessIndex = -1;
            //    double secondHighestFitness = -1.0;
            //    int secondHighestFitnessIndex = -1;

            //    string[] fileNames = Directory.GetFiles(Directory.GetCurrentDirectory(), nameof(NNBrain) + "*").Select(fileName => Path.GetFileName(fileName).Substring(nameof(NNBrain).Length, Path.GetFileName(fileName).Length - nameof(NNBrain).Length).Split('.')[0]).ToArray();
            //    for (int i = 0; i < fileNames.Length; i++)
            //    {
            //        double fitness = 0.0;
            //        NNBrain brain = new NNBrain();
            //        brain.LoadToThis(fileNames[i]);
            //        fitness = NNBrain.GetFitness(10000, brain, 1000);
            //        if (fileNames[i] == "Kiddo")
            //        {

            //        }
            //        else if (fitness >= highestFitness)
            //        {
            //            secondHighestFitness = highestFitness;
            //            secondHighestFitnessIndex = highestFitnessIndex;
            //            highestFitness = fitness;
            //            highestFitnessIndex = i;
            //        }
            //        else if (fitness > secondHighestFitness)
            //        {
            //            secondHighestFitness = fitness;
            //            secondHighestFitnessIndex = i;
            //        }
            //    }
            //    Console.WriteLine($"{fileNames[highestFitnessIndex]} is the fittest brain, with an average fitness of {highestFitness}.");
            //    Console.WriteLine($"{fileNames[secondHighestFitnessIndex]} is the second fittest brain, with an average fitness of {secondHighestFitness}.");
            //}

            //// running the fittest ["Kiddo" == "Fitness4095Generation66773"]
            //Console.ReadLine();
            //while (true)
            //{
            //    NNBrain brain = new NNBrain();
            //    brain.LoadToThis("Kiddo");
            //    Game game = new Game(brain);
            //    game.RunGameForDisplay(10000, 0.01);
            //}
        }
    }
}