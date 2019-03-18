using System;
using System.Runtime.Serialization;

namespace CodenjoyBot.Interfaces
{
    public interface IDataLogger : ILogger, ISerializable
    {
        void Log(string battleBotInstanceName, DateTime startTime, uint time, Exception e);

        void Log(string battleBotInstanceName, DateTime startTime, uint time, string board, string response);

        void LogDead(string battleBotInstanceName, DateTime startTime, uint time);
    }
}
