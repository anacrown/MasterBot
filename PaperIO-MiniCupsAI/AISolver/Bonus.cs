using CodenjoyBot.Board;
using PaperIO_MiniCupsAI.DataContract;

namespace PaperIO_MiniCupsAI
{
    public class Bonus
    {
        public Point Position { get; } = Point.Empty;

        public int Ticks => JBonus.Ticks;

        public JBonusType BonusType => JBonus.BonusType;

        public JBonus JBonus { get; }

        public Bonus(JBonus jBonus)
        {
            JBonus = jBonus;
        }

        public Bonus(JPacket jPacket, JBonus jBonus) : this(jBonus)
        {
            if (jBonus.Position != null)
                Position = jBonus.Position / jPacket.Params.Width;
        }
    }
}