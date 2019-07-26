using System;
using System.Collections.Generic;
using System.Linq;
using BotBase.Board;

namespace BattleBot_SuperAI.BattleSolver
{
    public class Enemy
    {
        public class TargetEnemy
        {
            public Enemy Enemy;
            public Cross Cross;
            public BattleCell FirePosition;
        }

        public BattleCell Cell { get; }
        public Cross CurrentCross { get; }
        public Cross[] PerspectiveCrosses { get; }
        public TargetEnemy[] FirePositions { get; }

        public bool IsMove { get; private set; }

        public bool IsRespawn { get; private set; }
        public SolverCommand Command { get; }
        public bool IsDetectShot { get; }

        public Enemy(BattleCell cell)
        {
            Cell = cell;
            CurrentCross = new Cross(Cell, Cell.Board.Time);

            //отступлю 3 клетки от танка для определения огневых позиций(чтобы не стрелять из опасной зоны и не попадать в нее после выстрела)
            PerspectiveCrosses = PerspectivePositions().Select((t, i) => new Cross(t, (uint)i)).ToArray();

            Command = GetCommand();

            IsRespawn = GetIsRespawn();

            IsMove = IsRespawn || Command != SolverCommand.Empty;

            IsDetectShot = DetectShot();

            FirePositions = !IsMove
                ? FindFirePositionsForStaing().ToArray()
                : FindFirePositionsForMooving().ToArray();
        }

        private bool GetIsRespawn()
        {
            var lastBoard = Cell.Board.FrameBuffer[Cell.Board.Time - 1]?.Board;
            if (lastBoard == null)
                return true;

            return Command == SolverCommand.Empty && !lastBoard[Cell.Pos].IsTank;
        }

