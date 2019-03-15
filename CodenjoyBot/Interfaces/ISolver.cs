using System.Runtime.Serialization;

namespace CodenjoyBot.Interfaces
{
    public interface ISolver : ILogger, ISupportControls, ISerializable
    {
        void Initialize();

        string Answer(Board.Board board);
    }
}
