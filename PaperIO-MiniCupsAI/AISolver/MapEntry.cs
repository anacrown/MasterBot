using CodenjoyBot.Board;

namespace PaperIO_MiniCupsAI
{
    public class MapEntry
    {
        public Point Position { get; set; }

        public bool BChecked { get; set; }

        public bool BWatched { get; set; }

        public int Weight { get; set; }
    }
}