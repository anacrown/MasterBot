using System.Collections.Generic;
using System.Linq;
using BotBase.Board;
using PaperIoStrategy.DataContract;
using Point = BotBase.Board.Point;

namespace PaperIoStrategy.AISolver
{
    public class Player
    {
        public Board Board { get; }
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

        public Map2 LineMap { get; set; }

        public Border Border { get; set; }

        public Dictionary<Direction, Map> PossibleMaps { get; set; } = new Dictionary<Direction, Map>();

        public Player(Board board, string name)
        {
            Board = board;
            Name = name;

            JPlayer = Board.JPacket.Params.Players[name];

            Line = JPlayer.Lines.Select(point => point.ToGrid(Board.JPacket.Params.Width));

            Territory = JPlayer.Territory.Select(point => point.ToGrid(Board.JPacket.Params.Width)).ToArray();

            Position = JPlayer.Position.ToGrid(Board.JPacket.Params.Width, Direction);

            Bonuses = JPlayer.Bonuses.Select(jb => new Bonus(jb)).ToArray();

            Speed = GetSpeed();

            IsCenterCell = (JPlayer.Position - Board.JPacket.Params.Width / 2) % Board.JPacket.Params.Width == 0;

            foreach (var bonus in Bonuses)
            {
                var desk = GetShift();

                var rest = Direction == Direction.Up || Direction == Direction.Right ? desk : Board.JPacket.Params.Width - desk;

                bonus.Pixels = (bonus.Moves * Board.JPacket.Params.Width - rest % Board.JPacket.Params.Width);
            }

//            var xs = Territory.Select(t => t.X).ToArray();
//            var ys = Territory.Select(t => t.Y).ToArray();
//            BBox = new BBox(Board, Territory, new Point(xs.Min(), ys.Min()), new Point(xs.Max(), ys.Max()));

            Border = new Border(Board, Territory);
        }

        public int GetSpeed() => Board.GetSpeed(Board.JPacket.Params.Speed, Board.JPacket.Params.Width, Bonuses.Select(t => t.BonusType).ToArray());

        public int GetShift() => (int)((JPlayer.Position - Board.JPacket.Params.Width / 2) % Board.JPacket.Params.Width).Abs();

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
                        Speed = Board.GetSpeed(Board.JPacket.Params.Speed, Board.JPacket.Params.Width, bonuses.Skip(i).Select(b => b.BonusType).ToArray()),
                        Pixels = bonus.Pixels - pixels
                    });

                    pixels += bonus.Pixels;
                }
            }

            snapshots.Add(new SpeedSnapshot()
            {
                Speed = Board.JPacket.Params.Speed,
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
            var S = path.Length * Board.JPacket.Params.Width - (int)(JPlayer.Position - path.First().FromGrid(Board.JPacket.Params.Width)).Abs();
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