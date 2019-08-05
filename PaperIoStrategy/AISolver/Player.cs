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

        public bool IsCenterCell { get; }

        public JPlayer JPlayer { get; }

        public Direction Direction => JPlayer.Direction;

        public int Speed { get; private set; }

        public int Score => JPlayer.Score;

        public IEnumerable<Point> Line { get; }

        public IEnumerable<Point> Territory { get; }

        public IEnumerable<Bonus> Bonuses { get; }

        public Map Map { get; set; }

        public Dictionary<Direction, Map> PossibleMaps { get; set; } = new Dictionary<Direction, Map>();

        public Player(string name, JPacket jPacket)
        {
            Name = name;

            _jPacket = jPacket;

            JPlayer = jPacket.Params.Players[name];

            Line = JPlayer.Lines.Select(point => point.ToGrid(jPacket.Params.Width));

            Territory = JPlayer.Territory.Select(point => point.ToGrid(jPacket.Params.Width));

            Position = JPlayer.Position.ToGrid(jPacket.Params.Width, Direction);

            Bonuses = JPlayer.Bonuses.Select(jb => new Bonus(jb)).ToArray();

            Speed = GetSpeed();

            IsCenterCell = (JPlayer.Position - jPacket.Params.Width / 2) % jPacket.Params.Width == 0;

            foreach (var bonus in Bonuses)
            {
                var desk = GetShift();

                var rest = Direction == Direction.Up || Direction == Direction.Right ? desk : jPacket.Params.Width - desk;

                bonus.Pixels = (bonus.Moves * jPacket.Params.Width - rest % jPacket.Params.Width);
            }
        }

        public int GetSpeed() => Board.GetSpeed(_jPacket.Params.Speed, _jPacket.Params.Width, Bonuses.Select(t => t.BonusType).ToArray());

        public int GetShift() => (int)((JPlayer.Position - _jPacket.Params.Width / 2) % _jPacket.Params.Width).Abs();

        public SpeedSnapshot[] GetSpeedSnapshots()
        {
            var pixels = 0;
            var snapshots = new List<SpeedSnapshot>();
            if (Bonuses.Any())
            {
                var bonuses = Bonuses.Where(b => b.BonusType != JBonusType.Saw).OrderBy(b => b.Pixels).ToArray();
                for (var i = 0; i < bonuses.Length; i++)
                {
                    var bonus = bonuses[i];
                    snapshots.Add(new SpeedSnapshot()
                    {
                        Speed = Board.GetSpeed(_jPacket.Params.Speed, _jPacket.Params.Width, bonuses.Skip(i).Select(b => b.BonusType).ToArray()),
                        Pixels = bonus.Pixels - pixels
                    });

                    pixels += bonus.Pixels;
                }
            }

            snapshots.Add(new SpeedSnapshot()
            {
                Speed = _jPacket.Params.Speed,
                Pixels = int.MaxValue
            });

            return snapshots.ToArray();
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
                var v = GetSpeed();
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