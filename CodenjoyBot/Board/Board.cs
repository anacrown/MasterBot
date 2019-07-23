using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CodenjoyBot.DataProvider;

namespace CodenjoyBot.Board
{
    public class Board<T> : IEnumerable<T>, IEnumerable
    {
        private readonly DateTime _startTime;
        private readonly string _instanceName;

        public T[] Cells { get; set; }

        public Size Size { get; set; }

        public DataFrame Frame { get; }

        public Board(string instanceName, DateTime startTime, DataFrame frame)
        {
            _instanceName = instanceName;
            _startTime = startTime;
            Frame = frame;
        }

        public IEnumerator<T> GetEnumerator() => new BoardEnumerator<T>(Cells);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[int i, int j] => Cells[i + j * Size.Width];

        public T this[Point p] => this[p.X, p.Y];

        public override string ToString() => Frame.Board;

        public class BoardEnumerator<TU> : IEnumerator<TU>
        {
            private readonly IEnumerator _enumerator;

            public BoardEnumerator(IEnumerable<TU> cells) => _enumerator = cells.GetEnumerator();

            public void Dispose() { }
            
            public bool MoveNext() => _enumerator.MoveNext();

            public void Reset()
            {
                _enumerator.Reset();
            }

            public TU Current
            {
                get
                {
                    return (TU)_enumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }
        }
    }
}
