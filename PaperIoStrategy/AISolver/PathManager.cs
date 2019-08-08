using System.Collections.Generic;
using System.Linq;
using BotBase.Board;

namespace PaperIoStrategy.AISolver
{
    public class PathManager
    {
        public Board Board { get; }
        public Player Player { get; }

        private readonly List<IEnumerable<Point>> Lane;
        private readonly ILookup<Point, IEnumerable<Point>> Rays;

        public PathManager(Board board, Player player)
        {
            Board = board;
            Player = player;

            Lane = new List<IEnumerable<Point>>();

            var rays = new List<IEnumerable<Point>>();

            foreach (var borderCell in player.Border.Where(b => b.IsBoundary))
            {
                foreach (var outDirection in borderCell.OutDirections)
                {
                    IEnumerable<Point> line;
                    if (Player.BBox[borderCell.Position].IsBound)
                        line = borderCell.Position.GetLine(outDirection, Player.Board.Size).While(p => !player.BBox[p].IsCorner && !player.Border[p].IsBoundary).ToArray();
                    else
                        line = borderCell.Position.GetLine(outDirection, Player.Board.Size).While(p => !player.BBox[p].IsBound && !player.Border[p].IsBoundary).ToArray();

                    if (player.BBox[line.Last()].IsBound)
                        rays.Add(line);
                    else
                        Lane.Add(line);
                }
            }

            Rays = rays.ToLookup(line => line.Last(), line => line);
        }

//        public IEnumerable<Point[]> GetPaths()
//        {
////            foreach (var ray in Rays)
////            {
////                var boundPath = new List<Point>();
////                var bound = ray.Key; // в эту точку
////
////                do
////                {
////                    bound = bound[Player.BBox[bound].Direction];
////                    boundPath.Add(bound);
////
////                    if (Rays.ContainsKey(bound))
////                    {
////                        var path = new List<Point>();
////                        path.AddRange(ray.Value);
////                        path.AddRange(boundPath);
////
////                        path.AddRange(Rays[bound].Reverse().Skip(1));
////
////                        yield return path.ToArray();
////                    }
////
////                } while (bound != ray.Key);
////            }
//
////            foreach (var group  in Rays)
////            {
////                if (group.Count() == 2)
////                {
////                    yield return group.First().Concat(group.Last().Reverse().Skip(1)).ToArray();
////                }
////
////                if (group.Count() == 1)
////                {
////                    var ray = group.First();
////
////                    var boundPath = new List<Point>();
////                    var bound = group.Key; // в эту точку
////
////                    do
////                    {
////                        bound = bound[Player.BBox[bound].Direction];
////                        boundPath.Add(bound);
////
////                        if (Rays.Contains(bound))
////                        {
////                            var path = new List<Point>();
////                            path.AddRange(ray);
////                            path.AddRange(boundPath);
////
////                            var ray2 = Rays[bound].SingleOrDefault() ?? Rays[bound].First(r => Player.BBox[r.First()].Direction == )
////
////                            path.AddRange(Rays[bound].Reverse().Skip(1));
////
////                            yield return path.ToArray();
////                        }
////
////                    } while (bound != ray.Key);
////                }
////            }
//
//        }
    }
}