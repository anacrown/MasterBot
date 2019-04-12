using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodenjoyBot.Board;

namespace BattleBot_SuperAI.BattleSolver
{
    public class BattleBoard : IEnumerable<BattleCell>
    {
        private readonly Board _board;
        public FrameBuffer<BattleBoard> FrameBuffer { get; }

        private readonly BattleCell[] _cells;

        public int Size => _board.Size;

        public BattleCell Player { get; }

        public Enemy.TargetEnemy Target { get; set; }
        public BattleCell[] PathToTarget { get; set; }

        public Map PlayerMap { get; }

        public ABetterMap ABetterMap { get; }

        public Enemy[] Enemies { get; }
        public Bullet[] Bullets { get; }
        public uint Time => _board.Frame.Time;

        public BattleBoard(Board board, FrameBuffer<BattleBoard> _frameBuffer)
        {
            _board = board;
            _cells = _board.Select(t => new BattleCell(t, this)).ToArray();

            FrameBuffer = _frameBuffer;
            Player = _cells.FirstOrDefault(t => t.MetaType == BattleCell.CellMetaType.TANK);
            if (Player == null)
            {
                //WebSocketDataLogger.Instance.LogDead(instanceName, startTime, time);
                Player = _frameBuffer[board.Frame.Time - 1]?.Board.Player;
            }

            var weights = GetWeights();
            // именно в таком порядке, Map поменяет массив
            ABetterMap = new ABetterMap(weights, Size);
            PlayerMap = new Map(Player.Pos, weights, Size);

            Enemies = this.Where(t => t.IsEnemy).Select(t => new Enemy(t)).ToArray();

            Bullets = _cells.Where(t => t.MetaType == BattleCell.CellMetaType.BULLET).Select(t => new Bullet(t)).ToArray();
        }

        public BattleCell this[int i, int j] => _cells[i + j * Size];
        public BattleCell this[Point p] => this[p.X, p.Y];

        private int[,] GetWeights()
        {
            var weights = new int[Size, Size];
            foreach (var cell in this)
                weights[cell.X, cell.Y] = cell.Strength;

            return weights;
        }

        public class BoardEnumerator : IEnumerator<BattleCell>
        {
            private readonly IEnumerator _enumerator;

            public BoardEnumerator(IEnumerable<BattleCell> cells)
            {
                _enumerator = cells.GetEnumerator();
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public BattleCell Current => (BattleCell)_enumerator.Current;

            object IEnumerator.Current => Current;
        }

        public IEnumerator<BattleCell> GetEnumerator() => new BoardEnumerator(_cells);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => _board.ToString();
    }
}