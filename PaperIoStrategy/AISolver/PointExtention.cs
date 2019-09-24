using BotBase.Board;

namespace PaperIoStrategy.AISolver
{
    static class PointExtention
    {
        public static Point ToGrid(this Point point, int width)
        {
//            double w2 = width / 2;
//            var x = Math.Round(((double) point.X) / width);
//            var y = Math.Round(((double) point.Y) / width);

//            return new Point((int)x, (int)y);

            return point / width;
        }

        public static Point ToGrid(this Point point, int width, Direction direction)
        {
            var corner = (point - width / 2);
            var p = corner / width;
            return !(corner % width).IsEmpty && (direction == Direction.Left || direction == Direction.Down) ? p[direction.Invert()] : p;
        }

        public static Point FromGrid(this Point point, int width) => point * width + width / 2;
    }
}
