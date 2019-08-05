using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BotBase.Board;

namespace PaperIoStrategy.AISolver
{
    public class Border : IEnumerable<Point>
    {
        private readonly Size _boardSize;
        private readonly Cell[] _territory;
        private IEnumerable<Point> _borderPoints;

        public Border(Size boardSize, Cell[] territory)
        {
            _boardSize = boardSize;
            _territory = territory;

            var startCell = _territory.Min(c => c.X).MinSingle(c => c.Y);


        }

        public IEnumerator<Point> GetEnumerator() => new BorderEnumerator<Point>(_borderPoints.ToArray());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class BorderEnumerator<TPoint> : IEnumerator<TPoint>
        {
            private int _current = 0;
            private readonly TPoint[] _points;

            public BorderEnumerator(TPoint[] points)
            {
                _points = points;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                _current = (_current + 1) % _points.Length;
                return true;
            }

            public void Reset() => _current = 0;

            public object Current => _points[_current];

            TPoint IEnumerator<TPoint>.Current => _points[_current];
        }
    }
}