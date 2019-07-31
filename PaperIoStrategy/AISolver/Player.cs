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

        public Map Map { get; set; }

        public Player(string name, JPacket jPacket)
        {
            Name = name;

            JPlayer = jPacket.Params.Players[name];

            Line = JPlayer.Lines.Select(point => point.ToGrid(jPacket.Params.Width));

            Territory = JPlayer.Territory.Select(point => point.ToGrid(jPacket.Params.Width));

            Position = JPlayer.Position.ToGrid(jPacket.Params.Width, Direction);

            Bonuses = JPlayer.Bonuses.Select(jb => new Bonus(jb));
        }
    }
}