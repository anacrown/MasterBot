using System;
using Newtonsoft.Json;

namespace PaperIoStrategy.DataContract
{
    internal class JPacketTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsPacketType = (JPacketType)value;

            switch (jsPacketType)
            {
                case JPacketType.StartGame:
                    writer.WriteValue("start_game");
                    break;
                case JPacketType.EndGame:
                    writer.WriteValue("end_game");
                    break;
                case JPacketType.Tick:
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
                case "start_game": return JPacketType.StartGame;
                case "end_game": return JPacketType.EndGame;
                case "tick": return JPacketType.Tick;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}