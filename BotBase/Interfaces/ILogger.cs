using System;
using BotBase.BotInstance;

namespace BotBase.Interfaces
{
    public interface ILogger
    {
        event EventHandler<LogRecord> LogDataReceived;
    }
}
