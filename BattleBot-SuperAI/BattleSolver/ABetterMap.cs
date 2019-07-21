using System;
using System.Linq;
using CodenjoyBot.Board;

namespace BattleBot_SuperAI.BattleSolver
{
    public class ABetterMap
    {
        private readonly int[,] _map;
        private readonly int[,] _weights;

        public int Size { get; }

        public int this[int i, int j]
        {
            get
            {
                return this._map[i, j];
            }
            set
            {
                this._map[i, j] = value;
            }
        }

        public int this[CodenjoyBot.Board.Point p]
        {
            get
            {
                return this[p.X, p.Y];
            }
            set
            {
                this[p.X, p.Y] = value;
            }
        }

        public ABetterMap(int[,] weights, int size)
        {
            this.Size = size;
            this._weights = weights;
            this._map = new int[size, size];
            this.Calc();
        }

        private void Calc()
        {
            for (int x = 0; x < this.Size; ++x)
            {
                for (int y = 0; y < this.Size; ++y)
                {
                    CodenjoyBot.Board.Point index = new CodenjoyBot.Board.Point(x, y);
                    this[index] = this._weights[x, y] == 0 ? index.GetCrossVicinity(new System.Drawing.Size(this.Size, this.Size)).Count<CodenjoyBot.Board.Point>((Func<CodenjoyBot.Board.Point, bool>)(t => this._weights[t.X, t.Y] == 0)) * 10 : 0;
                }
            }
        }
    }
}