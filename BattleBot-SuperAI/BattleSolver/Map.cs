using System.Collections.Generic;
using System.Linq;
using CodenjoyBot.Board;

namespace BattleBot_SuperAI.BattleSolver
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
            var list = new List<Point>() { Start };

            var step = 0;
            while (list.Count > 0)
            {
                foreach (var point in list)
                {
                    if (_weights[point.X, point.Y] < 0) continue;

                    if (_weights[point.X, point.Y] == 0)
                    {
                        //MainWindow.Log(0, $"map {point} = {step}");
                        _map[point.X, point.Y] = step;
                        foreach (var neighbor in point.GetCrossVicinity(Size))
                        {
                            if (_weights[neighbor.X, neighbor.Y] >= 0 && !list.Contains(neighbor) && !add.Contains(neighbor))
                                add.Enqueue(neighbor);
                        }

                        remove.Enqueue(point);
                    }

                    _weights[point.X, point.Y]--;
                    //MainWindow.Log(0, $"weights {point} = {_weights[point.X, point.Y]}");
                }

                while (remove.Count > 0)
                {
                    //MainWindow.Log(0, $"remove {remove.Peek()}");
                    list.Remove(remove.Dequeue());
                }

                while (add.Count > 0)
                {
                    //MainWindow.Log(0, $"add {add.Peek()}");
                    list.Add(add.Dequeue());
                }

                step++;
                //Thread.Sleep(2000);
            }
        }

        public BattleCell[] Path(BattleCell cell)
        {
            var current = cell;
            var path = new List<BattleCell> { current };

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