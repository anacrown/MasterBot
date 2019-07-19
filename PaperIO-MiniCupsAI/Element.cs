using System.Collections.Generic;
using CodenjoyBot.Board;

namespace PaperIO_MiniCupsAI
{
    public enum Element
    {
        ME, ME_LINE, ME_TERRITORY, PLAYER, PLAYER_LINE, PLAYER_TERRITORY, NONE, EXPLORER, FLASH, SAW
    }
    public static class CellExtention
    {
        private static Dictionary<string, Element> _elements = new Dictionary<string, Element>()
        {
            {"ME", Element.ME },
            {"ME_LINE", Element.ME_LINE },
            {"ME_TERRITORY", Element.ME_TERRITORY },
            {"PLAYER", Element.PLAYER },
            {"PLAYER_LINE", Element.PLAYER_LINE },
            {"PLAYER_TERRITORY", Element.PLAYER_TERRITORY },
            {"NONE", Element.NONE },
            {"EXPLORER", Element.EXPLORER },
            {"FLASH", Element.FLASH },
            {"SAW", Element.SAW }
        };

        public static Element GetElement(this Cell cell)
        {
            return _elements[cell.C];
        }
    }
}