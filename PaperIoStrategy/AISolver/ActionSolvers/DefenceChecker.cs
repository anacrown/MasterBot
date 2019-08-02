using System.Collections.Generic;
using System.Linq;
using BotBase.Board;
using PaperIoStrategy.AISolver.ActionSolvers.Interfaces;

namespace PaperIoStrategy.AISolver.ActionSolvers
{
    public class DefenseChecker : IActionComponentBase
    {
        public bool CanIGoTo(Board board, Direction direction)
        {
            var nextPoint = board.Player.Position[direction];
            if (!nextPoint.OnBoard(board.Size) || board[nextPoint].Element == Element.ME_LINE)
                return false;
            if (board[nextPoint].Element == Element.ME_TERRITORY)
                return true;
            return BExistReversPath(board, nextPoint);
        }

        public IEnumerable<Direction> Order(Board board, IEnumerable<Direction> directions)
        {
            return directions.Where(d => CanIGoTo(board, d));
        }

        private bool BExistReversPath(Board board, Point point)
        {
            var checkedPoints = new List<Point> { board.Player.Position };
            checkedPoints.AddRange(board.Player.Line);

            var map = new Map(board.Size, checkedPoints.ToArray());
            map.Check(point);

//            var entries = board.IPlayer.Territory.Select(p => map[p]).OrderBy(e => e.Weight);
//
//            Point[] path = null;
//            foreach (var entry in entries)
//                if ((path = map.Tracert(entry.Position).Reverse().ToArray()).Length > 0)
//                    break;
//
//            if (path == null || path.Length == 0)
//                return false;
//
//            if (checkedPoints.Select(p => board.EnemiesMap(p)).Min() - 1 <= entries.First().Weight)
//                return false;
//
//            var eMove = path.Select(p => board.EnemiesMap(p)).Min() - 1;
//
//            for (var i = 0; i < path.Length; i++)
//            {
//                var iMove = 1 + map[path[i]].Weight;
//                if (iMove >= eMove) return false;
//            }
//
//            return true;

//            var path = board.GetPathToHome(map, checkedPoints, 1);
//            if (path == null) return false;

            return true;
        }
    }
}
