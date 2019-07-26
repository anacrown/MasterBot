using BotBase.Board;

namespace BattleBot_SuperAI.BattleSolver
{
    public class CrossCell
    {
        public BattleCell Cell { get; }
        public BattleCell Centre { get; }
        public int BulletTime { get; }

        public Point Pos => Cell.Pos;
        public int X => Pos.X;
        public int Y => Pos.Y;

        public CrossCell(BattleCell centre, BattleCell cell, int bulletTime)
        {
            Cell = cell;
            Centre = centre;
            BulletTime = bulletTime;
        }
    }
}