using System;
using System.Runtime.Serialization;
using CodenjoyBot.DataProvider;

namespace CodenjoyBot.Interfaces
{
    public interface IDataLogger : ILogger, ISerializable
    {
        void Log(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame, Exception e);

        void Log(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame);

        void Log(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, string response);

        void LogDead(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame);
    }

    public interface ILaunchInfo
    {
        DateTime StartTime { get; }
        string BotInstanceName { get; }
        string BotInstanceTitle { get; }
    }
}
