using System.Collections.Generic;
using CodenjoyBot.Board;

namespace BattleBot_SimpleAI.BattleSolver
{
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

        public static Element GetElement(this Cell cell) => _elements[cell.C];

        public static int GetStrength(this Cell cell)
        {
            switch (cell.GetElement())
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


                case Element.PLAYER_TANK_UP:
                case Element.PLAYER_TANK_RIGHT:
                case Element.PLAYER_TANK_DOWN:
                case Element.PLAYER_TANK_LEFT:
                default: return 0;
            }
        }
    }
}