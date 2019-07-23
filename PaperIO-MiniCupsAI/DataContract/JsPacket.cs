using System;
using System.Collections.Generic;
using CodenjoyBot.Board;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace PaperIO_MiniCupsAI.DataContract
{
    [Serializable]
    internal class JsPacket
    {
        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(JsPacketTypeConverter))]
        public JsPacketType PacketType { get; set; }

        [JsonProperty(PropertyName = "params")]
        public JsPacketParams Params { get; set; }
    }

    internal enum JsPacketType { StartGame, EndGame, Tick }

    internal class JsPacketTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsPacketType = (JsPacketType)value;

            switch (jsPacketType)
            {
                case JsPacketType.StartGame:
                    writer.WriteValue("start_game");
                    break;
                case JsPacketType.EndGame:
                    writer.WriteValue("end_game");
                    break;
                case JsPacketType.Tick:
                    writer.WriteValue("tick");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;

            switch (enumString)
            {
                case "start_game": return JsPacketType.StartGame;
                case "end_game": return JsPacketType.EndGame;
                case "tick": return JsPacketType.Tick;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    [Serializable]
    internal class JsPacketParams
    {
        [JsonProperty(PropertyName = "x_cells_count")]
        public int XCellsCount { get; set; }

        [JsonProperty(PropertyName = "y_cells_count")]
        public int YCellsCount { get; set; }

        [JsonProperty(PropertyName = "speed")]
        public int Speed { get; set; }

        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        [JsonProperty(PropertyName = "tick_num")]
        public int Tick { get; set; }

        [JsonProperty(PropertyName = "players")]
        public Dictionary<string, JsPlayer> Players { get; set; }

        //bonuses

    }

    internal class JsPlayer
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
        [JsonConverter(typeof(JsPointConverter))]
        public Point Position { get; set; }

        [JsonProperty(PropertyName = "direction")]
        [JsonConverter(typeof(JsDirectionConverter))]
        public Direction Direction { get; set; }

        [JsonProperty(PropertyName = "lines")]
        [JsonConverter(typeof(JsPointArrayConverter))]
        public List<Point> Lines { get; set; }

        [JsonProperty(PropertyName = "territory")]
        [JsonConverter(typeof(JsPointArrayConverter))]
        public List<Point> Territory { get; set; }

        [JsonProperty(PropertyName = "bonuses")]
        public List<JsBonus> Bonuses { get; set; }
    }

    internal class JsDirectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var direction = (Direction)value;

            switch (direction)
            {
                case Direction.Up:
                    writer.WriteValue("up");
                    break;
                case Direction.Right:
                    writer.WriteValue("right");
                    break;
                case Direction.Down:
                    writer.WriteValue("down");
                    break;
                case Direction.Left:
                    writer.WriteValue("left");
                    break;
                case Direction.Unknown:
                    writer.WriteNull();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;

            switch (enumString)
            {
                case "up": return Direction.Up;
                case "right": return Direction.Right;
                case "down": return Direction.Down;
                case "left": return Direction.Left;
                default: return Direction.Unknown;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    internal class JsPointConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var point = (Point) value;

           writer.WriteStartArray();
           writer.WriteValue(point.X);
           writer.WriteValue(point.Y);
           writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);

            return new Point(array[0].Value<int>(), array[1].Value<int>());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Point);
        }
    }

    internal class JsPointArrayConverter : JsonConverter
    {
        private JsPointConverter _jsPointConverter = new JsPointConverter();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var array = (IEnumerable<Point>) value;

            writer.WriteStartArray();

            foreach (var point in array)
            {
                _jsPointConverter.WriteJson(writer, point, serializer);
            }

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);

            var list = new List<Point>();
            foreach (var jToken in array)
            {
                list.Add((Point)_jsPointConverter.ReadJson(jToken.CreateReader(), null, null, serializer));
            }

            return list;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IEnumerable<Point>);
        }
    }

    internal class JsBonus
    {
        //* `type` — тип бонуса ('n' - Ускорение (Нитро), 's' - Замедление, 'saw' - Пила) 
        //* `ticks` — сколько еще клеток будет активен бонус
        //* `position` — координаты бонуса, массив из двух элементов - (x, y)

        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(JsBonusTypeConverter))]
        public JsBonusType BonusType { get; set; }

        [JsonProperty(PropertyName = "ticks")]
        public int Ticks { get; set; }

        [JsonProperty(PropertyName = "position")]
        [JsonConverter(typeof(JsPointConverter))]
        public Point Position { get; set; }
    }

    internal enum JsBonusType { SpeedUp, SlowDown, Saw }

    internal class JsBonusTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsBonusType = (JsBonusType)value;

            switch (jsBonusType)
            {
                case JsBonusType.SpeedUp:
                    writer.WriteValue("n");
                    break;
                case JsBonusType.SlowDown:
                    writer.WriteValue("s");
                    break;
                case JsBonusType.Saw:
                    writer.WriteValue("saw");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;

            switch (enumString)
            {
                case "n": return JsBonusType.SpeedUp;
                case "s": return JsBonusType.SlowDown;
                case "saw": return JsBonusType.Saw;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
