using System;
using System.Runtime.Serialization;

namespace BotBase.Interfaces
{
    public interface IDataLogger : ILogger, ISerializable
    {
        void Log(string name, DateTime startTime, DataFrame frame, Exception e);

        void Log(string name, DateTime startTime, DataFrame frame);

        void Log(string name, DateTime startTime, DateTime time, uint frameNumber, string response);

        void LogDead(string name, DateTime startTime, DataFrame frame);
    }

    public interface ILaunchInfo
    {
        DateTime StartTime { get; }
        string BotInstanceName { get; }
        string BotInstanceTitle { get; }
    }
}
