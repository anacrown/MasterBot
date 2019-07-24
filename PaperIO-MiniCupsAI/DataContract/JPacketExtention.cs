using System.ComponentModel;

namespace PaperIO_MiniCupsAI.DataContract
{
    public static class JPacketExtention
    {
        public static JPacket Merge(this JPacket jPacket, JPacket additional)
        {
            if (jPacket.PacketType != JPacketType.Tick) throw new InvalidEnumArgumentException(nameof(jPacket));
            if (additional.PacketType != JPacketType.StartGame) throw new InvalidEnumArgumentException(nameof(additional));

            jPacket.Params.Speed = additional.Params.Speed;
            jPacket.Params.Width = additional.Params.Width;
            jPacket.Params.XCellsCount = additional.Params.XCellsCount;
            jPacket.Params.YCellsCount = additional.Params.YCellsCount;

            return jPacket;
        }
    }
}