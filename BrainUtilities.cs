﻿using MlApiNet.Snake;

namespace ImplementationExamples
{
    internal static class BrainUtilities
    {
        public static bool CheckColumnIncrementBlocked(Game.GameState gameState)
        {
            if (
                    gameState.Head.Column + 1 == Game.BoardSize
                    || gameState.BoardMatrix[gameState.Head.Row, gameState.Head.Column + 1] == true
                    )
            {
                return true;
            }
            return false;
        }

        public static bool CheckColumnDecrementBlocked(Game.GameState gameState)
        {
            if (
                    gameState.Head.Column - 1 < 0
                    || gameState.BoardMatrix[gameState.Head.Row, gameState.Head.Column - 1] == true
                    )
            {
                return true;
            }
            return false;
        }

        public static bool CheckRowIncrementBlocked(Game.GameState gameState)
        {
            if (
                    gameState.Head.Row + 1 == Game.BoardSize
                    || gameState.BoardMatrix[gameState.Head.Row + 1, gameState.Head.Column] == true
                    )
            {
                return true;
            }
            return false;
        }

        public static bool CheckRowDecrementBlocked(Game.GameState gameState)
        {
            if (
                gameState.Head.Row - 1 < 0
                || gameState.BoardMatrix[gameState.Head.Row - 1, gameState.Head.Column] == true
                )
            {
                return true;
            }
            return false;
        }

        public static bool CheckInFrontOfHeadBlocked(Game.GameState gameState)
        {
            if (
                gameState.Head.Row + gameState.HeadDirectionNESW.Row == Game.BoardSize
                || gameState.Head.Row + gameState.HeadDirectionNESW.Row < 0
                || (gameState.BoardMatrix[gameState.Head.Row + gameState.HeadDirectionNESW.Row, gameState.Head.Column] == true
                && gameState.HeadDirectionNESW.Row != 0)
                || gameState.Head.Column + gameState.HeadDirectionNESW.Column == Game.BoardSize
                || gameState.Head.Column + gameState.HeadDirectionNESW.Column < 0
                || (gameState.BoardMatrix[gameState.Head.Row, gameState.Head.Column + gameState.HeadDirectionNESW.Column] == true
                && gameState.HeadDirectionNESW.Column != 0)
                )
            {
                return true;
            }
            return false;
        }

        public static bool CheckToRightOfHeadBlocked(Game.GameState gameState)
        {
            if (gameState.HeadDirectionNESW.Row == 1)
            {
                return CheckColumnIncrementBlocked(gameState);
            }
            else if (gameState.HeadDirectionNESW.Row == -1)
            {
                return CheckColumnDecrementBlocked(gameState);
            }
            else if (gameState.HeadDirectionNESW.Column == 1)
            {
                return CheckRowDecrementBlocked(gameState);
            }
            return CheckRowIncrementBlocked(gameState);
        }

        public static bool CheckToLeftOfHeadBlocked(Game.GameState gameState)
        {
            if (gameState.HeadDirectionNESW.Row == 1)
            {
                return CheckColumnDecrementBlocked(gameState);
            }
            else if (gameState.HeadDirectionNESW.Row == -1)
            {
                return CheckColumnIncrementBlocked(gameState);
            }
            else if (gameState.HeadDirectionNESW.Column == 1)
            {
                return CheckRowIncrementBlocked(gameState);
            }
            return CheckRowDecrementBlocked(gameState);
        }
    }
}
