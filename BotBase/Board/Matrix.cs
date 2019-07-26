using System.Collections;
using System.Collections.Generic;

namespace BotBase.Board
{
    public class Matrix<T> : IEnumerable<T> where T : new()
    {
        public Size Size { get; }

        public T[] Cells { get; set; }

        public Matrix(Size size)
        {
            Size = size;
            Cells = new T[size.Width * size.Height];
            for (var index = 0; index < Cells.Length; ++index)
                Cells[index] = new T();
        }

        public IEnumerator<T> GetEnumerator() => new MatrixEnumerator<T>(Cells);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[int i, int j]
        {
            get => Cells[i + j * Size.Width];
            set => Cells[i + j * Size.Width] = value;
        }

        public T this[Point p] => this[p.X, p.Y];

        public class MatrixEnumerator<TU> : IEnumerator<TU>
        {
            private readonly IEnumerator _enumerator;

            public MatrixEnumerator(IEnumerable<TU> cells) => _enumerator = cells.GetEnumerator();

            public void Dispose() { }

            public bool MoveNext() => _enumerator.MoveNext();

            public void Reset() => _enumerator.Reset();

            public TU Current => (TU)_enumerator.Current;

            object IEnumerator.Current => Current;
        }
    }
}