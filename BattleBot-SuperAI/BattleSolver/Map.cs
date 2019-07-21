using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CodenjoyBot.Board;

namespace BattleBot_SuperAI.BattleSolver
{
    public class Map
    {
        private readonly int[,] _map;
        private readonly int[,] _weights;

        public Size Size { get; }

        public CodenjoyBot.Board.Point Start { get; }

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

        public Map(CodenjoyBot.Board.Point start, int[,] weights, Size size)
        {
            this.Start = start;
            this.Size = size;
            this._weights = weights;
            this._map = new int[size.Width, size.Height];
            this.Dijkstra();
        }

        private void Dijkstra()
        {
            Queue<CodenjoyBot.Board.Point> pointQueue1 = new Queue<CodenjoyBot.Board.Point>();
            Queue<CodenjoyBot.Board.Point> pointQueue2 = new Queue<CodenjoyBot.Board.Point>();
            List<CodenjoyBot.Board.Point> pointList = new List<CodenjoyBot.Board.Point>()
      {
        this.Start
      };
            int num = 0;
            while (pointList.Count > 0)
            {
                foreach (CodenjoyBot.Board.Point point1 in pointList)
                {
                    if (this._weights[point1.X, point1.Y] >= 0)
                    {
                        if (this._weights[point1.X, point1.Y] == 0)
                        {
                            this._map[point1.X, point1.Y] = num;
                            foreach (CodenjoyBot.Board.Point point2 in point1.GetCrossVicinity(this.Size))
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