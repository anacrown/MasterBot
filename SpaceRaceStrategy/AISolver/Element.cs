using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceRaceStrategy.AISolver;

namespace SpaceRaceStrategy
{
    public enum Element
    {
        NONE, EXPLOSION, WALL, HERO, OTHER_HERO, DEAD_HERO, GOLD, BOMB, STONE, BULLET_PACK, BULLET
    }
    public static class CellExtention
    {
        private static Dictionary<string, Element> _elements = new Dictionary<string, Element>()
        {
            {" ", Element.NONE },
            {"x", Element.EXPLOSION },
            {"☼", Element.WALL },
            {"☺", Element.HERO },
            {"☻", Element.OTHER_HERO },
            {"+", Element.DEAD_HERO },
            {"$", Element.GOLD },
            {"♣", Element.BOMB },
            {"0", Element.STONE },
            {"7", Element.BULLET_PACK },
            {"*", Element.BULLET }
        };

        public static Element GetElement(this Cell cell)
        {
            return _elements[cell.C];
        }
    }
}
