using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodenjoyBot.Board;
using PaperIO_MiniCupsAI.ActionSolvers.Interfaces;

namespace PaperIO_MiniCupsAI.ActionSolvers
{
    public class DefenseChecker : IActionComponentBase
    {
        public bool CanIGoTo(Board board, Direction direction)
        {
            var nextPoint = board.IPlayer.Position[direction];
            if (!nextPoint.OnBoard(board.Size) || board[nextPoint].Element == Element.ME_LINE)
                return false;
            if (board[nextPoint].Element == Element.ME_TERRITORY)
                return true;
            return BExistReversPath(board, nextPoint);
        }

        private bool BExistReversPath(Board board, Point point)
        {
            var checkedPoints = new List<Point> { board.IPlayer.Position };
            checkedPoints.AddRange(board.IPlayer.Line);

            var map = new Map(board.Size, checkedPoints.ToArray());
            map.Check(point);

            var entries = board.IPlayer.Territory.Select(p => map[p]).OrderBy(e => e.Weight);

            Point[] path = null;
            foreach (var entry in entries)
                if ((path = map.Tracert(entry.Position).Reverse().ToArray()).Length > 0)
                    break;

            if (path == null || path.Length == 0)
                return false;

            if (checkedPoints.Select(p => board.EnemiesMap(p)).Min() - 1 <= entries.First().Weight)
                return false;

            var eMove = path.Select(p => board.EnemiesMap(p)).Min() - 1;

            for (var i = 0; i < path.Length; i++)
            {
                var iMove = 1 + map[path[i]].Weight;
                if (iMove >= eMove) return false;
            }

            return true;
        }
    }
}
