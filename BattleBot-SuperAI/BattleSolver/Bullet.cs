using System.Collections.Generic;
using System.Linq;
using CodenjoyBot.Board;

namespace BattleBot_SuperAI.BattleSolver
{
    public class Bullet
    {
        public BattleCell BattleCell { get; }

        private readonly Dictionary<Direction, BattleCell> _possibleOwners =
            new Dictionary<Direction, BattleCell>();

        public IEnumerable<Direction> PossibleDirections => _possibleOwners.Keys;
        public Direction Direction => _possibleOwners.Count == 1 ? _possibleOwners.First().Key : Direction.Unknown;

        public IEnumerable<BattleCell> PossibleOwners => _possibleOwners.Values;
        public BattleCell Owner => _possibleOwners.Count == 1 ? _possibleOwners.First().Value : null;

        public bool IsPlayer => Owner?.MetaType == BattleCell.CellMetaType.TANK;

        public bool IsPossiblePlayer => PossibleOwners?.FirstOrDefault(t => t?.MetaType == BattleCell.CellMetaType.TANK) != null;

        public BattleCell GetPossibleOwnerFrom(Direction direction) =>
            _possibleOwners.ContainsKey(direction) ? _possibleOwners[direction] : null;

        public IEnumerable<Point> GetAffectedArea()
        {
            yield return BattleCell.Pos;

            foreach (var possibleDirection in PossibleDirections)
            {
                foreach (var point in GetAffectedArea(possibleDirection))
                {
                    yield return point;
                }
            }
        }

        private IEnumerable<Point> GetAffectedArea(Direction direction) =>
            BattleCell.GetLine(direction, 2, false).Select(t => t.Pos);

        public Bullet(BattleCell cell)
        {
            BattleCell = cell;

            Check(Direction.Left, Direction.Right);
            Check(Direction.Right, Direction.Left);
            Check(Direction.Up, Direction.Down);
            Check(Direction.Down, Direction.Up);
        }

        private void Check(Direction directionFrom, Direction directionTo)
        {
            var from1 = BattleCell.Pos[directionFrom];
            if (!from1.OnBoard(BattleCell.Board.Size)) return;

            var from1Cell = BattleCell.Board[from1];
            if (from1Cell.IsTank)
            {
                if (from1Cell.Direction == directionTo)
                    _possibleOwners.Add(directionTo, from1Cell);
            }

            if (from1Cell.MetaType != BattleCell.CellMetaType.GROUND && from1Cell.MetaType != BattleCell.CellMetaType.BULLET) return;

            var from2 = from1[directionFrom];
            if (!from2.OnBoard(BattleCell.Board.Size)) return;

            var from2Cell = BattleCell.Board[from2];
            if (from2Cell.IsTank)
            {
                if (from2Cell.Direction == directionTo)
                    _possibleOwners.Add(directionTo, from2Cell);
            }
            else
            {
                var lastBoard = BattleCell.Board.FrameBuffer[BattleCell.Board.Time - 1]?.Board;
                if (lastBoard == null) return;

                var lastFrom2Cell = lastBoard[from2];

                if (lastFrom2Cell.MetaType != BattleCell.CellMetaType.BULLET) return;

                var lastBullet = lastBoard.Bullets.First(t => t.BattleCell.Pos == from2);

                _possibleOwners.Add(directionTo, lastBullet.GetPossibleOwnerFrom(directionTo));
            }

        }
    }
}