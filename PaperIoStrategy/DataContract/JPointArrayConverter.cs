using System;
using System.Collections.Generic;
using BotBase.Board;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaperIoStrategy.DataContract
{
    internal class JPointArrayConverter : JsonConverter
    {
        private JPointConverter _jsPointConverter = new JPointConverter();

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
}