using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaperIO_MiniCupsAI.DataContract
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
