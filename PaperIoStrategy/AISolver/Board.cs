using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading.Tasks;
using BotBase;
using BotBase.Board;
using PaperIoStrategy.DataContract;
using Point = BotBase.Board.Point;

namespace PaperIoStrategy.AISolver
{
    public class Board : Board<Cell>
    {
        public JPacket JPacket { get; }

        public Dictionary<string, Player> Players { get; }

        public IEnumerable<Bonus> Bonuses { get; }

        public Direction[] PossibleDirections { get; }

        public Player Player => Players != null && Players.ContainsKey("i") ? Players["i"] : null;

        public IEnumerable<Player> Enemies => Players?.Where(pair => pair.Key != "i").Select(pair => pair.Value);

        public Matrix<int> EnemiesMap;

        public List<Point[]> Paths = new List<Point[]>();

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

            Bonuses = jPacket.Params.Bonuses?.Select(jb => new Bonus(jPacket, jb)).ToArray();

            if (Bonuses != null && Bonuses.Any())
            {
                foreach (var bonus in Bonuses)
                {
                    switch (bonus.BonusType)
                    {
                        case JBonusType.SpeedUp:
                            this[bonus.Position].Element = Element.FLASH;
                            break;
                        case JBonusType.SlowDown:
                            this[bonus.Position].Element = Element.EXPLORER;
                            break;
                        case JBonusType.Saw:
                            this[bonus.Position].Element = Element.SAW;
                            break;
                        default: break;
                    }

                    bonus.Map = new Map(this);
                }

                Parallel.ForEach(Bonuses, bonus =>
                {
                    var speed = GetSpeed(jPacket.Params.Speed, jPacket.Params.Width, bonus.BonusType);
                    bonus.Map.Check(bonus.Position, jPacket.Params.Width, 0, new SpeedSnapshot()
                    {
                        Speed = speed,
                        Pixels = int.MaxValue
                    });
                });
            }

            if ((Players = JPacket.Params.Players?.ToDictionary(jp => jp.Key, jp => new Player(this, jp.Key))) != null)
            {
                foreach (var player in Players.Values)
                {
                    var checkedPoints = player.Line.ToList();
                    if (player.Direction != Direction.Unknown)
                    {
                        var backPoint = player.Position[player.Direction.Invert()];
                        if (backPoint.OnBoard(Size)) checkedPoints.Add(backPoint);
                    }

                    player.Map = new Map(this, player, checkedPoints.ToArray());
                }

                foreach (var player in Enemies)
                {
                    foreach (var point in player.Territory) this[point].Element = Element.PLAYER_TERRITORY;
                    foreach (var point in player.Line) this[point].Element = Element.PLAYER_LINE;
                    this[player.Position].Element = Element.PLAYER;
                }

                if (Player != null)
                {
                    foreach (var point in Player.Territory) this[point].Element = Element.ME_TERRITORY;
                    foreach (var point in Player.Line) this[point].Element = Element.ME_LINE;
                    this[Player.Position].Element = Element.ME;
                }

                Parallel.ForEach(Players.Values, player =>
                {
                    var startWeight = player.IsCenterCell ? 0 : (player.JPlayer.Position - player.Position[player.Direction].FromGrid(jPacket.Params.Width)).Abs() / player.GetSpeed();

                    player.Map.Check(player.IsCenterCell ? player.Position : player.Position[player.Direction],
                        jPacket.Params.Width, startWeight, player.GetSpeedSnapshots());

                    player.LineMap = new Map2(this, player);
                    player.LineMap.Check(player.Line.ToArray());
                });

                EnemiesMap = new Matrix<int>(Size);
                for (var i = 0; i < Size.Width; i++)
                for (var j = 0; j < Size.Height; j++)
                    EnemiesMap[i, j] = Enemies.Any() ? Enemies.Select(e => e.Map[i, j].Weight).Min() : int.MaxValue;
            }

            if (Player != null)
            {
                PossibleDirections = Player == null ? null : Point.CrossNeighbors.Keys
                    .Where(d => Player.Direction.Invert() != d && Player.Position[d].OnBoard(Size))
                    .Where(d => Player.Position[d].OnBoard(Size) && this[Player.Position[d]].Element != Element.ME_LINE)
                    .ToArray();
                
                var speedSnapshots = Player.GetSpeedSnapshots();
                speedSnapshots[0].Pixels -= JPacket.Params.Width;

                foreach (var direction in PossibleDirections)
                {
                    var position = Player.IsCenterCell
                        ? Player.Position[direction]
                        : Player.Position[Player.Direction][direction];
                    if (position.OnBoard(Size))
                    {
                        //TODO: обработать случай, если position - бонус!

                        var checkedPoints = Player.Line.ToList();
                        if (Player.Direction != Direction.Unknown)
                        {
                            var backPoint = position[Player.Direction.Invert()];
                            if (backPoint.OnBoard(Size))
                                checkedPoints.Add(backPoint);
                        }

                        Player.PossibleMaps.Add(direction, new Map(this, Player, checkedPoints.Count == 0 ? new Point[] { } : checkedPoints.ToArray()));

                        Player.PossibleMaps[direction].Check(position, JPacket.Params.Width, 0, speedSnapshots);
                    }
                }
            }
        }

        private static readonly Dictionary<JBonusType, int> BonusSpeed = new Dictionary<JBonusType, int>();

        public static int GetSpeed(int defaultSpeed, int width, params JBonusType[] bonuses)
        {
            if (bonuses.Any(t => t == JBonusType.SlowDown) &&
                bonuses.Any(t => t == JBonusType.SpeedUp))
                return defaultSpeed;

            if (bonuses.Any(t => t == JBonusType.SlowDown))
            {
                if (!BonusSpeed.ContainsKey(JBonusType.SlowDown))
                {
                    var speed = defaultSpeed - 1;
                    for (; width % speed != 0 && speed > 0; speed--) ;
                    BonusSpeed[JBonusType.SlowDown] = speed;
                }

                return BonusSpeed[JBonusType.SlowDown];
            }
            if (bonuses.Any(t => t == JBonusType.SpeedUp))
            {
                if (!BonusSpeed.ContainsKey(JBonusType.SpeedUp))
                {
                    var speed = defaultSpeed + 1;
                    for (; width % speed != 0 && speed < width; speed++) ;
                    BonusSpeed[JBonusType.SpeedUp] = speed;
                }

                return BonusSpeed[JBonusType.SpeedUp];
            }

            return defaultSpeed;
        }
    }
}