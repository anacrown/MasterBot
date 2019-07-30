using System.Collections.Generic;
using System.Linq;
using BotBase.Board;
using PaperIoStrategy.DataContract;
using Point = BotBase.Board.Point;

namespace PaperIoStrategy.AISolver
{
    public class Player
    {
        public string Name { get; }

        public Point Position { get; }

        public JPlayer JPlayer { get; }

        public Direction Direction => JPlayer.Direction;

        public int Score => JPlayer.Score;

        public IEnumerable<Point> Line { get; }

        public IEnumerable<Point> Territory { get; }

        public IEnumerable<Bonus> Bonuses { get; }

        public Map Map { get; }

        public Player(string name, JPacket jPacket)
        {
            Name = name;

            JPlayer = jPacket.Params.Players[name];

            Line = JPlayer.Lines.Select(point => point / jPacket.Params.Width);

            Territory = JPlayer.Territory.Select(point => point / jPacket.Params.Width);

            Position = JPlayer.Position / jPacket.Params.Width;

            Bonuses = JPlayer.Bonuses.Select(jb => new Bonus(jb));

            var checkedPoints = Line.ToList();
            if (Direction != Direction.Unknown)
                checkedPoints.Add(Position[Direction.Invert()]);

            Map = new Map(new Size(jPacket.Params.XCellsCount, jPacket.Params.YCellsCount), checkedPoints.ToArray());
        }
    }
}