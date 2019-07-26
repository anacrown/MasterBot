using System;
using BotBase.Board;
using Newtonsoft.Json;

namespace PaperIoStrategy.DataContract
{
    internal class JDirectionConverter : JsonConverter
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
}