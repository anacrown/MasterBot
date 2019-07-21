using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Point = CodenjoyBot.Board.Point;

namespace PaperIO_MiniCupsAI
{
    public class Matrix<T> : IEnumerable<T>, IEnumerable where T : new()
    {
        public Size Size { get; }

        public T[] Cells { get; set; }

        public Matrix(Size size)
        {
            Size = size;
            Cells = new T[size.Width * size.Height];
            for (int index = 0; index < Cells.Length; ++index)
                Cells[index] = new T();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MatrixEnumerator<T>(Cells);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int i, int j]
        {
            get
            {
                return Cells[i + j * Size.Width];
            }
            set
            {
                Cells[i + j * Size.Width] = value;
            }
        }

        public T this[Point p]
        {
            get
            {
                return this[p.X, p.Y];
            }
        }

        public class MatrixEnumerator<TU> : IEnumerator<TU>, IDisposable, IEnumerator
        {
            private readonly IEnumerator _enumerator;

            public MatrixEnumerator(IEnumerable<TU> cells)
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