using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using BattleBot_SuperAI.Controls;
using CodenjoyBot.Annotations;
using CodenjoyBot.Board;
using CodenjoyBot.Interfaces;

namespace BattleBot_SuperAI.BattleSolver
{
    [Serializable]
    public class BattleSolver : ISolver, INotifyPropertyChanged
    {
        private int _size;
        private Board _board;
        private UIElement _control;
        private UIElement _debugControl;
        
        public UIElement Control => _control ?? (_control = new BattleSolverControl(this));

        public UIElement DebugControl => _debugControl ?? (_debugControl = new BattleSolverDebugControl(this));

        public BattleSolver() { }

        protected BattleSolver(SerializationInfo info, StreamingContext context) : this()
        {
            
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }

        public void Initialize()
        {
            
        }

        public int Size
        {
            get => _size;
            set
            {
                if (value == _size) return;
                _size = value;
                OnPropertyChanged();
            }
        }

        public Board Board
        {
            get => _board;
            private set
            {
                if (Equals(value, _board)) return;
                _board = value;
                OnBoardChanged();
                OnPropertyChanged();
            }
        }

        public string Answer(Board board)
        {
            Board = board;

            return "";
        }

        public event EventHandler<LogRecord> LogDataReceived;
        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);

        public event EventHandler<Board> BoardChanged;
        protected virtual void OnBoardChanged() => BoardChanged?.Invoke(this, Board);
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
        private static Dictionary<char, Element> _elements = new Dictionary<char, Element>()
        {
            { ' ',Element.GROUND                                             },
            { '☼',Element.WALL                                               },
            { 'Ѡ',Element.DEAD                                               },
            { '╬',Element.CONSTRUCTION                                       },
            { '╩',Element.CONSTRUCTION_DESTROYED_DOWN                        },
            { '╦',Element.CONSTRUCTION_DESTROYED_UP                          },
            { '╠',Element.CONSTRUCTION_DESTROYED_LEFT                        },
            { '╣',Element.CONSTRUCTION_DESTROYED_RIGHT                       },
            { '╨',Element.CONSTRUCTION_DESTROYED_DOWN_TWICE                  },
            { '╥',Element.CONSTRUCTION_DESTROYED_UP_TWICE                    },
            { '╞',Element.CONSTRUCTION_DESTROYED_LEFT_TWICE                  },
            { '╡',Element.CONSTRUCTION_DESTROYED_RIGHT_TWICE                  },
            { '│',Element.CONSTRUCTION_DESTROYED_LEFT_RIGHT                  },
            { '─',Element.CONSTRUCTION_DESTROYED_UP_DOWN                     },
            { '┌',Element.CONSTRUCTION_DESTROYED_UP_LEFT                     },
            { '┐',Element.CONSTRUCTION_DESTROYED_RIGHT_UP                    },
            { '└',Element.CONSTRUCTION_DESTROYED_DOWN_LEFT                   },
            { '┘',Element.CONSTRUCTION_DESTROYED_DOWN_RIGHT                  },
            { '•',Element.BULLET                                             },
            
            { '▲',Element.TANK_UP                                            },
            { '►',Element.TANK_RIGHT                                         },
            { '▼',Element.TANK_DOWN                                          },
            { '◄',Element.TANK_LEFT                                          },
            
            { '?',Element.OTHER_TANK_UP                                      },
            { '»',Element.OTHER_TANK_RIGHT                                   },
            { '¿',Element.OTHER_TANK_DOWN                                    },
            { '«',Element.OTHER_TANK_LEFT                                    },
            
            { '˄',Element.PLAYER_TANK_UP                                     },
            { '˃',Element.PLAYER_TANK_RIGHT                                  },
            { '˅',Element.PLAYER_TANK_DOWN                                   },
            { '˂',Element.PLAYER_TANK_LEFT                                   }
        };

        public static Element GetElement(this Cell cell)
        {
            return _elements[cell.C];
        }
    }
}
