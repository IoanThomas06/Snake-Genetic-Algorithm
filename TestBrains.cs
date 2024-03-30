/*-----------------------------|
|Author     Ioan Steffan Thomas|
|Product Date        28/03/2024|
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
                return Console.ReadKey().Key.ToString() switch
                {
                    "D" => 1,
                    "A" => -1,
                    _ => 0
                };
            }
        }
    }
}
