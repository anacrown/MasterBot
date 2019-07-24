using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CodenjoyBot.Board;
using PaperIO_MiniCupsAI.DataContract;
using Point = CodenjoyBot.Board.Point;

namespace PaperIO_MiniCupsAI
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

            Map = new Map(new Size(jPacket.Params.XCellsCount, jPacket.Params.YCellsCount));
        }
    }
}