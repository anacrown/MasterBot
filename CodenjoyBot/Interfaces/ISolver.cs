using System;
using System.Runtime.Serialization;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;

namespace CodenjoyBot.Interfaces
{
    public interface ISolver : ILogger, ISupportControls, ISerializable
    {
        void Initialize();
        string Answer(Board.Board board);
    }
}
