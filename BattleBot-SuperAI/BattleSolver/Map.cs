using System;
using System.Collections.Generic;
using System.Linq;
using Point = BotBase.Board.Point;
using Size = BotBase.Board.Size;

namespace BattleBot_SuperAI.BattleSolver
{
    public class Map
    {
        private readonly int[,] _map;
        private readonly int[,] _weights;

        public Size Size { get; }

        public Point Start { get; }

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

        public int this[Point p]
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

        public Map(Point start, int[,] weights, Size size)
        {
            this.Start = start;
            this.Size = size;
            this._weights = weights;
            this._map = new int[size.Width, size.Height];
            this.Dijkstra();
        }

        private void Dijkstra()
        {
            Queue<Point> pointQueue1 = new Queue<Point>();
            Queue<Point> pointQueue2 = new Queue<Point>();
            List<Point> pointList = new List<Point>()
      {
        this.Start
      };
            int num = 0;
            while (pointList.Count > 0)
            {
                foreach (Point point1 in pointList)
                {
                    if (this._weights[point1.X, point1.Y] >= 0)
                    {
                        if (this._weights[point1.X, point1.Y] == 0)
                        {
                            this._map[point1.X, point1.Y] = num;
                            foreach (Point point2 in point1.GetCrossVicinity(this.Size))
                            {
                                if (this._weights[point2.X, point2.Y] >= 0 && !pointList.Contains(point2) && !pointQueue1.Contains(point2))
                                    pointQueue1.Enqueue(point2);
                            }
                            pointQueue2.Enqueue(point1);
                        }
                        --this._weights[point1.X, point1.Y];
                    }
                }
                while (pointQueue2.Count > 0)
                    pointList.Remove(pointQueue2.Dequeue());
                while (pointQueue1.Count > 0)
                    pointList.Add(pointQueue1.Dequeue());
                ++num;
            }
        }

        public BattleCell[] Path(BattleCell cell)
        {
            BattleCell battleCell = cell;
            List<BattleCell> battleCellList = new List<BattleCell>()
      {
        battleCell
      };
            while (battleCell != null && battleCell.Pos != this.Start)
            {
                int min = ((IEnumerable<BattleCell>)battleCell.GetCrossVicinity()).Select<BattleCell, int>((Func<BattleCell, int>)(t => this[t.Pos])).Min();
                battleCell = ((IEnumerable<BattleCell>)battleCell.GetCrossVicinity()).FirstOrDefault<BattleCell>((Func<BattleCell, bool>)(t => this[t.Pos] == min));
                if (!(battleCell?.Pos == this.Start))
                    battleCellList.Add(battleCell);
                else
                    break;
            }
            battleCellList.Reverse();
            return battleCellList.ToArray();
        }
    }
}