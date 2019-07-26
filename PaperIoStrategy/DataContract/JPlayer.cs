using System.Collections.Generic;
using BotBase.Board;
using Newtonsoft.Json;

namespace PaperIoStrategy.DataContract
{
    public class JPlayer
    {
        // * `score` — количество очков игрока
        // * `territory` — массив координат клеток, принадлежащих территории игрока
        // * `position` — текущее положение игрока
        // * `lines` — массив координат клеток шлейфа
        // * `direction` — направление движения игрока ("left", "right", "up", "down")
        // * `bonuses` — массив активных бонусов игрока

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }

        [JsonProperty(PropertyName = "position")]
        [JsonConverter(typeof(JPointConverter))]
        public Point Position { get; set; }

        [JsonProperty(PropertyName = "direction")]
        [JsonConverter(typeof(JDirectionConverter))]
        public Direction Direction { get; set; }

        [JsonProperty(PropertyName = "lines")]
        [JsonConverter(typeof(JPointArrayConverter))]
        public List<Point> Lines { get; set; }

        [JsonProperty(PropertyName = "territory")]
        [JsonConverter(typeof(JPointArrayConverter))]
        public List<Point> Territory { get; set; }

        [JsonProperty(PropertyName = "bonuses")]
        public List<JBonus> Bonuses { get; set; }
    }
}