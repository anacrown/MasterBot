using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using CodenjoyBot.Board;
using CodenjoyBot.Interfaces;

namespace BattleBot_SuperAI
{
    public class BattleSolver : ISolver
    {
        public event EventHandler<LogRecord> LogDataReceived;
        public UIElement Control { get; }
        public UIElement DebugControl { get; }

        protected BattleSolver(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public string Answer(Board board)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<Board> BoardChanged;
        protected virtual void OnBoardChanged(Board e) => BoardChanged?.Invoke(this, e);
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
