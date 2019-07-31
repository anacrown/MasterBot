using System;
using BotBase.Board;
using PaperIoStrategy.DataContract;

namespace PaperIoStrategy.AISolver
{
    public class Bonus
    {
        public Point Position { get; } = Point.Empty;

        public int Moves => JBonus.Moves;

        public int Ticks { get; set; }

        public JBonusType BonusType => JBonus.BonusType;

        public int Speed
        {
            get
            {
                switch (BonusType)
                {
                    case JBonusType.SpeedUp: return 6;
                    case JBonusType.SlowDown: return 3;
                    default: return 5;
                }
            }
        }

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