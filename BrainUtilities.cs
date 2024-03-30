/*-----------------------------|
|Author     Ioan Steffan Thomas|
|Product Date        28/03/2024|
|-----------------------------*/
using MlApiNet.Snake;

namespace ImplementationExamples
{
    internal static class BrainUtilities
    {
        private static bool CheckColumnIncrementBlocked(Game.GameState gameState)
        {
            return (
            gameState.Head.Column + 1 == Game.BoardSize
            || gameState.BoardMatrix[gameState.Head.Row, gameState.Head.Column + 1] == true
            );
        }

        private static bool CheckColumnDecrementBlocked(Game.GameState gameState)
        {
            return (
            gameState.Head.Column - 1 < 0
            || gameState.BoardMatrix[gameState.Head.Row, gameState.Head.Column - 1] == true
            );
        }

        private static bool CheckRowIncrementBlocked(Game.GameState gameState)
        {
            return (
            gameState.Head.Row + 1 == Game.BoardSize
            || gameState.BoardMatrix[gameState.Head.Row + 1, gameState.Head.Column] == true
            );
        }

        private static bool CheckRowDecrementBlocked(Game.GameState gameState)
        {
            return (
            gameState.Head.Row - 1 < 0
            || gameState.BoardMatrix[gameState.Head.Row - 1, gameState.Head.Column] == true
            );
        }

        public static bool CheckInFrontOfHeadBlocked(Game.GameState gameState)
        {
            if (gameState.HeadDirectionNESW.Row != 0)
            {
                return (
                gameState.Head.Row + gameState.HeadDirectionNESW.Row == Game.BoardSize
                || gameState.Head.Row + gameState.HeadDirectionNESW.Row < 0
                || gameState.BoardMatrix[gameState.Head.Row + gameState.HeadDirectionNESW.Row, gameState.Head.Column] == true
                );
            }
            //else only occurs when: gameState.HeadDirectionNESW.Column != 0
            return (
            gameState.Head.Column + gameState.HeadDirectionNESW.Column == Game.BoardSize
            || gameState.Head.Column + gameState.HeadDirectionNESW.Column < 0
            || gameState.BoardMatrix[gameState.Head.Row, gameState.Head.Column + gameState.HeadDirectionNESW.Column] == true
            );
        }

        public static bool CheckToRightOfHeadBlocked(Game.GameState gameState)
        {
            // default case only occurs for patterns: (0, -1)
            return (gameState.HeadDirectionNESW.Row, gameState.HeadDirectionNESW.Column) switch
            {
                (1, 0)  => CheckColumnIncrementBlocked(gameState),
                (-1, 0) => CheckColumnDecrementBlocked(gameState),
                (0, 1)  => CheckRowDecrementBlocked(gameState),
                _ => CheckRowIncrementBlocked(gameState)
            };
        }

        public static bool CheckToLeftOfHeadBlocked(Game.GameState gameState)
        {
            // default case only occurs for patterns: (0, -1)
            return (gameState.HeadDirectionNESW.Row, gameState.HeadDirectionNESW.Column) switch
            {
                (1, 0) => CheckColumnDecrementBlocked(gameState),
                (-1, 0) => CheckColumnIncrementBlocked(gameState),
                (0, 1) => CheckRowIncrementBlocked(gameState),
                _ => CheckRowDecrementBlocked(gameState)
            };
        }
    }
}
