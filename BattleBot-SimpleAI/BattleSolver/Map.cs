using System.Collections.Generic;
using System.Linq;
using BotBase.Board;
using Point = BotBase.Board.Point;
using Size = BotBase.Board.Size;

namespace BattleBot_SimpleAI.BattleSolver
{
    public class Map
    {
        private readonly int[,] _map;
        private readonly int[,] _weights;

        public int Size { get; }
        public Point Start { get; }

        public int this[int i, int j]
        {
            get => _map[i, j];
            set => _map[i, j] = value;
        }

        public int this[Point p]
        {
            get => this[p.X, p.Y];
            set => this[p.X, p.Y] = value;
        }

        public Map(Point start, int[,] weights, int size)
        {
            Start = start;
            Size = size;

            _weights = weights;
            _map = new int[size, size];

            Dijkstra();
        }

        private void Dijkstra()
        {
            var add = new Queue<Point>();
            var remove = new Queue<Point>();
            var list = new List<Point> { Start };

            var step = 0;
            while (list.Count > 0)
            {
                foreach (var point in list)
                {
                    if (_weights[point.X, point.Y] < 0) continue;

                    if (_weights[point.X, point.Y] == 0)
                    {
                        _map[point.X, point.Y] = step;
                        foreach (var neighbor in point.GetCrossVicinity(new Size(Size, Size)))
                        {
                            if (_weights[neighbor.X, neighbor.Y] >= 0 && !list.Contains(neighbor) && !add.Contains(neighbor))
                                add.Enqueue(neighbor);
                        }

                        remove.Enqueue(point);
                    }

                    _weights[point.X, point.Y]--;
                }

                while (remove.Count > 0) list.Remove(remove.Dequeue());
                while (add.Count > 0) list.Add(add.Dequeue());

                step++;
            }
        }

        public CellBase[] Path(CellBase to)
        {
            var current = to;
            var path = new List<CellBase> { current };

            while (current != null && current.Pos != Start)
            {
                var min = current.GetCrossVicinity().Select(t => this[t.Pos]).Min();
                current = current.GetCrossVicinity().FirstOrDefault(t => this[t.Pos] == min);

                if (current?.Pos == Start) break;
                path.Add(current);
            }

            path.Reverse();

            return path.ToArray();
        }
    }
}