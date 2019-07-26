using System;
using System.Runtime.Serialization;
using BotBase.BotInstance;

namespace BotBase.Interfaces
{
    public interface ISolver : ILogger, ISerializable
    {
        void Initialize();

        bool Answer(string instanceName, DateTime startTime, DataFrame frame, out string response);
    }
}
