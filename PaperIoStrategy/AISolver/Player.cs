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

        public Matrix<uint> Times { get; set; }

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
            var path = Map.Tracert(p).ToArray();
            if (path == null || path.Length == 0) return 0;

            uint ticks = 0;
            var bonuses = Bonuses.Where(b => b.Pixels > 0).Select(b => (type: b.BonusType, pixels: b.Pixels)).ToArray();
            var S = (path.Length - 1) * _jPacket.Params.Width + (path.First().FromGrid(_jPacket.Params.Width) - JPlayer.Position).Abs();
            while (S > 0)
            {
                int s;
                var v = GetSpeed(bonuses.Select(b => b.type).ToArray());
                if (bonuses.Any())
                {
                    s = bonuses.Select(b => b.pixels).Min(); // минимальный остаток пути с текушей скоростью
                    s = Math.Min(S, s);
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
                    bonuses = bonuses.Where(b => b.pixels > 0).Select(b => (type: b.type, pixels: b.pixels - s)).ToArray();
                }
            }

            return ticks;
        }
    }
}