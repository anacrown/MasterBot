using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;
using PaperIO_MiniCupsAI.DataContract;
using Point = CodenjoyBot.Board.Point;

namespace PaperIO_MiniCupsAI
{
    public class Board : Board<Cell>
    {
        public JPacket JPacket { get; }

        public Dictionary<string, Player> Players { get; }

        public IEnumerable<Bonus> Bonuses { get; }

        public Player IPlayer => Players.ContainsKey("i") ? Players["i"] : null;

        public IEnumerable<Player> Enemies => Players?.Where(pair => pair.Key != "i").Select(pair => pair.Value);

        public Board(string instanceName, DateTime startTime, DataFrame frame, JPacket jPacket) : base(instanceName, startTime, frame)
        {
            JPacket = jPacket;

            Size = new Size(JPacket.Params.XCellsCount, JPacket.Params.YCellsCount);

            Cells = new Cell[Size.Width * Size.Height];
            for (var index = 0; index < Cells.Length; ++index)
                Cells[index] = new Cell(Point.Empty, this);

            for (var x = 0; x < Size.Width; ++x)
                for (var y = 0; y < Size.Height; ++y)
                    this[x, y].Pos = new Point(x, y);

            Bonuses = jPacket.Params.Bonuses?.Select(jb => new Bonus(jPacket, jb));

            Players = JPacket.Params.Players?.ToDictionary(jp => jp.Key, jp => new Player(jp.Key, JPacket));

            if (Players != null && Players.Count > 0)
            {
                foreach (var player in Enemies)
                {
                    foreach (var point in player.Territory) this[point].Element = Element.PLAYER_TERRITORY;
                    foreach (var point in player.Line) this[point].Element = Element.PLAYER_LINE;
                    this[player.Position].Element = Element.PLAYER;
                }

                foreach (var point in IPlayer.Territory) this[point].Element = Element.ME_TERRITORY;
                foreach (var point in IPlayer.Line) this[point].Element = Element.ME_LINE;
                this[IPlayer.Position].Element = Element.ME;

                Parallel.ForEach(Players.Values, player => { player.Map.Check(player.Position); });
            }
        }
    }
}