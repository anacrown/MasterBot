using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodenjoyBot.DataProvider;

namespace CodenjoyBot.Board
{
    public class Board : IEnumerable<Cell>
    {
        private readonly Cell[] _cells;
        
        private readonly DateTime _startTime;
        private readonly string _instanceName;

        public int Size { get; }
        public DataFrame Frame { get; }

        public class BoardEnumerator : IEnumerator<Cell>
        {
            private readonly IEnumerator _enumerator;

            public BoardEnumerator(IEnumerable<Cell> cells) => _enumerator = cells.GetEnumerator();

            public void Dispose() { }

            public bool MoveNext() => _enumerator.MoveNext();

            public void Reset() => _enumerator.Reset();

            public Cell Current => (Cell)_enumerator.Current;

            object IEnumerator.Current => Current;
        }

        public Board(string instanceName, DateTime startTime, DataFrame frame)
        {
            _instanceName = instanceName;
            _startTime = startTime;
            Frame = frame;
//            Size = (int)Math.Sqrt(Frame.Board.Length);
//            _cells = Frame.Board.Select((t, i) => new Cell(t, new Point(i % Size, i / Size), this)).ToArray();
        }

        public IEnumerator<Cell> GetEnumerator() => new BoardEnumerator(_cells);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Cell this[int i, int j] => _cells[i + j * Size];
        public Cell this[Point p] => this[p.X, p.Y];

        public override string ToString() => Frame.Board;
    }
}
