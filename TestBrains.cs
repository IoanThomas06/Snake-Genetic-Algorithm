/*-----------------------------|
|Author     Ioan Steffan Thomas|
|Product Date        15/03/2024|
|-----------------------------*/
using MlApiNet.Snake;
using System;

namespace ImplementingExample
{
    public partial class Brains
    {
        public class UserInputBrain : IBrain
        {
            public int DecideNextMove(Game.GameState gameState)
            {
                ConsoleKeyInfo input = Console.ReadKey();
                if (input.Key.ToString() == "D")
                {
                    return 1;
                }
                else if (input.Key.ToString() == "A")
                {
                    return -1;
                }
                return 0;
            }
        }
    }
}
