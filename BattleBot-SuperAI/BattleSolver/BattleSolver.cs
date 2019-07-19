using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using BattleBot_SuperAI.Controls;
using CodenjoyBot.Annotations;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;
using CodenjoyBot.Interfaces;
using Point = CodenjoyBot.Board.Point;

namespace BattleBot_SuperAI.BattleSolver
{
    [Serializable]
    public class BattleSolver : ISolver, INotifyPropertyChanged
    {
        private UIElement _control;
        private UIElement _debugControl;
        private readonly FrameBuffer<BattleBoard> _frameBuffer;

        public UIElement Control => _control ?? (_control = new BattleSolverControl(this));

        public UIElement DebugControl => _debugControl ?? (_debugControl = new BattleSolverDebugControl(this));

        public BattleSolver()
        {
            _frameBuffer = new FrameBuffer<BattleBoard>(100);
        }

        public void Initialize()
        {
            _frameBuffer.Clear();
        }

        protected BattleSolver(SerializationInfo info, StreamingContext context) : this()
        {
            
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }

        public string Answer(Board board)
        {
            OnBoardChanged(board);

            var command = SolverCommand.Empty;
            var battleBoard = new BattleBoard(board, _frameBuffer);
            _frameBuffer.AddFrame(new Frame<BattleBoard>(battleBoard, board.Frame.Time));

            Enemy.TargetEnemy target = null;

            var enemy = battleBoard.Enemies.FirstOrDefault(e => battleBoard.Player.GetCrossVicinity().Select(t => t.Pos).Contains(e.Cell.Pos));
            if (enemy != null)
            {
                target = new Enemy.TargetEnemy()
                {
                    Enemy = enemy,
                    Cross = enemy.CurrentCross,
                    FirePosition = battleBoard.Player
                };
            }
            else
            {
                var positions = battleBoard.Enemies.SelectMany(e => e.FirePositions).ToArray();
                if (positions.Length > 0)
                {
                    var min = positions.Select(t => battleBoard.PlayerMap[t.FirePosition.Pos]).Min();
                    target = positions.FirstOrDefault(p => battleBoard.PlayerMap[p.FirePosition.Pos] == min);
                }
                else
                {
                    var m = battleBoard.Enemies.Select(e => battleBoard.PlayerMap[e.Cell.Pos]).Min();
                    enemy = battleBoard.Enemies.FirstOrDefault(e => battleBoard.PlayerMap[e.Cell.Pos] == m);

                    if (enemy != null)
                    {
                        target = new Enemy.TargetEnemy()
                        {
                            Enemy = enemy,
                            Cross = enemy.CurrentCross,
                            FirePosition = battleBoard.Player
                        };
                    }
                }
            }

            if (target != null)
            {
                battleBoard.PathToTarget = battleBoard.PlayerMap.Path(target.FirePosition);
                var targetCanShotInNextMove = target.Enemy.CanShotInNextMove();
                OnLogDataReceived(board.Frame, $"Target Can shot in next move: {targetCanShotInNextMove}");

                // не умру на сл ходу и есть точка откуда смогу убить
                if (!IsDieNextTurn(battleBoard) || (!targetCanShotInNextMove && battleBoard.Player.GetCrossVicinity().Select(cell => cell.Pos).Contains(target.Enemy.Cell.Pos)))
                {
                    // есть враг на диагонали
                    if (battleBoard.Player.Pos.IsDiagonal(target.Enemy.Cell.Pos) && target.Enemy.IsMove &&
                        targetCanShotInNextMove)
                    {
                        OnLogDataReceived(board.Frame, "IsDiagonal!");
                        command = SolverCommand.Stop;
                    }
                    else
                    {
                        if (battleBoard.PathToTarget.Length > 0)
                        {
                            //target в соседней клетке 
                            if (battleBoard.Player.GetCrossVicinity().Select(v => v.Pos).Contains(target.Enemy.Cell.Pos) &&
                                !targetCanShotInNextMove)
                            {
                                command = GetCommandForMove(battleBoard, target.Enemy.Cell) | SolverCommand.Act;
                            }
                            else
                            // до огневой позиции придется пройтись
                            if (battleBoard.PathToTarget.First().Pos != battleBoard.Player.Pos)
                            {
                                if (!IsDieNextTurn(battleBoard, battleBoard.PathToTarget.First().Pos))
                                {
                                    OnLogDataReceived(board.Frame, $"Move to {target.FirePosition.Pos}");
                                    command = GetCommandForMove(battleBoard, battleBoard.PathToTarget.First());

                                    if (battleBoard.PathToTarget.First().MetaType == BattleCell.CellMetaType.CONSTRUCTION)
                                        command = command | SolverCommand.Act;
                                }
                            }
                            // мы на огневой позиции => разворачиваемся и стреляем
                            else
                            {
                                var direction = battleBoard.Player.Pos.GetDirectionTo(target.Cross.Centre.Pos);
                                if (direction != Direction.Unknown && (!IsDieNextTurn(battleBoard, battleBoard.Player.Pos[direction]) || (!targetCanShotInNextMove && battleBoard.Player.GetCrossVicinity().Select(cell => cell.Pos).Contains(target.Enemy.Cell.Pos))))
                                {
                                    OnLogDataReceived(board.Frame, $"Shot {target.Enemy.Cell.Pos}");
                                    command = direction.GetCommand() | SolverCommand.Act;
                                }
                            }
                        }

                        battleBoard.Target = target;
                    }
                }
                else
                {
                    OnLogDataReceived(board.Frame, "DieNextTurn!");
                    command = LeaveToBetterPlace(battleBoard);
                }
            }

            command = command == SolverCommand.Empty ? LeaveToBetterPlace(battleBoard) : command;

            return command.ToString();
        }

