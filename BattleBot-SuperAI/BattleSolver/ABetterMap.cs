using System.Linq;
using BotBase.Board;

namespace BattleBot_SuperAI.BattleSolver
{
    public class ABetterMap
    {
        private readonly int[,] _map;
        private readonly int[,] _weights;

        public int Size { get; }

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

        public ABetterMap(int[,] weights, int size)
        {
            Size = size;
            _weights = weights;
            _map = new int[size, size];

            Calc();
        }

        private void Calc()
        {
            for (var x = 0; x < Size; ++x)
            {
                for (var y = 0; y < Size; ++y)
                {
                    var index = new Point(x, y);
                    this[index] = _weights[x, y] == 0 ? index.GetCrossVicinity(new Size(Size, Size)).Count(t => _weights[t.X, t.Y] == 0) * 10 : 0;
                }
            }
        }
    }
}