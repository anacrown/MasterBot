using System;
using System.Runtime.Serialization;
using BotBase;
using BotBase.BotInstance;

namespace CodenjoyBot.Interfaces
{
    public interface ISolver : ILogger, ISupportControls, ISerializable
    {
        void Initialize();

        bool Answer(string instanceName, DateTime startTime, DataFrame frame, out string response);
    }
}
