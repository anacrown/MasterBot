﻿using System.Collections.Generic;
using System.Linq;
using BotBase.Board;

namespace BattleBot_SimpleAI.BattleSolver
{
    public static class BoardExtention
    {
        public static IEnumerable<Cell> GetEnemies(this Board<Cell> board) => board.Where(t =>
            CellExtention.GetElement(t) == Element.PLAYER_TANK_DOWN ||
            CellExtention.GetElement(t) == Element.PLAYER_TANK_LEFT ||
            CellExtention.GetElement(t) == Element.PLAYER_TANK_RIGHT ||
            CellExtention.GetElement(t) == Element.PLAYER_TANK_UP ||
            CellExtention.GetElement(t) == Element.OTHER_TANK_DOWN ||
            CellExtention.GetElement(t) == Element.OTHER_TANK_LEFT ||
            CellExtention.GetElement(t) == Element.OTHER_TANK_RIGHT ||
            CellExtention.GetElement(t) == Element.OTHER_TANK_UP);

        public static Cell GetMe(this Board<Cell> board) => board.FirstOrDefault(t =>
            t.GetElement() == Element.TANK_UP ||
            t.GetElement() == Element.TANK_DOWN ||
            t.GetElement() == Element.TANK_LEFT ||
            t.GetElement() == Element.TANK_RIGHT);

        public static bool IsGameOver(this Board<Cell> board) => board.Count(t =>
                                                               t.GetElement() == Element.TANK_UP ||
                                                               t.GetElement() == Element.TANK_DOWN ||
                                                               t.GetElement() == Element.TANK_LEFT ||
                                                               t.GetElement() == Element.TANK_RIGHT) == 0;

        public static int[,] GetWeights(this Board<Cell> board)
        {
            var weights = new int[board.Size.Width, board.Size.Height];
            foreach (var cell in board)
                weights[cell.X, cell.Y] = cell.GetStrength();
            return weights;
        }
    }
}