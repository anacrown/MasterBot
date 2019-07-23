using System;
using System.Runtime.Serialization;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;

namespace CodenjoyBot.Interfaces
{
    public interface ISolver : ILogger, ISupportControls, ISerializable
    {
        void Initialize();

        bool Answer(string instanceName, DateTime startTime, DataFrame frame, IDataProvider dataProvider, out string response);
    }
}
