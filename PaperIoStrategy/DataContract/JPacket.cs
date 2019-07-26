using Newtonsoft.Json;

namespace PaperIoStrategy.DataContract
{
    public class JPacket
    {
        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(JPacketTypeConverter))]
        public JPacketType PacketType { get; set; }

        [JsonProperty(PropertyName = "params")]
        public JPacketParams Params { get; set; }
    }
}
