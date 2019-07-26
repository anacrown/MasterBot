using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BotBase.Board;
using Point = BotBase.Board.Point;

namespace BattleBot_SuperAI.BattleSolver
{
    public class BattleBoard : IEnumerable<BattleCell>, IEnumerable
    {
        private readonly Board<Cell> _board;
        private readonly BattleCell[] _cells;

        public FrameBuffer<BattleBoard> FrameBuffer { get; }

        public int Size
        {
            get
            {
                return this._board.Size.Width;
            }
        }

        public BattleCell Player { get; }

        public Enemy.TargetEnemy Target { get; set; }

        public BattleCell[] PathToTarget { get; set; }

        public Map PlayerMap { get; }

        public ABetterMap ABetterMap { get; }

        public Enemy[] Enemies { get; }

        public Bullet[] Bullets { get; }

        public uint Time
        {
            get
            {
                return this._board.Frame.Time;
            }
        }

        public BattleBoard(Board<Cell> board, FrameBuffer<BattleBoard> _frameBuffer)
        {
            this._board = board;
            this._cells = this._board.Select<Cell, BattleCell>((Func<Cell, BattleCell>)(t => new BattleCell(t, this))).ToArray<BattleCell>();
            this.FrameBuffer = _frameBuffer;
            this.Player = ((IEnumerable<BattleCell>)this._cells).FirstOrDefault<BattleCell>((Func<BattleCell, bool>)(t => t.MetaType == BattleCell.CellMetaType.TANK));
            if (this.Player == null)
                this.Player = _frameBuffer[board.Frame.Time - 1U]?.Board.Player;
            int[,] weights = this.GetWeights();
            this.ABetterMap = new ABetterMap(weights, this.Size);
            this.PlayerMap = new Map(this.Player.Pos, weights, new Size(this.Size, this.Size));
            this.Enemies = this.Where<BattleCell>((Func<BattleCell, bool>)(t => t.IsEnemy)).Select<BattleCell, Enemy>((Func<BattleCell, Enemy>)(t => new Enemy(t))).ToArray<Enemy>();
            this.Bullets = ((IEnumerable<BattleCell>)this._cells).Where<BattleCell>((Func<BattleCell, bool>)(t => t.MetaType == BattleCell.CellMetaType.BULLET)).Select<BattleCell, Bullet>((Func<BattleCell, Bullet>)(t => new Bullet(t))).ToArray<Bullet>();
        }

        public BattleCell this[int i, int j]
        {
            get
            {
                return this._cells[i + j * this.Size];
            }
        }

        public BattleCell this[Point p]
        {
            get
            {
                return this[p.X, p.Y];
            }
        }

        private int[,] GetWeights()
        {
            int[,] numArray = new int[this.Size, this.Size];
            foreach (BattleCell battleCell in this)
                numArray[battleCell.X, battleCell.Y] = battleCell.Strength;
            return numArray;
        }

        public IEnumerator<BattleCell> GetEnumerator()
        {
            return (IEnumerator<BattleCell>)new BattleBoard.BoardEnumerator((IEnumerable<BattleCell>)this._cells);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        public override string ToString()
        {
            return this._board.ToString();
        }

        public class BoardEnumerator : IEnumerator<BattleCell>, IDisposable, IEnumerator
        {
            private readonly IEnumerator _enumerator;

            public BoardEnumerator(IEnumerable<BattleCell> cells)
            {
                this._enumerator = (IEnumerator)cells.GetEnumerator();
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return this._enumerator.MoveNext();
            }

            public void Reset()
            {
                this._enumerator.Reset();
            }

            public BattleCell Current
            {
                get
                {
                    return (BattleCell)this._enumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (object)this.Current;
                }
            }
        }
    }
}