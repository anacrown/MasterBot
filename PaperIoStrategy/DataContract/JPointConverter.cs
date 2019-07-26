using System;
using BotBase.Board;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaperIoStrategy.DataContract
{
    internal class JPointConverter : JsonConverter
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
}