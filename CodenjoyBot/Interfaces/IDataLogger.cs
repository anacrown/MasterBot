using System;
using System.Runtime.Serialization;

namespace CodenjoyBot.Interfaces
{
    public interface IDataLogger : ILogger, ISerializable
    {
        void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, Exception e);

        void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, string board, string response);

        void LogDead(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time);
    }

    public interface ILaunchInfo
    {
        DateTime StartTime { get; }
        string BotInstanceName { get; }
        string BotInstanceTitle { get; }
    }
}
