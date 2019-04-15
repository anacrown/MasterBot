using System;
using System.Runtime.Serialization;
using CodenjoyBot.DataProvider;

namespace CodenjoyBot.Interfaces
{
    public interface IDataLogger : ILogger, ISerializable
    {
        void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame, Exception e);

        void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame, string response);

        void LogDead(CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame);
    }

    public interface ILaunchInfo
    {
        DateTime StartTime { get; }
        string BotInstanceName { get; }
        string BotInstanceTitle { get; }
    }
}
