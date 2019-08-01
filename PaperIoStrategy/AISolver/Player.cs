using System;
using System.Collections.Generic;
using System.Linq;
using BotBase.Board;
using PaperIoStrategy.DataContract;
using Point = BotBase.Board.Point;

namespace PaperIoStrategy.AISolver
{
    public class Player
    {
        private readonly JPacket _jPacket;
        public string Name { get; }

        public Point Position { get; }

        public JPlayer JPlayer { get; }

        public Direction Direction => JPlayer.Direction;

        public int Speed { get; private set; }

        public int Score => JPlayer.Score;

        public IEnumerable<Point> Line { get; }

        public IEnumerable<Point> Territory { get; }

        public IEnumerable<Bonus> Bonuses { get; }

        public Map Map { get; set; }

        public Player(string name, JPacket jPacket)
        {
            Name = name;

            _jPacket = jPacket;

            JPlayer = jPacket.Params.Players[name];

            Line = JPlayer.Lines.Select(point => point.ToGrid(jPacket.Params.Width));

            Territory = JPlayer.Territory.Select(point => point.ToGrid(jPacket.Params.Width));

            Position = JPlayer.Position.ToGrid(jPacket.Params.Width, Direction);

            Bonuses = JPlayer.Bonuses.Select(jb => new Bonus(jb)).ToArray();

            Speed = GetSpeed(Bonuses.Select(b => b.BonusType).ToArray());

            foreach (var bonus in Bonuses)
            {
                var desk = (int)((JPlayer.Position - jPacket.Params.Width / 2) % jPacket.Params.Width).Abs();

                var rest = Direction == Direction.Up || Direction == Direction.Right ? desk : jPacket.Params.Width - desk;

                bonus.Pixels = (bonus.Moves * jPacket.Params.Width - rest % jPacket.Params.Width);
            }
        }

        private int GetSpeed(params JBonusType[] bonuses)
        {
            if (bonuses.Any(t => t == JBonusType.SlowDown) &&
                bonuses.Any(t => t == JBonusType.SpeedUp))
                return _jPacket.Params.Speed;

            if (bonuses.Any(t => t == JBonusType.SlowDown))
                return _jPacket.Params.Speed - 2;
            if (bonuses.Any(t => t == JBonusType.SpeedUp))
                return _jPacket.Params.Speed + 1;
            return _jPacket.Params.Speed;
        }

        public uint GetTimeForPoint(Point p)
        {
            var path = Map.Tracert(p);
            if (path == null || path.Length == 0) return 0;

            uint ticks = 0;
            var bonuses = Bonuses.ToArray();
            var S = path.Length * _jPacket.Params.Width - (int)(JPlayer.Position - path.First().FromGrid(_jPacket.Params.Width)).Abs();
            while (S > 0)
            {
                int s;
                var v = GetSpeed(bonuses.Where(b => b.Pixels > 0).Select(b => b.BonusType).ToArray());
                if (bonuses.Any())
                {
                    var bonus = bonuses.Min(b => b.Pixels).First(); // минимальный остаток пути с текушей скоростью
                    s = bonus.Pixels;
                }
                else
                {
                    s = S;
                }

                var t = s / v;

                S -= s;
                ticks += (uint)t;

                if (bonuses.Any())
                {
                    foreach (var b in bonuses)
                    {
                        if (b.Pixels > 0)
                            b.Pixels -= s;
                    }
                }
            }

            return ticks;
        }
    }
}