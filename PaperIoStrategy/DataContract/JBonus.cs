using BotBase.Board;
using Newtonsoft.Json;

namespace PaperIoStrategy.DataContract
{
    public class JBonus
    {
        //* `type` — тип бонуса ('n' - Ускорение (Нитро), 's' - Замедление, 'saw' - Пила) 
        //* `ticks` — сколько еще клеток будет активен бонус
        //* `position` — координаты бонуса, массив из двух элементов - (x, y)

        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(JBonusTypeConverter))]
        public JBonusType BonusType { get; set; }

        [JsonProperty(PropertyName = "ticks")]
        public int Ticks { get; set; }

        [JsonProperty(PropertyName = "position")]
        [JsonConverter(typeof(JPointConverter))]
        public Point Position { get; set; }
    }
}