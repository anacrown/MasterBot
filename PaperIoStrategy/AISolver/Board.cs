using System;
using System.Collections.Generic;
using System.Linq;
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
                foreach (var bonuse in Bonuses)
                {
                    switch (bonuse.BonusType)
                    {
                        case JBonusType.SpeedUp:
                            this[bonuse.Position].Element = Element.FLASH;
                            break;
                        case JBonusType.SlowDown:
                            this[bonuse.Position].Element = Element.EXPLORER;
                            break;
                        case JBonusType.Saw:
                            this[bonuse.Position].Element = Element.SAW;
                            break;
                        default: break;
                    }
                }
            }

            if ((Players = JPacket.Params.Players?.ToDictionary(jp => jp.Key, jp => new Player(jp.Key, JPacket))) != null)
            {
                foreach (var player in Players.Values)
                {
                    var checkedPoints = player.Line.ToList();
                    if (player.Direction != Direction.Unknown)
                    {
                        var backPoint = player.Position[player.Direction.Invert()];
                        if (player.Direction != Direction.Unknown && backPoint.OnBoard(Size))
                            checkedPoints.Add(player.Position[player.Direction.Invert()]);
                    }

                    player.Map = new Map(new Size(jPacket.Params.XCellsCount, jPacket.Params.YCellsCount), checkedPoints.ToArray());
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

                Parallel.ForEach(Players.Values, player => { player.Map.Check(player.Position); });
            }
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

            var map = new Map(Size, checkedPoints.ToArray());
            map.Check(Player.Position[direction]);

            return GetPathToHome(map, checkedPoints, 1);
        }

        public IEnumerable<Direction> PossibleDirections => Player == null ? null : Point.Neighbor.Keys
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

        public string GetResponse()
        {
            Direction direction;

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

            var PathsToHome = new Dictionary<Direction, Point[]>();
            
            foreach (var d in PossibleDirections)
            {
                var path = GatPathToHomeAfterMove(d);
                if (path != null) PathsToHome.Add(d, path.ToArray());
            }
            
            if (PathsToHome.Count == 0)
            {
                var path = GetMinPathToHome(Player.Map, Player.Line);
                if (path != null)
                    PathsToHome.Add(Player.Position.GetDirectionTo(path.First()), path.ToArray());
            }
            
            var squares = PathsToHome.Keys.ToDictionary(d => d, d => Square(d, PathsToHome[d])).OrderByDescending(pair => pair.Value);
            
            direction = !squares.Any() ? PossibleDirections.First() : squares.First().Key;
            
            return $"{{\"command\": \"{direction.GetCommand()}\"}}";
        }
    }

    public static class EnumerableExtention
    {
        public static T MinSingle<T>(this IEnumerable<T> collection, Func<T, int> selector) => collection.Aggregate((r, x) => selector(r) < selector(x) ? r : x);

        public static IEnumerable<T> Min<T>(this IEnumerable<T> collection, Func<T, int> selector)
        {
            var enumerable = collection as T[] ?? collection.ToArray();
            var min = enumerable.Select(selector).Min();
            return enumerable.Where(t => selector(t) == min);
        }

        public static T MaxSingle<T>(this IEnumerable<T> collection, Func<T, int> selector) => collection.Aggregate((r, x) => selector(r) > selector(x) ? r : x);

        public static IEnumerable<T> Max<T>(this IEnumerable<T> collection, Func<T, int> selector)
        {
            var enumerable = collection as T[] ?? collection.ToArray();
            var max = enumerable.Select(selector).Max();
            return enumerable.Where(t => selector(t) == max);
        }


    }
}