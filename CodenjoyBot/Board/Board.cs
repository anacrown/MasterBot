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
            this._instanceName = instanceName;
            this._startTime = startTime;
            this.Frame = frame;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)new CodenjoyBot.Board.Board<T>.BoardEnumerator<T>((IEnumerable<T>)this.Cells);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        public T this[int i, int j]
        {
            get
            {
                return this.Cells[i + j * this.Size.Width];
            }
        }

        public T this[Point p]
        {
            get
            {
                return this[p.X, p.Y];
            }
        }

        public override string ToString()
        {
            return this.Frame.Board;
        }

        public class BoardEnumerator<TU> : IEnumerator<TU>, IDisposable, IEnumerator
        {
            private readonly IEnumerator _enumerator;

            public BoardEnumerator(IEnumerable<TU> cells)
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

            public TU Current
            {
                get
                {
                    return (TU)this._enumerator.Current;
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
