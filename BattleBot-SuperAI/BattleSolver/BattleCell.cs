using System;
using System.Collections.Generic;
using System.Linq;
using BotBase.Board;
using Point = BotBase.Board.Point;
using Size = BotBase.Board.Size;

namespace BattleBot_SuperAI.BattleSolver
{
    public class BattleCell
    {
        private readonly Cell _cell;

        public enum CellMetaType
        {
            GROUND, WALL, DEAD,
            CONSTRUCTION, BULLET, TANK, PLAYER_TANK, OTHER_TANK
        }

        public BattleBoard Board { get; }
        public Element Type { get; }
        public CellMetaType MetaType { get; }

        public Point Pos => _cell.Pos;
        public int X => Pos.X;
        public int Y => Pos.Y;

        public int PlayerWeight => Board.PlayerMap[Pos];
        public bool IsEnemy => MetaType == CellMetaType.PLAYER_TANK || MetaType == CellMetaType.OTHER_TANK;

        public bool IsTank => MetaType == CellMetaType.TANK || MetaType == CellMetaType.PLAYER_TANK ||
                              MetaType == CellMetaType.OTHER_TANK;

        public bool IsElapse => MetaType == CellMetaType.GROUND ||
                                MetaType == CellMetaType.BULLET ||
                                MetaType == CellMetaType.DEAD ||
                                MetaType == CellMetaType.TANK || IsEnemy;

        public Direction Direction { get; }

        public int Strength { get; }
        public BattleCell(Cell cell, BattleBoard board)
        {
            _cell = cell;
            Board = board;
            Type = cell.GetElement();
            Direction = GetDirection(Type);
            MetaType = MetaTypeFromType(Type);

            Strength = GetStrength(Type);
        }

        public BattleCell[] GetCrossVicinity() => Pos.GetCrossVicinity(new BotBase.Board.Size(Board.Size, Board.Size)).Select(t => Board[t]).ToArray();

        public override string ToString() => _cell.ToString();

        private static int GetStrength(Element type)
        {
            switch (type)
            {
                case Element.CONSTRUCTION: return 3;
                case Element.CONSTRUCTION_DESTROYED_DOWN: return 2;
                case Element.CONSTRUCTION_DESTROYED_UP: return 2;
                case Element.CONSTRUCTION_DESTROYED_LEFT: return 2;
                case Element.CONSTRUCTION_DESTROYED_RIGHT: return 2;
                case Element.CONSTRUCTION_DESTROYED_DOWN_TWICE: return 1;
                case Element.CONSTRUCTION_DESTROYED_UP_TWICE: return 1;
                case Element.CONSTRUCTION_DESTROYED_LEFT_TWICE: return 1;
                case Element.CONSTRUCTION_DESTROYED_RIGHT_TWICE: return 1;
                case Element.CONSTRUCTION_DESTROYED_LEFT_RIGHT: return 1;
                case Element.CONSTRUCTION_DESTROYED_UP_DOWN: return 1;
                case Element.CONSTRUCTION_DESTROYED_UP_LEFT: return 1;
                case Element.CONSTRUCTION_DESTROYED_RIGHT_UP: return 1;
                case Element.CONSTRUCTION_DESTROYED_DOWN_LEFT: return 1;
                case Element.CONSTRUCTION_DESTROYED_DOWN_RIGHT: return 1;

                case Element.GROUND:
                    return 0;

                case Element.BULLET:
                    return 0;

                case Element.DEAD:
                case Element.TANK_UP:
                case Element.TANK_RIGHT:
                case Element.TANK_DOWN:
                case Element.TANK_LEFT:
                case Element.OTHER_TANK_UP:
                case Element.OTHER_TANK_RIGHT:
                case Element.OTHER_TANK_DOWN:
                case Element.OTHER_TANK_LEFT:
                    return 0;

                case Element.WALL:
                    return 10;


                default: return 0;
            }
        }

        private static Direction GetDirection(Element type)
        {
            switch (type)
            {
                case Element.TANK_UP:
                case Element.PLAYER_TANK_UP:
                case Element.OTHER_TANK_UP:

                    return Direction.Up;

                case Element.TANK_RIGHT:
                case Element.PLAYER_TANK_RIGHT:
                case Element.OTHER_TANK_RIGHT:

                    return Direction.Right;

                case Element.TANK_DOWN:
                case Element.PLAYER_TANK_DOWN:
                case Element.OTHER_TANK_DOWN:

                    return Direction.Down;

                case Element.TANK_LEFT:
                case Element.PLAYER_TANK_LEFT:
                case Element.OTHER_TANK_LEFT:
                    return Direction.Left;

                default: return Direction.Unknown;
            }
        }