        private SolverCommand LeaveToBetterPlace(BattleBoard board)
        {
            //MainWindow.Log(board.Time, "LeaveToBetterPlace");

            var betterPoint = board.Player.Pos;
            var savePoints = board.Player.Pos.GetCrossVicinity(board.Size).Where(t => !IsDieNextTurn(board, t)).ToArray();
            if (savePoints.Length > 0)
            {
                var max = savePoints.Select(t => board.ABetterMap[t]).Max();
                var betterPoints = savePoints.Where(t => board.ABetterMap[t] == max).ToArray();
                betterPoint = betterPoints.Length > 1
                    ? betterPoints[new Random().Next(betterPoints.Length)]
                    : betterPoints.FirstOrDefault();
            }

            return betterPoint == null ? SolverCommand.Empty : GetCommandForMove(board, board[betterPoint]);
        }

        private SolverCommand GetCommandForMove(BattleBoard board, BattleCell targetCell)
        {
            var player = board.Player.Pos;
            var target = targetCell.Pos;

            if (player == target)
                return SolverCommand.Empty;

            if (player.X == target.X)
            {
                return player.Y > target.Y ? SolverCommand.Up : SolverCommand.Down;
            }

            if (player.Y == target.Y)
            {
                return player.X > target.X ? SolverCommand.Left : SolverCommand.Right;
            }

            //return player.Y < target.Y ? SolverCommand.Right : SolverCommand.Left;
            throw new Exception();
        }

        private bool IsDieNextTurn(BattleBoard board, Point point = null)
        {
            if (board.Bullets.SelectMany(bullet => bullet.GetAffectedArea()).Any(p => (point ?? board.Player.Pos) == p))
                return true;

            if (board.Enemies.SelectMany(enemy => enemy.GetAffectedArea()).Any(p => (point ?? board.Player.Pos) == p))
                return true;

            return false;
        }

        public event EventHandler<LogRecord> LogDataReceived;
        protected virtual void OnLogDataReceived(DataFrame frame, string message) => LogDataReceived?.Invoke(this, new LogRecord(frame, message));

