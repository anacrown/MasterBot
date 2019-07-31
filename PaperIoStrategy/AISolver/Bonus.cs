using BotBase.Board;
using PaperIoStrategy.DataContract;

namespace PaperIoStrategy.AISolver
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
                Position = jBonus.Position.ToGrid(jPacket.Params.Width);
        }
    }
}