        public IEnumerable<BattleCell> GetLine(Direction direction, int deep = -1, bool stopInTank = true)
        {
            var point = Pos[direction];

            while (point.OnBoard(new Size(Board.Size, Board.Size)))
            {
                if (deep == 0 || Board[point].MetaType != CellMetaType.GROUND && Board[point].MetaType != CellMetaType.BULLET && !Board[point].IsTank)
                    yield break;

                yield return Board[point];

                if (stopInTank && Board[point].IsTank) yield break;

                point = point[direction];
                deep--;
            }
        }

        private Element TypeFromChar(char c)
        {
            switch (c)
            {
                case ' ': return Element.GROUND;
                case '☼': return Element.WALL;
                case 'Ѡ': return Element.DEAD;
                case '╬': return Element.CONSTRUCTION;
                case '╩': return Element.CONSTRUCTION_DESTROYED_DOWN;
                case '╦': return Element.CONSTRUCTION_DESTROYED_UP;
                case '╠': return Element.CONSTRUCTION_DESTROYED_LEFT;
                case '╣': return Element.CONSTRUCTION_DESTROYED_RIGHT;
                case '╨': return Element.CONSTRUCTION_DESTROYED_DOWN_TWICE;
                case '╥': return Element.CONSTRUCTION_DESTROYED_UP_TWICE;
                case '╞': return Element.CONSTRUCTION_DESTROYED_LEFT_TWICE;
                case '╡': return Element.CONSTRUCTION_DESTROYED_RIGHT_TWICE;
                case '│': return Element.CONSTRUCTION_DESTROYED_LEFT_RIGHT;
                case '─': return Element.CONSTRUCTION_DESTROYED_UP_DOWN;
                case '┌': return Element.CONSTRUCTION_DESTROYED_UP_LEFT;
                case '┐': return Element.CONSTRUCTION_DESTROYED_RIGHT_UP;
                case '└': return Element.CONSTRUCTION_DESTROYED_DOWN_LEFT;
                case '┘': return Element.CONSTRUCTION_DESTROYED_DOWN_RIGHT;
                case '•': return Element.BULLET;

                case '▲': return Element.TANK_UP;
                case '►': return Element.TANK_RIGHT;
                case '▼': return Element.TANK_DOWN;
                case '◄': return Element.TANK_LEFT;

                case '?': return Element.OTHER_TANK_UP;
                case '»': return Element.OTHER_TANK_RIGHT;
                case '¿': return Element.OTHER_TANK_DOWN;
                case '«': return Element.OTHER_TANK_LEFT;

                case '˄': return Element.PLAYER_TANK_UP;
                case '˃': return Element.PLAYER_TANK_RIGHT;
                case '˅': return Element.PLAYER_TANK_DOWN;
                case '˂': return Element.PLAYER_TANK_LEFT;

                default: throw new Exception($"invalide char {c}");
            }
        }
        private static CellMetaType MetaTypeFromType(Element type)
        {
            switch (type)
            {
                case Element.GROUND:
                    return CellMetaType.GROUND;
                case Element.WALL:
                    return CellMetaType.WALL;
                case Element.DEAD:
                    return CellMetaType.DEAD;

                case Element.CONSTRUCTION:
                case Element.CONSTRUCTION_DESTROYED_DOWN:
                case Element.CONSTRUCTION_DESTROYED_UP:
                case Element.CONSTRUCTION_DESTROYED_LEFT:
                case Element.CONSTRUCTION_DESTROYED_RIGHT:
                case Element.CONSTRUCTION_DESTROYED_DOWN_TWICE:
                case Element.CONSTRUCTION_DESTROYED_UP_TWICE:
                case Element.CONSTRUCTION_DESTROYED_LEFT_TWICE:
                case Element.CONSTRUCTION_DESTROYED_RIGHT_TWICE:
                case Element.CONSTRUCTION_DESTROYED_LEFT_RIGHT:
                case Element.CONSTRUCTION_DESTROYED_UP_DOWN:
                case Element.CONSTRUCTION_DESTROYED_UP_LEFT:
                case Element.CONSTRUCTION_DESTROYED_RIGHT_UP:
                case Element.CONSTRUCTION_DESTROYED_DOWN_LEFT:
                case Element.CONSTRUCTION_DESTROYED_DOWN_RIGHT:
                    return CellMetaType.CONSTRUCTION;

                case Element.BULLET:
                    return CellMetaType.BULLET;

                case Element.TANK_UP:
                case Element.TANK_RIGHT:
                case Element.TANK_DOWN:
                case Element.TANK_LEFT:
                    return CellMetaType.TANK;

                case Element.PLAYER_TANK_UP:
                case Element.PLAYER_TANK_RIGHT:
                case Element.PLAYER_TANK_DOWN:
                case Element.PLAYER_TANK_LEFT:
                    return CellMetaType.PLAYER_TANK;

                case Element.OTHER_TANK_UP:
                case Element.OTHER_TANK_RIGHT:
                case Element.OTHER_TANK_DOWN:
                case Element.OTHER_TANK_LEFT:
                    return CellMetaType.OTHER_TANK;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}