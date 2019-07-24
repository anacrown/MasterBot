using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaperIO_MiniCupsAI.DataContract
{
    public class JPacketParams
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
        public Dictionary<string, JPlayer> Players { get; set; }

        [JsonProperty(PropertyName = "bonuses")]
        public List<JBonus> Bonuses { get; set; }
    }
}