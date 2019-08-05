using System;
using System.Collections.Generic;
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

        public Player Player => Players != null && Players.ContainsKey("i") ? Players["i"] : null;

        public IEnumerable<Player> Enemies => Players?.Where(pair => pair.Key != "i").Select(pair => pair.Value);

        public BetterMap BetterMap { get; }

        public Border Border { get; }

        public List<Point[]> Paths = new List<Point[]>();

        //---------------------

        public int EnemiesMap(Point p) => EnemiesMap(p.X, p.Y);
        public int EnemiesMap(int i, int j)
        {
            var min = int.MaxValue;
            var enemyMapEntries = Enemies.Select(enemy => enemy.Map[i, j]).ToArray();
            foreach (var enemyMapEntry in enemyMapEntries)
            {
                if (enemyMapEntry == null) continue;
                if (min > enemyMapEntry.Weight)
                    min = enemyMapEntry.Weight;
            }

            return min;
        }

        //---------------------

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

                    bonus.Map = new Map(Size);
                }

                Parallel.ForEach(Bonuses, bonus =>
                {
                    var speed = GetSpeed(jPacket.Params.Speed, jPacket.Params.Width, bonus.BonusType);
                    bonus.Map.Check(bonus.Position, jPacket.Params.Width, 0, new SpeedSnapshot()
                    {
                        Speed = speed,
                        Pixels = Int32.MaxValue
                    });
                });
            }

            if ((Players = JPacket.Params.Players?.ToDictionary(jp => jp.Key, jp => new Player(jp.Key, JPacket))) != null)
            {
                foreach (var player in Players.Values)
                {
                    var checkedPoints = player.Line.ToList();
                    if (player.Direction != Direction.Unknown)
                    {
                        var backPoint = player.Position[player.Direction.Invert()];
                        if (backPoint.OnBoard(Size)) checkedPoints.Add(backPoint);
                    }

                    player.Map = new Map(Size, checkedPoints.ToArray());
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
                    var startWeight = player.IsCenterCell ? 0 : (player.GetShift()) / player.GetSpeed();

                    player.Map.Check(player.IsCenterCell ? player.Position : player.Position[player.Direction],
                        jPacket.Params.Width, startWeight, player.GetSpeedSnapshots());
                });
            }

            if (Player != null)
            {
                Border = new Border(this, Player.Territory);

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

                        Player.PossibleMaps.Add(direction, new Map(Size, checkedPoints.Count == 0 ? new Point[] { } : checkedPoints.ToArray()));

                        Player.PossibleMaps[direction].Check(position, JPacket.Params.Width, 0, speedSnapshots);
                    }
                }
            }

            BetterMap = new BetterMap(this);
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

        public IEnumerable<Point> GetMinPathToHome(Map map, IEnumerable<Point> line)
        {
            var entries = Player.Territory.Select(p => map[p]).OrderBy(e => e.Weight);
            foreach (var entry in entries)
            {
                var path = map.Tracert(entries.First().Position);
                if (path.Length > 0) return path;
            }

            return null;
        }

        public IEnumerable<Point> GetPathToHome(Map map, IEnumerable<Point> line, int move = 0)
        {
            try
            {
                var entries = Player.Territory.Select(p => map[p]).OrderBy(e => e.Weight);

                foreach (var entry in entries)
                {
                    Point[] path;
                    if ((path = map.Tracert(entry.Position)).Length > 0)
                    {
                        if (path.Length == 0) continue;

                        var eMove = path.Select(EnemiesMap).Min() - 1 - move;

                        var c = false;
                        foreach (var p in path)
                        {
                            var iMove = 1 + map[p].Weight;
                            if (iMove < eMove) continue;
                            c = true;
                            break;
                        }
                        if (c) continue;

                        if (line.Select(EnemiesMap).Min() - 1 - move <= path.Length)
                            continue;

                        return path;
                    }
                }
            }
            catch (Exception e)
            {

            }

            return null;
        }

        public IEnumerable<Point> GatPathToHomeAfterMove(Direction direction)
        {
            var checkedPoints = new List<Point> { Player.Position };
            checkedPoints.AddRange(Player.Line);

            return GetPathToHome(Player.PossibleMaps[direction], checkedPoints, 1);
        }

        public IEnumerable<Direction> PossibleDirections => Player == null ? null : Point.CrossNeighbors.Keys
            .Where(d => Player.Direction.Invert() != d && Player.Position[d].OnBoard(Size))
            .Where(d => Player.Position[d].OnBoard(Size) && this[Player.Position[d]].Element != Element.ME_LINE);

        public int Square(Direction direction, Point[] path)
        {
            var s = 0;

            var points = new List<Point>() { Player.Position, Player.Position[direction] };

            foreach (var point in Player.Line)
            {
                if (!points.Contains(point))
                    points.Add(point);
            }

            foreach (var point in path)
            {
                if (!points.Contains(point))
                    points.Add(point);
            }

            var allX = points.Select(p => p.X).ToArray();
            var minX = allX.Min();
            var maxX = allX.Max();

            for (var i = minX; i <= maxX; i++)
            {
                var x = i;
                var pairY = points
                    .Where(p => p.X == x)
                    .Select(p => p.Y)
                    .ToArray();

                s += pairY.Max() - pairY.Min() + 1;
            }

            return s;
        }

        Point[] GetPathToHomeAfterMove(Direction direction)
        {
            var reversePoint = Player.Territory.MinSingle(EnemiesMap);

            if (EnemiesMap(Player.Position[direction]) <= Player.PossibleMaps[direction][reversePoint].Weight)
                return null;

            if (Player.Line.Any(p => EnemiesMap(p) <= Player.PossibleMaps[direction][reversePoint].Weight))
                return null;

            var path = Player.PossibleMaps[direction].Tracert(reversePoint);

            if (path.Any(p => EnemiesMap(p) <= Player.PossibleMaps[direction][reversePoint].Weight))
                return null;

            return path;
        }

        public string GetResponse()
        {
            Direction direction = Direction.Unknown;

            //
            //            if (Bonuses.Any())
            //            {
            //                var path = Player.Map.Tracert(Bonuses.First().Position);
            //                direction = Player.Position.GetDirectionTo(path.First());
            //                Paths.Add(path);
            //            }
            //            else
            //            {
            //                if (Player.Territory.Contains(Player.Position))
            //                {
            //                    direction = PossibleDirections.FirstOrDefault(d =>
            //                        this[Player.Position[d]].Element == Element.ME_TERRITORY);
            //                }
            //                else
            //                {
            //                    var path = Player.Map.Tracert(Player.Territory.First());
            //                    direction = Player.Position.GetDirectionTo(path.First());
            //                    Paths.Add(path);
            //                }
            //            }

//            if (Player.Position.GetCrossVicinity(Size).All(t => this[t].Element == Element.ME_TERRITORY))
//                direction = Player.Position.GetDirectionTo(PossibleDirections.Select(d => Player.Position[d])
//                    .OrderBy(p => BetterMap[p].Weight).First());
//            else
//            {
//                var PathsToHome = new Dictionary<Direction, Point[]>();
//
//                foreach (var d in PossibleDirections)
//                {
//                    var path = GatPathToHomeAfterMove(d);
//                    if (path != null) PathsToHome.Add(d, path.ToArray());
//                }
//
//                if (PathsToHome.Count == 0)
//                {
//                    var path = GetMinPathToHome(Player.Map, Player.Line);
//                    if (path != null)
//                        PathsToHome.Add(Player.Position.GetDirectionTo(path.First()), path.ToArray());
//                }
//
//                var squares = PathsToHome.Keys.ToDictionary(d => d, d => Square(d, PathsToHome[d]))
//                    .OrderByDescending(pair => pair.Value);
//
//                direction = !squares.Any() ? PossibleDirections.First() : squares.First().Key;
//            }

            Paths.Add(Border.GetAlongPath(Border.FirstOrDefault(c => c.IsBoundary)?.Position).ToArray());

            return $"{{\"command\": \"{direction.GetCommand()}\"}}";
        }
    }
}