        //TODO: Переписать на Point[Direction] or Cell.GetLine(...)
        public IEnumerable<BattleCell> PerspectivePositions()
        {
            switch (Cell.Direction)
            {
                case Direction.Up:

                    for (var j = Cell.Y; j > 0; j--)
                    {
                        var cell = Cell.Board[Cell.X, j];
                        if (!cell.IsElapse && j != Cell.Y)
                            yield break;

                        yield return cell;
                    }

                    break;
                case Direction.Right:

                    for (var i = Cell.X; i < Cell.Board.Size; i++)
                    {
                        var cell = Cell.Board[i, Cell.Y];
                        if (!cell.IsElapse && i != Cell.X)
                            yield break;

                        yield return cell;
                    }

                    break;
                case Direction.Down:

                    for (var j = Cell.Y; j < Cell.Board.Size; j++)
                    {
                        var cell = Cell.Board[Cell.X, j];
                        if (!cell.IsElapse && j != Cell.Y)
                            yield break;

                        yield return cell;
                    }

                    break;
                case Direction.Left:

                    for (var i = Cell.X; i > 0; i--)
                    {
                        var cell = Cell.Board[i, Cell.Y];
                        if (!cell.IsElapse && i != Cell.X)
                            yield break;

                        yield return cell;
                    }

                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        public IEnumerable<Point> GetAffectedArea()
        {
            if (!IsMove) yield break;

            yield return Cell.Pos;

            foreach (var point in GetAffectedArea(Direction.Up))
                yield return point;

            foreach (var point in GetAffectedArea(Direction.Right))
                yield return point;

            foreach (var point in GetAffectedArea(Direction.Down))
                yield return point;

            foreach (var point in GetAffectedArea(Direction.Left))
                yield return point;
        }

        public bool IsMustDie => Cell.Board.Bullets.Any(bullet => bullet.PossibleDirections
            .SelectMany(direction => bullet.BattleCell.GetLine(direction)).Select(t => t.Pos)
            .Contains(Cell.Pos));

        private IEnumerable<Point> GetAffectedArea(Direction direction) => Cell.GetLine(direction, 2, false).Select(t => t.Pos);

        //предполагает что если перед танком кто-то умер(танк или сбита пуля) на расстоянии не более 2х клеток то это сделал ОН) 
        private bool DetectShot()
        {
            if (!IsMove) return false;

            var front1 = Cell.Pos[Cell.Direction];

            if (!front1.OnBoard(Cell.Board.Size))
                return false;

            if (Cell.Board[front1].MetaType == BattleCell.CellMetaType.BULLET)
                return true;

            if (Cell.Board.Time > 0 && (Cell.Board[front1].MetaType == BattleCell.CellMetaType.CONSTRUCTION || Cell.Board[front1].MetaType == BattleCell.CellMetaType.GROUND))
            {
                var lastBoard = Cell.Board.FrameBuffer[Cell.Board.Time - 1]?.Board;
                if (lastBoard != null && Cell.Board[front1].Strength == lastBoard[front1].Strength - 1)
                    return true;
            }

            var front2 = front1[Cell.Direction];

            if (!front2.OnBoard(Cell.Board.Size))
                return false;

            if (Cell.Board[front2].MetaType == BattleCell.CellMetaType.BULLET)
                return true;

            if (Cell.Board.Time > 0 && (Cell.Board[front2].MetaType == BattleCell.CellMetaType.CONSTRUCTION || Cell.Board[front2].MetaType == BattleCell.CellMetaType.GROUND))
            {
                var lastBoard = Cell.Board.FrameBuffer[Cell.Board.Time - 1]?.Board;
                if (lastBoard != null && Cell.Board[front2].Strength == lastBoard[front2].Strength - 1)
                    return true;
            }

            return false;
        }

        // предполагаю что если противник стрелял на прошлом ходу, то не сможет на следующем
        public bool CanShotInNextMove()
        {
            var last1Board = Cell.Board.FrameBuffer[Cell.Board.Time - 1]?.Board;
            var last2Board = Cell.Board.FrameBuffer[Cell.Board.Time - 2]?.Board;

            if (last1Board == null || last2Board == null || Command == SolverCommand.Empty)
                return true;

            var cell1 = last1Board[Cell.Pos[Command.ToDirection().Invert()]];
            if (!cell1.IsEnemy) cell1 = last1Board[Cell.Pos];
            if (!cell1.IsEnemy) throw new Exception();

            var enemy1 = last1Board.Enemies.First(enemy => enemy.Cell.Pos == cell1.Pos);

            if (enemy1.IsDetectShot)
            {
                return false;
            }

            if (enemy1.Command == SolverCommand.Empty)
                return true;

            var cell2 = last2Board[enemy1.Cell.Pos[enemy1.Command.ToDirection().Invert()]];
            if (!cell2.IsEnemy) cell2 = last2Board[enemy1.Cell.Pos];
            if (!cell2.IsEnemy) throw new Exception();

            var enemy2 = last2Board.Enemies.First(enemy => enemy.Cell.Pos == cell2.Pos);


            if (enemy2.IsDetectShot)
            {
                return false;
            }

            return true;
        }

        public SolverCommand GetCommand()
        {
            if (Cell.Board.Time == 0)
                return SolverCommand.Empty;

            var prevBoard = Cell.Board.FrameBuffer[Cell.Board.Time - 1];

            if (prevBoard?.Board[Cell.Pos[Cell.Direction.Invert()]].MetaType == Cell.MetaType && prevBoard?.Board[Cell.Pos].MetaType != Cell.MetaType)
                return Cell.Direction.GetCommand();


            if (prevBoard?.Board[Cell.Pos].MetaType == Cell.MetaType && prevBoard?.Board[Cell.Pos].Direction != Cell.Direction)
                return Cell.Direction.GetCommand();

            return SolverCommand.Empty;
        }

        private IEnumerable<TargetEnemy> FindFirePositionsForMooving()
        {
            foreach (var cross in PerspectiveCrosses)
            {
                foreach (var crossCell in cross.Cells)
                {
                    if (crossCell.BulletTime + Cell.Board.PlayerMap[crossCell.Pos] == cross.Time)
                        yield return new TargetEnemy { Enemy = this, Cross = cross, FirePosition = crossCell.Cell };
                }
            }
        }

        private IEnumerable<TargetEnemy> FindFirePositionsForStaing()
        {
            foreach (var crossCell in CurrentCross.Cells)
            {
                yield return new TargetEnemy { Enemy = this, Cross = CurrentCross, FirePosition = crossCell.Cell };
            }
        }
    }
}