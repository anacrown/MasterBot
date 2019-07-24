using System;
using Newtonsoft.Json;

namespace PaperIO_MiniCupsAI.DataContract
{
    internal class JBonusTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsBonusType = (JBonusType)value;

            switch (jsBonusType)
            {
                case JBonusType.SpeedUp:
                    writer.WriteValue("n");
                    break;
                case JBonusType.SlowDown:
                    writer.WriteValue("s");
                    break;
                case JBonusType.Saw:
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
                case "n": return JBonusType.SpeedUp;
                case "s": return JBonusType.SlowDown;
                case "saw": return JBonusType.Saw;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}