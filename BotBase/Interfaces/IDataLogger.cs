using System;
using System.Runtime.Serialization;
using BotBase.BotInstance;

namespace BotBase.Interfaces
{
    public interface IDataLogger : ILogger, ISerializable
    {
        void Log(string name, DateTime startTime, DataFrame frame, Exception e);

        void Log(string name, DateTime startTime, DataFrame frame);

        void Log(string name, DateTime startTime, uint time, string response);

        void LogDead(string name, DateTime startTime, DataFrame frame);
    }

    public interface ILaunchInfo
    {
        DateTime StartTime { get; }
        string BotInstanceName { get; }
        string BotInstanceTitle { get; }
    }
}