        public event EventHandler<Board> BoardChanged;
        protected virtual void OnBoardChanged(Board board) => BoardChanged?.Invoke(this, board);
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public enum Element
    {
        GROUND, WALL, DEAD,
        CONSTRUCTION,
        CONSTRUCTION_DESTROYED_DOWN,
        CONSTRUCTION_DESTROYED_UP,
        CONSTRUCTION_DESTROYED_LEFT,
        CONSTRUCTION_DESTROYED_RIGHT,
        CONSTRUCTION_DESTROYED_DOWN_TWICE,
        CONSTRUCTION_DESTROYED_UP_TWICE,
        CONSTRUCTION_DESTROYED_LEFT_TWICE,
        CONSTRUCTION_DESTROYED_RIGHT_TWICE,
        CONSTRUCTION_DESTROYED_LEFT_RIGHT,
        CONSTRUCTION_DESTROYED_UP_DOWN,
        CONSTRUCTION_DESTROYED_UP_LEFT,
        CONSTRUCTION_DESTROYED_RIGHT_UP,
        CONSTRUCTION_DESTROYED_DOWN_LEFT,
        CONSTRUCTION_DESTROYED_DOWN_RIGHT,
        BULLET,
        TANK_UP,
        TANK_RIGHT,
        TANK_DOWN,
        TANK_LEFT,
        PLAYER_TANK_UP,
        PLAYER_TANK_RIGHT,
        PLAYER_TANK_DOWN,
        PLAYER_TANK_LEFT,
        OTHER_TANK_UP,
        OTHER_TANK_RIGHT,
        OTHER_TANK_DOWN,
        OTHER_TANK_LEFT
    }

    public static class CellExtention
    {
        private static Dictionary<string, Element> _elements = new Dictionary<string, Element>()
        {
            { " ",Element.GROUND                                             },
            { "☼",Element.WALL                                               },
            { "Ѡ",Element.DEAD                                               },
            { "╬",Element.CONSTRUCTION                                       },
            { "╩",Element.CONSTRUCTION_DESTROYED_DOWN                        },
            { "╦",Element.CONSTRUCTION_DESTROYED_UP                          },
            { "╠",Element.CONSTRUCTION_DESTROYED_LEFT                        },
            { "╣",Element.CONSTRUCTION_DESTROYED_RIGHT                       },
            { "╨",Element.CONSTRUCTION_DESTROYED_DOWN_TWICE                  },
            { "╥",Element.CONSTRUCTION_DESTROYED_UP_TWICE                    },
            { "╞",Element.CONSTRUCTION_DESTROYED_LEFT_TWICE                  },
            { "╡",Element.CONSTRUCTION_DESTROYED_RIGHT_TWICE                  },
            { "│",Element.CONSTRUCTION_DESTROYED_LEFT_RIGHT                  },
            { "─",Element.CONSTRUCTION_DESTROYED_UP_DOWN                     },
            { "┌",Element.CONSTRUCTION_DESTROYED_UP_LEFT                     },
            { "┐",Element.CONSTRUCTION_DESTROYED_RIGHT_UP                    },
            { "└",Element.CONSTRUCTION_DESTROYED_DOWN_LEFT                   },
            { "┘",Element.CONSTRUCTION_DESTROYED_DOWN_RIGHT                  },
            { "•",Element.BULLET                                             },
              
            { "▲",Element.TANK_UP                                            },
            { "►",Element.TANK_RIGHT                                         },
            { "▼",Element.TANK_DOWN                                          },
            { "◄",Element.TANK_LEFT                                          },
              
            { "?",Element.OTHER_TANK_UP                                      },
            { "»",Element.OTHER_TANK_RIGHT                                   },
            { "¿",Element.OTHER_TANK_DOWN                                    },
            { "«",Element.OTHER_TANK_LEFT                                    },
              
            { "˄",Element.PLAYER_TANK_UP                                     },
            { "˃",Element.PLAYER_TANK_RIGHT                                  },
            { "˅",Element.PLAYER_TANK_DOWN                                   },
            { "˂",Element.PLAYER_TANK_LEFT                                   }
        };

        public static Element GetElement(this Cell cell)
        {
            return _elements[cell.C];
        }
    }
}
